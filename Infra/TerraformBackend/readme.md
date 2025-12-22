## connect to azure
az login
az logout
az account clear
az account show
az login --tenant 75690c9b-997f-40ff-bd22-89c222a03127
az login --use-device-code

## create resource group for terraform backend
.\01-create-resource-group.ps1

## create terraform backend
# Run with default parameters
.\02-create-terraform-backend.ps1

# Or customize parameters
.\02-create-terraform-backend.ps1 `
    -ResourceGroupName "rg-custne-tf" `
    -StorageAccountName "tfstate2233custne" `
    -ContainerName "tfstate" `
    -Location "spaincentral" `
    -Sku "Standard_LRS"


#output

Add this backend configuration to your Terraform code:

terraform {
  backend "azurerm" {
    resource_group_name  = "rg-custne-tf"
    storage_account_name = "tfstate3189custne"
    container_name       = "tfstate"
    key                  = "terraform.tfstate"
  }
}

Or use these values in your backend config file:
resource_group_name  = "rg-custne-tf"
storage_account_name = "tfstate3189custne"
container_name       = "tfstate"
key                  = "terraform.tfstate"

Storage Account Key (keep secure):

Done!

Create a `backend.tf` file in your Terraform project:

```hcl
terraform {
  backend "azurerm" {
    resource_group_name  = "my-terraform-rg"
    storage_account_name = "tfstate1234"
    container_name       = "tfstate"
    key                  = "terraform.tfstate"
  }
}
```



Then run:
```bash
terraform init
```

# delete resource group for terraform backend
03-delete-resource-group.ps1

