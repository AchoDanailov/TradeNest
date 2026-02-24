const MIN_QTY = 1;

const prodValidationEl = document.querySelector("#prod-qty-validation>small");
const availableQtyOnUi = Number(document.querySelector("#available-quantity-ui")
    ?.textContent?.trim()?.split(' ')?.shift());
const prodQtyInputEl = document.querySelector("#quantityInput");
const submitBtnEl = document.querySelector("#add-to-order-btn");
const formEl = document.querySelector("#add-to-order-form");

prodQtyInputEl?.addEventListener("change", (e) => {
    if(prodQtyInputEl.value < MIN_QTY || prodQtyInputEl.value > availableQtyOnUi) {
        prodValidationEl.textContent = "Invalid quantity.";

        submitBtnEl?.setAttribute("disabled", "true");
        submitBtnEl.style.opacity="0.8";
    } else {
        prodValidationEl.textContent = "";

        submitBtnEl?.removeAttribute("disabled");
        submitBtnEl.style.opacity="1";
    }
});
    
formEl?.addEventListener("submit", async (e) => {
    e.preventDefault();
    
    const formData = new FormData(e.target);
    const id = formData.get("id");
    const qty = formData.get("quantity");
    const returnUrl = formData.get("returnUrl");

    try {
        const servRes = await fetch(`/Orders/VerifyProdQty`, {
            method: "POST",
            headers: {
                "Content-Type": "application/json",
            },
            body: JSON.stringify({
                Id: id,
                Quantity: qty,
                ReturnUrl: returnUrl,
            }),
        });
        if(!servRes.ok) {
            throw new Error(servRes);
        }

        const isValidProdQty = await servRes.json();
        if(isValidProdQty === false) {
            prodValidationEl.textContent
                = "Not enough quantity. It looks like you have already added to your order some of the last available quantity.";

            return;
        }

        e.target.submit();
    } catch (err) {
        console.error("Remote validation error.", err);
        e.target.submit()
    }
});