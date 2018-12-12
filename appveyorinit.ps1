Install-Product node ''
npm i -g semantic-release @semantic-release/exec
if (test-path ./nextversion.txt)
{
    Remove-Item ./nextversion.txt
}
semantic-release -b $env:APPVEYOR_REPO_BRANCH -d
$nextversion = Get-Content ./nextversion.txt
$tag = $env:GitVersion_PreReleaseTagWithDash
if (![string]::IsNullOrEmpty($env:GitVersion_BuildMetaData))
{
    $builnumber = "$nextversion$tag+$env:GitVersion_BuildMetaData"
}
else 
{
    $builnumber = "$nextversion$tag"
}
Update-AppveyorBuild -version $builnumber
dotnet restore