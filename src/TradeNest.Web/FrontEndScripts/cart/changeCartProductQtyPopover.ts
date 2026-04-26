import { Popover } from "bootstrap";
import DOMPurify from "dompurify";

import type { UpdateCartProductQty } from "../types/products.ts";

import { updateCartProductQty } from "../api/cartsService.js";
import { getCurrProdDetails } from "../api/productsService.js";
import { isValidQty } from "../utils/dataValidationUtils.js";
import { showErrorSwal } from "../utils/domUtils.js";

const changeQtyButtonEls = document.querySelectorAll(".change-qty-button");
const popoverTemplateEl = document.querySelector<HTMLDivElement>(".change-qty-popover")!;

changeQtyButtonEls.forEach(changeQtyBtn => {
    const popoverObj = new Popover(changeQtyBtn, {
        content: () => popoverTemplateEl.innerHTML,
        placement: "top",
        container: ".cart-container",
        trigger: "manual",
        html: true,
        sanitize: true,
        sanitizeFn: ((content: string) => DOMPurify.sanitize(content)) as unknown as () => void, // => missmatch between the bootstrap impl and the types for bootstrap.
    });

    changeQtyBtn.addEventListener("click", async (e) => {
        e.preventDefault();

        const productId = changeQtyBtn.getAttribute("data-product-id");
        const currProdDetails = await getCurrProdDetails(productId as string);
        if(!currProdDetails) {
            return;
        }

        const currProductQtySpan = popoverTemplateEl.querySelector<HTMLSpanElement>(".curr-prod-qty")!;
        currProductQtySpan.textContent = currProdDetails.quantityInStock as unknown as string;

        popoverObj.show();
    });

    // shown.bs.popover => bootstrap popover event triggered on the popover instance being shown
    changeQtyBtn.addEventListener("shown.bs.popover", () => {
        const popoverId = changeQtyBtn.getAttribute("aria-describedby"); // => generated popover instance id on the DOM
        const injectedPopoverInstanceEl = document.querySelector<HTMLDivElement>(`#${popoverId}`)!;

        const closePopoverBtn = injectedPopoverInstanceEl
            .querySelector<HTMLButtonElement>(".close-popover")!;

        const saveChangesBtn = injectedPopoverInstanceEl
            .querySelector<HTMLButtonElement>(".save-changes-btn")!;

        const qtyInputField = injectedPopoverInstanceEl.querySelector<HTMLInputElement>(".qty-input")!;
        qtyInputField.value = changeQtyBtn.closest(".cart-product")!
            .querySelector(".quantity-already-added")!.textContent
            .split(": ").pop()!;

        const qtyInputFieldValidationMessagesContainer = injectedPopoverInstanceEl
            .querySelector(".validation-error-container");

        const currProductQty = Number(injectedPopoverInstanceEl
            .querySelector(".curr-prod-qty")!.textContent);

        if(!isValidQty(Number(qtyInputField.value), currProductQty)) {
            saveChangesBtn?.setAttribute("disabled", "true");
            saveChangesBtn.style.opacity = "0.8";
        }


        closePopoverBtn.addEventListener("click", () => popoverObj.hide());

        qtyInputField.addEventListener("input", () => {
            if(!isValidQty(Number(qtyInputField.value), currProductQty)) {
                qtyInputFieldValidationMessagesContainer!.textContent = "Invalid quantity!";
                saveChangesBtn?.setAttribute("disabled", "true");
                saveChangesBtn.style.opacity="0.8";
            } else {
                qtyInputFieldValidationMessagesContainer!.textContent = "";
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
            } as UpdateCartProductQty;

            const requestVerificationToken = document
                .querySelectorAll<HTMLInputElement>("input[name=__RequestVerificationToken]")[0]!
                .value!;

            const success = await updateCartProductQty(updatedCartProdPayload, requestVerificationToken);
            if(!success) {
                await showErrorSwal();
                return;
            }

            document.location.href = "/Cart";
        });
    });
});
