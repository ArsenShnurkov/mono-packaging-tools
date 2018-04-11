#!/bin/bash

source "../settings.sh"

cp test.csproj.orig test.csproj || die
${MONO} --debug ${TOOL} --in=test.csproj --remove-projectreference="ICSharpCode.TextEditor" || die
echo "Success"
