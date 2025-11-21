# 🛒 Firmeza Client - Portal de Compras

Aplicación web moderna desarrollada con React + TypeScript + Vite para que los clientes de Firmeza puedan comprar materiales de construcción y alquilar vehículos en línea.

## ✨ Características

- 🔐 **Autenticación JWT**: Registro e inicio de sesión seguro
- 🛍️ **Catálogo de Productos**: Visualización y búsqueda de productos
- 🛒 **Carrito de Compras**: Gestión completa del carrito
- 💳 **Checkout**: Proceso de compra con confirmación por email
- 📱 **Responsive**: Diseño adaptativo para todos los dispositivos
- ⚡ **Rápido**: Construido con Vite para máximo rendimiento

## 🛠️ Tecnologías

- **React 18** - Biblioteca UI
- **TypeScript** - Tipado estático
- **Vite** - Build tool ultra-rápido
- **TailwindCSS** - Estilos utilitarios
- **Zustand** - Gestión de estado
- **React Router** - Navegación
- **Axios** - Cliente HTTP
- **Vitest** - Testing

## 📦 Instalación

### Requisitos Previos

- Node.js 18+ 
- npm o yarn
- API de Firmeza corriendo en http://localhost:5001

### Pasos

1. **Instalar dependencias**
```bash
cd Firmeza.Client
npm install
```

2. **Configurar variables de entorno**

Crear archivo `.env`:
```env
VITE_API_URL=http://localhost:5001/api
```

3. **Ejecutar en modo desarrollo**
```bash
npm run dev
```

La aplicación estará disponible en: http://localhost:3000

## 🚀 Scripts Disponibles

```bash
# Desarrollo
npm run dev

# Compilar para producción
npm run build

# Vista previa de producción
npm run preview

# Ejecutar pruebas
npm run test
```

## 📁 Estructura del Proyecto

```
Firmeza.Client/
├── src/
│   ├── components/          # Componentes reutilizables
│   │   ├── Navbar.tsx
│   │   └── ProductCard.tsx
│   ├── pages/              # Páginas/Vistas
│   │   ├── Login.tsx
│   │   ├── Register.tsx
│   │   ├── Products.tsx
│   │   └── Cart.tsx
│   ├── services/           # Servicios API
│   │   └── api.ts
│   ├── store/              # Estado global (Zustand)
│   │   ├── authStore.ts
│   │   └── cartStore.ts
│   ├── types/              # Definiciones TypeScript
│   │   └── index.ts
│   ├── App.tsx             # Componente raíz
│   ├── main.tsx            # Punto de entrada
│   └── index.css           # Estilos globales
├── public/                 # Archivos estáticos
├── index.html              # HTML principal
├── package.json
├── vite.config.ts
├── tailwind.config.js
└── tsconfig.json
```

## 🔐 Autenticación

La aplicación utiliza JWT (JSON Web Tokens) para autenticación:

1. **Registro**: El usuario se registra con email y contraseña
2. **Login**: Obtiene un token JWT válido por 24 horas
3. **Almacenamiento**: El token se guarda en localStorage
4. **Requests**: El token se incluye automáticamente en todas las peticiones
5. **Expiración**: Si el token expira, se redirige al login

## 🛒 Flujo de Compra

1. **Explorar Productos**: Ver catálogo completo
2. **Buscar**: Filtrar productos por nombre
3. **Agregar al Carrito**: Seleccionar productos y cantidades
4. **Revisar Carrito**: Ver resumen y totales
5. **Finalizar Compra**: Procesar pedido
6. **Confirmación**: Recibir email con comprobante

## 🎨 Diseño

El diseño utiliza TailwindCSS con una paleta de colores personalizada:

- **Primary**: Azul (#0ea5e9)
- **Backgrounds**: Grises claros
- **Acentos**: Verde (éxito), Rojo (error), Amarillo (advertencia)

### Componentes Reutilizables

- `.btn` - Botones base
- `.btn-primary` - Botón primario
- `.btn-secondary` - Botón secundario
- `.btn-outline` - Botón con borde
- `.input` - Campos de entrada
- `.card` - Tarjetas
- `.badge` - Etiquetas

## 📡 API Endpoints Utilizados

### Autenticación
- `POST /api/auth/register` - Registro
- `POST /api/auth/login` - Login

### Productos
- `GET /api/products` - Listar productos
- `GET /api/products?search=...` - Buscar productos

### Ventas
- `POST /api/sales` - Crear venta

## 🧪 Pruebas

```bash
# Ejecutar pruebas
npm run test

# Ejecutar con coverage
npm run test -- --coverage
```

## 🐳 Docker

### Dockerfile

```dockerfile
FROM node:18-alpine AS build
WORKDIR /app
COPY package*.json ./
RUN npm install
COPY . .
RUN npm run build

FROM nginx:alpine
COPY --from=build /app/dist /usr/share/nginx/html
COPY nginx.conf /etc/nginx/nginx.conf
EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]
```

### Ejecutar con Docker

```bash
# Construir imagen
docker build -t firmeza-client .

# Ejecutar contenedor
docker run -p 3000:80 firmeza-client
```

## 🔧 Configuración de Producción

### Variables de Entorno

```env
VITE_API_URL=https://api.firmeza.com/api
```

### Build de Producción

```bash
npm run build
```

Los archivos compilados estarán en `dist/`

### Desplegar en Netlify/Vercel

1. Conectar repositorio
2. Configurar build command: `npm run build`
3. Configurar publish directory: `dist`
4. Agregar variables de entorno

## 📱 Responsive Design

La aplicación es completamente responsive:

- **Mobile**: < 640px
- **Tablet**: 640px - 1024px
- **Desktop**: > 1024px

## 🔒 Seguridad

- ✅ Tokens JWT con expiración
- ✅ Validación de formularios
- ✅ Sanitización de inputs
- ✅ HTTPS en producción
- ✅ CORS configurado
- ✅ No se exponen endpoints de admin

## 🚧 Roadmap

- [ ] Perfil de usuario
- [ ] Historial de compras
- [ ] Wishlist
- [ ] Comparador de productos
- [ ] Chat de soporte
- [ ] Notificaciones push

## 📄 Licencia

MIT

## 👥 Equipo

Desarrollado por el equipo de Firmeza

---

**Firmeza Client** - Compra fácil, rápida y segura 🏗️
