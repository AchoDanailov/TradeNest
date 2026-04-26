const MIN_QTY = 1;

const prodValidationEl = document.querySelector("#prod-qty-validation>small");
const availableQtyOnUi = Number(document.querySelector("#available-quantity-ui")
    ?.textContent?.trim()?.split(' ')?.shift());
const prodQtyInputEl = document.querySelector<HTMLInputElement>("#quantityInput")!;
const submitBtnEl = document.querySelector<HTMLButtonElement>("#add-to-order-btn")!;
const formEl = document.querySelector<HTMLFormElement>("#add-to-order-form")!;

prodQtyInputEl?.addEventListener("change", () => {
    if (Number(prodQtyInputEl.value) < MIN_QTY ||
        Number(prodQtyInputEl.value) > availableQtyOnUi
    ) {
        prodValidationEl!.textContent = "Invalid quantity.";

        submitBtnEl.setAttribute("disabled", "true");
        submitBtnEl.style.opacity="0.8";
    } else {
        prodValidationEl!.textContent = "";

        submitBtnEl.removeAttribute("disabled");
        submitBtnEl.style.opacity="1";
    }
});