
import { requireAuth, getBackendBaseUrl } from '/src/auth.ts';

requireAuth();

const productsTableBody = document.querySelector('#productsTable tbody');
const addProductBtn = document.getElementById('addProductBtn');
const token = localStorage.getItem('authToken');

if (!token) {
    alert('You are not logged in!');
    window.location.href = '/login-page/';
    throw new Error('Auth token not found');
}

function fetchAndRenderProducts() {
    fetch(`${getBackendBaseUrl()}/api/products`, {
        headers: {
            'Authorization': `Bearer ${token}`
        }
    })
    .then(response => {
        if (!response.ok) {
            throw new Error(`Failed to fetch products: ${response.status}`);
        }
        return response.json();
    })
    .then(products => {
        if (productsTableBody) {
            productsTableBody.innerHTML = ''; // Clear existing rows
            products.forEach((product: any) => {
                const row = document.createElement('tr');
                row.innerHTML = `
                    <td>${product.id}</td>
                    <td>${product.name}</td>
                    <td>${product.price}</td>
                    <td>
                        <button class="editBtn" data-id="${product.id}">Edit</button>
                        <button class="deleteBtn" data-id="${product.id}">Delete</button>
                    </td>
                `;
                productsTableBody.appendChild(row);
            });
        }
    })
    .catch(error => {
        console.error('Error fetching products:', error);
        if (productsTableBody) {
            productsTableBody.innerHTML = '<tr><td colspan="4">Failed to load products.</td></tr>';
        }
    });
}

addProductBtn?.addEventListener('click', () => {
    window.location.href = 'edit-product.html';
});

productsTableBody?.addEventListener('click', (event) => {
    const target = event.target as HTMLElement;
    const id = target.dataset.id;

    if (target.classList.contains('editBtn')) {
        window.location.href = `edit-product.html?id=${id}`;
    }

    if (target.classList.contains('deleteBtn')) {
        if (confirm('Are you sure you want to delete this product?')) {
            fetch(`${getBackendBaseUrl()}/api/products/${id}`, {
                method: 'DELETE',
                headers: {
                    'Authorization': `Bearer ${token}`
                }
            })
            .then(response => {
                if (response.ok) {
                    fetchAndRenderProducts(); // Refresh the table
                } else {
                    alert('Failed to delete product.');
                }
            })
            .catch(error => {
                console.error('Error deleting product:', error);
                alert('An error occurred while deleting the product.');
            });
        }
    }
});

// Initial load
fetchAndRenderProducts();
