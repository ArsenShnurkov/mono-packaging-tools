#!/bin/bash

source "../settings.sh"

cp test.csproj.orig test.csproj || die
${MONO} --debug ${TOOL} --in=test.csproj --replace-reference="NDepend.Path" --package-hintpath="/var/tmp" || die
echo "Success"
