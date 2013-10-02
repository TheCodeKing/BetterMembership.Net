param($installPath, $toolsPath, $package, $project)

try {
	$projectPath = split-path $project.FullName -parent
	$files = get-childitem $projectPath *.cs -rec
	if ($files)
	{
		foreach ($file in $files)
		{
			$content = (Get-Content $file.PSPath)
			if ($content -match "//WebSecurity\.InitializeDatabaseConnection")
			{
				$content = ($content -join "`r`n") -replace "//WebSecurity\.InitializeDatabaseConnection", "WebSecurity.InitializeDatabaseConnection"
				Set-Content $file.PSPath $content 
			}
		}
	}


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
		$node.removeAttribute("defaultProvider")
	}
	
    $xml.Save($configPath)
	
} catch {
}