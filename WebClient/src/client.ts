import createClient from "openapi-fetch";
import type { paths } from "./openapi-types.ts";

export const DEFAULT_BASE_URL = "http://localhost:5234";

export function createApiClient(baseUrl: string = DEFAULT_BASE_URL) {
  return createClient<paths>({ baseUrl });
}

export type ApiClient = ReturnType<typeof createApiClient>;

export type RegisterRequest = {
  appId: string;
  appName: string;
  pass: string;
};

export type SendMailRequest = {
  to: string;
  subject: string;
  body: string;
};

export async function registerClientApp(
  client: ApiClient,
  request: RegisterRequest
) {
  return client.POST("/client-app/register", { body: request });
}

export async function sendMail(
  client: ApiClient,
  token: string,
  request: SendMailRequest
) {
  return client.POST("/mail/send", {
    body: request,
    headers: {
      Authorization: `Bearer ${token}`
    }
  });
}
