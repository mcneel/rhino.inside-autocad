param($SupportedApplicationOutputPath, $UserTestsPath,$OutputPath)

# Ensure the output directory exists
$directory = Split-Path -Path $OutputPath -Parent
if (-not (Test-Path $directory)) {
    New-Item -ItemType Directory -Path $directory -Force | Out-Null
}

# Read the SupportedApplication JSON output
$supportedAppJson = Get-Content -Path $SupportedApplicationOutputPath -Raw | ConvertFrom-Json

# Read the UserTests JSON
$userTestsJson = Get-Content -Path $UserTestsPath -Raw | ConvertFrom-Json

# Combine both into a single object
$combinedData = @{
    SupportedApplication = $supportedAppJson
    UserTests = @($userTestsJson)
    }

# Convert to JSON and write to output file
$json = $combinedData | ConvertTo-Json -Depth 10 -Compress

$json | Out-File -FilePath $OutputPath -Encoding UTF8