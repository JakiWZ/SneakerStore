function buildFavoriteCard(sneaker) {
  const card = document.createElement("div");
  card.className = "card";

  const title = document.createElement("h3");
  const link = document.createElement("a");
  link.href = `product.html?id=${sneaker.id}`;
  link.textContent = sneaker.name;
  title.append(link);

  const brand = document.createElement("p");
  brand.textContent = `${sneaker.brand} — ${sneaker.model}`;

  const price = document.createElement("p");
  price.className = "price";
  price.textContent = `€ ${Number(sneaker.price).toFixed(2)}`;

  const barcode = document.createElement("div");
  barcode.className = "barcode";

  const actions = document.createElement("div");
  actions.className = "card-actions";
  const removeBtn = document.createElement("button");
  removeBtn.className = "btn-danger";
  removeBtn.textContent = "Remove from favorites";
  removeBtn.addEventListener("click", async () => {
    await fetch(`/api/Favorite/${sneaker.id}`, { method: "DELETE", headers: authHeaders() });
    await loadFavoritesPage();
  });
  actions.append(removeBtn);

  card.append(title, brand, price, barcode, actions);
  return card;
}

async function loadFavoritesPage() {
  const grid = document.getElementById("favoritesGrid");
  if (!grid) return;

  const res = await fetch("/api/Favorite", { headers: authHeaders() });
  const favorites = await res.json();

  grid.textContent = "";

  if (favorites.length === 0) {
    const empty = document.createElement("p");
    empty.textContent = "No favorites yet.";
    grid.append(empty);
    return;
  }

  favorites.forEach((sneaker) => grid.append(buildFavoriteCard(sneaker)));
}

document.addEventListener("DOMContentLoaded", () => {
  renderNav();
  loadFavoritesPage();
});
