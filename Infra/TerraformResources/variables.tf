# General Variables
variable "subscription_id" {
  description = "Azure subscription ID"
  type        = string
  default     = null
}

variable "resource_group_name" {
  description = "Name of the resource group"
  type        = string
  default     = "rg-custne-app"
}

variable "location" {
  description = "Azure region for resources"
  type        = string
  default     = "spaincentral"
}

variable "environment" {
  description = "Environment name (dev, staging, prod)"
  type        = string
  default     = "dev"
}

variable "project_name" {
  description = "Project name prefix for resource naming"
  type        = string
  default     = "custne"
}

# SQL Server Variables
variable "sql_admin_username" {
  description = "SQL Server administrator username"
  type        = string
  default     = "sqladmin"
}

variable "sql_admin_password" {
  description = "SQL Server administrator password"
  type        = string
  sensitive   = true
}

variable "sql_databases" {
  description = "List of SQL databases to create"
  type = list(object({
    name     = string
    sku_name = string
  }))
  default = [
    {
      name     = "db1"
      sku_name = "S0"
    },
    {
      name     = "db2"
      sku_name = "S0"
    },
    {
      name     = "db3"
      sku_name = "S0"
    }
  ]
}

# Key Vault Variables
variable "kv_sku_name" {
  description = "Key Vault SKU"
  type        = string
  default     = "standard"
}

# Service Bus Variables
variable "servicebus_sku" {
  description = "Service Bus SKU (Basic, Standard, Premium)"
  type        = string
  default     = "Standard"
}

variable "servicebus_queues" {
  description = "List of Service Bus queues to create"
  type        = list(string)
  default     = ["qu1", "qu2", "qu3"]
}

# Storage Account Variables
variable "storage_account_tier" {
  description = "Storage account tier"
  type        = string
  default     = "Standard"
}

variable "storage_account_replication_type" {
  description = "Storage account replication type"
  type        = string
  default     = "LRS"
}

variable "storage_containers" {
  description = "List of blob containers to create"
  type        = list(string)
  default     = ["ct1", "ct2", "ct3"]
}

# Tags
variable "tags" {
  description = "Tags to apply to all resources"
  type        = map(string)
  default = {
    Environment = "Development"
    ManagedBy   = "Terraform"
    Project     = "Custne"
  }
}
