const API_BASE_URL = "http://localhost:5066/api";

const authPage = document.querySelector(".auth-page");
const dashboard = document.getElementById("dashboard");

const loginForm = document.getElementById("login-form");
const loginMessage = document.getElementById("login-message");

const logoutButton = document.getElementById("logout-button");

let token = null;
let currentUserId = null;
const urlParams = new URLSearchParams(window.location.search);
const requestedUserId = urlParams.get("userId");
function updateUserIdInUrl(userId) {
    const url = new URL(window.location.href);

    url.searchParams.set("userId", userId);

    window.history.replaceState({}, "", url);
}
/* =========================
   PROFILE
   ========================= */

const profileButton =
    document.getElementById("profile-button");

profileButton.addEventListener("click", async () => {
const requestedUserId =
    new URLSearchParams(window.location.search).get("userId");
    try {

        const response = await fetch(
            `${API_BASE_URL}/users/${requestedUserId || currentUserId}`,
            {
                headers: {
                    "Authorization": `Bearer ${token}`
                }
            }
        );

        const data = await response.json();

        console.log("Profile:", data);

        const profileResult =
            document.getElementById("profile-result");

        const domInput =
            decodeURIComponent(window.location.hash.substring(1));

        profileResult.innerHTML = `

            <div class="profile-details">

                <p>
                    <strong>Username:</strong>
                    ${data.username}
                </p>

                <p>
                    <strong>Email:</strong>
                    ${data.email}
                </p>

                <p>
                    <strong>Account ID:</strong>
                    ${data.id}
                </p>

                <p>
                    <strong>Role:</strong>
                    ${data.role}
                </p>

                <button id="close-profile-button">
                    Close
                </button>

            </div>
        `;
        profileResult.innerHTML += domInput;

        profileResult.style.display = "block";

        document
            .getElementById("close-profile-button")
            .addEventListener("click", () => {

                profileResult.style.display = "none";
                profileResult.innerHTML = "";

            });

    } catch (error) {

        console.error(
            "Profile request failed:",
            error
        );

    }

});


/* =========================
   TRANSFERS
   ========================= */

const transferButton =
    document.getElementById("transfer-button");

const transferResult =
    document.getElementById("transfer-result");

const sendTransferButton =
    document.getElementById("send-transfer-button");

const transferMessage =
    document.getElementById("transfer-message");


/* OPEN TRANSFERS */

transferButton.addEventListener("click", () => {

    transferResult.style.display = "block";

    if (!document.getElementById("close-transfer-button")) {

        const closeButton =
            document.createElement("button");

        closeButton.id =
            "close-transfer-button";

        closeButton.textContent =
            "Close";

        closeButton.addEventListener("click", () => {

            transferResult.style.display = "none";

            transferResult.innerHTML = `
                <div class="form-group">
                    <label for="recipient-id">
                        Recipient ID
                    </label>

                    <input
                        type="number"
                        id="recipient-id"
                        placeholder="Enter recipient ID"
                        min="1"
                    >
                </div>

                <div class="form-group">
                    <label for="transfer-amount">
                        Amount
                    </label>

                    <input
                        type="number"
                        id="transfer-amount"
                        placeholder="0.00"
                        min="0.01"
                        step="0.01"
                    >
                </div>

                <div class="form-group">
                    <label for="transfer-description">
                        Description
                    </label>

                    <input
                        type="text"
                        id="transfer-description"
                        placeholder="e.g. Rent"
                    >
                </div>

                <button id="send-transfer-button">
                    Send Transfer
                </button>

                <p id="transfer-message"></p>
            `;

        });

        transferResult.appendChild(closeButton);

    }

});


/* SEND TRANSFER */

