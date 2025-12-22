# Azure Resource Group Creation Script

# Variables
$resourceGroupName = "rg-custne-tf"
$location = "spaincentral"

# Create resource group
New-AzResourceGroup -Name $resourceGroupName -Location $location

Write-Host "Resource group '$resourceGroupName' created in '$location'"
