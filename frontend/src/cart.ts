import './style.css'
import { requireAuth } from './auth'

requireAuth()

const el = document.getElementById('cartItems')
if (!el) throw new Error('Missing #cartItems')

type CartApiItem = { productId?: number; ProductId?: number; quantity?: number; Quantity?: number }
type Product = Record<string, any>

let productMap = new Map<number, Product>()
let cartItems: { productId: number; quantity: number }[] = []

const checkoutBtn = document.getElementById('checkoutBtn') as HTMLButtonElement | null

checkoutBtn?.addEventListener('click', async () => {
    const token = localStorage.getItem('authToken') // при теб ключът е authToken (виж config.ts)
    if (!token) {
        alert('Please login first.')
        window.location.href = '/login-page/index.html'
        return
    }

    const res = await fetch('https://localhost:7110/api/checkout/create-session', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
            Authorization: `Bearer ${token}`,
        },
    })

    if (!res.ok) {
        const txt = await res.text()
        alert(txt || 'Checkout failed.')
        return
    }

    const data = await res.json()
    if (!data?.url) {
        alert('Missing Stripe session URL.')
        return
    }

    window.location.href = data.url
})

async function loadCartPage() {
  el!.innerHTML = 'Loading cart...'

  const token = localStorage.getItem('authToken')
  if (!token) {
    el!.innerHTML = `<p style="color:#b00020;">Missing token. Please login again.</p>`
    return
  }

  // 1) Load products so we can display names/prices
  const productsRes = await fetch('https://localhost:7110/api/products', {
    method: 'GET',
    headers: {
      'Content-Type': 'application/json',
      Authorization: `Bearer ${token}`,
    },
  })

  if (!productsRes.ok) {
    const errText = await productsRes.text().catch(() => '')
    el!.innerHTML = `<p style="color:#b00020;">Failed to load products (${productsRes.status}). ${escapeHtml(errText)}</p>`
    return
  }

  const products = (await productsRes.json()) as Product[]
  productMap = new Map<number, Product>()
  for (const p of products) {
    const id = Number(p.id ?? p.Id)
    if (Number.isFinite(id)) productMap.set(id, p)
  }

  // 2) Load cart
  await refreshCart()
  renderCart()
}

async function refreshCart() {
  const token = localStorage.getItem('authToken')!
  const cartRes = await fetch('https://localhost:7110/api/cart', {
    method: 'GET',
    headers: {
      'Content-Type': 'application/json',
      Authorization: `Bearer ${token}`,
    },
  })

  if (!cartRes.ok) {
    const errText = await cartRes.text().catch(() => '')
    el!.innerHTML = `<p style="color:#b00020;">Failed to load cart (${cartRes.status}). ${escapeHtml(errText)}</p>`
    cartItems = []
    return
  }

  const cart = await cartRes.json()
  const rawItems = (cart.items ?? cart.Items ?? cart.Items ?? []) as CartApiItem[]

  cartItems = rawItems.map((it) => ({
    productId: Number(it.productId ?? it.ProductId),
    quantity: Number(it.quantity ?? it.Quantity ?? 0),
  })).filter((x) => Number.isFinite(x.productId) && x.quantity > 0)
}

function renderCart() {
  if (!cartItems.length) {
    el!.innerHTML = '<p>Your cart is empty.</p>'
    return
  }

  let total = 0

  const rows = cartItems.map((it) => {
    const p = productMap.get(it.productId)
    const name = String(p?.name ?? p?.Name ?? `Product #${it.productId}`)
    const priceNum = Number(p?.price ?? p?.Price ?? 0)

    total += priceNum * it.quantity

    return `
      <div class="productCard">
        <div style="display:flex; justify-content:space-between; align-items:flex-start; gap:12px;">
          <div>
            <div style="font-weight:800; font-size:18px;">${escapeHtml(name)}</div>
            <div style="opacity:.8; margin-top:4px;">Quantity: <strong>${it.quantity}</strong></div>
            <div style="font-weight:900; margin-top:10px;">$${priceNum.toFixed(2)}</div>
          </div>

          <div class="qtyControls">
            <button class="qtyBtn" type="button" data-action="minus" data-productid="${it.productId}">−</button>
            <button class="qtyBtn" type="button" data-action="plus" data-productid="${it.productId}">+</button>
          </div>
        </div>
      </div>
    `
  })

  el!.innerHTML = `
    <div style="display:grid; gap:12px;">
      ${rows.join('')}
      <div style="font-weight:900; font-size:18px; margin-top:8px;">
        Total: $${total.toFixed(2)}
      </div>
    </div>
  `
}

// ✅ Attach ONCE (no { once: true })
el.addEventListener('click', async (e) => {
  const target = e.target as HTMLElement
  const btn = target.closest<HTMLButtonElement>('.qtyBtn')
  if (!btn) return

  const action = btn.dataset.action
  const productId = Number(btn.dataset.productid)
  if (!Number.isFinite(productId)) return

  const current = cartItems.find((x) => x.productId === productId)
  if (!current) return

  const token = localStorage.getItem('authToken')
  if (!token) {
    alert('Missing token. Please login again.')
    return
  }

  btn.disabled = true

  try {
    if (action === 'plus') {
      const newQty = current.quantity + 1

      const res = await fetch(
        `https://localhost:7110/api/cart/update?productId=${encodeURIComponent(String(productId))}&quantity=${encodeURIComponent(String(newQty))}`,
        {
          method: 'PUT',
          headers: {
            'Content-Type': 'application/json',
            Authorization: `Bearer ${token}`,
          },
        }
      )

      if (!res.ok) {
        const errText = await res.text().catch(() => '')
        alert(`Update failed (${res.status}). ${errText}`)
        return
      }
    }

    if (action === 'minus') {
      const newQty = current.quantity - 1

      if (newQty <= 0) {
        const res = await fetch(
          `https://localhost:7110/api/cart/remove?productId=${encodeURIComponent(String(productId))}`,
          {
            method: 'DELETE',
            headers: {
              'Content-Type': 'application/json',
              Authorization: `Bearer ${token}`,
            },
          }
        )

        if (!res.ok) {
          const errText = await res.text().catch(() => '')
          alert(`Remove failed (${res.status}). ${errText}`)
          return
        }
      } else {
        const res = await fetch(
          `https://localhost:7110/api/cart/update?productId=${encodeURIComponent(String(productId))}&quantity=${encodeURIComponent(String(newQty))}`,
          {
            method: 'PUT',
            headers: {
              'Content-Type': 'application/json',
              Authorization: `Bearer ${token}`,
            },
          }
        )

        if (!res.ok) {
          const errText = await res.text().catch(() => '')
          alert(`Update failed (${res.status}). ${errText}`)
          return
        }
      }
    }

    // ✅ refresh and re-render after successful change
    await refreshCart()
    renderCart()
  } catch (err) {
    // ✅ this will show CORS/network failures
    alert(`Request error: ${(err as Error).message}`)
  } finally {
    btn.disabled = false
  }
})

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

void loadCartPage()
