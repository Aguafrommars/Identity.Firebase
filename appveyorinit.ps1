npm i -g semantic-release @semantic-release/exec @semantic-release/changelog @semantic-release/git @semantic-release/release-notes-generator @semantic-release/commit-analyzer 
if (test-path ./nextversion.txt)
{
    Remove-Item ./nextversion.txt
}
semantic-release -b $env:APPVEYOR_REPO_BRANCH -d
if (test-path ./nextversion.txt)
{
    $nextversion = Get-Content ./nextversion.txt
}
else 
{
    $nextversion = $env:GitVersion_MajorMinorPatch
}

appveyor SetVariable -Name SemVer -Value $nextversion

if (![string]::IsNullOrEmpty($env:GitVersion_PreReleaseLabel))
{
    $nextversion = "$nextversion-$env:GitVersion_PreReleaseLabel$env:GitVersion_CommitsSinceVersionSourcePadded"
}

appveyor UpdateBuild -Version $nextversion
$builnumbersuffix = Get-Date -Format "mmddyyyy-HHmm"
$builnumber = "$builnumber-$builnumbersuffix"

dotnet restore
