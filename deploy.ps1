# deploy.ps1 — Crea/actualiza toda la infraestructura en Azure
# Uso: .\deploy.ps1

Set-Location "$PSScriptRoot\infra"

Write-Host "==> terraform init" -ForegroundColor Cyan
terraform init

Write-Host "`n==> terraform plan" -ForegroundColor Cyan
terraform plan -var-file="terraform.tfvars" -var-file="secrets.tfvars"

Write-Host "`n==> terraform apply" -ForegroundColor Cyan
terraform apply -var-file="terraform.tfvars" -var-file="secrets.tfvars" -auto-approve

Write-Host "`n==> Outputs:" -ForegroundColor Green
terraform output api_url
terraform output swagger_url
terraform output mysql_fqdn
