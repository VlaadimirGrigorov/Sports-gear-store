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

/**
 * След връщане от Questionnaire:
 * - /?products=all => зареждаме всички продукти
 * - /?products=recommended => зареждаме препоръчани продукти
 */

const token = localStorage.getItem('authToken')
if (!token) throw new Error('No token found (user not logged in)')

const productsEl = document.getElementById('products')
const productsTitleEl = document.getElementById('productsTitle')

type Product = Record<string, unknown>

function getDisplayName(p: Product): string {
  return (p['name'] ?? p['title'] ?? p['productName'] ?? p['label'] ?? 'Unnamed product') as string
}

function getDisplayPrice(p: Product): string | null {
  const price = p['price'] ?? p['Price'] ?? p['unitPrice'] ?? p['UnitPrice']
  if (price === undefined || price === null) return null
  return String(price)
}

function getDisplayImage(p: Product): string | null {
  const img = p['imageUrl'] ?? p['imageURL'] ?? p['image'] ?? p['ImageUrl'] ?? p['ImageURL']
  if (!img) return null
  return String(img)
}

function renderProducts(title: string, products: Product[]) {
  if (!productsEl) return
  if (productsTitleEl) productsTitleEl.textContent = title

  if (!products.length) {
    productsEl.innerHTML = '<p>No products found.</p>'
    return
  }

  productsEl.innerHTML = `
    <div class="productsGrid">
      ${products.map((p) => {
        const name = escapeHtml(getDisplayName(p))
        const category = escapeHtml(String(p['category'] ?? p['Category'] ?? ''))
        const desc = escapeHtml(String(p['description'] ?? p['Description'] ?? ''))
        const price = getDisplayPrice(p)
        const priceText = price ? `$${escapeHtml(price)}` : ''

        return `
          <div class="productCard">
            <h3 class="productName">${name}</h3>
            <p class="productCategory">${category}</p>
            <p class="productDesc">${desc}</p>
            <div class="productPrice">${priceText}</div>
            <button class="addToCartBtn" type="button">Add to Cart</button>
          </div>
        `
      }).join('')}
    </div>
  `
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

async function loadAllProducts() {
  if (!productsEl) return
  productsEl.innerHTML = 'Loading products...'

  const res = await fetch('https://localhost:7110/api/products', {
    method: 'GET',
    headers: {
      'Content-Type': 'application/json',
      Authorization: `Bearer ${token}`,
    },
  })

  if (!res.ok) {
    productsEl.innerHTML = `<p style="color:#b00020;">Failed to load products (${res.status}). ${escapeHtml(res.error ?? '')}</p>`
    return
  }

  const products = (await res.json()) as Product[]
  renderProducts('All products', products ?? [])
}

async function loadRecommendedProducts() {
  if (!productsEl) return
  productsEl.innerHTML = 'Loading recommended products...'

  const res = await fetch('https://localhost:7110/api/products/recommended', {
    method: 'GET',
    headers: {
      'Content-Type': 'application/json',
      Authorization: `Bearer ${token}`,
    },
  })

  if (!res.ok) {
    productsEl.innerHTML = `<p style="color:#b00020;">Failed to load recommended products (${res.status}). ${escapeHtml(res.error ?? '')}</p>`
    return
  }

  const products = (await res.json()) as Product[]
  renderProducts('Recommended products', products ?? [])
}

async function initProductsFromRedirectFlag() {
  if (!productsEl) return

  const params = new URLSearchParams(window.location.search)
  const mode = params.get('products')

  // If we came back after successful submit
  if (mode === 'recommended') {
    await loadRecommendedProducts()
    history.replaceState({}, '', '/')
    return
  }

  // Default behavior: when home loads (including after login), show all products
  await loadAllProducts()
  if (mode) history.replaceState({}, '', '/')
}

void initProductsFromRedirectFlag()

