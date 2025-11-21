# Script de Configuración Rápida de PostgreSQL
# Autor: Firmeza Team
# Fecha: 20/11/2025

Write-Host "==================================================" -ForegroundColor Cyan
Write-Host "   Configuración de PostgreSQL para Firmeza-app" -ForegroundColor Cyan
Write-Host "==================================================" -ForegroundColor Cyan
Write-Host ""

# Verificar si PostgreSQL está instalado
Write-Host "Paso 1: Verificando instalación de PostgreSQL..." -ForegroundColor Yellow

$postgresPath = "C:\Program Files\PostgreSQL"
$psqlExe = $null

# Buscar psql.exe en las versiones instaladas
if (Test-Path $postgresPath) {
    $versions = Get-ChildItem $postgresPath -Directory | Sort-Object Name -Descending
    foreach ($version in $versions) {
        $testPath = Join-Path $version.FullName "bin\psql.exe"
        if (Test-Path $testPath) {
            $psqlExe = $testPath
            Write-Host "✓ PostgreSQL encontrado en: $($version.FullName)" -ForegroundColor Green
            break
        }
    }
}

if ($null -eq $psqlExe) {
    Write-Host "✗ PostgreSQL no está instalado" -ForegroundColor Red
    Write-Host ""
    Write-Host "Por favor elige una opción:" -ForegroundColor Yellow
    Write-Host "1. Instalar PostgreSQL localmente (recomendado)"
    Write-Host "   Descarga desde: https://www.postgresql.org/download/windows/"
    Write-Host ""
    Write-Host "2. Usar Docker (si tienes Docker instalado)"
    Write-Host "   docker run --name firmeza-postgres -e POSTGRES_PASSWORD=12345 -e POSTGRES_DB=FirmezaDB -p 5432:5432 -d postgres:16"
    Write-Host ""
    Write-Host "3. Usar base de datos en la nube (ElephantSQL - gratis)"
    Write-Host "   https://www.elephantsql.com/"
    Write-Host ""
    Read-Host "Presiona Enter para salir"
    exit
}

# Verificar si dotnet-ef está instalado
Write-Host ""
Write-Host "Paso 2: Verificando Entity Framework Core Tools..." -ForegroundColor Yellow

try {
    $efVersion = dotnet ef --version 2>&1
    if ($LASTEXITCODE -eq 0) {
        Write-Host "✓ dotnet-ef está instalado: $efVersion" -ForegroundColor Green
    }
    else {
        Write-Host "! Instalando dotnet-ef..." -ForegroundColor Yellow
        dotnet tool install --global dotnet-ef
        if ($LASTEXITCODE -eq 0) {
            Write-Host "✓ dotnet-ef instalado correctamente" -ForegroundColor Green
        }
    }
}
catch {
    Write-Host "! Instalando dotnet-ef..." -ForegroundColor Yellow
    dotnet tool install --global dotnet-ef
}

# Solicitar contraseña de PostgreSQL
Write-Host ""
Write-Host "Paso 3: Configuración de la base de datos" -ForegroundColor Yellow
$dbPassword = Read-Host "Ingresa la contraseña del usuario 'postgres' (configurada: 12345)" 

if ([string]::IsNullOrWhiteSpace($dbPassword)) {
    $dbPassword = "12345"
    Write-Host "Usando contraseña por defecto: 12345" -ForegroundColor Gray
}

# Actualizar appsettings.json
Write-Host ""
Write-Host "Paso 4: Actualizando archivo de configuración..." -ForegroundColor Yellow
$appSettingsPath = ".\Firmeza.Admin\appsettings.json"
$connectionString = "Host=localhost; Database=FirmezaDB; Username=postgres; Password=$dbPassword"

