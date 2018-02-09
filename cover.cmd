tools\dotCover\dotCover.exe cover /TargetExecutable="C:\Program Files\dotnet\dotnet.exe" /TargetArguments="test" /TargetWorkingDir="test\Aguacongas.Firebase.Test" /Output="coverage\Aguacongas.Firebase.Test.snapshot" /Filters="+:Aguacongas.Firebase"
tools\dotCover\dotCover.exe cover /TargetExecutable="C:\Program Files\dotnet\dotnet.exe" /TargetArguments="test" /TargetWorkingDir="test\Aguacongas.Identity.Firebase.Test" /Output="coverage\Aguacongas.Identity.Firebase.Test.snapshot" /Filters="+:Aguacongas.Identity.Firebase"

tools\dotCover\dotCover.exe merge /Source="coverage\Aguacongas.Identity.Firebase.Test.snapshot;coverage\Aguacongas.Firebase.Test.snapshot" /Output="coverage\coverage.snapshot"
tools\dotCover\dotCover.exe report /Source="coverage\coverage.snapshot" /Output="coverage\coverage.xml" /ReportType="DetailedXML"
tools\dotCover\dotCover.exe report /Source="coverage\coverage.snapshot" /Output="docs\index.html" /ReportType="HTML"

tools\ReportGenerator\ReportGenerator.exe "-reports:coverage\coverage.xml" "-targetdir:docs" "-reporttypes:Badges"