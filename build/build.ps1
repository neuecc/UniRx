param([hashtable] $set)

Push-Location $PSScriptRoot

. .\Get-MSBuildPath.ps1

if ($set -eq $null)
{
    $set = @{}
}

if ($Script:msbuild -eq $null)
{
    $Script:msbuild = Get-MSBuildPath
}

function Invoke-Build
{
    [CmdletBinding(
        PositionalBinding=$true)]
    Param(
        [Parameter(
            ValueFromPipeline=$true,
            ValueFromPipelineByPropertyName=$true,
            Position=0)]
        [string] $Path
    )

    process {
        ./NuGet.exe restore $Path

        . $Script:msbuild $Path /t:Build /p:Configuration=Release 
    }
}

function Invoke-BuildDependency
{
    [CmdletBinding(
        PositionalBinding=$true)]
    Param(
        [Parameter(
            ValueFromPipeline=$true,
            ValueFromPipelineByPropertyName=$true,
            Position=0)]
        [string] $ProjectName)

    process {
        if ($set.ContainsKey($ProjectName))
        {
            return;
        }

        $set.Add($ProjectName, $false)

        try
        {
            Push-Location $('..\..\' + $ProjectName + '\build')
            & .\build.ps1 $set
        }
        finally
        {
            Pop-Location
        }
    }
}

if (Test-Path dependency.txt)
{
    Get-Content ./dependency.txt | Invoke-BuildDependency
}

Get-Content ./solutions.txt | Invoke-Build

if (Test-Path postbuild.ps1)
{
    . ./postbuild.ps1
}

Pop-Location
