# 🚀 Guía de Despliegue - Firmeza API

## 📋 Tabla de Contenidos

1. [Despliegue Local](#despliegue-local)
2. [Despliegue con Docker](#despliegue-con-docker)
3. [Despliegue en Azure](#despliegue-en-azure)
4. [Despliegue en AWS](#despliegue-en-aws)
5. [Configuración de Producción](#configuración-de-producción)
6. [Monitoreo y Logs](#monitoreo-y-logs)

---

## 🖥️ Despliegue Local

### Requisitos
- .NET 8.0 SDK
- PostgreSQL 15+
- Cuenta de Gmail (para SMTP)

### Pasos

1. **Clonar el repositorio**
```bash
git clone https://github.com/your-repo/Firmeza-app.git
cd Firmeza-app
```

2. **Configurar Base de Datos**
```bash
# Crear base de datos
psql -U postgres
CREATE DATABASE FirmezaDB;
\q
```

3. **Configurar appsettings.json**
```bash
cd Firmeza.Api
cp appsettings.example.json appsettings.json
# Editar appsettings.json con tus credenciales
```

4. **Aplicar Migraciones**
```bash
dotnet ef database update --project ../Firmeza.Infrastructure
```

5. **Ejecutar la API**
```bash
dotnet run
```

La API estará disponible en: http://localhost:5001

---

## 🐳 Despliegue con Docker

### Opción 1: Docker Compose (Recomendado)

```bash
# En la raíz del proyecto
docker-compose up -d
```

Servicios disponibles:
- **Base de datos**: localhost:5432
- **Admin Panel**: http://localhost:5000
- **API**: http://localhost:5001

### Opción 2: Solo API con Docker

```bash
# Construir imagen
docker build -t firmeza-api -f Firmeza.Api/Dockerfile .

# Ejecutar contenedor
docker run -d \
  -p 5001:8080 \
  -e ConnectionStrings__DefaultConnection="Host=host.docker.internal;Database=FirmezaDB;Username=postgres;Password=your_password" \
  -e Jwt__Key="YourSecretKeyHere" \
  -e Jwt__Issuer="FirmezaApi" \
  -e Jwt__Audience="FirmezaClient" \
  --name firmeza-api \
  firmeza-api
```

### Comandos Útiles

```bash
# Ver logs
docker-compose logs -f api

# Reiniciar servicio
docker-compose restart api

# Detener todo
docker-compose down

# Limpiar volúmenes
docker-compose down -v
```

---

## ☁️ Despliegue en Azure

### Opción 1: Azure App Service

1. **Crear recursos en Azure**
```bash
# Login
az login

# Crear grupo de recursos
az group create --name firmeza-rg --location eastus

# Crear Azure Database for PostgreSQL
az postgres flexible-server create \
  --resource-group firmeza-rg \
  --name firmeza-db \
  --location eastus \
  --admin-user firmezaadmin \
  --admin-password YourSecurePassword123! \
  --sku-name Standard_B1ms \
  --version 15

# Crear App Service Plan
az appservice plan create \
  --name firmeza-plan \
  --resource-group firmeza-rg \
  --sku B1 \
  --is-linux

# Crear Web App
az webapp create \
  --resource-group firmeza-rg \
  --plan firmeza-plan \
  --name firmeza-api \
  --runtime "DOTNETCORE:8.0"
```

2. **Configurar Variables de Entorno**
```bash
az webapp config appsettings set \
  --resource-group firmeza-rg \
  --name firmeza-api \
  --settings \
    ConnectionStrings__DefaultConnection="Host=firmeza-db.postgres.database.azure.com;Database=FirmezaDB;Username=firmezaadmin;Password=YourSecurePassword123!;SslMode=Require" \
    Jwt__Key="YourProductionSecretKey" \
    Jwt__Issuer="FirmezaApi" \
    Jwt__Audience="FirmezaClient" \
    Email__SmtpHost="smtp.gmail.com" \
    Email__SmtpPort="587" \
    Email__SmtpUser="your-email@gmail.com" \
    Email__SmtpPassword="your-app-password"
```

3. **Desplegar desde GitHub**
```bash
az webapp deployment source config \
  --name firmeza-api \
  --resource-group firmeza-rg \
  --repo-url https://github.com/your-repo/Firmeza-app \
  --branch main \
  --manual-integration
```

### Opción 2: Azure Container Instances

```bash
# Construir y pushear imagen
az acr create --resource-group firmeza-rg --name firmezaregistry --sku Basic
az acr build --registry firmezaregistry --image firmeza-api:latest -f Firmeza.Api/Dockerfile .

# Crear container instance
az container create \
  --resource-group firmeza-rg \
  --name firmeza-api \
  --image firmezaregistry.azurecr.io/firmeza-api:latest \
  --dns-name-label firmeza-api \
  --ports 80 \
  --environment-variables \
    ConnectionStrings__DefaultConnection="..." \
    Jwt__Key="..." \
    ASPNETCORE_ENVIRONMENT="Production"
```

---

## 🌐 Despliegue en AWS

### Opción 1: AWS Elastic Beanstalk

1. **Instalar EB CLI**
```bash
pip install awsebcli
```

2. **Inicializar aplicación**
```bash
cd Firmeza.Api
eb init -p "64bit Amazon Linux 2023 v3.1.0 running .NET 8" firmeza-api --region us-east-1
```

3. **Crear entorno**
```bash
eb create firmeza-api-env \
  --database.engine postgres \
  --database.username firmezaadmin \
  --database.password YourSecurePassword123!
```

4. **Configurar variables de entorno**
```bash
eb setenv \
  Jwt__Key="YourProductionSecretKey" \
  Jwt__Issuer="FirmezaApi" \
  Jwt__Audience="FirmezaClient" \
  Email__SmtpHost="smtp.gmail.com" \
  Email__SmtpPort="587"
```

5. **Desplegar**
```bash
eb deploy
```

### Opción 2: AWS ECS (Fargate)

1. **Crear repositorio ECR**
```bash
aws ecr create-repository --repository-name firmeza-api
```

2. **Construir y pushear imagen**
```bash
# Login
aws ecr get-login-password --region us-east-1 | docker login --username AWS --password-stdin YOUR_ACCOUNT_ID.dkr.ecr.us-east-1.amazonaws.com

# Build y push
docker build -t firmeza-api -f Firmeza.Api/Dockerfile .
docker tag firmeza-api:latest YOUR_ACCOUNT_ID.dkr.ecr.us-east-1.amazonaws.com/firmeza-api:latest
docker push YOUR_ACCOUNT_ID.dkr.ecr.us-east-1.amazonaws.com/firmeza-api:latest
```

3. **Crear cluster y servicio** (usar AWS Console o CloudFormation)

---

## 🔧 Configuración de Producción

### appsettings.Production.json

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=production-db.example.com;Database=FirmezaDB;Username=firmeza;Password=***;SslMode=Require"
  },
  "Jwt": {
    "Key": "USE_ENVIRONMENT_VARIABLE",
    "Issuer": "FirmezaApi",
    "Audience": "FirmezaClient"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Warning",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "api.firmeza.com"
}
```

### Variables de Entorno (Recomendado)

```bash
# Nunca commitear secretos en appsettings.json
# Usar variables de entorno en producción

export ConnectionStrings__DefaultConnection="Host=..."
export Jwt__Key="YourVerySecureProductionKey"
export Email__SmtpPassword="your-app-password"
```

### Checklist de Seguridad

- [ ] Cambiar JWT Key por una clave segura única
- [ ] Usar SSL/TLS para base de datos
- [ ] Configurar CORS solo para dominios específicos
- [ ] Habilitar HTTPS redirect
- [ ] Usar secretos de Azure/AWS para credenciales
- [ ] Configurar rate limiting
- [ ] Habilitar logging de seguridad
- [ ] Actualizar contraseña del admin por defecto

### Configurar CORS para Producción

En `Program.cs`:

```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("Production", policy =>
    {
        policy.WithOrigins(
            "https://firmeza.com",
            "https://www.firmeza.com",
            "https://app.firmeza.com"
        )
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials();
    });
});

// En el pipeline
app.UseCors("Production");
```

---

## 📊 Monitoreo y Logs

### Application Insights (Azure)

```bash
# Instalar paquete
dotnet add package Microsoft.ApplicationInsights.AspNetCore

# En Program.cs
builder.Services.AddApplicationInsightsTelemetry();
```

### CloudWatch (AWS)

```bash
# Instalar paquete
dotnet add package AWS.Logger.AspNetCore

# En appsettings.json
{
  "AWS.Logging": {
    "Region": "us-east-1",
    "LogGroup": "firmeza-api",
    "LogLevel": {
      "Default": "Information"
    }
  }
}
```

### Serilog (Universal)

```bash
# Instalar paquetes
dotnet add package Serilog.AspNetCore
dotnet add package Serilog.Sinks.File
dotnet add package Serilog.Sinks.Console

# En Program.cs
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/firmeza-api-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();
```

### Health Checks

```csharp
// En Program.cs
builder.Services.AddHealthChecks()
    .AddNpgSql(connectionString)
    .AddSmtpHealthCheck(options => {
        options.Host = "smtp.gmail.com";
        options.Port = 587;
    });

app.MapHealthChecks("/health");
```

---

## 🔄 CI/CD

### GitHub Actions

Crear `.github/workflows/deploy.yml`:

```yaml
name: Deploy API

on:
  push:
    branches: [ main ]

jobs:
  build-and-deploy:
    runs-on: ubuntu-latest
    
    steps:
    - uses: actions/checkout@v3
    
    - name: Setup .NET
      uses: actions/setup-dotnet@v3
      with:
        dotnet-version: 8.0.x
    
    - name: Restore dependencies
      run: dotnet restore
    
    - name: Build
      run: dotnet build --no-restore --configuration Release
    
    - name: Test
      run: dotnet test --no-build --configuration Release
    
    - name: Publish
      run: dotnet publish Firmeza.Api/Firmeza.Api.csproj -c Release -o ./publish
    
    - name: Deploy to Azure
      uses: azure/webapps-deploy@v2
      with:
        app-name: firmeza-api
        publish-profile: ${{ secrets.AZURE_WEBAPP_PUBLISH_PROFILE }}
        package: ./publish
```

---

## 📝 Backup y Recuperación

### Backup de Base de Datos

```bash
# PostgreSQL backup
pg_dump -h your-host -U postgres -d FirmezaDB > backup_$(date +%Y%m%d).sql

# Restaurar
psql -h your-host -U postgres -d FirmezaDB < backup_20240101.sql
```

### Backup Automatizado (Azure)

```bash
# Configurar backup automático
az postgres flexible-server backup create \
  --resource-group firmeza-rg \
  --name firmeza-db \
  --backup-name manual-backup-$(date +%Y%m%d)
```

---

## 🎯 Performance

### Optimizaciones

1. **Enable Response Compression**
```csharp
builder.Services.AddResponseCompression();
app.UseResponseCompression();
```

2. **Enable Response Caching**
```csharp
builder.Services.AddResponseCaching();
app.UseResponseCaching();
```

3. **Connection Pooling**
```
ConnectionStrings__DefaultConnection="...;Pooling=true;MinPoolSize=5;MaxPoolSize=100"
```

4. **Redis Cache** (opcional)
```bash
dotnet add package Microsoft.Extensions.Caching.StackExchangeRedis

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = "localhost:6379";
});
```

---

## 🆘 Troubleshooting

### Error: "Unable to connect to database"
- Verificar que PostgreSQL esté corriendo
- Verificar cadena de conexión
- Verificar firewall/security groups

### Error: "JWT token invalid"
- Verificar que la clave JWT sea la misma
- Verificar que el token no haya expirado
- Verificar formato: "Bearer TOKEN"

### Error: "SMTP authentication failed"
- Verificar credenciales de Gmail
- Usar contraseña de aplicación, no contraseña normal
- Verificar que 2FA esté habilitado

---

## 📞 Soporte

Para más información:
- Documentación: `/Firmeza.Api/README.md`
- Ejemplos: `/Firmeza.Api/API_EXAMPLES.md`
- Arquitectura: `/Firmeza.Api/ARCHITECTURE.md`
