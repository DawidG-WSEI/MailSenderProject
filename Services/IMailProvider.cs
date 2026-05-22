namespace MailSender.Services;

public interface IMailProvider
{
    //Używamy Task, ponieważ wysyłanie maili przez sieć to operacja asynchroniczna
    Task SendEmailAsync(string to, string subject, string body);    
}
