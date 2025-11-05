using SendGrid.Helpers.Mail;
using SendGrid;
using System.Net;
using Auth.Api.DTOs;
using Auth.Api.Interfaces;

namespace Auth.Api.Services;

public class EmailService : IEmailService
{
    private readonly string _sendGridApiKey;

    public EmailService(IConfiguration configuration)
    {
        _sendGridApiKey = configuration["Settings:SendGrid:ApiKey"] ??
            throw new ArgumentNullException("SendGrid API Key não encontrada.");
    }

    public async Task<ResponseModel> SendEmailAsync(RequestEmail request)
    {
        try
        {
            var client = new SendGridClient(_sendGridApiKey);

            // Definindo o remetente e o destinatário
            var from = new EmailAddress(request.From, "Recuperação de senha");
            var to = new EmailAddress(request.To);

            // Corpo do e-mail em HTML
            var bodyHtml = $@"
                    <div style='font-family: Arial, sans-serif; max-width: 600px; margin: auto; padding: 20px; border: 1px solid #ddd; border-radius: 8px;'>
                        <h2 style='color: #333;'>Olá,</h2>
                        <p>Recebemos uma solicitação para redefinir sua senha. Clique no botão abaixo para criar uma nova senha:</p>
                        <div style='text-align: center; margin: 20px 0;'>
                            <a href='{request.ResetLink}' style='background-color: #007bff; color: white; padding: 12px 20px; border-radius: 5px; text-decoration: none; font-size: 16px;'>
                                🔑 Redefinir Senha
                            </a>
                        </div>
                        <p>Se você não solicitou essa alteração, ignore este e-mail. Sua senha permanecerá segura.</p>
                        <hr style='border: none; border-top: 1px solid #ddd; margin: 20px 0;' />
                        <p style='color: #888; font-size: 12px;'>Atenciosamente, <br/>Equipe Minha Empresa</p>
                    </div>
                ";

            // Criando a mensagem de e-mail
            var msg = MailHelper.CreateSingleEmail(from, to, request.Subject, bodyHtml, bodyHtml);

            // Enviando o e-mail
            var response = await client.SendEmailAsync(msg);

            // Verificando a resposta
            if (response.StatusCode == HttpStatusCode.Accepted)
            {
                // Email enviado com sucesso
                return new ResponseModel("E-mail enviado com sucesso.", StatusCodes.Status200OK);
            }
            else
            {
                var errorDetails = await response.Body.ReadAsStringAsync();
                Console.WriteLine($"Erro ao enviar e-mail: {errorDetails}");
                return new ResponseModel($"Erro ao enviar e-mail.\nDetalhe do(s) erro(s): {errorDetails}", StatusCodes.Status400BadRequest);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro inesperado ao enviar e-mail: {ex.Message}");
            return new ResponseModel($"Erro inesperado ao enviar e-mail: {ex.Message}",StatusCodes.Status400BadRequest);
        }
    }
}
