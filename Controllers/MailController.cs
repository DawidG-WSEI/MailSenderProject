using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MailSender.Models.Requests;
using MailSender.Models.Responses;
using MailSender.Services;

namespace MailSender.Controllers;

[ApiController]
[Route("mail")]
[Authorize] // status 401 jeśli token się nie zgadza
public class MailController : ControllerBase
{
    private readonly IMailProvider _mailProvider;
    private readonly MessageFormatter _messageFormatter;

    public MailController(IMailProvider mailProvider, MessageFormatter messageFormatter)
    {
        _mailProvider = mailProvider;
        _messageFormatter = messageFormatter;
    }

    [HttpPost("send")]
    public async Task<IActionResult> SendMail([FromBody] SendMailRequest request)
    {
        // Odczyd danych z tokenu (Claims)
        var appId = User.FindFirstValue("appId") ?? string.Empty;
        var appName = User.FindFirstValue("appName") ?? string.Empty;

        //format tematu i treści
        var formattedSubject = _messageFormatter.FormatSubject(request.Subject);
        var formattedBody = _messageFormatter.FormatBody(request.Body);

        try
        {
            await _mailProvider.SendEmailAsync(request.To, formattedSubject, formattedBody);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new {error = "Wystąpił bład z API", details = ex.Message});
        }

        var response = new SendMailResponse{
            AppId = appId,
            AppName = appName,
            Status = "queued",
            Email = new EmailDetails{
                To = request.To,
                Subject = formattedSubject,
                Body = formattedBody
            }
        };
        
        return Accepted(response);
    }

}