sendTransferButton.addEventListener(
    "click",
    async () => {

        const recipientId =
            document.getElementById(
                "recipient-id"
            ).value;

        const amount =
            document.getElementById(
                "transfer-amount"
            ).value;

        const description =
            document.getElementById(
                "transfer-description"
            ).value;

        transferMessage.textContent =
            "Processing transfer...";

        try {

            const response = await fetch(
                `${API_BASE_URL}/transactions`,
                {
                    method: "POST",

                    headers: {
                        "Content-Type":
                            "application/json",

                        "Authorization":
                            `Bearer ${token}`
                    },

                    body: JSON.stringify({
                        recipientId:
                            Number(recipientId),

                        amount:
                            Number(amount),

                        description:
                            description
                    })
                }
            );

            const data =
                await response.json();

            if (!response.ok) {

                transferMessage.textContent =
                    data.message ||
                    "Transfer failed.";

                return;
            }

            transferMessage.textContent =
                `Transfer successful. Transaction ID: ${data.id}`;

            console.log(
                "Transaction:",
                data
            );

        } catch (error) {

            console.error(
                "Transfer failed:",
                error
            );

            transferMessage.textContent =
                "Could not connect to NovaBank.";

        }

    }
);
/* =========================
   TRANSACTIONS
   ========================= */

const transactionsButton =
    document.getElementById("transactions-button");

const transactionsResult =
    document.getElementById("transactions-result");

transactionsButton.addEventListener("click", async () => {

    transactionsResult.style.display = "block";

    transactionsResult.innerHTML = `
        <div class="transactions-header">
            <h4>Recent Transactions</h4>
            <button id="close-transactions-button">
                Close
            </button>
        </div>

        <p class="transactions-loading">
            Loading transactions...
        </p>
    `;

    document
        .getElementById("close-transactions-button")
        .addEventListener("click", () => {

            transactionsResult.style.display = "none";
            transactionsResult.innerHTML = "";

        });

    try {

        const response = await fetch(
            `${API_BASE_URL}/transactions`,
            {
                headers: {
                    "Authorization": `Bearer ${token}`
                }
            }
        );

        const transactions =
            await response.json();

        if (!response.ok) {

            transactionsResult.innerHTML = `
                <div class="transactions-header">
                    <h4>Recent Transactions</h4>
                    <button id="close-transactions-button">
                        Close
                    </button>
                </div>

                <p>
                    Could not load transactions.
                </p>
            `;

            document
                .getElementById("close-transactions-button")
                .addEventListener("click", () => {

                    transactionsResult.style.display = "none";
                    transactionsResult.innerHTML = "";

                });

            return;
        }

        if (transactions.length === 0) {

            transactionsResult.innerHTML = `
                <div class="transactions-header">
                    <h4>Recent Transactions</h4>
                    <button id="close-transactions-button">
                        Close
                    </button>
                </div>

                <p>
                    No transactions found.
                </p>
            `;

            document
                .getElementById("close-transactions-button")
                .addEventListener("click", () => {

                    transactionsResult.style.display = "none";
                    transactionsResult.innerHTML = "";

                });

            return;
        }

        transactionsResult.innerHTML = `
            <div class="transactions-header">

                <div>
                    <h4>Recent Transactions</h4>
                    <p>
                        Your latest banking activity
                    </p>
                </div>

                <button id="close-transactions-button">
                    Close
                </button>

            </div>

            <div class="transaction-list">

                ${transactions.map(transaction => `

                    <div class="transaction-item">

                        <div class="transaction-main">

                            <div class="transaction-icon">
                                ↔
                            </div>

                            <div>

                                <strong>
                                    ${transaction.description}
                                </strong>

                                <span>
                                    Transaction #${transaction.id}
                                </span>

                            </div>

                        </div>

                        <div class="transaction-details">

                            <span>
                                From #${transaction.senderId}
                            </span>

                            <span>
                                To #${transaction.recipientId}
                            </span>

                        </div>

                        <div class="transaction-amount">
                            €${Number(transaction.amount).toFixed(2)}
                        </div>

                    </div>

                `).join("")}

            </div>
        `;

        document
            .getElementById("close-transactions-button")
            .addEventListener("click", () => {

                transactionsResult.style.display = "none";
                transactionsResult.innerHTML = "";

            });

    } catch (error) {

        console.error(
            "Transactions request failed:",
            error
        );

        transactionsResult.innerHTML = `
            <div class="transactions-header">

                <h4>Recent Transactions</h4>

                <button id="close-transactions-button">
                    Close
                </button>

            </div>

            <p>
                Could not connect to NovaBank.
            </p>
        `;

        document
            .getElementById("close-transactions-button")
            .addEventListener("click", () => {

                transactionsResult.style.display = "none";
                transactionsResult.innerHTML = "";

            });

    }

});
/* =========================
   SECURITY CENTER
   ========================= */

