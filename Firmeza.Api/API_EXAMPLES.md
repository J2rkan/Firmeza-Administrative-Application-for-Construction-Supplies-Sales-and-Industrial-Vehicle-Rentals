# Ejemplos de Uso de la API Firmeza

## 🔐 Autenticación

### 1. Registrar un nuevo cliente

```bash
curl -X POST http://localhost:5001/api/auth/register \
  -H "Content-Type: application/json" \
  -d '{
    "email": "cliente@example.com",
    "password": "Cliente@123",
    "confirmPassword": "Cliente@123"
  }'
```

**Respuesta:**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "email": "cliente@example.com",
  "roles": ["Client"],
  "expiration": "2024-01-02T10:00:00Z"
}
```

### 2. Login (Administrador)

```bash
curl -X POST http://localhost:5001/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "email": "admin@firmeza.com",
    "password": "Admin@123"
  }'
```

**Respuesta:**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "email": "admin@firmeza.com",
  "roles": ["Administrator"],
  "expiration": "2024-01-02T10:00:00Z"
}
```

---

## 📦 Productos

### 1. Listar todos los productos (público)

```bash
curl -X GET http://localhost:5001/api/products
```

**Respuesta:**
```json
[
  {
    "id": 1,
    "name": "Cemento Portland",
    "description": "Cemento de alta resistencia",
    "price": 25.50,
    "stock": 100
  },
  {
    "id": 2,
    "name": "Arena Fina",
    "description": "Arena para construcción",
    "price": 15.00,
    "stock": 200
  }
]
```

### 2. Buscar productos

```bash
curl -X GET "http://localhost:5001/api/products?search=cemento"
```

### 3. Obtener un producto por ID

```bash
curl -X GET http://localhost:5001/api/products/1
```

### 4. Crear un producto (solo Admin)

```bash
curl -X POST http://localhost:5001/api/products \
  -H "Authorization: Bearer TU_TOKEN_ADMIN" \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Ladrillo Rojo",
    "description": "Ladrillo de arcilla cocida",
    "price": 0.50,
    "stock": 5000
  }'
```

**Respuesta:**
```json
{
  "id": 3,
  "name": "Ladrillo Rojo",
  "description": "Ladrillo de arcilla cocida",
  "price": 0.50,
  "stock": 5000
}
```

### 5. Actualizar un producto (solo Admin)

```bash
curl -X PUT http://localhost:5001/api/products/3 \
  -H "Authorization: Bearer TU_TOKEN_ADMIN" \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Ladrillo Rojo Premium",
    "description": "Ladrillo de arcilla cocida de alta calidad",
    "price": 0.75,
    "stock": 4500
  }'
```

### 6. Eliminar un producto (solo Admin)

```bash
curl -X DELETE http://localhost:5001/api/products/3 \
  -H "Authorization: Bearer TU_TOKEN_ADMIN"
```

---

## 👥 Clientes

### 1. Listar todos los clientes (solo Admin)

```bash
curl -X GET http://localhost:5001/api/clients \
  -H "Authorization: Bearer TU_TOKEN_ADMIN"
```

**Respuesta:**
```json
[
  {
    "id": 1,
    "name": "Juan Pérez",
    "document": "12345678",
    "email": "juan@example.com",
    "phone": "555-1234",
    "address": "Calle Principal 123"
  }
]
```

### 2. Buscar clientes

```bash
curl -X GET "http://localhost:5001/api/clients?search=juan" \
  -H "Authorization: Bearer TU_TOKEN_ADMIN"
```

### 3. Crear un cliente (solo Admin)

```bash
curl -X POST http://localhost:5001/api/clients \
  -H "Authorization: Bearer TU_TOKEN_ADMIN" \
  -H "Content-Type: application/json" \
  -d '{
    "name": "María García",
    "document": "87654321",
    "email": "maria@example.com",
    "phone": "555-5678",
    "address": "Avenida Central 456"
  }'
```

### 4. Actualizar un cliente (solo Admin)

```bash
curl -X PUT http://localhost:5001/api/clients/2 \
  -H "Authorization: Bearer TU_TOKEN_ADMIN" \
  -H "Content-Type: application/json" \
  -d '{
    "name": "María García López",
    "document": "87654321",
    "email": "maria.garcia@example.com",
    "phone": "555-5678",
    "address": "Avenida Central 456, Piso 2"
  }'
```

---

## 🛒 Ventas

### 1. Crear una venta

```bash
curl -X POST http://localhost:5001/api/sales \
  -H "Authorization: Bearer TU_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "clientId": 1,
    "saleDetails": [
      {
        "productId": 1,
        "quantity": 10
      },
      {
        "productId": 2,
        "quantity": 5
      }
    ]
  }'
```

