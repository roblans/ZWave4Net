Param(
    [Parameter(Mandatory=$False)]
    [string]$path=$pwd,
    [Parameter(Mandatory=$False,Position=1)]
    [string]$Revision=$env:BUILD_BUILDID
)
 
function Help {
    "Sets the AssemblyVersion and AssemblyFileVersion of AssemblyInfo.cs files`n"
    ".\SetAssemblyVersion.ps1 -Revision [Revision] -path [SearchPath]`n"
    "   [Revision]     The assembly revision number to set. Revision is the last number in the 4-part version number. E.g. 4 in 1.2.3.4"
    "   [SearchPath]   The path to search for AssemblyInfo files. Defaults to current folder. `n"
}

function Update-SourceVersion
{
    Param ([string]$rev)
    
    foreach ($o in $input) 
    {
        $versionPattern = "([0-9]+)\.([0-9]+)\.([0-9]+)\.([0-9]*)"
        $replacePattern = "`$1.`$2`.`$3.$rev"

        Write-Host "Updating revision of '$($o.FullName)' to $rev"

        $assemblyVersionPattern = "AssemblyVersion\(`"$versionPattern`"\)"
        $fileVersionPattern = "AssemblyFileVersion\(`"$versionPattern`"\)"
        $assemblyVersion = "AssemblyVersion(`"$replacePattern`")";
        $fileVersion = "AssemblyFileVersion(`"$replacePattern`")";
        
        (Get-Content -encoding UTF8 $o.FullName) | ForEach-Object  { 
           % {$_ -replace $assemblyVersionPattern, $assemblyVersion } |
           % {$_ -replace $fileVersionPattern, $fileVersion }
        } | Out-File $o.FullName -encoding UTF8 -force
    }
}
function Update-AllAssemblyInfoFiles ( $rev )
{
    Write-Host "Searching '$path'"   
    Get-ChildItem -Path $path -Recurse "AssemblyInfo.cs" `
    | ? { $_.fullname -notmatch '\\packages\\' } `
    | Update-SourceVersion -rev $rev
}

# validate arguments   
if (($Revision -eq '/?') -or ($Revision -notmatch "[0-9]+")) {
    Help
    exit 1;
}
 
Update-AllAssemblyInfoFiles $Revision