# Resource Group Outputs
output "resource_group_name" {
  description = "Name of the resource group"
  value       = azurerm_resource_group.main.name
}

output "resource_group_location" {
  description = "Location of the resource group"
  value       = azurerm_resource_group.main.location
}

# ACR Outputs
output "acr_name" {
  description = "Name of the Azure Container Registry"
  value       = azurerm_container_registry.acr.name
}

output "acr_login_server" {
  description = "Login server URL of the Azure Container Registry"
  value       = azurerm_container_registry.acr.login_server
}

output "acr_admin_username" {
  description = "Admin username of the Azure Container Registry"
  value       = azurerm_container_registry.acr.admin_username
  sensitive   = true
}

output "acr_admin_password" {
  description = "Admin password of the Azure Container Registry"
  value       = azurerm_container_registry.acr.admin_password
  sensitive   = true
}

output "acr_id" {
  description = "ID of the Azure Container Registry"
  value       = azurerm_container_registry.acr.id
}

# Log Analytics Workspace Outputs
output "log_analytics_workspace_name" {
  description = "Name of the Log Analytics Workspace"
  value       = azurerm_log_analytics_workspace.law.name
}

output "log_analytics_workspace_id" {
  description = "ID of the Log Analytics Workspace"
  value       = azurerm_log_analytics_workspace.law.id
}

output "log_analytics_workspace_workspace_id" {
  description = "Workspace ID of the Log Analytics Workspace"
  value       = azurerm_log_analytics_workspace.law.workspace_id
}

output "log_analytics_workspace_primary_shared_key" {
  description = "Primary shared key of the Log Analytics Workspace"
  value       = azurerm_log_analytics_workspace.law.primary_shared_key
  sensitive   = true
}

# Container App Environment Outputs
output "container_app_environment_name" {
  description = "Name of the Container App Environment"
  value       = azurerm_container_app_environment.appenv.name
}

output "container_app_environment_id" {
  description = "ID of the Container App Environment"
  value       = azurerm_container_app_environment.appenv.id
}

output "container_app_environment_default_domain" {
  description = "Default domain of the Container App Environment"
  value       = azurerm_container_app_environment.appenv.default_domain
}

# Container App Outputs
output "container_apigateway_name" {
  description = "Name of the Container App (api gateway)"
  value       = azurerm_container_app.apigateway.name
}

output "container_apigateway_id" {
  description = "ID of the Container App (api gateway)"
  value       = azurerm_container_app.apigateway.id
}

output "container_apigateway_fqdn" {
  description = "FQDN of the Container App (api gateway)"
  value       = azurerm_container_app.apigateway.latest_revision_fqdn
}

output "container_apigateway_url" {
  description = "URL of the Container App (api gateway)"
  value       = "https://${azurerm_container_app.apigateway.latest_revision_fqdn}"
}

# Container App (web) Outputs
output "container_web_name" {
  description = "Name of the Container App (web)"
  value       = azurerm_container_app.web.name
}

output "container_web_id" {
  description = "ID of the Container App (web)"
  value       = azurerm_container_app.web.id
}

output "container_web_fqdn" {
  description = "FQDN of the Container App (web)"
  value       = azurerm_container_app.web.latest_revision_fqdn
}

output "container_web_url" {
  description = "URL of the Container App (web)"
  value       = "https://${azurerm_container_app.web.latest_revision_fqdn}"
}

# SQL Server Outputs
output "sql_server_name" {
  description = "Name of the SQL Server"
  value       = azurerm_mssql_server.main.name
}

output "sql_server_fqdn" {
  description = "Fully qualified domain name of the SQL Server"
  value       = azurerm_mssql_server.main.fully_qualified_domain_name
}

output "sql_database_names" {
  description = "Names of the SQL databases"
  value       = [for db in azurerm_mssql_database.databases : db.name]
}

# Key Vault Outputs
output "key_vault_name" {
  description = "Name of the Key Vault"
  value       = azurerm_key_vault.main.name
}

output "key_vault_uri" {
  description = "URI of the Key Vault"
  value       = azurerm_key_vault.main.vault_uri
}

output "key_vault_id" {
  description = "ID of the Key Vault"
  value       = azurerm_key_vault.main.id
}

# Service Bus Outputs
output "servicebus_namespace_name" {
  description = "Name of the Service Bus namespace"
  value       = azurerm_servicebus_namespace.main.name
}

output "servicebus_namespace_endpoint" {
  description = "Endpoint of the Service Bus namespace"
  value       = azurerm_servicebus_namespace.main.endpoint
}

output "servicebus_queue_names" {
  description = "Names of the Service Bus queues"
  value       = [for queue in azurerm_servicebus_queue.queues : queue.name]
}

# Storage Account Outputs
output "storage_account_name" {
  description = "Name of the Storage Account"
  value       = azurerm_storage_account.main.name
}

output "storage_account_primary_endpoint" {
  description = "Primary blob endpoint of the Storage Account"
  value       = azurerm_storage_account.main.primary_blob_endpoint
}

output "storage_container_names" {
  description = "Names of the storage containers"
  value       = [for container in azurerm_storage_container.containers : container.name]
}

# Summary Output
output "deployment_summary" {
  description = "Summary of deployed resources"
  value = {
    resource_group           = azurerm_resource_group.main.name
    acr_name                 = azurerm_container_registry.acr.name
    acr_login_server         = azurerm_container_registry.acr.login_server
    log_analytics            = azurerm_log_analytics_workspace.law.name
    container_app_env        = azurerm_container_app_environment.appenv.name
    container_apigateway_url = "https://${azurerm_container_app.apigateway.latest_revision_fqdn}"
    container_web_url        = "https://${azurerm_container_app.web.latest_revision_fqdn}"
    sql_server               = azurerm_mssql_server.main.name
    databases                = [for db in azurerm_mssql_database.databases : db.name]
    key_vault                = azurerm_key_vault.main.name
    service_bus              = azurerm_servicebus_namespace.main.name
    service_bus_queues       = [for queue in azurerm_servicebus_queue.queues : queue.name]
    storage_account          = azurerm_storage_account.main.name
    storage_containers       = [for container in azurerm_storage_container.containers : container.name]
  }
}
