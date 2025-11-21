using System.Net;
using System.Net.Mail;

namespace Firmeza.Api.Services;

public interface IEmailService
{
    Task SendEmailAsync(string to, string subject, string body);
    Task SendWelcomeEmailAsync(string to, string userName);
    Task SendPurchaseConfirmationAsync(string to, string userName, int orderId, decimal total);
}

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task SendEmailAsync(string to, string subject, string body)
    {
        try
        {
            var smtpHost = _configuration["Email:SmtpHost"] ?? "smtp.gmail.com";
            var smtpPort = int.Parse(_configuration["Email:SmtpPort"] ?? "587");
            var smtpUser = _configuration["Email:SmtpUser"] ?? throw new InvalidOperationException("SMTP User not configured");
            var smtpPass = _configuration["Email:SmtpPassword"] ?? throw new InvalidOperationException("SMTP Password not configured");
            var fromEmail = _configuration["Email:FromEmail"] ?? smtpUser;
            var fromName = _configuration["Email:FromName"] ?? "Firmeza";

            using var client = new SmtpClient(smtpHost, smtpPort)
            {
                EnableSsl = true,
                Credentials = new NetworkCredential(smtpUser, smtpPass)
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(fromEmail, fromName),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };

            mailMessage.To.Add(to);

            await client.SendMailAsync(mailMessage);
            _logger.LogInformation("Email sent successfully to {To}", to);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending email to {To}", to);
            throw;
        }
    }

    public async Task SendWelcomeEmailAsync(string to, string userName)
    {
        var subject = "¡Bienvenido a Firmeza!";
        var body = $@"
            <html>
            <body style='font-family: Arial, sans-serif;'>
                <h2>¡Hola {userName}!</h2>
                <p>Gracias por registrarte en Firmeza.</p>
                <p>Estamos emocionados de tenerte con nosotros.</p>
                <p>Ahora puedes explorar nuestros productos y realizar compras.</p>
                <br/>
                <p>Saludos,<br/>El equipo de Firmeza</p>
            </body>
            </html>
        ";

        await SendEmailAsync(to, subject, body);
    }

    public async Task SendPurchaseConfirmationAsync(string to, string userName, int orderId, decimal total)
    {
        var subject = $"Confirmación de compra - Orden #{orderId}";
        var body = $@"
            <html>
            <body style='font-family: Arial, sans-serif;'>
                <h2>¡Gracias por tu compra, {userName}!</h2>
                <p>Tu orden ha sido procesada exitosamente.</p>
                <div style='background-color: #f5f5f5; padding: 15px; margin: 20px 0;'>
                    <p><strong>Número de orden:</strong> #{orderId}</p>
                    <p><strong>Total:</strong> ${total:F2}</p>
                    <p><strong>Fecha:</strong> {DateTime.Now:dd/MM/yyyy HH:mm}</p>
                </div>
                <p>Recibirás un correo adicional cuando tu pedido sea enviado.</p>
                <br/>
                <p>Saludos,<br/>El equipo de Firmeza</p>
            </body>
            </html>
        ";

        await SendEmailAsync(to, subject, body);
    }
}
