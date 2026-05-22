using System.Net.Http.Headers;

namespace MailSender.Services;

public class MessageFormatter
{
    public string FormatSubject(string subject)
    {
        if (string.IsNullOrWhiteSpace(subject))
            return subject;
        
        return subject.EndsWith('?') ? $"[Q] {subject}" : subject;
    }

    public string FormatBody(string body)
    {
        string studentSurname = "Grzymkowski"; // <---- Tutaj każdy wpisuje swoje nazwisko
        if (string.IsNullOrWhiteSpace(body))
            return body;

        return body.Replace(studentSurname, $"[student.suname]{studentSurname}[/student.suname]");
    }
}
