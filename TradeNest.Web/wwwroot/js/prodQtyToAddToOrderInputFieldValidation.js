const MIN_QTY = 1;
    
const prodQtyInputEl = document.querySelector("#quantityInput");
const prodValidationEl = document.querySelector("#prod-qty-validation>small");
const submitBtn = document.querySelector("#prod-add-to-order");

const availableAmount = Number(document.querySelector("#available-quantity")
    .textContent.trim().split(' ').shift());

prodQtyInputEl?.addEventListener("change", (e) => {
    if(prodQtyInputEl.value < MIN_QTY || prodQtyInputEl.value > availableAmount) {
        prodValidationEl.textContent
            = "Invalid quantity.";

        submitBtn.setAttribute("disabled", "true");
        submitBtn.style.opacity="0.8";
    } else {
        prodValidationEl.textContent = "";

        submitBtn.removeAttribute("disabled");
        submitBtn.style.opacity="1";
    }
});