$result = 0

$location = Get-Location;

$envar = Get-Childitem env: -Name 

if (-not($envar -contains 'APPVEYOR_PULL_REQUEST_NUMBER'))
{
	if ($isLinux) {
		Get-ChildItem -rec `
		| Where-Object { $_.Name -like "*.IntegrationTest.csproj" `
			-Or $_.Name -like "*.Test.csproj" `
			} `
		| ForEach-Object { 
			Set-Location $_.DirectoryName
			dotnet test
		
			if ($LASTEXITCODE -ne 0) {
				$result = $LASTEXITCODE
			}
		}
	} else {
		Get-ChildItem -rec `
		| Where-Object { $_.Name -like "*.IntegrationTest.csproj" `
			-Or $_.Name -like "*.Test.csproj" `
			} `
		| ForEach-Object { 
			&('dotnet') ('test', $_.FullName, '--logger', "trx;LogFileName=$_.trx", '-c', 'Release', '/p:CollectCoverage=true', '/p:CoverletOutputFormat=cobertura')    
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
		ReportGenerator\tools\net47\ReportGenerator.exe "-reports:$merge" "-targetdir:coverage\docs" "-reporttypes:HtmlInline;Badges"
	}
} else {
	Get-ChildItem -rec `
	| Where-Object { $_.Name -like "*.Test.csproj" } `
	| ForEach-Object { 
		Set-Location $_.DirectoryName
		&('dotnet') ('test', $_.FullName, '--logger', "trx;LogFileName=$_.trx", '-c', 'Release', '/p:CollectCoverage=true', '/p:CoverletOutputFormat=cobertura', '/p:Include=[Aguacongas.*]*')    
	
		if ($LASTEXITCODE -ne 0) {
			$result = $LASTEXITCODE
		}
	}	
}
Set-Location $location;
exit $result
