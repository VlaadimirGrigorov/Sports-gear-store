import { requireAuth } from './auth'
requireAuth()

async function finalizeOrder() {
    const token = localStorage.getItem('authToken')
    if (!token) {
        alert('Missing token. Please login again.')
        return
    }

    // Създаваме order след "успешно плащане"
    const res = await fetch('https://localhost:7110/api/orders', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
            Authorization: `Bearer ${token}`,
        },
    })

    if (!res.ok) {
        const errText = await res.text().catch(() => '')
        alert(`Order creation failed (${res.status}). ${errText}`)
        return
    }

    // Готово: отиваме към Orders
    window.location.href = '/orders/orders.html'
}

void finalizeOrder()
