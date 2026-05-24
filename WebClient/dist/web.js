import { createApiClient, registerClientApp, sendMail, DEFAULT_BASE_URL } from "./client.js";
// setup trash
const registerForm = document.getElementById("register-form");
const form = document.getElementById("mail-form");
const result = document.getElementById("result");
const resultElement = result;
const registerInputs = {
    appId: document.getElementById("app-id"),
    appName: document.getElementById("app-name"),
    pass: document.getElementById("app-pass")
};
const messageInputs = {
    to: document.getElementById("odbiorca"),
    subject: document.getElementById("temat"),
    body: document.getElementById("tekst"),
    token: document.getElementById("token")
};
const apiClient = createApiClient(DEFAULT_BASE_URL);
// utilsy
function setResult(message, isError) {
    resultElement.textContent = message;
    resultElement.className = isError ? "result error" : "result success";
}
const getTrimmedValue = (input) => input.value.trim();
function requireFields(values, message) {
    const hasMissing = Object.values(values).some((value) => !value);
    if (hasMissing) {
        setResult(message, true);
        return false;
    }
    return true;
}
function handleApiResult(result, onSuccess) {
    if (result.data) {
        onSuccess?.(result.data);
        setResult(JSON.stringify(result.data), false);
        return;
    }
    setResult(`Błąd HTTP ${result.response?.status ?? 0}: ${result.error ? JSON.stringify(result.error) : "Brak odpowiedzi."}`, true);
}
// Mięso
registerForm.addEventListener("submit", async (event) => {
    event.preventDefault();
    const appId = getTrimmedValue(registerInputs.appId);
    const appName = getTrimmedValue(registerInputs.appName);
    const pass = getTrimmedValue(registerInputs.pass);
    if (!requireFields({ appId, appName, pass }, "Wypełnij wszystkie pola")) {
        return;
    }
    setResult("Generuję token...", false);
    try {
        const result = await registerClientApp(apiClient, { appId, appName, pass });
        handleApiResult(result, (data) => {
            const payload = data;
            if (payload?.key) {
                messageInputs.token.value = payload.key;
            }
        });
    }
    catch (e) {
        const message = e instanceof Error ? e.message : "Nieznany błąd.";
        setResult(message, true);
    }
});
form.addEventListener("submit", async (event) => {
    event.preventDefault();
    const token = getTrimmedValue(messageInputs.token);
    const body = getTrimmedValue(messageInputs.body);
    const subject = getTrimmedValue(messageInputs.subject);
    const to = getTrimmedValue(messageInputs.to);
    if (!requireFields({ token, body, subject, to }, "Wypełnij wszystkie pola.")) {
        return;
    }
    setResult("Wysyłam wiadomość...", false);
    try {
        const result = await sendMail(apiClient, token, { to, subject, body });
        handleApiResult(result);
    }
    catch (e) {
        const message = e instanceof Error ? e.message : "Niespodziewany błąd. Spróbuj ponownie.";
        setResult(message, true);
    }
});
