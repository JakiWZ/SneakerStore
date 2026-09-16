function buildSneakerCard(sneaker, favoriteIds) {
  const card = document.createElement("div");
  card.className = "card";

  const title = document.createElement("h3");
  const link = document.createElement("a");
  link.href = `product.html?id=${sneaker.id}`;
  link.textContent = sneaker.name;
  title.append(link);

  const brand = document.createElement("p");
  brand.textContent = `${sneaker.brand} — ${sneaker.model}`;

  const size = document.createElement("p");
  size.textContent = `Size: ${sneaker.size}`;

  const price = document.createElement("p");
  price.className = "price";
  price.textContent = `€ ${Number(sneaker.price).toFixed(2)}`;

  const barcode = document.createElement("div");
  barcode.className = "barcode";

  card.append(title, brand, size, price, barcode);

  if (isAdmin()) {
    const actions = document.createElement("div");
    actions.className = "card-actions";

    const editBtn = document.createElement("button");
    editBtn.className = "btn-secondary";
    editBtn.textContent = "Edit";
    editBtn.addEventListener("click", () => openEditForm(sneaker));

    const deleteBtn = document.createElement("button");
    deleteBtn.className = "btn-danger";
    deleteBtn.textContent = "Delete";
    deleteBtn.addEventListener("click", () => deleteSneaker(sneaker.id));

    actions.append(editBtn, deleteBtn);
    card.append(actions);
  }

  if (isUser()) {
    const actions = document.createElement("div");
    actions.className = "card-actions";

    const cartBtn = document.createElement("button");
    cartBtn.className = "btn-primary";
    cartBtn.textContent = "Add to cart";
    cartBtn.addEventListener("click", async () => {
      await fetch("/api/Cart", {
        method: "POST",
        headers: { "Content-Type": "application/json", ...authHeaders() },
        body: JSON.stringify({ sneakerId: sneaker.id, quantity: 1 }),
      });
      cartBtn.textContent = "Added";
      setTimeout(() => (cartBtn.textContent = "Add to cart"), 1200);
    });

    const favBtn = document.createElement("button");
    const isFavorite = favoriteIds && favoriteIds.has(sneaker.id);
    favBtn.className = "btn-secondary";
    favBtn.textContent = isFavorite ? "♥ Favorited" : "♡ Favorite";
    favBtn.addEventListener("click", async () => {
      if (favBtn.textContent.includes("♥")) {
        await fetch(`/api/Favorite/${sneaker.id}`, { method: "DELETE", headers: authHeaders() });
        favBtn.textContent = "♡ Favorite";
      } else {
        await fetch(`/api/Favorite/${sneaker.id}`, { method: "POST", headers: authHeaders() });
        favBtn.textContent = "♥ Favorited";
      }
    });

    actions.append(cartBtn, favBtn);
    card.append(actions);
  }

  return card;
}

async function loadCatalog() {
  const grid = document.getElementById("catalogGrid");
  if (!grid) return;
  grid.textContent = "";

  const res = await fetch("/api/Sneaker");
  const sneakers = await res.json();

  let favoriteIds = new Set();
  if (isUser()) {
    const favRes = await fetch("/api/Favorite", { headers: authHeaders() });
    if (favRes.ok) {
      const favorites = await favRes.json();
      favoriteIds = new Set(favorites.map((f) => f.id));
    }
  }

  sneakers.forEach((sneaker) => grid.append(buildSneakerCard(sneaker, favoriteIds)));

  const addSection = document.getElementById("adminAddSection");
  if (addSection) addSection.classList.toggle("hidden", !isAdmin());
}

function readForm() {
  return {
    name: document.getElementById("formName").value,
    brand: document.getElementById("formBrand").value,
    model: document.getElementById("formModel").value,
    size: document.getElementById("formSize").value,
    quantityAvailable: Number(document.getElementById("formQuantity").value) || 0,
    price: Number(document.getElementById("formPrice").value) || 0,
  };
}

function clearForm() {
  document.getElementById("sneakerForm").reset();
  document.getElementById("formId").value = "";
}

function openEditForm(sneaker) {
  document.getElementById("formId").value = sneaker.id;
  document.getElementById("formName").value = sneaker.name;
  document.getElementById("formBrand").value = sneaker.brand;
  document.getElementById("formModel").value = sneaker.model;
  document.getElementById("formSize").value = sneaker.size;
  document.getElementById("formQuantity").value = sneaker.quantityAvailable;
  document.getElementById("formPrice").value = sneaker.price;
  window.scrollTo({ top: 0, behavior: "smooth" });
}

async function submitForm(event) {
  event.preventDefault();
  const id = document.getElementById("formId").value;
  const body = readForm();

  const headers = { "Content-Type": "application/json", ...authHeaders() };

  if (id) {
    await fetch(`/api/Sneaker/${id}`, { method: "PUT", headers, body: JSON.stringify(body) });
  } else {
    await fetch("/api/Sneaker", { method: "POST", headers, body: JSON.stringify(body) });
  }

  clearForm();
  await loadCatalog();
}

async function deleteSneaker(id) {
  await fetch(`/api/Sneaker/${id}`, { method: "DELETE", headers: authHeaders() });
  await loadCatalog();
}

function getQueryParam(name) {
  return new URLSearchParams(window.location.search).get(name);
}

async function loadProductDetail() {
  const container = document.getElementById("productDetail");
  if (!container) return;

  const id = getQueryParam("id");
  const res = await fetch(`/api/Sneaker/${id}`);

  const body = document.createElement("div");
  body.className = "product-detail-body";

  if (!res.ok) {
    body.textContent = "Product not found.";
    container.textContent = "";
    container.append(body);
    return;
  }

  const sneaker = await res.json();
  container.textContent = "";

  const title = document.createElement("h2");
  title.textContent = sneaker.name;

  const dl = document.createElement("dl");
  const fields = [
    ["Brand", sneaker.brand],
    ["Model", sneaker.model],
    ["Size", sneaker.size],
    ["Quantity available", sneaker.quantityAvailable],
    ["Price", `€ ${Number(sneaker.price).toFixed(2)}`],
  ];

  fields.forEach(([label, value]) => {
    const dt = document.createElement("dt");
    dt.textContent = label;
    const dd = document.createElement("dd");
    dd.textContent = value;
    dl.append(dt, dd);
  });

  const barcode = document.createElement("div");
  barcode.className = "barcode";

  body.append(title, dl, barcode);
  container.append(body);
}

document.addEventListener("DOMContentLoaded", () => {
  renderNav();
  loadCatalog();
  loadProductDetail();

  const form = document.getElementById("sneakerForm");
  if (form) form.addEventListener("submit", submitForm);
});