const securityButton =
    document.getElementById("security-button");

const securityResult =
    document.getElementById("security-result");

securityButton.addEventListener("click", () => {

    securityResult.style.display = "block";

    securityResult.innerHTML = `
        <div class="security-panel">

            <div class="security-panel-header">

                <div>
                    <h4>Security Center</h4>

                    <p>
                        Manage your account security.
                    </p>
                </div>

                <button id="close-security-button">
                    Close
                </button>

            </div>


            <div class="security-item">

                <div>
                    <strong>Password</strong>

                    <span>
                        Your account password
                    </span>
                </div>

                <span class="security-status">
                    Protected
                </span>

            </div>


            <div class="security-item">

                <div>
                    <strong>Authentication</strong>

                    <span>
                        JWT-based authentication
                    </span>
                </div>

                <span class="security-status">
                    Active
                </span>

            </div>


            <div class="security-item">

                <div>
                    <strong>Session</strong>

                    <span>
                        Current authenticated session
                    </span>
                </div>

                <span class="security-status">
                    Active
                </span>

            </div>


            <div class="security-item">

                <div>
                    <strong>Uploaded Documents</strong>

                    <span>
                        Identity documents and bank statements
                    </span>
                </div>

                <button id="upload-document-button">
                    Upload Document
                </button>

            </div>


            <input
                type="file"
                id="document-file-input"
                style="display: none;"
            >

        </div>
    `;

 document
        .getElementById("upload-document-button")
        .addEventListener("click", () => {

            document
                .getElementById("document-file-input")
                .click();

        });


 document
    .getElementById("document-file-input")
    .addEventListener("change", async () => {

        const fileInput =
            document.getElementById("document-file-input");

        const file = fileInput.files[0];

        if (!file) {
            return;
        }

        const formData = new FormData();

        formData.append("file", file);

        try {

            const response = await fetch(
                "http://localhost:5066/api/users/upload",
                {
                    method: "POST",
                    headers: {
                        "Authorization": `Bearer ${token}`
                    },
                    body: formData
                }
            );

            const result = await response.json();

            console.log("Upload response:", result);

            alert(
                `File uploaded successfully:\n${result.fileName}`
            );

        } catch (error) {

            console.error("Upload failed:", error);

            alert("Upload failed.");

        }

    });

    document
        .getElementById("close-security-button")
        .addEventListener("click", () => {

            securityResult.style.display = "none";
            securityResult.innerHTML = "";

        });

});
   
/*LOGIN
   ========================= */

loginForm.addEventListener(
    "submit",
    async (event) => {

        event.preventDefault();

        const username =
            document.getElementById(
                "username"
            ).value;

        const password =
            document.getElementById(
                "password"
            ).value;

        loginMessage.textContent =
            "Signing in...";

        try {

            const response =
                await fetch(
                    `${API_BASE_URL}/auth/login`,
                    {
                        method: "POST",

                        headers: {
                            "Content-Type":
                                "application/json"
                        },

                        body: JSON.stringify({
                            username:
                                username,

                            password:
                                password
                        })
                    }
                );

            if (!response.ok) {

                loginMessage.textContent =
                    "Invalid username or password.";

                return;
            }

            const data =
                await response.json();

            token = data.token;

            console.log(
                "JWT:",
                token
            );

            const payload =
                JSON.parse(
                    atob(
                        token.split(".")[1]
                    )
                );

            currentUserId =
                payload[
                    "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"
                ];

            console.log(
                "Current User ID:",
                currentUserId
            );

            /* Switch to dashboard */

            authPage.style.display =
                "none";

            dashboard.style.display =
                "block";

        } catch (error) {

            console.error(error);

            loginMessage.textContent =
                "Could not connect to NovaBank.";

        }

    }
);


/* =========================
   LOGOUT
   ========================= */

logoutButton.addEventListener(
    "click",
    () => {

        token = null;
        currentUserId = null;

        dashboard.style.display =
            "none";

        authPage.style.display =
            "flex";

        loginForm.reset();

        loginMessage.textContent = "";

    }
);