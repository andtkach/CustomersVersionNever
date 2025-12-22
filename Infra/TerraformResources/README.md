# Custne Application Infrastructure

This Terraform configuration creates the Azure infrastructure for the Custne application.

## Architecture Overview

This configuration deploys the following Azure resources:

- **Resource Group**: `rg-custne-app`
- **Azure SQL Server** with 3 databases:
  - `db1` - Main application database
  - `db2` - Application logs database
  - `db3` - Analytics database
- **Azure Key Vault** - Secure storage for secrets and connection strings
- **Azure Service Bus** - Message queue service with 3 queues:
  - `qu1` - Order processing queue
  - `qu2` - Notification queue
  - `qu3` - Background job queue
- **Azure Storage Account** - File storage with 3 containers:
  - `ct1` - Document storage
  - `ct2` - Image storage
  - `ct3` - Data export storage

## Prerequisites

1. Azure CLI installed and authenticated (`az login`)
2. Terraform installed (>= 1.0)
3. Terraform backend already created (using the TerraformBackend scripts)
4. Appropriate Azure permissions to create resources

## Quick Start

### 1. Initialize Terraform

```powershell
cd C:\temp\TerraformResources
terraform init
```

This will:
- Download required providers
- Initialize the remote backend in Azure Storage

### 2. Set SQL Admin Password

**IMPORTANT**: Change the SQL admin password before deploying!

Option A - Using command line (recommended):
```powershell
terraform plan -var="sql_admin_password=YourSecurePassword123!"
terraform apply -var="sql_admin_password=YourSecurePassword123!"
```

Option B - Using environment variable:
```powershell
$env:TF_VAR_sql_admin_password = "YourSecurePassword123!"
terraform plan
terraform apply
```

Option C - Edit terraform.tfvars (not recommended - don't commit):
```hcl
sql_admin_password = "YourSecurePassword123!"
```

### 3. Review the Plan

```powershell
terraform plan
```

Review the resources that will be created.

### 4. Apply the Configuration

```powershell
terraform apply
```

Type `yes` when prompted to confirm.

## Configuration

### Main Configuration Files

- `backend.tf` - Terraform backend configuration (Azure Storage)
- `main.tf` - Main resource definitions
- `variables.tf` - Input variable declarations
- `outputs.tf` - Output value definitions
- `terraform.tfvars` - Variable values (customize this)

### Customization

Edit `terraform.tfvars` to customize:

- Resource names and locations
- SQL database SKUs (S0, S1, S2, etc.)
- Service Bus tier (Basic, Standard, Premium)
- Storage account replication type
- Queue and container names
- Tags

## Resource Naming Convention

Resources follow this naming pattern:
- Resource Group: `rg-{project}-{resource}`
- SQL Server: `sql-{project}-{env}-{random}`
- Key Vault: `kv-{project}-{env}-{random}`
- Service Bus: `sb-{project}-{env}-{random}`
- Storage Account: `st{project}{env}{random}`

The random suffix ensures globally unique names.

## Security Features

### SQL Server
- Minimum TLS version 1.2
- Azure AD authentication enabled
- Firewall configured to allow Azure services

### Key Vault
- Soft delete enabled (7-day retention)
- RBAC authorization enabled
- Stores all connection strings securely:
  - `sql-connection-string`
  - `sql-admin-password`
  - `servicebus-connection-string`
  - `storage-connection-string`

### Storage Account
- Minimum TLS version 1.2
- Blob versioning enabled
- 7-day soft delete retention
- Private container access

## Accessing Secrets

After deployment, retrieve secrets from Key Vault:

```powershell
# Get SQL connection string
az keyvault secret show --vault-name <key-vault-name> --name sql-connection-string --query value -o tsv

# Get Service Bus connection string
az keyvault secret show --vault-name <key-vault-name> --name servicebus-connection-string --query value -o tsv

# Get Storage connection string
az keyvault secret show --vault-name <key-vault-name> --name storage-connection-string --query value -o tsv
```

Or use the Key Vault name from outputs:
```powershell
terraform output key_vault_name
```

## Outputs

After successful deployment, Terraform will output:

- Resource group name and location
- SQL Server name and FQDN
- Database names
- Key Vault name and URI
- Service Bus namespace and queues
- Storage account and containers

View outputs anytime:
```powershell
terraform output
terraform output -json
```

## Cost Optimization

Current configuration uses:
- **SQL Databases**: 3x S0 tier (~$15/month each)
- **Service Bus**: Standard tier (~$10/month base + usage)
- **Storage Account**: Standard LRS (minimal cost, usage-based)
- **Key Vault**: Standard tier (minimal cost, usage-based)

**Estimated monthly cost**: ~$60-80 USD (varies by region and usage)

To reduce costs for development:
- Use Basic tier for SQL databases: `sku_name = "Basic"`
- Use Basic tier for Service Bus: `servicebus_sku = "Basic"`

## Common Commands

```powershell
# Initialize Terraform
terraform init

# Validate configuration
terraform validate

# Format configuration files
terraform fmt

# Plan changes
terraform plan

# Apply changes
terraform apply

# Show current state
terraform show

# List all resources
terraform state list

# Destroy all resources (CAUTION!)
terraform destroy
```

## Troubleshooting

### "Name already exists"
Storage accounts and Key Vaults must be globally unique. The random suffix should handle this, but you can:
- Run `terraform destroy` and `terraform apply` again to get a new random suffix
- Manually set a unique suffix in variables

### "Insufficient permissions"
Ensure your Azure account has:
- Contributor role on the subscription or resource group
- Ability to create resources

### "Backend initialization failed"
Verify:
- Backend storage account exists
- You're authenticated to Azure (`az login`)
- backend.txt values match your backend storage

## Maintenance

### Updating Resources
1. Modify `terraform.tfvars` or `variables.tf`
2. Run `terraform plan` to review changes
3. Run `terraform apply` to apply changes

### Adding New Resources
1. Add resource definitions to `main.tf`
2. Add variables to `variables.tf` if needed
3. Add outputs to `outputs.tf` if needed
4. Update this README

### State Management
- State is stored in Azure Storage (configured in backend.tf)
- State is locked during operations to prevent conflicts
- Never edit state files manually


## Github
1. secrets.ACR_LOGIN_SERVER = Azure container registry Login server (acrcustnedev.azurecr.io)
2. secrets.ACR_USERNAME = Username (acrcustnedev)
3. secrets.ACR_PASSWORD = password ()
4. secrets.AZURE_CREDENTIALS (in cmd run: az ad sp create-for-rbac --name github-auth --role contributor --scopes /subscriptions/206e3290-051e-4353-964b-c224e47ffdc1/resourceGroups/rg-custne-app --json-auth --out json) 
5. secrets.DB_CONN = "Server=tcp:sql-custne-dev-njrwo7.database.windows.net,1433;Initial Catalog=AuthDb;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;User=sqladmin;Password=ChangeMe123!Strong;"



