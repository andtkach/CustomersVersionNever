# Data source to get current client configuration
data "azurerm_client_config" "current" {}

# Random suffix for globally unique names
resource "random_string" "suffix" {
  length  = 6
  special = false
  upper   = false
}

# Resource Group
resource "azurerm_resource_group" "main" {
  name     = var.resource_group_name
  location = var.location
  tags     = var.tags
}

# Azure Container Registry
resource "azurerm_container_registry" "acr" {
  location                      = azurerm_resource_group.main.location
  name                          = "acr${var.project_name}${var.environment}"
  resource_group_name           = azurerm_resource_group.main.name
  sku                           = "Standard"
  admin_enabled                 = true
  public_network_access_enabled = true
  tags                          = var.tags
}

# Azure Work Analytics Workspace
resource "azurerm_log_analytics_workspace" "law" {
  name                = "law-${var.project_name}-${var.environment}"
  location            = azurerm_resource_group.main.location
  resource_group_name = azurerm_resource_group.main.name
  sku                 = "PerGB2018"
  retention_in_days   = 30
}

# Azure Container App Environment
resource "azurerm_container_app_environment" "appenv" {
  name                       = "appenv-${var.project_name}-${var.environment}"
  location                   = azurerm_resource_group.main.location
  resource_group_name        = azurerm_resource_group.main.name
  log_analytics_workspace_id = azurerm_log_analytics_workspace.law.id
  tags                       = var.tags
}

# Azure Container Apps - Gateway
resource "azurerm_container_app" "apigateway" {
  container_app_environment_id = azurerm_container_app_environment.appenv.id
  name                         = "api-gateway-${var.project_name}-${var.environment}"
  resource_group_name          = azurerm_resource_group.main.name
  revision_mode                = "Multiple"
  template {
    min_replicas = 1
    max_replicas = 3
    container {
      name   = "api-gateway-container-${var.project_name}-${var.environment}"
      cpu    = 0.25
      image  = "mcr.microsoft.com/azuredocs/containerapps-helloworld:latest"
      memory = "0.5Gi"
    }
  }
  ingress {
    allow_insecure_connections = false
    external_enabled           = true
    target_port                = 8080
    traffic_weight {
      percentage      = 100
      label           = "primary"
      latest_revision = true
    }
  }
  tags = var.tags
}

# Azure Container Apps - Web
resource "azurerm_container_app" "web" {
  container_app_environment_id = azurerm_container_app_environment.appenv.id
  name                         = "web-${var.project_name}-${var.environment}"
  resource_group_name          = azurerm_resource_group.main.name
  revision_mode                = "Multiple"
  template {
    min_replicas = 1
    max_replicas = 3
    container {
      name   = "web-container-${var.project_name}-${var.environment}"
      cpu    = 0.25
      image  = "mcr.microsoft.com/azuredocs/containerapps-helloworld:latest"
      memory = "0.5Gi"
    }
  }
  ingress {
    allow_insecure_connections = false
    external_enabled           = true
    target_port                = 80
    traffic_weight {
      percentage      = 100
      label           = "primary"
      latest_revision = true
    }
  }
  tags = var.tags
}


# Azure SQL Server
resource "azurerm_mssql_server" "main" {
  name                         = "sql-${var.project_name}-${var.environment}"
  resource_group_name          = azurerm_resource_group.main.name
  location                     = azurerm_resource_group.main.location
  version                      = "12.0"
  administrator_login          = var.sql_admin_username
  administrator_login_password = var.sql_admin_password
  minimum_tls_version          = "1.2"

  azuread_administrator {
    login_username = "AzureAD Admin"
    object_id      = data.azurerm_client_config.current.object_id
  }

  tags = var.tags
}

# SQL Server Firewall Rule - Allow Azure Services
resource "azurerm_mssql_firewall_rule" "allow_azure_services" {
  name             = "AllowAzureServices"
  server_id        = azurerm_mssql_server.main.id
  start_ip_address = "0.0.0.0"
  end_ip_address   = "0.0.0.0"
}

# SQL Databases
resource "azurerm_mssql_database" "databases" {
  for_each = { for db in var.sql_databases : db.name => db }

  name           = each.value.name
  server_id      = azurerm_mssql_server.main.id
  collation      = "SQL_Latin1_General_CP1_CI_AS"
  sku_name       = each.value.sku_name
  zone_redundant = false
  license_type   = "LicenseIncluded"
  max_size_gb    = 2
  enclave_type   = "Default"

  lifecycle {
    prevent_destroy = false
  }

  tags = var.tags
}

