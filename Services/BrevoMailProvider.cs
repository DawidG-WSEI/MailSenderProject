using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace MailSender.Services;
// dotnet user-secrets init -> dodanie user-secrets do .csproj w celu przechowywania api-key 
// dotnet user-secrets set "Brevo:ApiKey" "API-KEY-VALUE" -> dodanie klucza
public class BrevoMailProvider : IMailProvider
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public BrevoMailProvider(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
    }
    public async Task SendEmailAsync(string to, string subject, string body)
    {
        var senderName = "Test"; 
        var senderEmail = "grzymus@op.pl"; // <--Email użyty podczas rejestracji w Brevo
        var apiKey = _configuration["Brevo:ApiKey"];
        if(string.IsNullOrEmpty(apiKey))
            throw new InvalidOperationException("Brak klucza API dla Brevo w konfiguracji!");
        
        // Budowa JSON zgodnie z API Brevo
        var payload = new {
            sender = new {name = senderName, email = senderEmail},
            to = new[] {new {email = to}},
            subject = subject,
            htmlContent = body
        };

        var jsonPayload = JsonSerializer.Serialize(payload);
        var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

        // Ustawienie nagłówków żądania
        _httpClient.DefaultRequestHeaders.Clear();
        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        // Brevo wymaga klucza w nagłówku "api-key"
        _httpClient.DefaultRequestHeaders.Add("api-key", apiKey);

        var response = await _httpClient.PostAsync("https://api.brevo.com/v3/smtp/email", content);
        response.EnsureSuccessStatusCode();

    }
}
