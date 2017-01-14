[CmdletBinding()]
Param(
    [ValidateSet("Release", "Debug")] 
    [string]   $config = "Release",
    [switch]   $v,
    [Parameter(Position=0,Mandatory=$false)]    
    [string]   $target = "default",
    [Parameter(Position=1,Mandatory=$false,ValueFromRemainingArguments=$true)]
    [string[]] $vargs
)

$Script = "./scripts/build.cake"
$Verbosity = "Normal"
if($v) {
    $Verbosity = "Diagnostic"
}

& ./scripts/build.ps1 -script $Script -target $target -configuration $config -verbosity $Verbosity -experimental -scriptArgs $vargs