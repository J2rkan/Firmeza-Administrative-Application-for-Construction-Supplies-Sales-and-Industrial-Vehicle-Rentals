# 🚀 Firmeza API

API RESTful desarrollada con ASP.NET Core 8.0 para el sistema de gestión Firmeza. Proporciona endpoints para la gestión de productos, clientes y ventas con autenticación JWT.

## 📋 Tabla de Contenidos

- [Características](#características)
- [Tecnologías](#tecnologías)
- [Requisitos Previos](#requisitos-previos)
- [Instalación](#instalación)
- [Configuración](#configuración)
- [Ejecución](#ejecución)
- [Endpoints](#endpoints)
- [Autenticación](#autenticación)
- [Pruebas](#pruebas)
- [Docker](#docker)
- [Arquitectura](#arquitectura)

## ✨ Características

- ✅ **Autenticación JWT** con roles (Administrator, Client)
- ✅ **CRUD completo** para Productos, Clientes y Ventas
- ✅ **AutoMapper** para mapeo de DTOs
- ✅ **Swagger/OpenAPI** para documentación interactiva
- ✅ **PostgreSQL** como base de datos
- ✅ **Envío de correos** con SMTP (Gmail)
- ✅ **Pruebas unitarias** con xUnit
- ✅ **Docker** para despliegue containerizado
- ✅ **CORS** configurado para integración con frontends

## 🛠 Tecnologías

- **Framework**: ASP.NET Core 8.0
- **ORM**: Entity Framework Core 8.0
- **Base de Datos**: PostgreSQL 15
- **Autenticación**: JWT Bearer Token
- **Documentación**: Swagger/Swashbuckle
- **Testing**: xUnit + Moq
- **Mapeo**: AutoMapper
- **Containerización**: Docker

## 📦 Requisitos Previos

- .NET 8.0 SDK
- PostgreSQL 15+
- Docker (opcional)
- Cuenta de Gmail con contraseña de aplicación (para envío de correos)

## 🔧 Instalación

### 1. Clonar el repositorio

```bash
git clone https://github.com/your-repo/Firmeza-app.git
cd Firmeza-app
```

### 2. Restaurar paquetes

```bash
dotnet restore
```

### 3. Configurar la base de datos

Asegúrate de tener PostgreSQL ejecutándose y crea la base de datos:

```sql
CREATE DATABASE FirmezaDB;
```

### 4. Aplicar migraciones

```bash
cd Firmeza.Api
dotnet ef database update
```

## ⚙️ Configuración

### appsettings.json

Configura el archivo `Firmeza.Api/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=FirmezaDB;Username=postgres;Password=tu_password"
  },
  "Jwt": {
    "Key": "TuClaveSecretaSuperSeguraParaJWT123456",
    "Issuer": "FirmezaApi",
    "Audience": "FirmezaClient"
  },
  "Email": {
    "SmtpHost": "smtp.gmail.com",
    "SmtpPort": "587",
    "SmtpUser": "tu-email@gmail.com",
    "SmtpPassword": "tu-app-password",
    "FromEmail": "tu-email@gmail.com",
    "FromName": "Firmeza"
  }
}
```

### Configurar Gmail para SMTP

1. Habilita la verificación en 2 pasos en tu cuenta de Gmail
2. Genera una contraseña de aplicación: https://myaccount.google.com/apppasswords
3. Usa esa contraseña en `Email:SmtpPassword`

## 🚀 Ejecución

### Modo Desarrollo

```bash
cd Firmeza.Api
dotnet run
```

La API estará disponible en:
- **HTTP**: http://localhost:5001
- **HTTPS**: https://localhost:5002
- **Swagger**: http://localhost:5001 (raíz)

### Usuario Administrador por Defecto

Al iniciar la aplicación, se crea automáticamente un usuario administrador:

- **Email**: admin@firmeza.com
- **Password**: Admin@123

## 📚 Endpoints

### Autenticación

| Método | Endpoint | Descripción | Auth |
|--------|----------|-------------|------|
| POST | `/api/auth/login` | Iniciar sesión | No |
| POST | `/api/auth/register` | Registrar cliente | No |
| POST | `/api/auth/register-admin` | Registrar admin | No |

### Productos

| Método | Endpoint | Descripción | Auth |
|--------|----------|-------------|------|
| GET | `/api/products` | Listar productos | No |
| GET | `/api/products/{id}` | Obtener producto | No |
| POST | `/api/products` | Crear producto | Admin |
| PUT | `/api/products/{id}` | Actualizar producto | Admin |
| DELETE | `/api/products/{id}` | Eliminar producto | Admin |

### Clientes

| Método | Endpoint | Descripción | Auth |
|--------|----------|-------------|------|
| GET | `/api/clients` | Listar clientes | Admin |
| GET | `/api/clients/{id}` | Obtener cliente | Admin |
| POST | `/api/clients` | Crear cliente | Admin |
| PUT | `/api/clients/{id}` | Actualizar cliente | Admin |
| DELETE | `/api/clients/{id}` | Eliminar cliente | Admin |

### Ventas

| Método | Endpoint | Descripción | Auth |
|--------|----------|-------------|------|
| GET | `/api/sales` | Listar ventas | Admin |
| GET | `/api/sales/{id}` | Obtener venta | Sí |
| POST | `/api/sales` | Crear venta | Sí |
| GET | `/api/sales/by-client/{id}` | Ventas por cliente | Sí |

## 🔐 Autenticación

### 1. Obtener Token

```bash
curl -X POST http://localhost:5001/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "email": "admin@firmeza.com",
    "password": "Admin@123"
  }'
```

Respuesta:
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "email": "admin@firmeza.com",
  "roles": ["Administrator"],
  "expiration": "2024-01-02T10:00:00Z"
}
```

### 2. Usar Token en Requests

```bash
curl -X GET http://localhost:5001/api/products \
  -H "Authorization: Bearer TU_TOKEN_AQUI"
```

### 3. Swagger con JWT

1. Abre Swagger en http://localhost:5001
2. Haz clic en el botón **Authorize** 🔒
3. Ingresa: `Bearer TU_TOKEN_AQUI`
4. Ahora puedes probar endpoints protegidos

## 🧪 Pruebas

### Ejecutar todas las pruebas

```bash
dotnet test
```

### Ejecutar pruebas específicas

```bash
dotnet test --filter "FullyQualifiedName~ProductsControllerTests"
```

### Ver cobertura

```bash
dotnet test /p:CollectCoverage=true
```

## 🐳 Docker

### Construir imagen

```bash
docker build -t firmeza-api -f Firmeza.Api/Dockerfile .
```

### Ejecutar con Docker Compose

```bash
docker-compose up -d
```

Servicios disponibles:
- **Base de datos**: localhost:5432
- **Admin Web**: http://localhost:5000
- **API**: http://localhost:5001

### Detener servicios

```bash
docker-compose down
```

### Ver logs

```bash
docker-compose logs -f api
```

## 🏗 Arquitectura

El proyecto sigue una arquitectura en capas:

```
Firmeza-app/
├── Firmeza.Core/              # Entidades y contratos
│   ├── Entities/              # Modelos de dominio
│   └── Interfaces/            # Interfaces de repositorios
├── Firmeza.Application/       # Lógica de aplicación
│   └── ViewModels/            # ViewModels para Razor
├── Firmeza.Infrastructure/    # Implementación de infraestructura
│   ├── Persistence/           # DbContext
│   ├── Repositories/          # Implementación de repositorios
│   └── Services/              # Servicios de infraestructura
├── Firmeza.Api/               # API RESTful
│   ├── Controllers/           # Controladores API
│   ├── DTOs/                  # Data Transfer Objects
│   ├── Mappings/              # Perfiles de AutoMapper
│   └── Services/              # Servicios de la API
├── Firmeza.Admin/             # Panel administrativo Razor
└── Firmeza.Test/              # Pruebas unitarias
```

## 📊 Diagramas

### Diagrama de Entidades

```
┌─────────────┐       ┌─────────────┐       ┌─────────────┐
│   Client    │       │    Sale     │       │   Product   │
├─────────────┤       ├─────────────┤       ├─────────────┤
│ Id          │◄──────│ ClientId    │       │ Id          │
│ Name        │       │ Date        │       │ Name        │
│ Document    │       │ Total       │       │ Description │
│ Email       │       └─────────────┘       │ Price       │
│ Phone       │              │              │ Stock       │
│ Address     │              │              └─────────────┘
└─────────────┘              │                     ▲
                             │                     │
                             ▼                     │
                      ┌─────────────┐              │
                      │ SaleDetail  │──────────────┘
                      ├─────────────┤
                      │ SaleId      │
                      │ ProductId   │
                      │ Quantity    │
                      │ UnitPrice   │
                      └─────────────┘
```

## 📝 Ejemplos de Uso

### Crear un Producto

```bash
curl -X POST http://localhost:5001/api/products \
  -H "Authorization: Bearer TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Laptop Dell XPS 15",
    "description": "Laptop de alta gama",
    "price": 1500.00,
    "stock": 10
  }'
```

### Crear una Venta

```bash
curl -X POST http://localhost:5001/api/sales \
  -H "Authorization: Bearer TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "clientId": 1,
    "saleDetails": [
      {
        "productId": 1,
        "quantity": 2
      },
      {
        "productId": 2,
        "quantity": 1
      }
    ]
  }'
```

## 🤝 Contribuir

1. Fork el proyecto
2. Crea una rama para tu feature (`git checkout -b feature/AmazingFeature`)
3. Commit tus cambios (`git commit -m 'Add some AmazingFeature'`)
4. Push a la rama (`git push origin feature/AmazingFeature`)
5. Abre un Pull Request

## 📄 Licencia

Este proyecto está bajo la Licencia MIT.

## 👥 Autores

- **Equipo Firmeza** - *Desarrollo inicial*

## 🙏 Agradecimientos

- ASP.NET Core Team
- Entity Framework Core Team
- Comunidad de .NET
