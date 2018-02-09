tools\dotCover\dotCover.exe cover /TargetExecutable="C:\Program Files\dotnet\dotnet.exe" /TargetArguments="test" /TargetWorkingDir="test\Aguacongas.Firebase.Test" /Output="test\Aguacongas.Firebase.Test\coverage.snapshot" /Filters="+:Aguacongas.Firebase"
tools\dotCover\dotCover.exe cover /TargetExecutable="C:\Program Files\dotnet\dotnet.exe" /TargetArguments="test" /TargetWorkingDir="test\Aguacongas.Identity.Firebase.Test" /Output="test\Aguacongas.Identity.Firebase.Test\coverage.snapshot" /Filters="+:Aguacongas.Identity.Firebase"

tools\dotCover\dotCover.exe merge /Source="test\Aguacongas.Firebase.Test\coverage.snapshot;test\Aguacongas.Firebase.Test\coverage.snapshot" /Output="test\coverage.snapshot"
tools\dotCover\dotCover.exe report /Source="test\coverage.snapshot" /Output="test\coverage.xml" /ReportType="DetailedXML"
tools\dotCover\dotCover.exe report /Source="test\coverage.snapshot" /Output="docs\coverage.html" /ReportType="HTML"

tools\ReportGenerator\ReportGenerator.exe "-reports:test\coverage.xml" "-targetdir:docs" "-reporttypes:Badges"