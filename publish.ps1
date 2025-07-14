.\clean.ps1
dotnet restore
$fileversion = "$env:SemVer.0" 
Get-ChildItem -Path src -rec `
| Where-Object { $_.Name -like "*.csproj"
     } `
| ForEach-Object { 
    dotnet msbuild $_.FullName -t:Build -p:Configuration=Release -p:OutputPath=..\..\artifacts\build -p:GeneratePackageOnBuild=true -p:Version=$env:NextVersion -p:FileVersion=$fileversion
    if ($LASTEXITCODE -ne 0) {
            throw "build failed" + $d.FullName
    }
  }