# destroy.ps1 — Borra TODOS los recursos de Azure de este proyecto
# Uso: .\destroy.ps1

Set-Location "$PSScriptRoot\infra"

Write-Host "==> DESTRUYENDO todos los recursos de Azure..." -ForegroundColor Red
terraform destroy -var-file="terraform.tfvars" -var-file="secrets.tfvars" -auto-approve

Write-Host "`n==> Recursos eliminados." -ForegroundColor Green
