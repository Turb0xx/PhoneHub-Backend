terraform {
  required_version = ">= 1.5"
  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "~> 4.0"
    }
  }
}

provider "azurerm" {
  features {}
  subscription_id = var.subscription_id
}

# ─── Resource Group ───────────────────────────────────────────────────────────
resource "azurerm_resource_group" "rg" {
  name     = var.resource_group_name
  location = var.app_location
  tags     = var.tags
}

# ─── MySQL Flexible Server ────────────────────────────────────────────────────
resource "azurerm_mysql_flexible_server" "mysql" {
  name                   = var.mysql_server_name
  resource_group_name    = azurerm_resource_group.rg.name
  location               = var.mysql_location
  administrator_login    = var.mysql_admin_login
  administrator_password = var.mysql_admin_password
  sku_name               = "B_Standard_B1ms"
  version                = "8.0.21"

  storage {
    size_gb            = 32
    auto_grow_enabled  = true
    io_scaling_enabled = true
  }

  backup_retention_days = 7
  tags                  = var.tags
}

# Permite que los servicios de Azure (App Service) se conecten al MySQL
resource "azurerm_mysql_flexible_server_firewall_rule" "allow_azure_services" {
  name                = "AllowAzureServices"
  resource_group_name = azurerm_resource_group.rg.name
  server_name         = azurerm_mysql_flexible_server.mysql.name
  start_ip_address    = "0.0.0.0"
  end_ip_address      = "0.0.0.0"
}

# Base de datos
resource "azurerm_mysql_flexible_database" "db" {
  name                = "DbPhoneHub"
  resource_group_name = azurerm_resource_group.rg.name
  server_name         = azurerm_mysql_flexible_server.mysql.name
  charset             = "utf8mb4"
  collation           = "utf8mb4_unicode_ci"
}

# ─── App Service Plan (Free F1) ───────────────────────────────────────────────
resource "azurerm_service_plan" "plan" {
  name                = var.app_service_plan_name
  resource_group_name = azurerm_resource_group.rg.name
  location            = var.app_location
  os_type             = "Linux"
  sku_name            = "F1"
  tags                = var.tags
}

# ─── App Service — .NET 9 API ─────────────────────────────────────────────────
resource "azurerm_linux_web_app" "api" {
  name                = var.app_service_name
  resource_group_name = azurerm_resource_group.rg.name
  location            = var.app_location
  service_plan_id     = azurerm_service_plan.plan.id

  site_config {
    always_on = false # F1 Free no soporta always_on
    application_stack {
      dotnet_version = "9.0"
    }
  }

  app_settings = {
    "ASPNETCORE_ENVIRONMENT"             = "Production"
    "ConnectionStrings__ConnectionMySql" = "Server=${azurerm_mysql_flexible_server.mysql.fqdn};Port=3306;Database=DbPhoneHub;Uid=${var.mysql_admin_login};Pwd=${var.mysql_admin_password};"
    "Authentication__SecretKey"          = var.jwt_secret_key
    "Authentication__Issuer"             = var.jwt_issuer
    "Authentication__Audience"           = var.jwt_audience
    "PasswordOptions__SaltKey"           = var.password_salt_key
    "PasswordOptions__Iterations"        = "1000"
    "WEBSITE_RUN_FROM_PACKAGE"           = "1"
  }

  tags = var.tags
}
