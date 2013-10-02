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
				$content = ($content -join "`r`n")
				$content = $content -replace "\s+\[Authorize\]", "`r`n    [Authorize]`r`n    [InitializeSimpleMembership]"
				$content = $content -replace "(?<=using [^;]+;)`r`n`r`n", "`r`n`using $($project.Properties.Item(`"DefaultNamespace`").Value).Filters;`r`n`r`n"
				
				Set-Content $file.PSPath $content 
			}
		}
	}

	$project = Get-Project
	$files = get-childitem $projectPath InitializeSimpleMembershipAttribute.cs -rec
	if ($files) {
		$files | %{ $project.ProjectItems.AddFromFile($_.FullName) }
	}

    $config = $project.ProjectItems | Where-Object { $_.Name -eq "Web.config" -or $_.Name -eq "App.config"}  
    $configPath = ($config.Properties | Where-Object { $_.Name -eq "FullPath" }).Value
    
    $xml = New-Object System.Xml.XmlDocument
    $xml.Load($configPath)
	
	$roleProviderNode = $xml.SelectSingleNode("/configuration/system.web/roleManager/providers/add") | ?{ $_.name -ne "BetterProvider" } | select -first 1
	$node = $xml.SelectSingleNode("/configuration/system.web/roleManager")
	if ($roleProviderNode) {
		$node.SetAttribute("defaultProvider", $roleProviderNode.name)
	} else {	
		$node.removeAttribute("defaultProvider")
		$node.removeAttribute("enabled")
	}

	$profileProviderNode = $xml.SelectSingleNode("/configuration/system.web/profile/providers/add") | ?{ $_.name -ne "BetterProvider" } | select -first 1
	$node = $xml.SelectSingleNode("/configuration/system.web/profile")
	if ($profileProviderNode) {
		$node.SetAttribute("defaultProvider", $profileProviderNode.name)
	} else {	
		$node.removeAttribute("defaultProvider")
		$node.removeAttribute("enabled")
	}

	$membershipProviderNode = $xml.SelectSingleNode("/configuration/system.web/membership/providers/add") | ?{ $_.name -ne "BetterProvider" } | select -first 1
	$node = $xml.SelectSingleNode("/configuration/system.web/membership")
	if ($membershipProviderNode.name) {
		$node.SetAttribute("defaultProvider", $membershipProviderNode.name)
	} else {
		$node.removeAttribute("defaultProvider")
	}
	
    $xml.Save($configPath)
	
} catch [Exception] {}