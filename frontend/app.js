const API_BASE_URL = "http://localhost:5066/api";

const loginForm = document.getElementById("login-form");
const loginMessage = document.getElementById("login-message");
const loadUsersButton = document.getElementById("load-users");
const usersContainer = document.getElementById("users-container");

let token = null;

loginForm.addEventListener("submit", async (event) => {
    event.preventDefault();

    const username = document.getElementById("username").value;
    const password = document.getElementById("password").value;

    try {
        const response = await fetch(`${API_BASE_URL}/auth/login`, {
            method: "POST",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify({
                username: username,
                password: password
            })
        });

        if (!response.ok) {
            loginMessage.textContent = "Login failed.";
            return;
        }

        const data = await response.json();

        token = data.token;

        loginMessage.textContent = "Login successful.";

        console.log("JWT:", token);
    } catch (error) {
        console.error(error);
        loginMessage.textContent = "Could not connect to API.";
    }
});

loadUsersButton.addEventListener("click", async () => {
    if (!token) {
        usersContainer.textContent = "Please login first.";
        return;
    }

    try {
        const response = await fetch(`${API_BASE_URL}/users`, {
            method: "GET",
            headers: {
                "Authorization": `Bearer ${token}`
            }
        });

        if (!response.ok) {
            usersContainer.textContent =
                `Request failed: ${response.status}`;
            return;
        }

        const users = await response.json();

        usersContainer.innerHTML = "";

        users.forEach((user) => {
            const card = document.createElement("div");

            card.className = "user-card";

            card.innerHTML = `
                <h3>${user.username}</h3>
                <p><strong>ID:</strong> ${user.id}</p>
                <p><strong>Email:</strong> ${user.email}</p>
                <p><strong>Role:</strong> ${user.role}</p>
            `;

            usersContainer.appendChild(card);
        });
    } catch (error) {
        console.error(error);
        usersContainer.textContent = "Could not connect to API.";
    }
});