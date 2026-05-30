output "api_url" {
  description = "URL publica de la API"
  value       = "https://${azurerm_linux_web_app.api.default_hostname}"
}

output "swagger_url" {
  description = "URL del Swagger UI"
  value       = "https://${azurerm_linux_web_app.api.default_hostname}/swagger"
}

output "mysql_fqdn" {
  description = "FQDN del servidor MySQL"
  value       = azurerm_mysql_flexible_server.mysql.fqdn
}

output "connection_string" {
  description = "Connection string de MySQL (sensible)"
  value       = "Server=${azurerm_mysql_flexible_server.mysql.fqdn};Port=3306;Database=DbPhoneHub;Uid=${var.mysql_admin_login};Pwd=${var.mysql_admin_password};"
  sensitive   = true
}

output "resource_group_name" {
  description = "Nombre del Resource Group creado"
  value       = azurerm_resource_group.rg.name
}
