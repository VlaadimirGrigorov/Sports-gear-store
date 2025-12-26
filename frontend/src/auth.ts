import { apiFetch } from './api'
import { setBackendBaseUrl as setBaseUrl, STORAGE_KEYS } from './config'

// Показваме тези функции и за страниците login/register.
export function setBackendBaseUrl(url: string) {
  setBaseUrl(url)
}

export function getToken(): string | null {
  return localStorage.getItem(STORAGE_KEYS.authToken)
}

export function isLoggedIn(): boolean {
  return !!getToken()
}

export function logout(opts: { redirect?: boolean } = { redirect: true }) {
  localStorage.removeItem(STORAGE_KEYS.authToken)
  if (opts.redirect) {
    window.location.href = '/login-page/index.html'
  }
}

export async function login(payload: { username: string; password: string }): Promise<{ ok: true } | { ok: false; error?: string }> {
  // В твоя бекенд login връща: { Token: "..." }
  // Ние приемаме и { token: "..." } за всеки случай.
  const res = await apiFetch<{ Token?: string; token?: string }>('/auth/login', {
    method: 'POST',
    body: JSON.stringify({
      Username: payload.username,
      Password: payload.password,
    }),
  })

  if (!res.ok) {
    const msg = res.status === 401 ? 'Невалидно потребителско име или парола.' : res.error
    return { ok: false, error: msg }
  }

  const token = res.data.Token ?? res.data.token
  if (!token) {
    return { ok: false, error: 'Липсва token в отговора от сървъра.' }
  }

  localStorage.setItem(STORAGE_KEYS.authToken, token)
  return { ok: true }
}

export async function register(payload: { username: string, email: string, password: string, firstName: string, lastName: string }): Promise<{ ok: true } | { ok: false; error?: string }> {
  // В бекенда ти е: POST /api/auth/register
  const res = await apiFetch<void>('/auth/register', {
    method: 'POST',
    body: JSON.stringify({
      username: payload.username,
      email: payload.email,
      password: payload.password,
      firstName: payload.firstName,
      lastName: payload.lastName,
    }), 
  })

  if (!res.ok) {
    if (res.status === 409) return { ok: false, error: 'Username или Email вече съществува.' }
    if (res.status === 400) return { ok: false, error: 'Невалидни данни. Провери полетата.' }
    return { ok: false, error: res.error }
  }

  return { ok: true }
}

/**
 * Ползвай това на страници, които са само за логнати потребители.
 */
export function requireAuth() {
  if (!isLoggedIn()) {
    window.location.href = '/login-page/index.html'
  }
}
