using System.Net;
using System.Net.Mail;

namespace SRV11_AutoRegistro.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;

    public EmailService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task EnviarConfirmacion(
        string correo,
        string token)
    {
        var smtp =
            _configuration["EmailSettings:SmtpServer"];

        var port =
            int.Parse(
                _configuration["EmailSettings:Port"]!);

        var sender =
            _configuration["EmailSettings:SenderEmail"];

        var password =
            _configuration["EmailSettings:Password"];

        var link =
            $"http://localhost:5000/autoregistro/confirmar/{token}";

        using var client = new SmtpClient(smtp, port)
        {
            EnableSsl = true,
            Credentials = new NetworkCredential(
                sender,
                password)
        };

        var mail = new MailMessage(
            sender!,
            correo);

        mail.Subject = "Confirmación de cuenta";

        mail.Body =
            $"Confirme su cuenta usando este enlace:\n\n{link}";

        await client.SendMailAsync(mail);
    }
}