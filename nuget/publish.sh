#!/bin/bash
nugetApiKey=$(awk 'BEGIN {FS="="} $1 == "nuget_org_api_key" {print $2}' $SECRET_STORE)
export nugetApiKey

rm servicehost*.nupkg
mono ../lib/nuget.exe pack ../src/csfe/csfe.csproj
nuget push csfe.1*.nupkg $nugetApiKey -Source https://www.nuget.org/api/v2/package