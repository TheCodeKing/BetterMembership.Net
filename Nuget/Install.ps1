param($installPath, $toolsPath, $package, $project)

$projectPath = split-path $project.FullName -parent
$files = get-childitem $projectPath *Controller.cs -rec

foreach ($file in $files)
{
	$content = (Get-Content $file.PSPath)
	if ($content -match "\[InitializeSimpleMembership\]")
	{
		$content = ($content -join "`r`n") -replace "\s+\[InitializeSimpleMembership\]", ""
		Set-Content $file.PSPath $content 
	}
}