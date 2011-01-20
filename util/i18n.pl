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

my$csvArg='&output=csv&gid=3';
my$savArg='&output=xls';
my$localcopy="Resources.xls";
#my$spreadsheetURL='https://spreadsheets.google.com/feeds/download/spreadsheets/Export?key=tNZna7OU_2RlRYv5Iv_csgg';
my$spreadsheetURL='http://spreadsheets.google.com/ccc?key=tNZna7OU_2RlRYv5Iv_csgg';
my$sav="$spreadsheetURL$savArg";

#Set name on file if Google access does not work
#$spreadsheetURL="file:g:/Users/go/dev/gc/gps-running/Resources.csv";
#$csvArg="";

my$data = $ARGV[0] || ".";#"$spreadsheetURL$csvArg";
my$root = $ARGV[1] || ".";
my$visualdiff = $ARGV[2] || "";
my$db;

##Debug: diff standard language differences
if($visualdiff eq ".")
{
    #default value is KDiff3 - shows char by char diffs
    $visualdiff="$ENV{PROGRAMFILES}/KDiff3/kdiff3.exe"
}

my$csv;
if($data ne ".")
{
    $csv = get($data);
}
else
{
    #Store a copy locally
    if($sav ne "")
    {
        `wget  --no-check-certificate -O $localcopy "$sav"`;
        #my $browser = LWP::UserAgent->new;
        #my$response=$browser->get("$data$savArg");
        #die "Error at $data$savArg\n ", $response->status_line, "\n Aborting"
        # unless $response->is_success;
        #getstore($sav,$localcopy) != 200 || print "Warning: Cannot save copy of $sav in $localcopy\n";
    }

    #Retrieve from Google Spreadsheet
    $data="$spreadsheetURL$csvArg";
    my$tfile="g_csv.tmp.$$";
    `wget  --no-check-certificate -O $tfile "$data"`;
    open F, "<:encoding(UTF-8)", "$tfile";
    while(<F>){$csv.=$_;}
    close F;
    unlink $tfile;
}
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
    my%xmlexists;
    my$spr_diff="spr_diff.tmp";
    my$resx_diff="resx_diff.tmp";

    for(my$i = 1; $i < @{$db->{codes}}; $i++) {
        my$res = {};
        $res->{"xsd:schema"} = dclone($sxml->{"xsd:schema"});
        $res->{"resheader"} = dclone($sxml->{"resheader"});
        if ($sxml->{"assembly"}) {
            $res->{"assembly"} = dclone($sxml->{"assembly"});
        }

        if ($i==1)
        {
            open(SPR, "> $spr_diff");
            open(RESX, "> $resx_diff");
        }
        foreach my$d (@{$sxml->{"data"}}) {
            if ($i==1)
            {
                if(!defined $d->{type})
                {
                    if(!defined $section->{$d->{name}} ||
                    !defined $section->{$d->{name}}[$i])
                    {
                        print STDOUT "Warning: spreadsheet no def for $d->{name}\n";
                    } elsif (!defined $d->{value})
                    {
                         print STDOUT "Warning: $sname no value for $d->{name}\n";
                    } else
                    {
                        my$spr=$section->{$d->{name}}[$i];
                        $spr=~s/\r\n/\n/g;
                        my$resx=$d->{value}[0];
                        $resx=~s/\r\n/\n/g;
                        if ( $spr ne $resx )
                        {
                            if($visualdiff ne "")
                            {
                                print SPR "$d->{name}\n$spr\n-------\n\n";
                                print RESX "$d->{name}\n$resx\n-------\n\n";
                            } else {
                                print STDOUT "\nWarning: Different definitions for $d->{name}: spreadsheet\n   $spr\n and $sname\n   $resx\n\n";
                            }
                        }
                    }
                    $xmlexists{$d->{name}}[$i]=$d->{value}[0];
                }
            } else {
                if (defined $section->{$d->{name}} &&
                    defined $section->{$d->{name}}[$i] &&
                    $section->{$d->{name}}[$i] ne "")
                {
                    my$o = dclone($d);
                    $o->{value}[0] = $section->{$d->{name}}[$i];
                    push @{$res->{"data"}}, $o;
                }
            }
        }

        if($i==1)
        {
            if($visualdiff ne "")
            {
                close SPR;
                close RESX;
                if(-e $visualdiff)
                {
                    print "Executing: \"$visualdiff\" $spr_diff $resx_diff\n";
                    system("\"$visualdiff\" $spr_diff $resx_diff");
                    unlink $spr_diff;
                    unlink $resx_diff;
                } else {
                    print "Cannot find \"$visualdiff\" $spr_diff $resx_diff , keeping temp files\n\n";
                }
            }
            foreach my$j (keys %{$section}) {
                if($j ne "\$this.Text" && !defined $xmlexists{$j})
                {
                    print STDOUT "Warning: resx no def for $j\n";
                }
            }
        }
        else
        {
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
                my$txt=XMLout($sorter, $res, KeyAttr => {}, RootName => "root");
                $txt=~s/\r\n/\n/g;
                print OUT $txt;
                close OUT or die "failed to close $out";
            }
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
            my$data2 = $cell->{Data}[0]{content};
            if (defined $data2) {
                chomp $data2;
                $row[$colnum-1] = $data2;
            }
            $colnum += 1;
        }
        $rows[$rownum++] = \@row;
    }
    return \@rows;
}
