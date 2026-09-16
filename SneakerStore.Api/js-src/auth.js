function getSession() {
  const token = localStorage.getItem("token");
  const role = localStorage.getItem("role");
  const username = localStorage.getItem("username");
  if (!token) return null;
  return { token, role, username };
}

function isAdmin() {
  return localStorage.getItem("role") === "Admin";
}

function isUser() {
  return localStorage.getItem("role") === "User";
}

function saveSession(token, role, username) {
  localStorage.setItem("token", token);
  localStorage.setItem("role", role);
  localStorage.setItem("username", username);
}

function logout() {
  localStorage.removeItem("token");
  localStorage.removeItem("role");
  localStorage.removeItem("username");
  window.location.href = "index.html";
}

function authHeaders() {
  const session = getSession();
  if (!session) return {};
  return { Authorization: "Bearer " + session.token };
}

async function login(username, password) {
  const res = await fetch("/api/Auth/login", {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify({ username, password }),
  });
  const data = await res.json();
  if (!res.ok) throw new Error(data.error || "Login failed.");
  saveSession(data.token, data.role, data.username);
}

async function register(username, password) {
  const res = await fetch("/api/Auth/register", {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify({ username, password }),
  });
  const data = await res.json().catch(() => ({}));
  if (!res.ok) throw new Error(data.error || "Registration failed.");
}

async function createAdmin(username, password) {
  const res = await fetch("/api/Auth/create-admin", {
    method: "POST",
    headers: { "Content-Type": "application/json", ...authHeaders() },
    body: JSON.stringify({ username, password }),
  });
  const data = await res.json().catch(() => ({}));
  if (!res.ok) throw new Error(data.error || "Admin creation failed.");
}

function renderNav() {
  const nav = document.getElementById("nav");
  if (!nav) return;
  nav.textContent = "";
  const session = getSession();
  if (session) {
    const span = document.createElement("span");
    span.textContent = `${session.username} (${session.role})`;

    const logoutLink = document.createElement("a");
    logoutLink.href = "#";
    logoutLink.textContent = "Logout";
    logoutLink.addEventListener("click", (e) => {
      e.preventDefault();
      logout();
    });

    nav.append(span);

    if (isUser()) {
      const favoritesLink = document.createElement("a");
      favoritesLink.href = "favorites.html";
      favoritesLink.textContent = "Favorites";

      const cartLink = document.createElement("a");
      cartLink.href = "cart.html";
      cartLink.textContent = "Cart";

      nav.append(favoritesLink, cartLink);
    }

    nav.append(logoutLink);
  } else {
    const loginLink = document.createElement("a");
    loginLink.href = "login.html";
    loginLink.textContent = "Login";
    nav.append(loginLink);
  }
}
