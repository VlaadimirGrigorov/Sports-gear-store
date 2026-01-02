import './style.css'
import { requireAuth } from './auth'

requireAuth()

const el = document.getElementById('orders')
if (!el) throw new Error('Missing #orders')

type Order = {
  id?: number
  Id?: number
  createdAt?: string
  CreatedAt?: string
  items?: any[]
  Items?: any[]
}

async function loadOrders() {
  el!.innerHTML = 'Loading orders...'

  const token = localStorage.getItem('authToken')
  if (!token) {
    el!.innerHTML = '<p style="color:#b00020;">Missing token. Please login again.</p>'
    return
  }

  // 1) Get orders
  const ordersRes = await fetch('https://localhost:7110/api/orders', {
    method: 'GET',
    headers: {
      'Content-Type': 'application/json',
      Authorization: `Bearer ${token}`,
    },
  })

  if (!ordersRes.ok) {
    const errText = await ordersRes.text().catch(() => '')
    el!.innerHTML = `<p style="color:#b00020;">Failed to load orders (${ordersRes.status}). ${escapeHtml(errText)}</p>`
    return
  }

  const orders = (await ordersRes.json()) as Order[]

  if (!orders.length) {
    el!.innerHTML = '<p>No orders yet.</p>'
    return
  }

  // 2) Get products for names/prices
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

  const products = (await productsRes.json()) as any[]
  const productMap = new Map<number, any>()
  for (const p of products) {
    const id = Number(p.id ?? p.Id)
    if (Number.isFinite(id)) productMap.set(id, p)
  }

  // 3) Render orders
  el!.innerHTML = orders
    .map((o) => {
      const id = o.id ?? o.Id
      const created = o.createdAt ?? o.CreatedAt
      const items = (o.items ?? o.Items ?? []) as any[]

      const lines = items.map((it) => {
        const pid = Number(it.productId ?? it.ProductId)
        const qty = Number(it.quantity ?? it.Quantity ?? 0)
        const p = productMap.get(pid)
        const name = p?.name ?? p?.Name ?? `Product #${pid}`
        const price = Number(p?.price ?? p?.Price ?? it.unitPrice ?? it.UnitPrice ?? 0)
        return `<li>${escapeHtml(String(name))} — Qty: ${qty} — $${price.toFixed(2)}</li>`
      })

      return `
        <div class="productCard" style="margin-bottom:12px;">
          <div style="font-weight:900;">Order #${id}</div>
          <div style="opacity:.8; margin:6px 0 10px;">${created ? new Date(created).toLocaleString() : ''}</div>
          <ul style="margin:0; padding-left:18px;">${lines.join('')}</ul>
        </div>
      `
    })
    .join('')
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

void loadOrders()
