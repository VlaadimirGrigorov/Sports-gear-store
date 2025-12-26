import { register, setBackendBaseUrl } from '/src/auth'

// ---- Конфигурация ----
// СМЕНИ го, ако бекендът ти слуша на друг порт.
setBackendBaseUrl('https://localhost:7110')

document.addEventListener('DOMContentLoaded', () => {
  const form = document.getElementById('registerForm')
  const messageDiv = document.getElementById('message')

  if (!form) {
    console.warn('Register form (#registerForm) не е намерена.')
    return
  }

  form.addEventListener('submit', async (e) => {
    e.preventDefault()

    const firstName = /** @type {HTMLInputElement} */ document.getElementById("firstName").value.trim();
    const lastName = /** @type {HTMLInputElement} */ document.getElementById("lastName").value.trim();
    const username = /** @type {HTMLInputElement} */ (document.getElementById('username'))?.value?.trim()
    const email = /** @type {HTMLInputElement} */ (document.getElementById('email'))?.value?.trim()
    const password = /** @type {HTMLInputElement} */ (document.getElementById('password'))?.value
    const confirmPassword = /** @type {HTMLInputElement} */ (document.getElementById('confirmPassword'))?.value

    if (!username || !email || !password || !confirmPassword) {
      showMessage(messageDiv, 'Моля, попълнете всички задължителни полета.', 'error')
      return
    }

    if (password !== confirmPassword) {
      showMessage(messageDiv, 'Паролите не съвпадат!', 'error')
      return
    }

    showMessage(messageDiv, 'Регистрация...', 'info')

    const result = await register({ username, email, password, firstName, lastName })

    if (!result.ok) {
      showMessage(messageDiv, result.error ?? 'Грешка при регистрация.', 'error')
      return
    }

    showMessage(messageDiv, 'Успешна регистрация! Пренасочване към вход...', 'success')
    setTimeout(() => {
      window.location.href = '/login-page/index.html'
    }, 1000)
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
