/**
 * Централно място за конфигурация на фронтенда.
 *
 * Важно: във Vite по-чисто е да държим URL-а в .env файл (VITE_API_BASE_URL),
 * но за учебен проект можем да го държим и тук.
 */

export const STORAGE_KEYS = {
  authToken: 'authToken',
} as const

// По подразбиране – твоя бекенд.
// СМЕНИ порта да съвпада с ASP.NET ("Now listening on: https://localhost:XXXX").
export let BACKEND_BASE_URL = 'https://localhost:7110'

export function setBackendBaseUrl(url: string) {
  BACKEND_BASE_URL = url.replace(/\/$/, '')
}
