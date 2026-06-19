namespace MailSender.Application.Interfaces;

public interface IMailSenderProvider
{
    // Uzywamy Task, poniewaz wysylanie maili przez siec to operacja asynchroniczna.
    Task SendEmailAsync(string to, string subject, string body);
}
