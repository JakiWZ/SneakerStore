function buildCartRow(item) {
  const row = document.createElement("div");
  row.className = "cart-row";

  const info = document.createElement("div");
  info.className = "cart-row-info";
  const name = document.createElement("p");
  name.className = "cart-row-name";
  name.textContent = item.name;
  const brand = document.createElement("p");
  brand.textContent = item.brand;
  info.append(name, brand);

  const qty = document.createElement("input");
  qty.type = "number";
  qty.min = "0";
  qty.value = item.quantity;
  qty.className = "cart-qty";
  qty.addEventListener("change", async () => {
    const quantity = Number(qty.value) || 0;
    await fetch(`/api/Cart/${item.sneakerId}`, {
      method: "PUT",
      headers: { "Content-Type": "application/json", ...authHeaders() },
      body: JSON.stringify({ quantity }),
    });
    await loadCartPage();
  });

  const lineTotal = document.createElement("p");
  lineTotal.className = "cart-row-total mono";
  lineTotal.textContent = `€ ${Number(item.lineTotal).toFixed(2)}`;

  const removeBtn = document.createElement("button");
  removeBtn.className = "btn-danger";
  removeBtn.textContent = "Remove";
  removeBtn.addEventListener("click", async () => {
    await fetch(`/api/Cart/${item.sneakerId}`, { method: "DELETE", headers: authHeaders() });
    await loadCartPage();
  });

  row.append(info, qty, lineTotal, removeBtn);
  return row;
}

async function loadCartPage() {
  const container = document.getElementById("cartContainer");
  if (!container) return;

  const res = await fetch("/api/Cart", { headers: authHeaders() });
  const cart = await res.json();

  container.textContent = "";

  if (cart.items.length === 0) {
    const empty = document.createElement("p");
    empty.textContent = "Your cart is empty.";
    container.append(empty);
    return;
  }

  cart.items.forEach((item) => container.append(buildCartRow(item)));

  const barcode = document.createElement("div");
  barcode.className = "barcode";
  container.append(barcode);

  const totalRow = document.createElement("div");
  totalRow.className = "cart-total";
  totalRow.textContent = `Total: € ${Number(cart.total).toFixed(2)}`;
  container.append(totalRow);

  const clearBtn = document.createElement("button");
  clearBtn.className = "btn-secondary";
  clearBtn.textContent = "Clear cart";
  clearBtn.addEventListener("click", async () => {
    await fetch("/api/Cart", { method: "DELETE", headers: authHeaders() });
    await loadCartPage();
  });
  container.append(clearBtn);
}

document.addEventListener("DOMContentLoaded", () => {
  renderNav();
  loadCartPage();
});
