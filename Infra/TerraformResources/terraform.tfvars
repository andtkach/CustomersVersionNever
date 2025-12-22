# General Configuration
subscription_id = "206e3290-051e-4353-964b-c224e47ffdc1"

resource_group_name = "rg-custne-app"
location            = "northeurope"
environment         = "dev"
project_name        = "custne"

# SQL Server Configuration
sql_admin_username = "sqladmin"
sql_admin_password = "ChangeMe123!Strong"

# SQL Databases Configuration
sql_databases = [
  {
    name     = "CacheDb"
    sku_name = "S0" # Standard tier, 10 DTUs
  },
  {
    name     = "FrontendDb"
    sku_name = "S0"
  },
  {
    name     = "BackendDb"
    sku_name = "S0"
  },
  {
    name     = "AuthDb"
    sku_name = "S0"
  }
]

# Key Vault Configuration
kv_sku_name = "standard"

# Service Bus Configuration
servicebus_sku = "Standard"
servicebus_queues = [
  "Institutions",
  "Customers",
  "Documents",
  "Addresses"
]

# Storage Account Configuration
storage_account_tier             = "Standard"
storage_account_replication_type = "LRS"
storage_containers = [
  "files",
  "logs"
]

# Tags
tags = {
  Environment = "Development"
  ManagedBy   = "Terraform"
  Project     = "Custne"
  CostCenter  = "IT"
}
