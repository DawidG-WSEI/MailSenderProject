namespace MailSender.Models.Responses;

public class SendMailResponse
{
    public string AppId {get; set;} = string.Empty;
    public string AppName {get; set;} = string.Empty;
    public string Status {get; set;} = "queued";
    public EmailDetails Email {get; set;} = new();

}

public class EmailDetails
{
    public string To {get; set;} = string.Empty;
    public string Subject {get; set;} = string.Empty;
    public string Body {get; set;} = string.Empty;
}