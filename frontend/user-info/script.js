
import { requireAuth, getBackendBaseUrl } from '/src/auth.ts';

requireAuth();

const userInfoDiv = document.getElementById('userInfo');
const token = localStorage.getItem('authToken');

if (!token) {
    if (userInfoDiv) {
        userInfoDiv.innerHTML = '<p>You are not logged in.</p>';
    }
} else {
    fetch(`${getBackendBaseUrl()}/api/userprofile`, {
        headers: {
            'Authorization': `Bearer ${token}`
        }
    })
    .then(response => {
        if (!response.ok) {
            throw new Error('Failed to fetch user profile');
        }
        return response.json();
    })
    .then(data => {
        if (userInfoDiv) {
            userInfoDiv.innerHTML = `
                <p><strong>Username:</strong> ${data.username}</p>
                <p><strong>Email:</strong> ${data.useremail}</p>
                <p><strong>First Name:</strong> ${data.firstName}</p>
                <p><strong>Last Name:</strong> ${data.lastName}</p>
            `;
        }
    })
    .catch(error => {
        if (userInfoDiv) {
            userInfoDiv.innerHTML = `<p>${error.message}</p>`;
        }
    });
}
