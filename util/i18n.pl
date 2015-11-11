#!/usr/bin/perl

#Copyright (C) 2009 Mark Williams
#Copyright (C) 2011 Gerhard Olsson

#This library is free software; you can redistribute it and/or
#modify it under the terms of the GNU Lesser General Public
#License as published by the Free Software Foundation; either
#version 3 of the License, or (at your option) any later version.
#
#This library is distributed in the hope that it will be useful,
#but WITHOUT ANY WARRANTY; without even the implied warranty of
#MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
#Lesser General Public License for more details.
#
#You should have received a copy of the GNU Lesser General Public
#License along with this library. If not, see <http://www.gnu.org/licenses/>.

use strict;
use warnings FATAL => qw/all/;
use Time::Local;
use XML::Parser;
use POSIX qw(strftime);
use Data::Dumper;
use XML::Simple;
use Storable qw(dclone);
use LWP::Simple;
binmode STDOUT, ":encoding(UTF-8)";

my $sheetId=3;
my$spreadsheetURL='https://docs.google.com/spreadsheets/export?id=1gy-zMCf1eyEjX49F5ZcV4eZTDDsGC7qKJIfOMy7082c&';

my$csvArg="exportFormat=csv&gid=$sheetId";
my$savArg='exportFormat=xlsx';
my$localcopy="Resources.xlsx";
my$sav="$spreadsheetURL$savArg";
my$verbose=1;
if(defined $ENV{VERBOSE})
{
  $verbose=$ENV{VERBOSE};
}

#"local" usage with .csv file requres only specing the file as first argument
#Set name on file if Google access does not work
#$spreadsheetURL="file:g:/Users/go/dev/gc/gps-running/Resources.csv";
#$csvArg="";
#$sav="";

my$data = $ARGV[0] || ".";#"$spreadsheetURL$csvArg";
my$root = $ARGV[1] || ".";
my$visualdiff = $ARGV[2] || "";
my$db;
my$noTranslate="##NOTRANSLATE##";

##Debug: diff standard language differences
if($visualdiff eq ".")
{
  #default value is KDiff3 - shows char by char diffs
  $visualdiff="$ENV{PROGRAMFILES}/KDiff3/kdiff3.exe"
}

#Special handling in testing the script
my$testMode=0;
if(defined $ENV{TEST_MODE})
{
  $testMode=$ENV{TEST_MODE};
  $sav="";
}

my$csv;
if($data ne ".")
{
  #File is likely local, add prefix
  if(-f $data)
  {
    my$pwd = $ENV{PWD};
    if ($data =~ /\w:/)
    {
      $pwd="";
    }
    else
    {
      $pwd =~ s%/cygdrive/(\w)%${1}:%;
      $pwd = "$pwd/";
    }
    $data="file:$pwd$data";
  }
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
  my$cmd="wget  --no-check-certificate -O $tfile \"$data\"";
  if($verbose>0)
  {
    print "$cmd\n";
  }
  `$cmd`;
  open F, "<:encoding(UTF-8)", "$tfile";
  while(<F>) {$csv.=$_;}
  close F;
  unlink $tfile;
}
die "Couldn't get $data" unless defined $csv;

