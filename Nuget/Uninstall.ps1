param($installPath, $toolsPath, $package, $project)

try {

	$projectPath = split-path $project.FullName -parent
	$files = get-childitem $projectPath AccountController.cs -rec
	if ($files)
	{
		foreach ($file in $files)
		{
			$content = (Get-Content $file.PSPath)
			if ($content -match "\s+\[Authorize\]")
			{
				$content = ($content -join "`r`n") -replace "\s+\[Authorize\]", "    [Authorize]\r\n    [InitializeSimpleMembership]"
				Set-Content $file.PSPath $content 
			}
		}
	}
	
    $timestamp = (Get-Date).ToString('yyyyMMddHHmmss')
    $projectName = [IO.Path]::GetFileName($project.ProjectName.Trim([IO.PATH]::DirectorySeparatorChar, [IO.PATH]::AltDirectorySeparatorChar))
    $catalogName = "aspnet-$projectName-$timestamp"
    $connectionString ="Data Source=(LocalDb)\v11.0;Initial Catalog=$catalogName;Integrated Security=SSPI;AttachDBFilename=|DataDirectory|\$catalogName.mdf"
    $connectionStringToken = 'Data Source=(LocalDb)\v11.0;'
    $config = $project.ProjectItems | Where-Object { $_.Name -eq "Web.config" }    
    $configPath = ($config.Properties | Where-Object { $_.Name -eq "FullPath" }).Value
    
    $xml = New-Object System.Xml.XmlDocument
    $xml.Load($configPath)
	
	$roleProviderNode = $xml.SelectSingleNode("/configuration/system.web/roleManager/providers/add") | ?{ $_.name -ne "BetterProvider" } | select -first 1
	$node = $xml.SelectSingleNode("/configuration/system.web/roleManager")
	write-host $roleProviderNode.outerXml
	if ($roleProviderNode) {
		$node.SetAttribute("defaultProvider", $roleProviderNode.name)
	} else {	
		$node.removeAttribute("defaultProvider")
		$node.removeAttribute("enabled")
	}

	$profileProviderNode = $xml.SelectSingleNode("/configuration/system.web/profile/providers/add") | ?{ $_.name -ne "BetterProvider" } | select -first 1
	$node = $xml.SelectSingleNode("/configuration/system.web/profile")
	write-host $profileProviderNode.outerXml
	if ($profileProviderNode) {
		$node.SetAttribute("defaultProvider", $profileProviderNode.name)
	} else {	
		$node.removeAttribute("defaultProvider")
		$node.removeAttribute("enabled")
	}

	$membershipProviderNode = $xml.SelectSingleNode("/configuration/system.web/membership/providers/add") | ?{ $_.name -ne "BetterProvider" } | select -first 1
	$node = $xml.SelectSingleNode("/configuration/system.web/membership")
	write-host $membershipProviderNode.outerXml
	if ($membershipProviderNode.name) {
		$node.SetAttribute("defaultProvider", $membershipProviderNode.name)
	} else {
		write-host "remove"
		$node.removeAttribute("defaultProvider")
	}
    
	write-host "Web.config transform"
	
    $xml.Save($configPath)
	
} catch {
}