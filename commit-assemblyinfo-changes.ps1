# Copyright (c) 2018 The nanoFramework project contributors
# See LICENSE file in the project root for full license information.

# skip updating assembly info changes if build is a pull-request or not a tag (can't commit when repo is in a tag)
if ($env:appveyor_pull_request_number -or $env:APPVEYOR_REPO_TAG -eq "true")
{
    'Skip committing assembly info changes...' | Write-Host -ForegroundColor White
}
else
{
    # updated assembly info files   
    git add "source\nanoFramework.System.Net.Http\Properties\AssemblyInfo.cs"
    git commit -m "Update assembly info file for v$env:GitVersion_NuGetVersionV2" -m"[version update]"
    git push origin --porcelain -q > $null
    
    'Updated assembly info...' | Write-Host -ForegroundColor White -NoNewline
    'OK' | Write-Host -ForegroundColor Green

    # this assembly does not have native implementation, no updates requried in that repo
}
