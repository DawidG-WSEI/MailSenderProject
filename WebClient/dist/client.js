import createClient from "openapi-fetch";
export const DEFAULT_BASE_URL = "http://localhost:5234";
export function createApiClient(baseUrl = DEFAULT_BASE_URL) {
    return createClient({ baseUrl });
}
export async function registerClientApp(client, request) {
    return client.POST("/client-app/register", { body: request });
}
export async function sendMail(client, token, request) {
    return client.POST("/mail/send", {
        body: request,
        headers: {
            Authorization: `Bearer ${token}`
        }
    });
}
