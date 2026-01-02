import './style.css'
import { requireAuth, logout } from './auth'

// Тази страница (/) приемаме, че е "защитена" – трябва да си логнат.
requireAuth()

let lastLoadedProducts: Product[] = []
let lastLoadedTitle = 'All products'

document.getElementById('logoutBtn')?.addEventListener('click', () => {
  logout()
  window.location.href = '/login-page/index.html'
})

document.getElementById('questionnaireBtn')?.addEventListener('click', () => {
  window.location.href = '/questionnaire/questionnaire.html'
})

document.getElementById('cartBtn')?.addEventListener('click', () => {
  window.location.href = '/cart/cart.html'
})

document.getElementById('ordersBtn')?.addEventListener('click', () => {
  window.location.href = '/orders/orders.html'
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

function renderProducts(title: string, products: Product[]) {
  lastLoadedTitle = title
  lastLoadedProducts = products

  const token = localStorage.getItem('authToken')
  if (!token) throw new Error('No token found (user not logged in)')

  if (!productsEl) return
  
  productsEl?.addEventListener('click', async (e) => {
    const target = e.target as HTMLElement
    const btn = target.closest<HTMLButtonElement>('.addToCartBtn')
    if (!btn) return

    const productId = btn.dataset.productid

    if (!productId) {
      alert('Missing productId on button. Check renderProducts().')
      return
    }

    const res = await fetch(`https://localhost:7110/api/cart/add?productId=${encodeURIComponent(productId)}&quantity=1`,
      {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
          Authorization: `Bearer ${token}`,
        },
      }
    )

    if (!res.ok) {
      const errText = await res.text().catch(() => '')
      alert(`Add to cart failed (${res.status}). ${errText}`)
      return
    }

    await refreshCartBadge()
  })

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
            <button class="addToCartBtn" type="button" data-productid="${String(p['id'] ?? p['Id'] ?? p['productId'] ?? '')}">
              Add to Cart
            </button>
          </div>
        `
      }).join('')}
    </div>
  `
}

async function refreshCartBadge() {
  const badge = document.getElementById('cartCount')
  if (!badge) return

  const token = localStorage.getItem('token')

  const res = await fetch('https://localhost:7110/api/cart', {
    method: 'GET',
    headers: {
      'Content-Type': 'application/json',
      Authorization: `Bearer ${token}`,
    },
  })

  if (!res.ok) {
    // if unauthorized or error, just show 0
    badge.textContent = '0'
    return
  }

  const cart = await res.json()
  const items = (cart.items ?? cart.Items ?? []) as any[]
  const totalQty = items.reduce((sum, it) => sum + (it.quantity ?? it.Quantity ?? 0), 0)

  badge.textContent = String(totalQty)
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

const searchInput = document.getElementById('productSearch') as HTMLInputElement | null

let searchTimer: number | undefined
let searchAbort: AbortController | null = null

searchInput?.addEventListener('input', () => {
  // debounce (wait until user stops typing)
  if (searchTimer) window.clearTimeout(searchTimer)

  searchTimer = window.setTimeout(async () => {
    const q = searchInput.value.trim()

    // If empty -> load all products again
    if (!q) {
      await loadAllProducts()
      return
    }

    // cancel previous request
    if (searchAbort) searchAbort.abort()
    searchAbort = new AbortController()

    if (productsEl) productsEl.innerHTML = 'Searching...'

    const token = localStorage.getItem('token')

    // ✅ adjust query parameter name if your BE expects different
    const url = `https://localhost:7110/api/products/search?name=${encodeURIComponent(q)}`

    try {
      const res = await fetch(url, {
        method: 'GET',
        headers: {
          'Content-Type': 'application/json',
          Authorization: `Bearer ${token}`,
        },
        signal: searchAbort.signal,
      })

      if (!res.ok) {
        const errText = await res.text().catch(() => '')
        if (productsEl) {
          productsEl.innerHTML = `<p style="color:#b00020;">Search failed (${res.status}). ${escapeHtml(errText)}</p>`
        }
        return
      }

      const products = (await res.json()) as Product[]
      renderProducts(`Search results for "${q}"`, products ?? [])
    } catch (e) {
      // ignore abort errors (user typed again)
      if ((e as any)?.name === 'AbortError') return
      if (productsEl) productsEl.innerHTML = `<p style="color:#b00020;">Search request failed.</p>`
    }
  }, 300) // 300ms debounce
})

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

void refreshCartBadge()

