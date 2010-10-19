#!/usr/bin/perl

use strict;
use warnings FATAL => qw/all/;
use Time::Local;
use XML::Parser;
use POSIX qw(strftime);
use Data::Dumper;
use XML::Simple;
use Storable qw(dclone);
use LWP::Simple;

my$data = $ARGV[0] || "file:g:/Users/go/dev/gc/gps-running/tmp/General6.csv";
#'http://spreadsheets.google.com/pub?key=rrP7dbsqMO8l4yFo9HXO1ZA&output=csv';
my$root = $ARGV[1] || ".";
my$db;

my$csv = get($data);
 die "Couldn't get $data" unless defined $csv;
if ($csv =~ m/\<\?xlm/) {
    my$config = XMLin($csv, ForceArray => 1, KeyAttr => {});

    my$prefix = "ss:";
    my$ws = $config->{"ss:Worksheet"} || $config->{"Worksheet"};
    
    $db = ParseWorksheet($ws->[0]{Table}[0]);
} else {
    $db = ParseCSV($csv);
}

$db = GenDB($db);

sub value
{
    my$h = $_[0][0];
    return join ",", (map {
        return ref $h->{$_} ? () : $h->{$_}
    } sort keys %$h);
}

sub sorted_keys {
    my($self, $name, $ref) = @_;
    my %sk;
    my@sk = reverse qw/xsd:import xsd:sequence xsd:schema resheader assembly data/;
    my$i = @sk;
    while ($i-- > 0) {
        $sk{$sk[$i]} = $i+1;
    }
    return sort {
        my$v1 = $sk{$a} || 0;
        my$v2 = $sk{$b} || 0;
        my$d = ($v2 <=> $v1) || ($a cmp $b);
        return $d if $d;
        return value($ref->{$a}) cmp value($ref->{$b});
    }  keys %$ref;
}

*XML::Simple::sorted_keys = *sorted_keys;

my$sorter = XML::Simple->new();

#print Dumper($db);
#exit 0;

foreach my$sname (keys %{$db->{sections}}) {
    my$section = $db->{sections}{$sname};
    my$sfile = "$root/$sname";
    my$sxml = XMLin($sfile, ForceArray => 1, KeyAttr => {});
    for(my$i = 2; $i < @{$db->{codes}}; $i++) {
        my$res = {};
        $res->{"xsd:schema"} = dclone($sxml->{"xsd:schema"});
        $res->{"resheader"} = dclone($sxml->{"resheader"});
        if ($sxml->{"assembly"}) {
            $res->{"assembly"} = dclone($sxml->{"assembly"});
        }

        foreach my$d (@{$sxml->{"data"}}) {
            if (defined $section->{$d->{name}} &&
                defined $section->{$d->{name}}[$i] &&
                $section->{$d->{name}}[$i] ne "")
            {
                my$o = dclone($d);
                $o->{value}[0] = $section->{$d->{name}}[$i];
                push @{$res->{"data"}}, $o;
            }
        }

        my$code = $db->{codes}[$i];
		if (! ($code =~ /^(#|xx|comment|$)/)){
		if($i==1){$code="";}
		else{$code.=".";}
		(my$out = $sfile) =~ s/.resx$/.${code}resx/;
        print "Writing: $out\n";
        #print Dumper(@{$res->{"data"}});
        open OUT, ">$out" or die "failed to create $out";
		#Set binmode to allow wide characters in the csv
		binmode OUT,":utf8";
        print OUT '<?xml version="1.0" encoding="utf-8"?>',"\n";
        print OUT XMLout($sorter, $res, KeyAttr => {}, RootName => "root");
        close OUT or die "failed to close $out";
		}
    }        
}

my$out = XMLout($db, KeyAttr => {});
$out =~ s%^<opt>%<?xml version="1.0" encoding="UTF-8" standalone="no" ?>%;
$out =~ s%</opt>$%%;

#print Dumper($db);

exit 0;

sub ParseCSV
{
    my($csv) = @_;
    $csv =~ s/\r//g;
    $csv .= "\n";
    my@lines = ();
	#$i=5;
    while ($csv =~ s/^((([^\n\"]|\\\")*(?<!\\)\"([^\"]|\\\")*(?<!\\)\")*([^\n\"]|\\\")*)\n//) {
        my$line = $1;
        chomp $line;
        my@fields = ();
        while ($line =~ s/^(([^,\"]|\\\")*((?<!\\)\"([^\"]|\\\")*(?<!\\)\")*([^,\"]|\\\")*),//) {
            push @fields, $1;
        }
        #print STDERR " " . @fields . @fields;
        #print STDERR @fields;
        #print STDERR "\n";
		push @fields, $line;
        @fields = map { s/^\"//; s/\"$//; s/\"\"/\"/g; s/^\s*$//; $_ } @fields;

        push @lines, \@fields;
    }

    die "CSV not empty: `$csv'\n" unless $csv =~ m/^\s*$/;

    return \@lines;
}

sub GenDB
{
    my($rows) = @_;
    my%translations = ();
	my@sections=();

    foreach my$row (@$rows) {
        my@row = @$row;

        while (@row && $row[$#row] eq "") {
            $#row = $#row - 1;
        }

        if (@row) {
            if ($row[0] eq "CountryCode") {
                $translations{codes} = \@row;
                next;
            } elsif ($row[0] =~ m%^[\w\\/]+\.resx$%) {
				@sections=();
				foreach my$i (@row){
				  if ($i =~ m%^[\w\\/]+\.resx$%) {
				    push @sections, $i;
				  }
				}
                next;
            }
            die "No section (resx) defined in data" unless $sections[0];
            die "No country codes defined in data" unless $translations{codes};
			foreach my$d (@sections){
              $translations{sections}{$d}{$row[0]} = \@row;
			}
        }
    }
    return \%translations;
}

sub ParseWorksheet
{
    my($table) = @_;
    my$rows = $table->{Row};
    my$rownum = 1;
    my@rows = ();

    print "Rows to process ", scalar @$rows, "\n";
    foreach my$row (@$rows) {
        if (defined $row->{"ss:Index"}) {
            $rownum = $row->{"ss:Index"};
        }
        my$title = "";
        my$colnum = 1;
        my@row = ();
        foreach my$cell (@{$row->{Cell}}) {
            if (defined $cell->{"ss:Index"}) {
                $colnum = $cell->{"ss:Index"};
            }
            my$data = $cell->{Data}[0]{content};
            if (defined $data) {
                chomp $data;
                $row[$colnum-1] = $data;
            }
            $colnum += 1;
        }

        $rows[$rownum++] = \@row;
    }

    return \@rows;
}
