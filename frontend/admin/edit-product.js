
import { requireAuth, getBackendBaseUrl } from '/src/auth.ts';

requireAuth();

const productForm = document.getElementById('productForm') as HTMLFormElement;
const formTitle = document.getElementById('formTitle');
const productIdInput = document.getElementById('productId') as HTMLInputElement;
const nameInput = document.getElementById('name') as HTMLInputElement;
const descriptionInput = document.getElementById('description') as HTMLTextAreaElement;
const priceInput = document.getElementById('price') as HTMLInputElement;
const categoryIdInput = document.getElementById('categoryId') as HTMLInputElement;
const imageUrlInput = document.getElementById('imageUrl') as HTMLInputElement;

const token = localStorage.getItem('authToken');
if (!token) {
    alert('You are not logged in!');
    window.location.href = '/login-page/';
    throw new Error('Auth token not found');
}

const params = new URLSearchParams(window.location.search);
const productId = params.get('id');

if (productId) {
    if (formTitle) formTitle.textContent = 'Edit Product';

    // Fetch product details for editing
    fetch(`${getBackendBaseUrl()}/api/products/${productId}`, {
        headers: { 'Authorization': `Bearer ${token}` }
    })
    .then(response => {
        if (!response.ok) {
            throw new Error(`Failed to fetch product: ${response.status}`);
        }
        return response.json();
    })
    .then(product => {
        productIdInput.value = product.id;
        nameInput.value = product.name;
        descriptionInput.value = product.description;
        priceInput.value = product.price;
        categoryIdInput.value = product.categoryId;
        imageUrlInput.value = product.imageUrl;
    })
    .catch(error => {
        console.error('Error fetching product details:', error);
        alert('Failed to load product details for editing.');
    });
} else {
    if (formTitle) formTitle.textContent = 'Add Product';
}

productForm?.addEventListener('submit', (event) => {
    event.preventDefault();

    const productData = {
        id: productId ? parseInt(productId, 10) : 0,
        name: nameInput.value,
        description: descriptionInput.value,
        price: parseFloat(priceInput.value),
        categoryId: parseInt(categoryIdInput.value, 10),
        imageUrl: imageUrlInput.value
    };

    const method = productId ? 'PUT' : 'POST';
    const url = productId
        ? `${getBackendBaseUrl()}/api/products/${productId}`
        : `${getBackendBaseUrl()}/api/products`;

    fetch(url, {
        method: method,
        headers: {
            'Content-Type': 'application/json',
            'Authorization': `Bearer ${token}`
        },
        body: JSON.stringify(productData)
    })
    .then(response => {
        if (response.ok) {
            window.location.href = '/admin/'; // Redirect to dashboard
        } else {
            response.text().then(text => {
                alert(`Failed to save product: ${text}`);
            });
        }
    })
    .catch(error => {
        console.error('Error saving product:', error);
        alert('An error occurred while saving the product.');
    });
});
