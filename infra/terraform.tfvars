# ─── Suscripcion Azure ────────────────────────────────────────────────────────
subscription_id = "6e1c64d6-1c37-47c6-adad-c6f335b38c04"

# ─── Infraestructura ──────────────────────────────────────────────────────────
resource_group_name   = "socialmedia-12586444"
app_location          = "brazilsouth"
mysql_location        = "chilecentral"
mysql_server_name     = "socialmedia26-12586444"
mysql_admin_login     = "socialmediauser"
app_service_plan_name = "myFreePlan"
app_service_name      = "socialmedia-api12586444"

# ─── JWT ──────────────────────────────────────────────────────────────────────
jwt_issuer   = "https://api.phonehub.local"
jwt_audience = "https://frontend.phonehub.local"
