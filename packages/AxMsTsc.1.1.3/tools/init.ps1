param($installPath, $toolsPath, $package, $project)

$project.Object.References | Where-Object { $_.Name -eq "Interop.MSTSCLib" } | ForEach-Object { $_.EmbedInteropTypes = $false }
$project.Object.References | Where-Object { $_.Name -eq "AxInterop.MSTSCLib" } | ForEach-Object { $_.EmbedInteropTypes = $false }
