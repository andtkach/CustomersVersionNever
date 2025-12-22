# Azure Resource Group Deletion Script

# Variables
$resourceGroupName = "rg-custne-tf"

# Delete resource group
Remove-AzResourceGroup -Name $resourceGroupName -Force

Write-Host "Resource group '$resourceGroupName' deleted"
