using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MailSender.Configuration;
using MailSender.Models.Requests;
using MailSender.Models.Responses;

namespace MailSender.Controllers;

[ApiController]
[Route("client-app")]
public class ClientAppController : ControllerBase
{   //  ****************************************************************************
    // * KOD TEJ KLASY ZOSTAŁ WYGENEROWANY ZA POMOCĄ AI:                            *
    // * model: Gemini 3.1 PRO                                                      *
    // * Prompt: Na podstawie obecnego kodu oraz na podstawie załączonego screena   *
    // * napisz kod klasy ClientAppController,                                      *
    // * gdzie zaimplementujesz logikę generowania tokenu JWT.                      *
    // * Klasy obsługujące obiekty request i response są już gotowe                 *
    //  ****************************************************************************
    private readonly IConfiguration _configuration;
    private readonly JwtSettings _jwtSettings;

    // Wstrzykujemy konfigurację (do hasła) oraz nasze opcje JWT
    public ClientAppController(IConfiguration configuration, IOptions<JwtSettings> jwtSettings)
    {
        _configuration = configuration;
        _jwtSettings = jwtSettings.Value;
    }

    [HttpPost("register")]
    [AllowAnonymous] // Endpoint jest publiczny, każdy może spróbować się zarejestrować
    public IActionResult Register([FromBody] RegisterRequest request)
    {
        // 1. Odczyt oczekiwanego hasła z appsettings.json
        var expectedPassword = _configuration["AppRegistration:Password"];

        // 2. Walidacja hasła
        if (request.Pass != expectedPassword)
        {
            // Bezpieczne pobranie dwóch ostatnich znaków hasła z konfiguracji dla komunikatu błędu
            var xx = expectedPassword?.Length >= 2 
                ? expectedPassword.Substring(expectedPassword.Length - 2) 
                : "XX";
            
            // Zwracamy status 403 Forbidden ze sprecyzowanym JSON-em błędu
            return StatusCode(403, new { error = $"Invalid index-based password {xx}" });
        }

        // 3. Budowanie zawartości tokenu (Claims)- tu przechowujemy appId i appName
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, request.AppId),
            new Claim("appId", request.AppId),
            new Claim("appName", request.AppName),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()) // Unikalny identyfikator tokenu
        };

        // 4. Konfiguracja szyfrowania
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        // 5. Generowanie obiektu tokenu
        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddDays(_jwtSettings.ExpiryDays), // Ważność 90 dni
            signingCredentials: credentials
            );

        // 6. Zamiana obiektu na faktyczny ciąg znaków (string)
        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

        // 7. Konstrukcja i zwrócenie ostatecznej odpowiedzi
        var response = new RegisterResponse
        {
            AppId = request.AppId,
            AppName = request.AppName,
            Key = tokenString
        };

        return Ok(response); // Zwraca status 200 OK wraz z obiektem response
    }
}