if ($csv =~ m/\<\?xlm/)
{
  my$config = XMLin($csv, ForceArray => 1, KeyAttr => {});

  my$prefix = "ss:";
  my$ws = $config->{"ss:Worksheet"} || $config->{"Worksheet"};
  
  $db = ParseWorksheet($ws->[0]{Table}[0]);
}
else
{
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

sub sorted_keys
{
  my($self, $name, $ref) = @_;
  my %sk;
  my@sk = reverse qw/xsd:import xsd:sequence xsd:schema resheader assembly data/;
  my$i = @sk;
  while ($i-- > 0)
  {
    $sk{$sk[$i]} = $i+1;
  }
  return sort {
    my$v1 = $sk{$a} || 0;
    my$v2 = $sk{$b} || 0;
    my$d = ($v2 <=> $v1) || ($a cmp $b);
    return $d if $d;
    return value($ref->{$a}) cmp value($ref->{$b});
  } keys %$ref;
}

*XML::Simple::sorted_keys = *sorted_keys;

my$sorter = XML::Simple->new();

#print Dumper($db);
#exit 0;

foreach my$sname (keys %{$db->{sections}})
{
  my$section = $db->{sections}{$sname};
  my$sfile = "$root/$sname";
  my$sxml = XMLin($sfile, ForceArray => 1, KeyAttr => {});
  my%xmlexists;
  my$spr_diff="";
  my$resx_diff="";

  for(my$i = 1; $i < @{$db->{codes}}; $i++)
  {
    my$res = {};
    $res->{"xsd:schema"} = dclone($sxml->{"xsd:schema"});
    $res->{"resheader"} = dclone($sxml->{"resheader"});
    if ($sxml->{"assembly"})
    {
      $res->{"assembly"} = dclone($sxml->{"assembly"});
    }
	
	my$nodef1="";
	my$nodef2="";
	my$noval1="";
	my$noval2="";
	my$isdiff1="";
	my$isdiff2="";

    foreach my$d (@{$sxml->{"data"}})
    {
      if ($i==1)
      {
        #First code is used for checks only
        if(!defined $d->{type})
        {
          if(!defined $section->{$d->{name}} ||
          !defined $section->{$d->{name}}[$i])
          {
            $nodef1 .= "$d->{name}\n";
            $nodef2 .= "$d->{value}[0]\n";
          }
          elsif (!defined $d->{value})
          {
             $noval1 .= "$d->{name}\n";
             $noval2 .= "$section->{$d->{name}}[$i]\n";
          }
          else
          {
            my$spr=$section->{$d->{name}}[$i];
            $spr=~s/\r\n/\n/g;
            my$resx=$d->{value}[0];
            $resx=~s/\r\n/\n/g;
            if ( $spr ne $resx )
            {
              if($visualdiff ne "")
              {
                $spr_diff.="$d->{name}\n$spr\n-------\n\n";
                $resx_diff.="$d->{name}\n$resx\n-------\n\n";
              }
              else
              {
                #print STDOUT "\nWarning: Different definitions for $d->{name}: spreadsheet\n   $spr\n and $sname\n   $resx\n\n";
                $isdiff1.="$d->{name}\t$spr\n";
                $isdiff2.="$d->{name}\t$resx\n";
              }
            }
          }
          $xmlexists{$d->{name}}[$i]=$d->{value}[0];
        }
      }
      else
      {
        if (defined $section->{$d->{name}} &&
          defined $section->{$d->{name}}[$i] &&
          $section->{$d->{name}}[$i] ne "")
        {
          my$o = dclone($d);
          $o->{value}[0] = $section->{$d->{name}}[$i];
          if($section->{$d->{name}}[$i] ne $noTranslate)
          {
            push @{$res->{"data"}}, $o;
          }
        }
      }
    }
	if($nodef1 ne "")
	{
      print STDOUT "\nWarning: spreadsheet no def:\n";
	  print STDOUT $nodef1;
      print STDOUT "\n";
	  print STDOUT $nodef2;
      print STDOUT "\n";
	}
	if($noval1 ne "")
	{
      print STDOUT "\nWarning: $sname no value:\n";
	  print STDOUT $noval1;
      print STDOUT "\n";
	  print STDOUT $noval2;
      print STDOUT "\n";
	}
	if($isdiff1 ne "")
	{
      print STDOUT "\nWarning: Different definitions for spreadsheet\n";
	  print STDOUT $isdiff1;
      print STDOUT "\n   $sname\n";
	  print STDOUT $isdiff2;
      print STDOUT "\n";
	}


    if($i==1)
    {
      if($visualdiff ne "" &&
         ($spr_diff ne "" || $resx_diff ne ""))
      {
        my$spr_diff_file="spr_diff.tmp";
        my$resx_diff_file="resx_diff.tmp";
        open(SPR, "> $spr_diff_file");
        open(RESX, "> $resx_diff_file");
        print SPR $spr_diff;
        print RESX $resx_diff;
        close SPR;
        close RESX;
        if(-e $visualdiff)
        {
          if($verbose>1)
          {
            print "Executing: \"$visualdiff\" $spr_diff_file $resx_diff_file\n";
          }
          system("\"$visualdiff\" $spr_diff_file $resx_diff_file");
          unlink $spr_diff_file;
          unlink $resx_diff_file;
        }
        else
        {
          print "Error: Cannot find \"$visualdiff\" $spr_diff_file $resx_diff_file , keeping temp files\n\n";
        }
      }
	  my$nospr="";
      foreach my$j (sort keys %{$section})
      {
        if($j ne "\$this.Text" &&
        !defined $xmlexists{$j})
        {
          $nospr.="$j\n";
        }
      }
	  if($nospr ne "")
	  {
          print STDOUT "Warning: resx no def:\n";
		  print STDOUT $nospr;
		  print STDOUT "\n";
	  }
    }
    else
    {
      #The codes to be written to resx
      my$code = $db->{codes}[$i];
      if (! ($code =~ /^(#|xx|comment|$)/))
      {
        my$missing=0;
        my$matching=0;
        my$missingText="";
        #check that references like {0} matches
        foreach my$j (sort keys %{$section})
        {
          if($j ne "\$this.Text" &&
            defined $xmlexists{$j} &&
            defined $section->{$j})
          {
            if (!defined $section->{$j}[$i] ||
              $section->{$j}[$i] eq "")
            {
              $missing++;
              if($verbose>1)
              {
                if ($missingText eq "")
                {
                  $missingText="Info: No def for $code\n";
                }
                $missingText.="\t$j";
                if($verbose>9)
                {
                  $missingText.="\t$xmlexists{$j}[1]"
                }
                $missingText.="\n";
              }
            }
            elsif($section->{$j}[$i] ne $noTranslate)
            {
              $matching++;
              while($xmlexists{$j}[1] =~ /(\{\d+\})/g)
              {
                my$m=$1;
                if ($section->{$j}[$i] !~ /\Q${m}\E/)
                {
                  print STDOUT "Error: Incorrect formatting for $code \"$j\", $m: $section->{$j}[$i] ($xmlexists{$j}[1])\n";
                }
              }
            }
          }
        }
 
        if ($matching==0)
        {
          print "Info: No translation for $code\n";
        }
        else
        {
          if($missing>0)
          {
            print $missingText;
          }
          if($i==1) {$code="";}
          else {$code.=".";}
          (my$out = $sfile) =~ s/.resx$/.${code}resx/;
          if($verbose>0)
          {
            printf "Writing (strings:%d, missing:%d): $out\n", $matching, $missing;
            #print Dumper(@{$res->{"data"}});
          }
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
}

my$out = XMLout($db, KeyAttr => {});
$out =~ s%^<opt>%<?xml version="1.0" encoding="UTF-8" standalone="no" ?>%;
$out =~ s%</opt>$%%;

if($verbose>99)
{
  print Dumper($db);
}
exit 0;

sub ParseCSV
{
  my($csv) = @_;
  $csv =~ s/\r//g;
  $csv .= "\n";
  my@lines = ();
  #$i=5;
  while ($csv =~ s/^((([^\n\"]|\\\")*(?<!\\)\"([^\"]|\\\")*(?<!\\)\")*([^\n\"]|\\\")*)\n//)
  {
    my$line = $1;
    chomp $line;
    if ($line =~ /^\$end/) { $csv=""; last;}
    my@fields = ();
    while ($line =~ s/^(([^,\"]|\\\")*((?<!\\)\"([^\"]|\\\")*(?<!\\)\")*([^,\"]|\\\")*),//)
    {
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

  foreach my$row (@$rows)
  {
    my@row = @$row;

    while (@row && $row[$#row] eq "")
    {
      $#row = $#row - 1;
    }

    if (@row)
    {
      if ($row[0] eq "CountryCode")
      {
        $translations{codes} = \@row;
        next;
      }
      elsif ($row[0] =~ m%^[\w\\/]+\.resx$%)
      {
        @sections=();
        foreach my$i (@row)
        {
          if ($i =~ m%^[\w\\/]+\.resx$%) {
            push @sections, $i;
          }
        }
        next;
      }
      die "No section (resx) defined in data" unless $sections[0];
      die "No country codes defined in data" unless $translations{codes};
      foreach my$d (@sections)
      {
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
  foreach my$row (@$rows)
  {
    if (defined $row->{"ss:Index"})
    {
      $rownum = $row->{"ss:Index"};
    }
    my$title = "";
    my$colnum = 1;
    my@row = ();
    foreach my$cell (@{$row->{Cell}})
    {
      if (defined $cell->{"ss:Index"})
      {
        $colnum = $cell->{"ss:Index"};
      }
      my$data2 = $cell->{Data}[0]{content};
      if (defined $data2)
      {
        chomp $data2;
        $row[$colnum-1] = $data2;
      }
      $colnum += 1;
    }
    $rows[$rownum++] = \@row;
  }
  return \@rows;
}
