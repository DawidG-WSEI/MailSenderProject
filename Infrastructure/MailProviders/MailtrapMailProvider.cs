using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using MailSender.Application.Interfaces;

namespace MailSender.Infrastructure.MailProviders;

// Ten provider wys³a wiadomosc do testowej skrzynki Email Sandbox w Mailtrap
public class MailtrapMailProvider : IMailSenderProvider
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    public MailtrapMailProvider(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
    }

    public async Task SendEmailAsync(string to, string subject, string body)
    {
        var apiToken = _configuration["Mailtrap:ApiToken"];
        var inboxId = _configuration["Mailtrap:InboxId"];

        if (string.IsNullOrEmpty(apiToken))
            throw new InvalidOperationException("Brak tokenu API dla Mailtrap w konfiguracji");

        if (string.IsNullOrEmpty(inboxId))
            throw new InvalidOperationException("Brak InboxID dla Mailtrap w konfiguracji");

        var payload = new
        {
            from = new { email = "halnywiatr3@gmail.com", name = "MailSender Test" }, //opcjonalnie email mo¿na zmieniæ na za³o¿one konto na mailtrap, jednak nie jest to wymóg konieczny.
            to = new[] { new { email = to } },
            subject = subject,
            html = body
        };

        var jsonPayload = JsonSerializer.Serialize(payload);
        var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
        _httpClient.DefaultRequestHeaders.Clear();
        _httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", apiToken);

        var url = $"https://sandbox.api.mailtrap.io/api/send/{inboxId}";
        var response = await _httpClient.PostAsync(url, content);
        response.EnsureSuccessStatusCode();
    }
}