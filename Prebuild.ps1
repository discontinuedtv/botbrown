Param (
	$ProjectDir
)

Write-Host $ProjectDir

Push-Location
cd $ProjectDir\www\
npm run build
pop-location