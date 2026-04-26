import bootstrap from "bootstrap";
import DOMPurify from "dompurify";
import Swal, { type SweetAlertOptions } from "sweetalert2";

import type { RequestCartProductQty } from "../types/products.ts";

import cartsService from "../api/cartsService.js";
import { getCurrProdDetails } from "../api/productsService.js";
import { isValidQty } from "../utils/dataValidationUtils.js";
import { showErrorSwal } from "../utils/domUtils.js";

const addToCartButtons = document.querySelectorAll(".add-to-cart-btn");
const popoverTemplateEl = document.querySelector<HTMLDivElement>(".choose-qty-popover")!;

addToCartButtons.forEach(addToCartBtn => {
    const popoverObj = new bootstrap.Popover(addToCartBtn, {
        content: () => popoverTemplateEl.innerHTML,
        placement: "top",
        container: ".products-container",
        trigger: "manual",
        html: true,
        sanitize: false,
        sanitizeFn: ((content: string) => DOMPurify.sanitize(content)) as unknown as () => void, // => missmatch between the bootstrap impl and the types for bootstrap.
    });

    addToCartBtn.addEventListener("click", async (event: Event) => {
        event.preventDefault();

        const productId = addToCartBtn.getAttribute("data-product-id") as string;

        const currProdDetails = await getCurrProdDetails(productId);
        if(!currProdDetails)  {
            return;
        }

        popoverTemplateEl.querySelector<HTMLSpanElement>(".curr-prod-qty")!.textContent
            = currProdDetails.quantityInStock as unknown as string;

        const prodInCart = await cartsService.getProdInCart(productId);
        if(!prodInCart) {
            return;
        }

        const currQtyInCartContainer = popoverTemplateEl
            .querySelector<HTMLParagraphElement>(".curr-qty-in-cart")!;
        if(prodInCart.quantityAdded > 0) {
            currQtyInCartContainer.textContent
                = `Current quantity in cart: ${prodInCart.quantityAdded}`;
        } else {
            currQtyInCartContainer.textContent = "";
        }

        popoverObj.show();
    });

    addToCartBtn.addEventListener("shown.bs.popover", () => {
        const popoverId = addToCartBtn.getAttribute("aria-describedby") as string;
        const injectedPopoverInstanceEl = document
            .querySelector<HTMLDivElement>(`#${popoverId}`)!;

        const closePopoverBtn = injectedPopoverInstanceEl
            .querySelector<HTMLButtonElement>(".close-popover")!;

        const submitBtn = injectedPopoverInstanceEl
            .querySelector<HTMLButtonElement>(".submit-btn")!;

        const qtyInputField = injectedPopoverInstanceEl.querySelector<HTMLInputElement>(".qty-input")!;
        const qtyInputFieldValidationMessagesContainer = injectedPopoverInstanceEl
            .querySelector<HTMLParagraphElement>(".validation-error-container")!;

        const currProductQtyInStock = Number(injectedPopoverInstanceEl
            .querySelector(".curr-prod-qty")!.textContent);
        const currProductQtyInCart = Number(injectedPopoverInstanceEl
            .querySelector(".curr-qty-in-cart")?.textContent
            .split(": ")
            .pop());

        if(currProductQtyInStock - currProductQtyInCart === 0) {
            injectedPopoverInstanceEl.querySelector(".input-label")!
                .classList.add("text-muted");
            qtyInputField!.setAttribute("disabled", "true");

            submitBtn?.setAttribute("disabled", "true");
            submitBtn!.style.opacity="0.8";

            qtyInputFieldValidationMessagesContainer.textContent
                = "You can not add more of this product to your cart."

            closePopoverBtn.addEventListener("click", () => popoverObj.hide());
            return;
        }

        if(!isValidQty(Number(qtyInputField.value), currProductQtyInStock - currProductQtyInCart)) {
            submitBtn?.setAttribute("disabled", "true");
            submitBtn.style.opacity="0.8";
        }

        closePopoverBtn.addEventListener("click", () => popoverObj.hide());

        qtyInputField.addEventListener("input", () => {
            if(!isValidQty(Number(qtyInputField.value), currProductQtyInStock - currProductQtyInCart)) {
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
            } as RequestCartProductQty;

            const requestVerificationToken = document
                .querySelectorAll<HTMLInputElement>("input[name=__RequestVerificationToken]")[0]!
                .value;

            const success = await cartsService
                .addToCart(requestedProductQtyPayload, requestVerificationToken);
            if(!success) {
                await showErrorSwal();
                return;
            }

            Swal.fire({
                icon: "success",
                title: "Congratulations!",
                text: "Product added to your cart successfully!",
                draggable: true,
                confirmButtonColor: "#0FAF9A",
                confirmButtonText: "Continue browsing products",
                denyButtonColor: "#198754",
                denyButtonText: "Go to cart",
                showDenyButton: true,
                showClass: { popup: ` animate__animated animate__fadeInUp animate__faster ` },
                hideClass: { popup: ` animate__animated animate__fadeOutDown animate__faster ` },
            } as SweetAlertOptions).then((result) => {
                if (result.isConfirmed)
                    window.location.href = "/Catalog";
                else if (result.isDenied)
                    window.location.href = "/Cart";
            });
        });
    });
});