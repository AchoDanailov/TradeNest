import getCurrProdQty from "./getCurrProdQty.js";
import { isValidQty } from "./dataValidationUtils.js";

const addToCartButtons = document.querySelectorAll(".add-to-cart-btn");
const popoverTemplateEl = document.querySelector(".choose-qty-popover");

addToCartButtons.forEach(addToCartBtn => {
    const popoverObj = new bootstrap.Popover(addToCartBtn, {
        content: () => popoverTemplateEl.innerHTML,
        placement: "top",
        container: ".products-container",
        trigger: "manual",
        html: true,
        sanitize: true,
        sanitizeFn: (popoverContent) => DOMPurify.sanitize(popoverContent),
    });
    
    addToCartBtn.addEventListener("click", async (e) => {
        e.preventDefault();
        
        const productId = addToCartBtn.getAttribute("data-product-id");
        
        const currProdQty = await getCurrProdQty(productId);
        if(currProdQty < 0) return;
        popoverTemplateEl.querySelector(".curr-prod-qty").textContent = currProdQty;

        const currProdQtyInCart = await getCurrProdQtyInCart(productId);
        const currQtyInCartContainer = popoverTemplateEl.querySelector(".curr-qty-in-cart");
        if(currProdQtyInCart > 0) {
            currQtyInCartContainer.textContent
                = `Current quantity in cart: ${currProdQtyInCart}`;
        } else {
            currQtyInCartContainer.textContent = "";
        }

        popoverObj.show();
    });

    addToCartBtn.addEventListener("shown.bs.popover", () => {
        const popoverId = addToCartBtn.getAttribute("aria-describedby"); 
        const injectedPopoverInstanceEl = document.getElementById(popoverId);
        
        const closePopoverBtn = injectedPopoverInstanceEl.querySelector(".close-popover");
        
        const submitBtn = injectedPopoverInstanceEl.querySelector(".submit-btn");
        submitBtn?.setAttribute("disabled", "true");
        submitBtn.style.opacity="0.8";
        
        const qtyInputField = injectedPopoverInstanceEl.querySelector(".qty-input");
        const qtyInputFieldValidationMessagesContainer = injectedPopoverInstanceEl
            .querySelector(".validation-error-container");
        
        const currProductQty = Number(injectedPopoverInstanceEl
            .querySelector(".curr-prod-qty").textContent);
        const currProductQtyInCart = Number(injectedPopoverInstanceEl
            .querySelector(".curr-qty-in-cart")?.textContent
            .split(": ")
            .pop());

        
        closePopoverBtn.addEventListener("click", () => popoverObj.hide());

        qtyInputField.addEventListener("input", () => {
            if(!isValidQty(Number(qtyInputField.value), currProductQty - currProductQtyInCart)) {
                qtyInputFieldValidationMessagesContainer.textContent = "Invalid quantity!";
                submitBtn?.setAttribute("disabled", "true");
                submitBtn.style.opacity="0.8";
            } else {
                qtyInputFieldValidationMessagesContainer.textContent = "";
                submitBtn?.removeAttribute("disabled");
                submitBtn.style.opacity="1";
            }
        });
        
        submitBtn.addEventListener("click", async (e) => {
            e.preventDefault();
            e.stopPropagation();
            
            popoverObj.hide();
            
            const productId = addToCartBtn.getAttribute("data-product-id");
            
            const requestedProductQtyPayload = {
                productId: productId,
                quantity: Number(qtyInputField.value),
            };

            const success = await addToCart(requestedProductQtyPayload);
            if(!success) {
                Swal.fire({
                    icon: "error",
                    title: "Oops...",
                    text: "Something went wrong! Please try again.",
                    draggable: true,
                    showClass: { popup: ` animate__animated animate__fadeInUp animate__faster ` },
                    hideClass: { popup: ` animate__animated animate__fadeOutDown animate__faster ` }
                });
                return;
            }

            Swal.fire({
                icon: "success",
                title: "Congratulations!",
                text: "Product added to your cart successfully!",
                draggable: true,
                confirmButtonColor: "#0FAF9A",
                showClass: { popup: ` animate__animated animate__fadeInUp animate__faster ` },
                hideClass: { popup: ` animate__animated animate__fadeOutDown animate__faster ` },
            }).then(() => {
                window.location.href = "/Catalog";
            });
        });
    });
});

async function getCurrProdQtyInCart(productId) {
    try {
        const res = await fetch(`/api/v1/cart/cartProducts/${productId}`);
        if(!res.ok)
            throw new Error(await res.json());
        
        const cartProductData = await res.json();
        return cartProductData.quantityAdded;
    } catch (err) {
        console.error("Error fetching product qty from user's cart.", err.status);
        return -1;
    }
}

async function addToCart(requestedProductQtyPayload) {
    try {
        const res = await fetch("/api/v1/cart/addProduct", {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify(requestedProductQtyPayload)
        });
        if(!res.ok) 
            throw new Error(await res.json());
            
        return await res.json();
    } catch (err)
    {
        console.error("Error adding product qty to user's cart.", err.status);
        return false;
    }
}