# Key Vault
resource "azurerm_key_vault" "main" {
  name                       = "kv-${var.project_name}-${var.environment}"
  location                   = azurerm_resource_group.main.location
  resource_group_name        = azurerm_resource_group.main.name
  tenant_id                  = data.azurerm_client_config.current.tenant_id
  sku_name                   = var.kv_sku_name
  soft_delete_retention_days = 7
  purge_protection_enabled   = false

  network_acls {
    default_action = "Allow"
    bypass         = "AzureServices"
  }

  tags = var.tags
}

# Key Vault Access Policy for current user/service principal
resource "azurerm_key_vault_access_policy" "deployer" {
  key_vault_id = azurerm_key_vault.main.id
  tenant_id    = data.azurerm_client_config.current.tenant_id
  object_id    = data.azurerm_client_config.current.object_id

  secret_permissions = [
    "Get",
    "List",
    "Set",
    "Delete",
    "Recover",
    "Backup",
    "Restore",
    "Purge"
  ]

  key_permissions = [
    "Get",
    "List",
    "Create",
    "Delete",
    "Recover",
    "Backup",
    "Restore",
    "Purge"
  ]

  certificate_permissions = [
    "Get",
    "List",
    "Create",
    "Delete",
    "Recover",
    "Backup",
    "Restore",
    "Purge"
  ]
}

# Store SQL Server connection string in Key Vault
resource "azurerm_key_vault_secret" "sql_connection_string" {
  name         = "sql-connection-string"
  value        = "Server=tcp:${azurerm_mssql_server.main.fully_qualified_domain_name},1433;Initial Catalog=${var.sql_databases[0].name};Persist Security Info=False;User ID=${var.sql_admin_username};Password=${var.sql_admin_password};MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"
  key_vault_id = azurerm_key_vault.main.id

  depends_on = [azurerm_key_vault_access_policy.deployer]
}

# Store SQL Server admin password in Key Vault
resource "azurerm_key_vault_secret" "sql_admin_password" {
  name         = "sql-admin-password"
  value        = var.sql_admin_password
  key_vault_id = azurerm_key_vault.main.id

  depends_on = [azurerm_key_vault_access_policy.deployer]
}

# Service Bus Namespace
resource "azurerm_servicebus_namespace" "main" {
  name                = "sb-${var.project_name}-${var.environment}"
  location            = azurerm_resource_group.main.location
  resource_group_name = azurerm_resource_group.main.name
  sku                 = var.servicebus_sku
  capacity            = var.servicebus_sku == "Premium" ? 1 : 0

  tags = var.tags
}

# Service Bus Queues
resource "azurerm_servicebus_queue" "queues" {
  for_each = toset(var.servicebus_queues)

  name         = each.value
  namespace_id = azurerm_servicebus_namespace.main.id

  max_size_in_megabytes = var.servicebus_sku == "Premium" ? 81920 : 1024
}

# Service Bus Authorization Rule - Reference the default rule created by Azure
data "azurerm_servicebus_namespace_authorization_rule" "main" {
  name         = "RootManageSharedAccessKey"
  namespace_id = azurerm_servicebus_namespace.main.id
}

# Store Service Bus connection string in Key Vault
resource "azurerm_key_vault_secret" "servicebus_connection_string" {
  name         = "servicebus-connection-string"
  value        = data.azurerm_servicebus_namespace_authorization_rule.main.primary_connection_string
  key_vault_id = azurerm_key_vault.main.id

  depends_on = [azurerm_key_vault_access_policy.deployer]
}

# Storage Account for files
resource "azurerm_storage_account" "main" {
  name                     = "st${var.project_name}${var.environment}"
  resource_group_name      = azurerm_resource_group.main.name
  location                 = azurerm_resource_group.main.location
  account_tier             = var.storage_account_tier
  account_replication_type = var.storage_account_replication_type
  min_tls_version          = "TLS1_2"

  blob_properties {
    versioning_enabled = true

    delete_retention_policy {
      days = 7
    }

    container_delete_retention_policy {
      days = 7
    }
  }

  tags = var.tags
}

# Storage Containers
resource "azurerm_storage_container" "containers" {
  for_each = toset(var.storage_containers)

  name                  = each.value
  storage_account_id    = azurerm_storage_account.main.id
  container_access_type = "private"
}

# Store Storage Account connection string in Key Vault
resource "azurerm_key_vault_secret" "storage_connection_string" {
  name         = "storage-connection-string"
  value        = azurerm_storage_account.main.primary_connection_string
  key_vault_id = azurerm_key_vault.main.id

  depends_on = [azurerm_key_vault_access_policy.deployer]
}
