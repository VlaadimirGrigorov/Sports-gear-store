import './style.css'
import { requireAuth, logout } from './auth'

// Тази страница (/) приемаме, че е "защитена" – трябва да си логнат.
requireAuth()

document.getElementById('logoutBtn')?.addEventListener('click', () => {
  logout()
  window.location.href = '/login-page/index.html'
})

document.getElementById('questionnaireBtn')?.addEventListener('click', () => {
  window.location.href = '/questionnaire/questionnaire.html'
})



// app.innerHTML = `
//   <div style="max-width: 720px; margin: 40px auto; font-family: system-ui;">
//     <h1>SportsGearStore</h1>
//     <p>✅ Frontend работи. ✅ Имаш токен. Следва: страници/функционалност.</p>
//     <div style="display:flex; gap: 12px; margin-top: 16px;">
//       <button id="logoutBtn" type="button">Logout</button>
//       <a href="/login-page/index.html">Login page</a>
//     </div>
//   </div>
// `

// document.getElementById('logoutBtn')?.addEventListener('click', () => logout())
