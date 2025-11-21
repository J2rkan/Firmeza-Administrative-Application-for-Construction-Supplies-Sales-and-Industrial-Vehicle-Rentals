# Script de inicio rápido para Firmeza API
# Este script ayuda a iniciar la API de forma rápida

Write-Host "🚀 Firmeza API - Inicio Rápido" -ForegroundColor Cyan
Write-Host "================================" -ForegroundColor Cyan
Write-Host ""

# Verificar si .NET está instalado
Write-Host "✓ Verificando .NET SDK..." -ForegroundColor Yellow
try {
    $dotnetVersion = dotnet --version
    Write-Host "  .NET SDK $dotnetVersion encontrado" -ForegroundColor Green
} catch {
    Write-Host "  ✗ .NET SDK no encontrado. Por favor instala .NET 8.0 SDK" -ForegroundColor Red
    exit 1
}

# Verificar si PostgreSQL está corriendo
Write-Host ""
Write-Host "✓ Verificando PostgreSQL..." -ForegroundColor Yellow
$pgRunning = Get-Process postgres -ErrorAction SilentlyContinue
if ($pgRunning) {
    Write-Host "  PostgreSQL está corriendo" -ForegroundColor Green
} else {
    Write-Host "  ⚠ PostgreSQL no parece estar corriendo" -ForegroundColor Yellow
    Write-Host "  Asegúrate de que PostgreSQL esté iniciado o usa Docker:" -ForegroundColor Yellow
    Write-Host "  docker run -d -p 5432:5432 -e POSTGRES_PASSWORD=your_password postgres:15-alpine" -ForegroundColor Cyan
}

# Restaurar paquetes
Write-Host ""
Write-Host "✓ Restaurando paquetes NuGet..." -ForegroundColor Yellow
Set-Location Firmeza.Api
dotnet restore
if ($LASTEXITCODE -eq 0) {
    Write-Host "  Paquetes restaurados correctamente" -ForegroundColor Green
} else {
    Write-Host "  ✗ Error al restaurar paquetes" -ForegroundColor Red
    exit 1
}

# Compilar
Write-Host ""
Write-Host "✓ Compilando proyecto..." -ForegroundColor Yellow
dotnet build
if ($LASTEXITCODE -eq 0) {
    Write-Host "  Compilación exitosa" -ForegroundColor Green
} else {
    Write-Host "  ✗ Error en la compilación" -ForegroundColor Red
    exit 1
}

# Información importante
Write-Host ""
Write-Host "================================" -ForegroundColor Cyan
Write-Host "📋 Información Importante" -ForegroundColor Cyan
Write-Host "================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "1. Configura appsettings.json antes de ejecutar:" -ForegroundColor Yellow
Write-Host "   - Cadena de conexión a PostgreSQL" -ForegroundColor White
Write-Host "   - Credenciales SMTP (Gmail)" -ForegroundColor White
Write-Host ""
Write-Host "2. Usuario administrador por defecto:" -ForegroundColor Yellow
Write-Host "   Email: admin@firmeza.com" -ForegroundColor White
Write-Host "   Password: Admin@123" -ForegroundColor White
Write-Host ""
Write-Host "3. La API estará disponible en:" -ForegroundColor Yellow
Write-Host "   HTTP: http://localhost:5001" -ForegroundColor White
Write-Host "   Swagger: http://localhost:5001" -ForegroundColor White
Write-Host ""

# Preguntar si desea ejecutar
Write-Host "¿Deseas ejecutar la API ahora? (S/N): " -ForegroundColor Cyan -NoNewline
$response = Read-Host

if ($response -eq "S" -or $response -eq "s") {
    Write-Host ""
    Write-Host "🚀 Iniciando Firmeza API..." -ForegroundColor Green
    Write-Host "Presiona Ctrl+C para detener" -ForegroundColor Yellow
    Write-Host ""
    dotnet run
} else {
    Write-Host ""
    Write-Host "Para ejecutar la API manualmente, usa:" -ForegroundColor Yellow
    Write-Host "  cd Firmeza.Api" -ForegroundColor Cyan
    Write-Host "  dotnet run" -ForegroundColor Cyan
    Write-Host ""
}
