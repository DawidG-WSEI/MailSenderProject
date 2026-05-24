# WebClient
Tutaj znajdziesz bibliotekę TypeScript/JavaScript wygenerowaną na podstawie OpenAPI oraz UI wygenerowane przy pomocy AI do wysyłki wiadomości.
## Wymagania

- Node.JS - u mnie 22
- Dotnet by backend działał

## 1. Pobranie OpenAPI (ogarnięte już, ale na przyszłość kiedyś tam)

Uruchom API (w głównym folderze projektu):

```ps1
dotnet run --launch-profile http
```

Pobierz specyfikację do pliku:

```ps1
Invoke-WebRequest http://localhost:5234/swagger/v1/swagger.json -OutFile ./WebClient/openapi/openapi.json
```

## 2. Instalacja dependencies (openapi ts generator)

> Personalnie jestem fanem pnpm, ale wy nie będziecie tego mieli u siebie xd - jak będizecie chcieli iść w web dev, to polecam - raz instalujecie a potem leci reuse po różnych projektach

```ps1
cd ./WebClient
npm install
```

## 3. Generowanie TypeScriptu dla openapi

```ps1
npm run generate-ts
# ALBO (to samo)
npx openapi-typescript .\openapi\openapi.json -o .\src\openapi-types.ts
```

## 4. Transpilacja do JavaScript

```ps1
npx tsc
```

Wynik zostanie zapisany w katalogu `dist`

## 5. Stronka

```ps1
npx serve .
```

Wchodzisz na localhost:3000 i masz stronkę. 
Dlaczego serwer? Bo by się nam statyczny plik odświeżał po submicie.

Jak masz już wszystko ustalone wpisujesz w appId oraz w AppName co chcesz (rejestracja apki dla uzyskania JWT), hasło wpisujesz wedle tego co jest zakonfigurowane na BackEndzie i generujesz token.
Potem ci się on sam wklei do wysyłki wiadomości - zadbałem o to :), a content tak samo - co chcesz.
Wyśle ci do recipienta jakiego sobie ustalisz

## Konfiguracja adresu API

Domyslny adres API jest ustawiony w `src/client.ts` jako:

- http://localhost:5234

Jesli chcesz zmienic adres, edytuj `DEFAULT_BASE_URL` i ponownie uruchom `npx tsc`

## Co gdzie i jak

- `src/client.ts` - klient oparty o openapi-fetch (typowany przez OpenAPI) - kompatybilny z naszym data modelem
- `src/web.ts` - logika UI (formularz i wysylka)
- `src/openapi-types.ts` - typy OpenApi - generowane skryptem - patrz instrukcje wyżej
- `index.html` - nasza stronka
- `openapi/openapi.json` - specyfikacja Swagger/OpenAPI - fetchujemy - patrz instrukcje wyżej
