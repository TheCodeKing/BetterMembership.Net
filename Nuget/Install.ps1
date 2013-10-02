param($installPath, $toolsPath, $package, $project)

Function Get-ProjectItem ($project, $name)
{
	if($name.Contains("\"))
	{
		$Item = $name.substring(0,$name.IndexOf('\'))
		$name = $name.substring($name.IndexOf('\')+1)
		try {
			$proj = $project.ProjectItems.Item($Item)
		} catch [Exception] {}

		if ($proj) {
			Get-ProjectItem -project $proj -name $name	
		} else {
			return
		}
	}
	else
	{
		try {
			return $project.ProjectItems.Item($name)
		} catch [Exception] {
			return
		}
	}
}

try {
	$withWebSecurity = $false
	$projectPath = split-path $project.FullName -parent
	$files = get-childitem $projectPath *Controller.cs -rec
	if ($files)
	{
		foreach ($file in $files)
		{
			$content = (Get-Content $file.PSPath)
			if ($content -match "\[InitializeSimpleMembership\]")
			{
				$withWebSecurity = $true
				$content = ($content -join "`r`n")
				$content = $content -replace "\s+\[InitializeSimpleMembership\]", ""
				$content = $content -replace "using $($project.Properties.Item(`"DefaultNamespace`").Value).Filters;`r`n", ""
				Set-Content $file.PSPath $content 
			}
		}
	}

	$project = Get-Project
	Get-ProjectItem –project $project –name "Filters\InitializeSimpleMembershipAttribute.cs" | %{ $_.Remove() }

    $timestamp = (Get-Date).ToString('yyyyMMddHHmmss')
    $projectName = [IO.Path]::GetFileName($project.ProjectName.Trim([IO.PATH]::DirectorySeparatorChar, [IO.PATH]::AltDirectorySeparatorChar))
    $catalogName = "aspnet-$projectName-$timestamp"
    $connectionString ="Data Source=(LocalDb)\v11.0;Initial Catalog=$catalogName;Integrated Security=SSPI;AttachDBFilename=|DataDirectory|\$catalogName.mdf"
    $config = $project.ProjectItems | Where-Object { $_.Name -eq "Web.config" -or $_.Name -eq "App.config"}    
    $configPath = ($config.Properties | Where-Object { $_.Name -eq "FullPath" }).Value
    
    $xml = New-Object System.Xml.XmlDocument
    $xml.Load($configPath)
	
	$node = $xml.SelectSingleNode("/configuration/system.web/roleManager")
	$node.SetAttribute("enabled", "true")
	$node.SetAttribute("defaultProvider", "BetterProvider")

	$node = $xml.SelectSingleNode("/configuration/system.web/profile")
	$node.SetAttribute("enabled", "true")
	$node.SetAttribute("defaultProvider", "BetterProvider")
	
	$node = $xml.SelectSingleNode("/configuration/system.web/membership")
	$node.SetAttribute("defaultProvider", "BetterProvider")
    
    $connectionStrings = $xml.SelectSingleNode("/configuration/connectionStrings")
    if (!$connectionStrings) {
        $connectionStrings = $xml.CreateElement("connectionStrings")
        $xml.configuration.AppendChild($connectionStrings) | Out-Null
    }
    
	$connectionStrings = $xml.SelectSingleNode("/configuration/connectionStrings")
    if ($connectionStrings.SelectNodes("add[@name='DefaultConnection']").count -eq 0) {
        $newConnectionNode = $xml.CreateElement("add")
        $newConnectionNode.SetAttribute("name", 'DefaultConnection')
        $newConnectionNode.SetAttribute("providerName", "System.Data.SqlClient")
        $newConnectionNode.SetAttribute("connectionString", $connectionString)
        $connectionStrings.AppendChild($newConnectionNode) | Out-Null
    }

	if (!$withWebSecurity) {
		$node = $xml.SelectSingleNode("/configuration/entityFramework/contexts/context[contains(@type, 'Models.UsersContext')]")
		$parent = $node.parentNode
		$parent.RemoveChild($node)
		if (!$parent.hasChildnodes) {
			$parent.parentNode.removeChild($parent);
		}
	}
    
    $xml.Save($configPath)
    
} catch [Exception] {
    Write-Error "Unable to update the web.config file at $configPath. Add the following connection string to your config: <add name=`"DefaultConnection`" providerName=`"System.Data.SqlClient`" connectionString=`"$connectionString`" />"
}