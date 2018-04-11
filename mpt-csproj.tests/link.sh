#!/bin/bash

TESTPATH=`pwd`/../mpt-csproj/bin/Debug

sudo ln -sf ${TESTPATH}/mpt-csproj.exe  /usr/share/mono-packaging-tools-1/mpt-csproj.exe
sudo ln -sf ${TESTPATH}/mpt-core.dll  /usr/share/mono-packaging-tools-1/mpt-core.dll
readlink /usr/share/mono-packaging-tools-1/mpt-csproj.exe
