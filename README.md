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

## Web Client (4.0)

```ps1
cd ./WebClient
```

I czytaj readme.md

## Mailtrap Email Sandbox i podział na warstwy clean architecture (4.5)

W wersji 4.5 struktura projektu została uporządkowana zgodnie z założeniami architektury warstwowej:

- folder `Application` zawiera interfejs providera oraz logikę formatowania wiadomości,
- folder `Infrastructure` zawiera pliki implementacje z dostawcami,
- kontrolery korzystają z interfejsu `IMailProvider`, dzięki czemu nie są bezpośrednio
  zależne od konkretnego dostawcy.

Dodano plik `MailtrapMailProvider.cs`, a domyślnym dostawcą wiadomości jest
obecnie Mailtrap Email Sandbox.
Wiadomości wysłane przez Mailtrap Sandbox nie trafiają do prawdziwej skrzynki
odbiorcy. Można je zobaczyć po zalogowaniu do skrzynki Email Sandbox w Mailtrap.

### Konfiguracja Mailtrap

1. Załóż konto w serwisie Mailtrap.
2. Wygeneruj token API.
3. W sekcji **Email Sandbox** utwórz testowy inbox.
4. W zakładce **Integrations** odszukaj identyfikator `InboxId`.
5. Dodaj token API oraz identyfikator inboxa do lokalnych sekretów projektu:

```bash
dotnet user-secrets set "Mailtrap:ApiToken" "TWÓJ_TOKEN_API"
dotnet user-secrets set "Mailtrap:InboxId" "TWOJE_INBOX_ID"
```

Po uruchomieniu aplikacji wiadomości można wysyłać przez endpoint `POST /mail/send`.
Wynik wysyłki będzie widoczny po zalogowaniu do wybranego inboxa w Mailtrap.

### Wybór providera

Aktywny provider jest rejestrowany w kontenerze Dependency Injection w pliku
`Program.cs`. Domyślnie używany jest Mailtrap:

```csharp
builder.Services.AddHttpClient<IMailProvider, MailtrapMailProvider>();
```

Aby ponownie używać Brevo, należy zamienić rejestrowaną implementację:

```csharp
builder.Services.AddHttpClient<IMailProvider, BrevoMailProvider>();
```