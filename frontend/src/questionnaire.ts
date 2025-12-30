import './style.css'
import { requireAuth } from './auth'

requireAuth()

type Question = {
  label: string
  answers: { answerValue: string; answerTags: string[] }[]
}

const container = document.getElementById('questionnaire')
const submitBtn = document.getElementById('questionnaireSubmitBtn') as HTMLButtonElement | null
const cancelBtn = document.getElementById('questionnaireCancelBtn') as HTMLButtonElement | null

if (!container) throw new Error('Missing #questionnaire')
if (!submitBtn) throw new Error('Missing #questionnaireSubmitBtn')
if (!cancelBtn) throw new Error('Missing #questionnaireCancelBtn')

let currentQuestionCount = 0

cancelBtn.addEventListener('click', () => {
  // go back home (change if you want another page)
  window.location.href = '/'
})

submitBtn.addEventListener('click', async () => {
  const ok = validateAll(currentQuestionCount)
  if (!ok) return

  const answers = collectAnswers(currentQuestionCount)
  console.log('Submitting:', answers)

  // TODO: POST to backend (example)
  // await fetch('http://localhost:8080/api/questionnaire/submit', {
  //   method: 'POST',
  //   headers: { 'Content-Type': 'application/json' },
  //   body: JSON.stringify(answers),
  // })
})

async function loadQuestions() {
  container!.innerHTML = 'Loading...'

  const res = await fetch('https://localhost:7110/api/questionnaire', {
    headers: { 'Content-Type': 'application/json' },
  })
  if (!res.ok) throw new Error(`Failed (${res.status})`)

  const questions = (await res.json()) as Question[]
  currentQuestionCount = questions.length
  renderQuestions(questions)
}

function renderQuestions(questions: Question[]) {
  container!.innerHTML = `
    ${questions
      .map(
        (q, qi) => `
        <fieldset data-qindex="${qi}" style="margin:16px 0; padding:12px; border:1px solid #ddd; border-radius:8px;">
          <legend style="font-weight:600;">${q.label}</legend>

          <div style="display:flex; flex-direction:column; gap:8px; margin-top:8px;">
            ${q.answers
              .map(
                (a) => `
                <label style="display:flex; gap:8px; align-items:center;">
                  <input
                    type="radio"
                    name="q_${qi}"
                    value="${escapeHtml(a.answerValue)}"
                    data-tags='${JSON.stringify(a.answerTags)}'
                  />
                  <span>${a.answerValue}</span>
                </label>
              `
              )
              .join('')}
          </div>

          <div class="error" style="margin-top:8px; color:#b00020; display:none;">
            Please answer this question.
          </div>
        </fieldset>
      `
      )
      .join('')}
  `
}

function validateAll(questionCount: number): boolean {
  let allOk = true
  let firstInvalidFieldset: HTMLElement | null = null

  for (let qi = 0; qi < questionCount; qi++) {
    const fieldset = document.querySelector<HTMLElement>(`fieldset[data-qindex="${qi}"]`)
    const chosen = document.querySelector<HTMLInputElement>(`input[name="q_${qi}"]:checked`)
    const errorEl = fieldset?.querySelector<HTMLElement>('.error')

    if (!chosen) {
      allOk = false
      if (errorEl) errorEl.style.display = 'block'
      if (!firstInvalidFieldset) firstInvalidFieldset = fieldset ?? null
    } else {
      if (errorEl) errorEl.style.display = 'none'
    }
  }

  if (firstInvalidFieldset) {
    firstInvalidFieldset.scrollIntoView({ behavior: 'smooth', block: 'center' })
  }

  return allOk
}

function collectAnswers(questionCount: number) {
  const results: { questionIndex: number; answerValue: string; answerTags: string[] }[] = []

  for (let qi = 0; qi < questionCount; qi++) {
    const chosen = document.querySelector<HTMLInputElement>(`input[name="q_${qi}"]:checked`)
    if (!chosen) continue

    const tagsRaw = chosen.getAttribute('data-tags') || '[]'
    const answerTags = JSON.parse(tagsRaw) as string[]

    results.push({
      questionIndex: qi,
      answerValue: chosen.value,
      answerTags,
    })
  }

  return results
}

function escapeHtml(s: string) {
  return s.replace(/[&<>"']/g, (ch) => {
    const map: Record<string, string> = {
      '&': '&amp;',
      '<': '&lt;',
      '>': '&gt;',
      '"': '&quot;',
      "'": '&#39;',
    }
    return map[ch] ?? ch
  })
}

loadQuestions().catch((e) => {
  container.innerHTML = `<p style="color:red;">${(e as Error).message}</p>`
})