**Respuesta:**
```json
{
  "id": 1,
  "date": "2024-01-01T10:30:00Z",
  "total": 330.00,
  "clientId": 1,
  "clientName": "Juan Pérez",
  "saleDetails": [
    {
      "id": 1,
      "productId": 1,
      "productName": "Cemento Portland",
      "quantity": 10,
      "unitPrice": 25.50,
      "subtotal": 255.00
    },
    {
      "id": 2,
      "productId": 2,
      "productName": "Arena Fina",
      "quantity": 5,
      "unitPrice": 15.00,
      "subtotal": 75.00
    }
  ]
}
```

### 2. Listar todas las ventas (solo Admin)

```bash
curl -X GET http://localhost:5001/api/sales \
  -H "Authorization: Bearer TU_TOKEN_ADMIN"
```

### 3. Obtener una venta por ID

```bash
curl -X GET http://localhost:5001/api/sales/1 \
  -H "Authorization: Bearer TU_TOKEN"
```

### 4. Obtener ventas de un cliente

```bash
curl -X GET http://localhost:5001/api/sales/by-client/1 \
  -H "Authorization: Bearer TU_TOKEN"
```

---

## 📝 Colección de Postman

### Importar en Postman

1. Crea una nueva colección llamada "Firmeza API"
2. Agrega una variable de entorno `baseUrl` = `http://localhost:5001`
3. Agrega una variable `token` que se actualizará después del login

### Script de Pre-request (para autenticación automática)

```javascript
// En la carpeta raíz de la colección
pm.sendRequest({
    url: pm.environment.get("baseUrl") + "/api/auth/login",
    method: 'POST',
    header: {
        'Content-Type': 'application/json'
    },
    body: {
        mode: 'raw',
        raw: JSON.stringify({
            email: "admin@firmeza.com",
            password: "Admin@123"
        })
    }
}, function (err, res) {
    if (!err) {
        var jsonData = res.json();
        pm.environment.set("token", jsonData.token);
    }
});
```

### Configurar Authorization

En cada request que requiera autenticación:
- Type: Bearer Token
- Token: `{{token}}`

---

## 🧪 Casos de Prueba

### Escenario 1: Flujo completo de compra

```bash
# 1. Registrar cliente
TOKEN=$(curl -s -X POST http://localhost:5001/api/auth/register \
  -H "Content-Type: application/json" \
  -d '{
    "email": "test@example.com",
    "password": "Test@123",
    "confirmPassword": "Test@123"
  }' | jq -r '.token')

# 2. Ver productos disponibles
curl -X GET http://localhost:5001/api/products

# 3. Crear cliente en el sistema (como admin)
ADMIN_TOKEN=$(curl -s -X POST http://localhost:5001/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "email": "admin@firmeza.com",
    "password": "Admin@123"
  }' | jq -r '.token')

CLIENT_ID=$(curl -s -X POST http://localhost:5001/api/clients \
  -H "Authorization: Bearer $ADMIN_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Test User",
    "document": "99999999",
    "email": "test@example.com",
    "phone": "555-9999",
    "address": "Test Address"
  }' | jq -r '.id')

# 4. Realizar compra
curl -X POST http://localhost:5001/api/sales \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d "{
    \"clientId\": $CLIENT_ID,
    \"saleDetails\": [
      {
        \"productId\": 1,
        \"quantity\": 2
      }
    ]
  }"
```

### Escenario 2: Gestión de inventario

```bash
# 1. Login como admin
ADMIN_TOKEN=$(curl -s -X POST http://localhost:5001/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "email": "admin@firmeza.com",
    "password": "Admin@123"
  }' | jq -r '.token')

# 2. Crear producto
PRODUCT_ID=$(curl -s -X POST http://localhost:5001/api/products \
  -H "Authorization: Bearer $ADMIN_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Producto Test",
    "description": "Descripción test",
    "price": 100.00,
    "stock": 50
  }' | jq -r '.id')

# 3. Actualizar stock
curl -X PUT http://localhost:5001/api/products/$PRODUCT_ID \
  -H "Authorization: Bearer $ADMIN_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Producto Test",
    "description": "Descripción test",
    "price": 100.00,
    "stock": 100
  }'

# 4. Verificar actualización
curl -X GET http://localhost:5001/api/products/$PRODUCT_ID
```

---

## 🔍 Códigos de Estado HTTP

| Código | Significado | Cuándo se usa |
|--------|-------------|---------------|
| 200 | OK | Operación exitosa (GET, PUT) |
| 201 | Created | Recurso creado exitosamente (POST) |
| 204 | No Content | Eliminación exitosa (DELETE) |
| 400 | Bad Request | Datos inválidos |
| 401 | Unauthorized | Token inválido o faltante |
| 403 | Forbidden | Sin permisos para la operación |
| 404 | Not Found | Recurso no encontrado |
| 500 | Internal Server Error | Error del servidor |

---

## 💡 Tips

1. **Guardar el token**: Después del login, guarda el token en una variable de entorno
2. **Renovar token**: Los tokens expiran en 24 horas, vuelve a hacer login si es necesario
3. **Usar jq**: Para parsear JSON en bash: `curl ... | jq '.token'`
4. **Swagger UI**: Para pruebas rápidas, usa Swagger en http://localhost:5001
