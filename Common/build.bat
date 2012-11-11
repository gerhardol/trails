REM Copyright (C) 2011 Gerhard Olsson
REM
REM This library is free software; you can redistribute it and/or
REM modify it under the terms of the GNU Lesser General Public
REM License as published by the Free Software Foundation; either
REM version 2 of the License, or (at your option) any later version.
REM
REM This library is distributed in the hope that it will be useful,
REM but WITHOUT ANY WARRANTY; without even the implied warranty of
REM MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
REM Lesser General Public License for more details.
REM
REM You should have received a copy of the GNU Lesser General Public
REM License along with this library. If not, see <http://www.gnu.org/licenses/>.

REM $(PluginId)
SET guid=%1
REM $(StPluginVersion)
SET StPluginVersion=%2
REM ProjectName)
SET ProjectName=%3
REM $(ProjectDir)
SET ProjectDir=%4
REM $(StPluginPath)
SET StPluginPath=%5
REM $(TargetDir)
SET TargetDir=%6
REM $(ConfigurationType)
SET ConfigurationType=%7

REM Set the plugin GUID in the main PropertyGroup for the plugin
IF "%guid%"=="" GOTO END

REM ST version, for plugin.xml file
SET StVersion=3.1.4415
IF "%StPluginVersion%"=="2" SET StVersion=2.1.3478

REM 7-zip must be configured, hardcoded path used
set sevenzip=%ProgramFiles%\7-zip\7z.exe
set sevenzip64=%ProgramW6432%\7-zip\7z.exe
if EXIST "%sevenzip64%" set sevenzip=%sevenzip64%

REM Plugin version
set PluginVersion=0.1
set perl=C:\Cygwin\bin\perl.exe
IF EXIST "%perl%" GOTO PERL_VERSION
echo "Cannot find %perl%, will hardcode version number %PluginVersion%"
GOTO endversion 

:PERL_VERSION
set cygwin=nodosfilewarning
set tempfile=%temp%\%ProjectName%-stpluginversion.tmp
%perl% -ne "if(/^^\[assembly: AssemblyVersion\(.([\.\d]*)(\.\*)*.\)\]/){print $1;}" %ProjectDir%\Properties\AssemblyInfo.cs > %tempfile%
set /p PluginVersion= < %tempfile%
rem DEL %tempfile%
:endversion

set stPlgFile=%ProjectDir%%ProjectName%-%PluginVersion%.st%StPluginVersion%plugin
IF NOT "%ConfigurationType%"=="Release" set stPlgFile="%ProjectDir%%ProjectName%-%PluginVersion%-%ConfigurationType%.st%StPluginVersion%plugin"
REM To move a .stplugin to common area, create environment variable (or set it below)
REM set stPlgoutdir=g:\Users\go\dev\web

REM Vista+ / XP compatibility
set C_APPDATA=%PROGRAMDATA%
IF "%C_APPDATA%"=="" set C_APPDATA=%ALLUSERSPROFILE%\Application Data
IF EXIST "%C_APPDATA%" GOTO COPY_REL
GOTO END_COPY_REL
:COPY_REL
set StTarget="%C_APPDATA%\%StPluginPath%\Update\%guid%\%ProjectName%"
IF NOT EXIST %StTarget% mkdir %StTarget%

REM XCOPY depreciated in Vista, use for XP compatibility
XCOPY  "%TargetDir%*.*" %StTarget% /I/Y/Q/E <NUL:
:END_COPY_REL

REM generate the plugin.xml file
ECHO ^<?xml version="1.0" encoding="utf-8" ?^> >  "%TargetDir%plugin.xml"
ECHO ^<plugin id="%guid%" minimumCommonVersion="%StVersion%" /^> >> "%TargetDir%plugin.xml"

IF EXIST "%stPlgFile%" DEL "%stPlgFile%"
IF EXIST "%sevenzip%" GOTO ZIP_PACKAGE

ECHO "Cannot find %sevenzip%, will not create plugin package"
GOTO END

:ZIP_PACKAGE
REM Include pdb for now also in release builds - helpful.
REM IF NOT "%ConfigurationType%"=="Release" GOTO DebugPluginPackage

:DebugPluginPackage
REM Create debug package, with pdb
"%sevenzip%" a -r -tzip "%stPlgFile%" "%TargetDir%*" -x!*.st*plugin -x!*.tmp -x!*.locked -x!%ProjectName%.xml
GOTO end

:ReleasePluginPackage
"%sevenzip%" a -r -tzip "%stPlgFile%" "%TargetDir%*" -x!*.st*plugin -x!*.tmp -x!*.locked -x!%ProjectName%.xml -x!*.pdb

IF "%stPlgoutdir%"=="" GOTO END
IF not EXIST "%stPlgoutdir%" GOTO END
COPY "%stPlgFile%" "%stPlgoutdir%"
GOTO end

:END
