#!/bin/bash

die() {
	echo "error"
	exit 1
}

MONO=/usr/bin/mono
TOOL=/usr/share/mono-packaging-tools-0/mpt-csproj.exe

cp test.csproj.orig test.csproj || die
${MONO} --debug ${TOOL} --in=test.csproj --replace-reference="Mono.Options, Version=4.4.0.0, PublicKeyToken=0738eb9f132ed756" || die
echo "Success"
