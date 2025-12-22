# Resource Group Outputs
output "resource_group_name" {
  description = "Name of the resource group"
  value       = azurerm_resource_group.main.name
}

output "resource_group_location" {
  description = "Location of the resource group"
  value       = azurerm_resource_group.main.location
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
    resource_group     = azurerm_resource_group.main.name
    sql_server         = azurerm_mssql_server.main.name
    databases          = [for db in azurerm_mssql_database.databases : db.name]
    key_vault          = azurerm_key_vault.main.name
    service_bus        = azurerm_servicebus_namespace.main.name
    service_bus_queues = [for queue in azurerm_servicebus_queue.queues : queue.name]
    storage_account    = azurerm_storage_account.main.name
    storage_containers = [for container in azurerm_storage_container.containers : container.name]
  }
}
