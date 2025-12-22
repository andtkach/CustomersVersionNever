# Script to create Azure Storage Account and Container for Terraform Backend
# Prerequisites: Azure CLI installed and authenticated (az login)

param(
    [string]$ResourceGroupName = "rg-custne-tf",
    [string]$StorageAccountName = "tfstate$(Get-Random -Minimum 1000 -Maximum 9999)custne",
    [string]$ContainerName = "tfstate",
    [string]$Location = "spaincentral",
    [string]$Sku = "Standard_LRS"
)

$ErrorActionPreference = "Stop"

Write-Host "======================================" -ForegroundColor Blue
Write-Host "Creating Terraform Backend Storage" -ForegroundColor Blue
Write-Host "======================================" -ForegroundColor Blue

# Check if resource group exists
Write-Host "`nChecking if resource group exists..." -ForegroundColor Blue
try {
    az group show --name $ResourceGroupName | Out-Null
    Write-Host "✓ Resource group '$ResourceGroupName' exists" -ForegroundColor Green
}
catch {
    Write-Host "Resource group '$ResourceGroupName' not found!" -ForegroundColor Red
    exit 1
}

# Create storage account
Write-Host "`nCreating storage account '$StorageAccountName'..." -ForegroundColor Blue
az storage account create `
    --name $StorageAccountName `
    --resource-group $ResourceGroupName `
    --location $Location `
    --sku $Sku `
    --kind StorageV2 `
    --allow-blob-public-access false `
    --min-tls-version TLS1_2

Write-Host "✓ Storage account created" -ForegroundColor Green

# Get storage account key
Write-Host "`nRetrieving storage account key..." -ForegroundColor Blue
$AccountKey = az storage account keys list `
    --resource-group $ResourceGroupName `
    --account-name $StorageAccountName `
    --query '[0].value' `
    --output tsv

Write-Host "✓ Storage account key retrieved" -ForegroundColor Green

# Create blob container
Write-Host "`nCreating blob container '$ContainerName'..." -ForegroundColor Blue
az storage container create `
    --name $ContainerName `
    --account-name $StorageAccountName `
    --account-key $AccountKey

Write-Host "✓ Blob container created" -ForegroundColor Green

# Enable versioning (recommended for Terraform state)
Write-Host "`nEnabling blob versioning..." -ForegroundColor Blue
az storage account blob-service-properties update `
    --account-name $StorageAccountName `
    --resource-group $ResourceGroupName `
    --enable-versioning true

Write-Host "✓ Blob versioning enabled" -ForegroundColor Green

# Output configuration for Terraform
Write-Host "`n======================================" -ForegroundColor Blue
Write-Host "Setup Complete!" -ForegroundColor Green
Write-Host "======================================" -ForegroundColor Blue
Write-Host "`nAdd this backend configuration to your Terraform code:`n"

$BackendConfig = @"
terraform {
  backend "azurerm" {
    resource_group_name  = "$ResourceGroupName"
    storage_account_name = "$StorageAccountName"
    container_name       = "$ContainerName"
    key                  = "terraform.tfstate"
  }
}
"@

Write-Host $BackendConfig -ForegroundColor Yellow

Write-Host "`nOr use these values in your backend config file:" -ForegroundColor Blue
Write-Host "resource_group_name  = `"$ResourceGroupName`""
Write-Host "storage_account_name = `"$StorageAccountName`""
Write-Host "container_name       = `"$ContainerName`""
Write-Host "key                  = `"terraform.tfstate`""

Write-Host "`nStorage Account Key (keep secure):" -ForegroundColor Blue
Write-Host $AccountKey -ForegroundColor Yellow

Write-Host "`nDone!" -ForegroundColor Green