try {
    # Leer el contenido del archivo JSON, eliminando comentarios si los hubiera
    $jsonContent = Get-Content $appSettingsPath -Raw
    $jsonWithoutComments = $jsonContent | Out-String | ForEach-Object { [System.Text.RegularExpressions.Regex]::Replace($_, "(?<![:""]\s*)//.*|/\*[\s\S]*?\*/", "") }
    $appSettingsContent = $jsonWithoutComments | ConvertFrom-Json

    $appSettingsContent.ConnectionStrings.DefaultConnection = $connectionString
    $appSettingsContent | ConvertTo-Json -Depth 10 | Set-Content $appSettingsPath -Encoding UTF8
    Write-Host "✓ appsettings.json actualizado correctamente" -ForegroundColor Green
}
catch {
    Write-Host "✗ Error al actualizar appsettings.json" -ForegroundColor Red
    Write-Host $_.Exception.Message -ForegroundColor Red
    Read-Host "Presiona Enter para salir"
    exit
}


# Crear script SQL temporal para crear la base de datos
$sqlScript = @'
DO $$
BEGIN
   IF NOT EXISTS (SELECT FROM pg_database WHERE datname = 'FirmezaDB') THEN
      CREATE DATABASE "FirmezaDB";
   END IF;
END $$;
'@

$sqlFile = Join-Path $env:TEMP "create_firmeza_db.sql"
$sqlScript | Out-File -FilePath $sqlFile -Encoding UTF8

Write-Host ""
Write-Host "Paso 5: Creando base de datos FirmezaDB..." -ForegroundColor Yellow

# Intentar crear la base de datos
$env:PGPASSWORD = $dbPassword
$output = & $psqlExe -U postgres -h localhost -p 5432 -f $sqlFile 2>&1

if ($LASTEXITCODE -eq 0 -or $output -like "*already exists*") {
    Write-Host "✓ Base de datos FirmezaDB lista" -ForegroundColor Green
}
else {
    Write-Host "✗ Error al crear la base de datos:" -ForegroundColor Red
    Write-Host $output -ForegroundColor Red
    Write-Host ""
    Write-Host "Verifica que:" -ForegroundColor Yellow
    Write-Host "- PostgreSQL esté corriendo (Servicios de Windows)" -ForegroundColor Yellow
    Write-Host "- La contraseña sea correcta" -ForegroundColor Yellow
    Write-Host "- El puerto 5432 esté disponible" -ForegroundColor Yellow
    Remove-Item $sqlFile -Force
    Read-Host "Presiona Enter para salir"
    exit
}

Remove-Item $sqlFile -Force

# Aplicar migraciones
Write-Host ""
Write-Host "Paso 6: Aplicando migraciones a la base de datos..." -ForegroundColor Yellow

try {
    $migrationsOutput = dotnet ef database update --project Firmeza.Infrastructure --startup-project Firmeza.Admin 2>&1
    
    if ($LASTEXITCODE -eq 0) {
        Write-Host "✓ Migraciones aplicadas correctamente" -ForegroundColor Green
        Write-Host ""
        Write-Host "==================================================" -ForegroundColor Green
        Write-Host "   CONFIGURACIÓN COMPLETADA EXITOSAMENTE" -ForegroundColor Green
        Write-Host "==================================================" -ForegroundColor Green
        Write-Host ""
        Write-Host "La base de datos está lista para usar." -ForegroundColor Cyan
        Write-Host ""
        Write-Host "Próximos pasos:" -ForegroundColor Yellow
        Write-Host "1. Ejecutar la aplicación: cd .\Firmeza.Admin; dotnet run" -ForegroundColor White
        Write-Host "2. Navegar a: https://localhost:5001" -ForegroundColor White
        Write-Host "3. Registrar un usuario administrador" -ForegroundColor White
        Write-Host "4. Crear algunos productos" -ForegroundColor White
        Write-Host "5. Probar la importación de Excel" -ForegroundColor White
        Write-Host ""
    }
    else {
        Write-Host "✗ Error al aplicar migraciones:" -ForegroundColor Red
        Write-Host $migrationsOutput -ForegroundColor Red
        Write-Host ""
        Write-Host "Intenta ejecutar manualmente:" -ForegroundColor Yellow
        Write-Host "dotnet ef database update --project Firmeza.Infrastructure --startup-project Firmeza.Admin" -ForegroundColor White
    }
} catch {
    Write-Host "✗ Ocurrió un error inesperado al ejecutar las migraciones." -ForegroundColor Red
    Write-Host $_.Exception.Message -ForegroundColor Red
}

Write-Host ""
Read-Host "Presiona Enter para salir"
