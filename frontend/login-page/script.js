import { login, logout, setBackendBaseUrl } from '/src/auth'

// ---- Конфигурация ----
// СМЕНИ го, ако бекендът ти слуша на друг порт.
// Можеш да го видиш в конзолата на ASP.NET: "Now listening on: https://localhost:XXXX"
setBackendBaseUrl('https://localhost:7110')

// При отваряне на login страницата: по желание чистим стар токен.
// Това симулира "logout" от клиента.
logout({ redirect: false })

document.addEventListener('DOMContentLoaded', () => {
  const loginForm = document.getElementById('loginForm')
  const messageDiv = document.getElementById('message')

  if (!loginForm) {
    console.warn('Login form (#loginForm) не е намерена.')
    return
  }

  loginForm.addEventListener('submit', async (e) => {
    e.preventDefault()

    const username = /** @type {HTMLInputElement} */ (document.getElementById('username'))?.value?.trim()
    const password = /** @type {HTMLInputElement} */ (document.getElementById('password'))?.value

    if (!username || !password) {
      showMessage(messageDiv, 'Моля, въведете потребителско име и парола.', 'error')
      return
    }

    showMessage(messageDiv, 'Влизане...', 'info')

    const result = await login({ username, password })

    if (!result.ok) {
      showMessage(messageDiv, result.error ?? 'Грешка при вход.', 'error')
      return
    }

    showMessage(messageDiv, 'Успешен вход! Пренасочване...', 'success')
    // Връщаме се към началната страница на приложението
    window.location.href = '/' // Vite сервира index.html на root
  })
})

function showMessage(el, text, type) {
  if (!el) {
    alert(text)
    return
  }
  el.className = type
  el.textContent = text
}
