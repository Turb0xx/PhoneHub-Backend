variable "subscription_id" {
  description = "Azure Subscription ID"
  type        = string
}

variable "resource_group_name" {
  description = "Nombre del Resource Group"
  type        = string
}

variable "app_location" {
  description = "Region del App Service y Resource Group"
  type        = string
}

variable "mysql_location" {
  description = "Region del servidor MySQL"
  type        = string
}

variable "mysql_server_name" {
  description = "Nombre del servidor MySQL (debe ser globalmente unico en Azure)"
  type        = string
}

variable "mysql_admin_login" {
  description = "Usuario administrador de MySQL"
  type        = string
}

variable "mysql_admin_password" {
  description = "Contrasena del administrador MySQL"
  type        = string
  sensitive   = true
}

variable "app_service_plan_name" {
  description = "Nombre del App Service Plan"
  type        = string
}

variable "app_service_name" {
  description = "Nombre del App Service (debe ser globalmente unico en Azure)"
  type        = string
}

variable "jwt_secret_key" {
  description = "Clave secreta para firmar tokens JWT"
  type        = string
  sensitive   = true
}

variable "jwt_issuer" {
  description = "JWT Issuer"
  type        = string
}

variable "jwt_audience" {
  description = "JWT Audience"
  type        = string
}

variable "password_salt_key" {
  description = "Salt para el hash de contrasenas de usuarios"
  type        = string
  sensitive   = true
}

variable "tags" {
  description = "Tags aplicados a todos los recursos"
  type        = map(string)
  default = {
    Project     = "PhoneHub"
    Environment = "Production"
  }
}
