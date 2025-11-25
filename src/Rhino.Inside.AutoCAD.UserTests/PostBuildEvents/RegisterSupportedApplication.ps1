param($Name, $InstallationPath, $Version, $OutputPath)

# Ensure the directory exists
$directory = Split-Path -Path $OutputPath -Parent
if (-not (Test-Path $directory)) {
    New-Item -ItemType Directory -Path $directory -Force | Out-Null
}

$json = @{
    Name = $Name
    InstallationPath = $InstallationPath
    InstalledVersions = @($Version)
    ProjectAzureInfo = @{
        ProjectUri = "https://dev.azure.com/Bimorph-Digital-Engineering"
        ProjectId = "bc886d12-4f2f-45a1-86aa-b06a53bbd0ed"
        AzureConnectionInfo = @{
            TenantId = "d2cbfce5-d5e0-4be1-8c54-e362d6a394bf"
            ClientId = "b4a921fe-359a-4a89-833e-5dc0e9237aad"
            AzureDevOpsScope = "499b84ac-1321-427f-aa17-267ca6975798/.default"
        }
    }
} | ConvertTo-Json -Compress

$json | Out-File -FilePath $OutputPath -Encoding UTF8