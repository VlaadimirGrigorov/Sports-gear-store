import { BACKEND_BASE_URL, STORAGE_KEYS } from './config'

type ApiResult<T> =
  | { ok: true; data: T }
  | { ok: false; status: number; error?: string }

function getToken(): string | null {
  return localStorage.getItem(STORAGE_KEYS.authToken)
}

/**
 * Малък wrapper около fetch, който:
 * - добавя Content-Type
 * - добавя Authorization: Bearer <token> (ако има)
 * - връща структуриран резултат
 */
export async function apiFetch<T>(path: string, options: RequestInit = {}): Promise<ApiResult<T>> {
  const token = getToken()
  const headers = new Headers(options.headers ?? {})

  // Ако пращаме body, обикновено е JSON
  if (!headers.has('Content-Type') && options.body) {
    headers.set('Content-Type', 'application/json')
  }

  if (token) {
    headers.set('Authorization', `Bearer ${token}`)
  }

  const url = `${BACKEND_BASE_URL}/api${path.startsWith('/') ? '' : '/'}${path}`

  try {
    const res = await fetch(url, { ...options, headers })

    // Опитваме да прочетем body като JSON, ако има
    const contentType = res.headers.get('content-type') ?? ''
    const hasJson = contentType.includes('application/json')

    if (!res.ok) {
      let errorText: string | undefined
      if (hasJson) {
        const err = await res.json().catch(() => undefined)
        errorText = (err?.message ?? err?.Message ?? err?.error) as string | undefined
      }
      return { ok: false, status: res.status, error: errorText }
    }

    if (hasJson) {
      const data = (await res.json()) as T
      return { ok: true, data }
    }

    // Ако endpoint връща празен отговор
    return { ok: true, data: undefined as unknown as T }
  } catch (e) {
    return { ok: false, status: 0, error: 'Network error (не успяхме да се свържем с бекенда).' }
  }
}
