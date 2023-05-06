$result = 0

if ($isLinux) {
    Get-ChildItem -rec `
    | Where-Object { $_.Name -like "*.Test.csproj" `
         } `
    | ForEach-Object { 
        Set-Location $_.DirectoryName
        dotnet test
    
        if ($LASTEXITCODE -ne 0) {
            $result = $LASTEXITCODE
        }
    }
} else {
    $prNumber = $env:APPVEYOR_PULL_REQUEST_NUMBER
    if ($prNumber) {
        $prArgs = "-d:sonar.pullrequest.key=$prNumber"
    } elseif ($env:APPVEYOR_REPO_BRANCH) {
        $prArgs = "-d:sonar.branch.name=$env:APPVEYOR_REPO_BRANCH"
    }

    dotnet sonarscanner begin /k:aguacongas_Identity.Firebase -o:aguafrommars -d:sonar.host.url=https://sonarcloud.io -d:sonar.login=$env:sonarqube -d:sonar.coverageReportPaths=coverage\SonarQube.xml $prArgs -v:$env:nextversion

    dotnet build -c Release

    Get-ChildItem -rec `
    | Where-Object { $_.Name -like "*.Test.csproj" `
         } `
    | ForEach-Object { 
        &('dotnet') ('test', $_.FullName, '--logger', "trx;LogFileName=$_.trx", '--no-build', '-c', 'Release', '--collect:"XPlat Code Coverage"')    
        if ($LASTEXITCODE -ne 0) {
            $result = $LASTEXITCODE
        }
      }

    $merge = ""
    Get-ChildItem -rec `
    | Where-Object { $_.Name -like "coverage.cobertura.xml" } `
    | ForEach-Object { 
        $path = $_.FullName
        $merge = "$merge;$path"
    }
    Write-Host $merge
    ReportGenerator\tools\net7.0\ReportGenerator.exe "-reports:$merge" "-targetdir:coverage" "-reporttypes:SonarQube"
    
    dotnet sonarscanner end -d:sonar.login=$env:sonarqube
}
exit $result
