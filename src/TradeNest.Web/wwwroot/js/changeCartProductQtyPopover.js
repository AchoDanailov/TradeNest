import getCurrProdQty from "./getCurrProdQty.js";
import { isValidQty } from "./dataValidationUtils.js";

const changeQtyButtonEls = document.querySelectorAll(".change-qty-button");
const popoverTemplateEl = document.querySelector(".change-qty-popover");

changeQtyButtonEls.forEach(changeQtyBtn => {
    const popoverObj = new bootstrap.Popover(changeQtyBtn, {
        content: () => popoverTemplateEl.innerHTML,
        placement: "top",
        container: ".cart-container",
        trigger: "manual",
        html: true,
        sanitize: true,
        sanitizeFn: (popoverContent) => DOMPurify.sanitize(popoverContent),
    });

    changeQtyBtn.addEventListener("click", async (e) => {
        e.preventDefault();
        
        const productId = changeQtyBtn.getAttribute("data-product-id");
        const currProdQty = await getCurrProdQty(productId);
        if(currProdQty < 0) {
            return;
        }

        const currProductQtySpan = popoverTemplateEl.querySelector(".curr-prod-qty");
        currProductQtySpan.textContent = currProdQty;

        popoverObj.show();
    });

    // shown.bs.popover => bootstrap popover event triggered on the popover instance being shown
    changeQtyBtn.addEventListener("shown.bs.popover", () => {
        const popoverId = changeQtyBtn.getAttribute("aria-describedby"); // => generated popover instance id on the DOM
        const injectedPopoverInstanceEl = document.getElementById(popoverId);
        
        const closePopoverBtn = injectedPopoverInstanceEl.querySelector(".close-popover");
        const saveChangesBtn = injectedPopoverInstanceEl.querySelector(".save-changes-btn");
        
        const qtyInputField = injectedPopoverInstanceEl.querySelector(".qty-input");
        qtyInputField.value = Number(changeQtyBtn.closest(".cart-product")
            .querySelector(".quantity-already-added").textContent
            .split(": ").pop());
        
        const qtyInputFieldValidationMessagesContainer = injectedPopoverInstanceEl
            .querySelector(".validation-error-container");
        
        const currProductQty = Number(injectedPopoverInstanceEl
            .querySelector(".curr-prod-qty").textContent);
        
        if(!isValidQty(Number(qtyInputField.value), currProductQty)) {
            saveChangesBtn?.setAttribute("disabled", "true");
            saveChangesBtn.style.opacity = "0.8";
        }
        

        closePopoverBtn.addEventListener("click", () => popoverObj.hide());

        qtyInputField.addEventListener("input", () => {
            if(!isValidQty(Number(qtyInputField.value), currProductQty)) {
                qtyInputFieldValidationMessagesContainer.textContent = "Invalid quantity!";
                saveChangesBtn?.setAttribute("disabled", "true");
                saveChangesBtn.style.opacity="0.8";
            } else {
                qtyInputFieldValidationMessagesContainer.textContent = "";
                saveChangesBtn?.removeAttribute("disabled");
                saveChangesBtn.style.opacity="1";
            }
        });
        
        saveChangesBtn.addEventListener("click", async (e) => {
            e.preventDefault();
            e.stopPropagation();
            
            const productId = changeQtyBtn.getAttribute("data-product-id");
            const cartId = changeQtyBtn.getAttribute("data-cart-id");
            
            const updatedCartProdPayload = {
                cartId: cartId,
                productId: productId,
                quantity: Number(qtyInputField.value),
            };

            const success = await saveChanges(updatedCartProdPayload);
            if(!success) {
                Swal.fire({
                    icon: "error",
                    title: "Oops...",
                    text: "Something went wrong! Please try again.",
                    draggable: true,
                    confirmButtonColor: "#0FAF9A",
                    showClass: { popup: ` animate__animated animate__fadeInUp animate__faster ` },
                    hideClass: { popup: ` animate__animated animate__fadeOutDown animate__faster ` }
                });
                return;
            }
            
            document.location.href = "/Cart";
        });
    });
});

async function saveChanges(updatedCartProdPayload) {
    try {
        const res = await fetch(
            `/api/v1/cart/${updatedCartProdPayload.cartId}?productId=${updatedCartProdPayload.productId}`, 
            {
                method: "PUT",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify(updatedCartProdPayload),
            }
        );
        if(!res.ok) 
            throw new Error(await res.json());
        
        return await res.json();
    } catch (err) {
        console.error("Error saving new cart product qty.", err.status);
        return false;
    }
}