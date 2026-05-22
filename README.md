# Mail Sender API

Prosty mikroserwis napisany w technologii ASP.NET Core (C#) na potrzeby projektu akademickiego. Aplikacja udostępnia uwierzytelnianie oparte na tokenach JWT oraz umożliwia wysyłanie e-maili za pośrednictwem zewnętrznego API dostawcy Brevo.


## 🛠️ Wymagania wstępne

Aby uruchomić projekt na swoim komputerze, potrzebujesz:
* Zainstalowanego [SDK .NET](https://dotnet.microsoft.com/download) (wersja 8.0).
* Konta w serwisie [Brevo](https://www.brevo.com/) oraz wygenerowanego klucza API. (Konto musi przejść weryfikację, aby można było korzystać z API)


## ⚙️ Szybkie uruchomienie

1. Sklonuj repozytorium na swój komputer lokalny.
   ```bash
   git clone https://github.com/DawidG-WSEI/MailSenderProject.git
   cd MailSender
   ```
2. Skonfiguruj wymagane hasło rejestracji w pliku `appsettings.json` (zamień ostatnie dwie liczby na ostanie dwie z twojego index.) 
3. Dodaj swój klucz API Brevo do lokalnych sekretów środowiska:
   ```bash
   dotnet user-secrets init
   dotnet user-secrets set "Brevo:ApiKey" "TWÓJ_KLUCZ_API"
   ```
4. W pliku `BrevoMailProvide.cs` w metodzie `SendEmailAsync()` zmień adres email na ten, który został użyty do rejestracji w **Brevo**.
5. W pliku `MessageFormatter.cs` w metodzie `FormatBody()` zmień nazwisko na swoje - takie są wymagania projektu.


## 🚀 Technologie
* .NET 8.0 (C#)
* ASP.NET Core Web API
* JSON Web Tokens (JWT)
* Brevo API (SMTP)
* Swagger UI


## ✨ Główne funkcje
* **Autoryzacja:** Zabezpieczony endpoint rejestracji generujący tokeny JWT dla klientów.
* **Wysyłka e-mail:** Integracja z zewnętrznym dostawcą usług mailowych.
* **Przetwarzanie wiadomości:** Automatyczne formatowanie tematu oraz oznaczanie nazwiska w treści.
* **Bezpieczeństwo:** Oddzielenie wrażliwych danych (klucze API) od kodu źródłowego za pomocą mechanizmu *User Secrets*.

