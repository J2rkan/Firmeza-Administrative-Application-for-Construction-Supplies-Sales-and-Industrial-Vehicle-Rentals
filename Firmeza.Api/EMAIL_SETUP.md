# Configuración de Email SMTP para Firmeza API

## Gmail SMTP

Para usar Gmail como servidor SMTP, sigue estos pasos:

### 1. Habilitar Verificación en 2 Pasos

1. Ve a tu cuenta de Google: https://myaccount.google.com/
2. Navega a **Seguridad**
3. Habilita **Verificación en 2 pasos**

### 2. Generar Contraseña de Aplicación

1. Ve a: https://myaccount.google.com/apppasswords
2. Selecciona **Correo** y **Otro (nombre personalizado)**
3. Ingresa "Firmeza API"
4. Copia la contraseña generada (16 caracteres)

### 3. Configurar appsettings.json

```json
{
  "Email": {
    "SmtpHost": "smtp.gmail.com",
    "SmtpPort": "587",
    "SmtpUser": "tu-email@gmail.com",
    "SmtpPassword": "xxxx xxxx xxxx xxxx",  // Contraseña de aplicación
    "FromEmail": "tu-email@gmail.com",
    "FromName": "Firmeza"
  }
}
```

### 4. Variables de Entorno (Producción)

Para producción, es recomendable usar variables de entorno:

```bash
export Email__SmtpUser="tu-email@gmail.com"
export Email__SmtpPassword="xxxx xxxx xxxx xxxx"
```

O en Docker:

```yaml
environment:
  - Email__SmtpUser=tu-email@gmail.com
  - Email__SmtpPassword=xxxx xxxx xxxx xxxx
```

## Otros Proveedores SMTP

### Outlook/Hotmail

```json
{
  "Email": {
    "SmtpHost": "smtp-mail.outlook.com",
    "SmtpPort": "587",
    "SmtpUser": "tu-email@outlook.com",
    "SmtpPassword": "tu-password"
  }
}
```

### SendGrid

```json
{
  "Email": {
    "SmtpHost": "smtp.sendgrid.net",
    "SmtpPort": "587",
    "SmtpUser": "apikey",
    "SmtpPassword": "TU_API_KEY_DE_SENDGRID"
  }
}
```

### SMTP Empresarial

```json
{
  "Email": {
    "SmtpHost": "smtp.tuempresa.com",
    "SmtpPort": "587",
    "SmtpUser": "notificaciones@tuempresa.com",
    "SmtpPassword": "password-seguro"
  }
}
```

## Solución de Problemas

### Error: "Authentication failed"

- Verifica que la contraseña de aplicación sea correcta
- Asegúrate de que la verificación en 2 pasos esté habilitada

### Error: "SMTP server requires a secure connection"

- Verifica que el puerto sea 587 (TLS) o 465 (SSL)
- El código usa `EnableSsl = true` por defecto

### Los correos van a spam

- Configura SPF, DKIM y DMARC en tu dominio
- Usa un dominio verificado
- Evita palabras spam en el asunto

## Plantillas de Email

El sistema incluye las siguientes plantillas:

1. **Bienvenida**: Se envía al registrar un nuevo cliente
2. **Confirmación de Compra**: Se envía al crear una venta

Puedes personalizar las plantillas en `Firmeza.Api/Services/EmailService.cs`

## Testing

Para probar el envío de correos sin configurar SMTP real, puedes usar:

### MailHog (Desarrollo Local)

```bash
docker run -d -p 1025:1025 -p 8025:8025 mailhog/mailhog
```

Luego configura:

```json
{
  "Email": {
    "SmtpHost": "localhost",
    "SmtpPort": "1025",
    "SmtpUser": "",
    "SmtpPassword": ""
  }
}
```

Los correos se verán en: http://localhost:8025
