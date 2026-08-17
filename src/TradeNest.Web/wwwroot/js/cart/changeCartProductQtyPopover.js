import { Popover } from "bootstrap";
import DOMPurify from "dompurify";
// import type { UpdateCartProductQty } from "../types/products.ts";
//
// import { updateCartProductQty } from "../api/cartsService.js";
import { getCurrProdDetails } from "../api/productsService.js";
import { isValidQty } from "../utils/dataValidationUtils.js";
import { showErrorSwal } from "../utils/domUtils.js";
const changeQtyButtonEls = document.querySelectorAll(".change-qty-button");
const popoverTemplateEl = document.querySelector(".change-qty-popover");
changeQtyButtonEls.forEach(changeQtyBtn => {
    const popoverObj = new Popover(changeQtyBtn, {
        content: () => popoverTemplateEl.innerHTML,
        placement: "top",
        container: ".cart-container",
        trigger: "manual",
        html: true,
        sanitize: true,
        sanitizeFn: ((content) => DOMPurify.sanitize(content)), // => missmatch between the bootstrap impl and the types for bootstrap.
    });
    changeQtyBtn.addEventListener("click", async (e) => {
        e.preventDefault();
        const productId = changeQtyBtn.getAttribute("data-product-id");
        if (!productId) {
            await showErrorSwal();
            return;
        }
        const currProdDetails = await getCurrProdDetails(productId);
        if (!currProdDetails) {
            await showErrorSwal();
            return;
        }
        const currProductQtySpan = popoverTemplateEl.querySelector(".curr-prod-qty");
        currProductQtySpan.textContent = `${currProdDetails.quantityInStock}`;
        popoverObj.show();
    });
    // shown.bs.popover => bootstrap popover event triggered on the popover instance being shown
    changeQtyBtn.addEventListener("shown.bs.popover", () => {
        const popoverId = changeQtyBtn.getAttribute("aria-describedby"); // => generated popover instance id on the DOM
        const injectedPopoverInstanceEl = document.querySelector(`#${popoverId}`);
        const closePopoverBtn = injectedPopoverInstanceEl
            .querySelector(".close-popover");
        const saveChangesBtn = injectedPopoverInstanceEl
            .querySelector(".save-changes-btn");
        const qtyInputField = injectedPopoverInstanceEl.querySelector(".qty-input");
        qtyInputField.value = changeQtyBtn.closest(".cart-product")
            .querySelector(".quantity-already-added").textContent
            .split(": ").pop();
        const qtyInputFieldValidationMessagesContainer = injectedPopoverInstanceEl
            .querySelector(".validation-error-container");
        const currProductQty = Number(injectedPopoverInstanceEl
            .querySelector(".curr-prod-qty").textContent);
        if (!isValidQty(Number(qtyInputField.value), currProductQty)) {
            saveChangesBtn?.setAttribute("disabled", "true");
            saveChangesBtn.style.opacity = "0.8";
        }
        closePopoverBtn.addEventListener("click", () => popoverObj.hide());
        qtyInputField.addEventListener("input", () => {
            if (!isValidQty(Number(qtyInputField.value), currProductQty)) {
                qtyInputFieldValidationMessagesContainer.textContent = "Invalid quantity!";
                saveChangesBtn?.setAttribute("disabled", "true");
                saveChangesBtn.style.opacity = "0.8";
            }
            else {
                qtyInputFieldValidationMessagesContainer.textContent = "";
                saveChangesBtn?.removeAttribute("disabled");
                saveChangesBtn.style.opacity = "1";
            }
        });
        saveChangesBtn.addEventListener("click", async (e) => {
            e.preventDefault();
            e.stopPropagation();
            popoverObj.hide();
            await showErrorSwal();
            return;
            // const productId = changeQtyBtn.getAttribute("data-product-id");
            // const cartId = changeQtyBtn.getAttribute("data-cart-id");
            //
            // if(!productId || !cartId) {
            //     popoverObj.toggle();
            //     await showErrorSwal();
            //     return;
            // }
            //
            // const updatedCartProdPayload: UpdateCartProductQty = {
            //     cartId: cartId,
            //     productId: productId,
            //     quantity: Number(qtyInputField.value),
            // };
            //
            // const requestVerificationToken = document
            //     .querySelectorAll<HTMLInputElement>("input[name=__RequestVerificationToken]")[0]!
            //     .value!;
            //
            // const success = await updateCartProductQty(updatedCartProdPayload, requestVerificationToken);
            // if(!success) {
            //     await showErrorSwal();
            //     return;
            // }
            //
            // document.location.href = "/Cart";
        });
    });
});
//# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJmaWxlIjoiY2hhbmdlQ2FydFByb2R1Y3RRdHlQb3BvdmVyLmpzIiwic291cmNlUm9vdCI6IiIsInNvdXJjZXMiOlsiLi4vLi4vLi4vRnJvbnRFbmRTY3JpcHRzL2NhcnQvY2hhbmdlQ2FydFByb2R1Y3RRdHlQb3BvdmVyLnRzIl0sIm5hbWVzIjpbXSwibWFwcGluZ3MiOiJBQUFBLE9BQU8sRUFBRSxPQUFPLEVBQUUsTUFBTSxXQUFXLENBQUM7QUFDcEMsT0FBTyxTQUFTLE1BQU0sV0FBVyxDQUFDO0FBRWxDLG9FQUFvRTtBQUNwRSxFQUFFO0FBQ0YsaUVBQWlFO0FBQ2pFLE9BQU8sRUFBRSxrQkFBa0IsRUFBRSxNQUFNLDJCQUEyQixDQUFDO0FBQy9ELE9BQU8sRUFBRSxVQUFVLEVBQUUsTUFBTSxpQ0FBaUMsQ0FBQztBQUM3RCxPQUFPLEVBQUUsYUFBYSxFQUFFLE1BQU0sc0JBQXNCLENBQUM7QUFFckQsTUFBTSxrQkFBa0IsR0FBRyxRQUFRLENBQUMsZ0JBQWdCLENBQUMsb0JBQW9CLENBQUMsQ0FBQztBQUMzRSxNQUFNLGlCQUFpQixHQUFHLFFBQVEsQ0FBQyxhQUFhLENBQWlCLHFCQUFxQixDQUFFLENBQUM7QUFFekYsa0JBQWtCLENBQUMsT0FBTyxDQUFDLFlBQVksQ0FBQyxFQUFFO0lBQ3RDLE1BQU0sVUFBVSxHQUFHLElBQUksT0FBTyxDQUFDLFlBQVksRUFBRTtRQUN6QyxPQUFPLEVBQUUsR0FBRyxFQUFFLENBQUMsaUJBQWlCLENBQUMsU0FBUztRQUMxQyxTQUFTLEVBQUUsS0FBSztRQUNoQixTQUFTLEVBQUUsaUJBQWlCO1FBQzVCLE9BQU8sRUFBRSxRQUFRO1FBQ2pCLElBQUksRUFBRSxJQUFJO1FBQ1YsUUFBUSxFQUFFLElBQUk7UUFDZCxVQUFVLEVBQUUsQ0FBQyxDQUFDLE9BQWUsRUFBRSxFQUFFLENBQUMsU0FBUyxDQUFDLFFBQVEsQ0FBQyxPQUFPLENBQUMsQ0FBMEIsRUFBRSx1RUFBdUU7S0FDbkssQ0FBQyxDQUFDO0lBRUgsWUFBWSxDQUFDLGdCQUFnQixDQUFDLE9BQU8sRUFBRSxLQUFLLEVBQUUsQ0FBQyxFQUFFLEVBQUU7UUFDL0MsQ0FBQyxDQUFDLGNBQWMsRUFBRSxDQUFDO1FBRW5CLE1BQU0sU0FBUyxHQUFHLFlBQVksQ0FBQyxZQUFZLENBQUMsaUJBQWlCLENBQUMsQ0FBQztRQUMvRCxJQUFJLENBQUMsU0FBUyxFQUFFLENBQUM7WUFDYixNQUFNLGFBQWEsRUFBRSxDQUFDO1lBQ3RCLE9BQU87UUFDWCxDQUFDO1FBRUQsTUFBTSxlQUFlLEdBQUcsTUFBTSxrQkFBa0IsQ0FBQyxTQUFTLENBQUMsQ0FBQztRQUM1RCxJQUFHLENBQUMsZUFBZSxFQUFFLENBQUM7WUFDbEIsTUFBTSxhQUFhLEVBQUUsQ0FBQztZQUN0QixPQUFPO1FBQ1gsQ0FBQztRQUVELE1BQU0sa0JBQWtCLEdBQUcsaUJBQWlCLENBQUMsYUFBYSxDQUFrQixnQkFBZ0IsQ0FBRSxDQUFDO1FBQy9GLGtCQUFrQixDQUFDLFdBQVcsR0FBRyxHQUFHLGVBQWUsQ0FBQyxlQUFlLEVBQUUsQ0FBQztRQUV0RSxVQUFVLENBQUMsSUFBSSxFQUFFLENBQUM7SUFDdEIsQ0FBQyxDQUFDLENBQUM7SUFFSCw0RkFBNEY7SUFDNUYsWUFBWSxDQUFDLGdCQUFnQixDQUFDLGtCQUFrQixFQUFFLEdBQUcsRUFBRTtRQUNuRCxNQUFNLFNBQVMsR0FBRyxZQUFZLENBQUMsWUFBWSxDQUFDLGtCQUFrQixDQUFDLENBQUMsQ0FBQyw4Q0FBOEM7UUFDL0csTUFBTSx5QkFBeUIsR0FBRyxRQUFRLENBQUMsYUFBYSxDQUFpQixJQUFJLFNBQVMsRUFBRSxDQUFFLENBQUM7UUFFM0YsTUFBTSxlQUFlLEdBQUcseUJBQXlCO2FBQzVDLGFBQWEsQ0FBb0IsZ0JBQWdCLENBQUUsQ0FBQztRQUV6RCxNQUFNLGNBQWMsR0FBRyx5QkFBeUI7YUFDM0MsYUFBYSxDQUFvQixtQkFBbUIsQ0FBRSxDQUFDO1FBRTVELE1BQU0sYUFBYSxHQUFHLHlCQUF5QixDQUFDLGFBQWEsQ0FBbUIsWUFBWSxDQUFFLENBQUM7UUFDL0YsYUFBYSxDQUFDLEtBQUssR0FBRyxZQUFZLENBQUMsT0FBTyxDQUFDLGVBQWUsQ0FBRTthQUN2RCxhQUFhLENBQUMseUJBQXlCLENBQUUsQ0FBQyxXQUFXO2FBQ3JELEtBQUssQ0FBQyxJQUFJLENBQUMsQ0FBQyxHQUFHLEVBQUcsQ0FBQztRQUV4QixNQUFNLHdDQUF3QyxHQUFHLHlCQUF5QjthQUNyRSxhQUFhLENBQUMsNkJBQTZCLENBQUMsQ0FBQztRQUVsRCxNQUFNLGNBQWMsR0FBRyxNQUFNLENBQUMseUJBQXlCO2FBQ2xELGFBQWEsQ0FBQyxnQkFBZ0IsQ0FBRSxDQUFDLFdBQVcsQ0FBQyxDQUFDO1FBRW5ELElBQUcsQ0FBQyxVQUFVLENBQUMsTUFBTSxDQUFDLGFBQWEsQ0FBQyxLQUFLLENBQUMsRUFBRSxjQUFjLENBQUMsRUFBRSxDQUFDO1lBQzFELGNBQWMsRUFBRSxZQUFZLENBQUMsVUFBVSxFQUFFLE1BQU0sQ0FBQyxDQUFDO1lBQ2pELGNBQWMsQ0FBQyxLQUFLLENBQUMsT0FBTyxHQUFHLEtBQUssQ0FBQztRQUN6QyxDQUFDO1FBR0QsZUFBZSxDQUFDLGdCQUFnQixDQUFDLE9BQU8sRUFBRSxHQUFHLEVBQUUsQ0FBQyxVQUFVLENBQUMsSUFBSSxFQUFFLENBQUMsQ0FBQztRQUVuRSxhQUFhLENBQUMsZ0JBQWdCLENBQUMsT0FBTyxFQUFFLEdBQUcsRUFBRTtZQUN6QyxJQUFHLENBQUMsVUFBVSxDQUFDLE1BQU0sQ0FBQyxhQUFhLENBQUMsS0FBSyxDQUFDLEVBQUUsY0FBYyxDQUFDLEVBQUUsQ0FBQztnQkFDMUQsd0NBQXlDLENBQUMsV0FBVyxHQUFHLG1CQUFtQixDQUFDO2dCQUM1RSxjQUFjLEVBQUUsWUFBWSxDQUFDLFVBQVUsRUFBRSxNQUFNLENBQUMsQ0FBQztnQkFDakQsY0FBYyxDQUFDLEtBQUssQ0FBQyxPQUFPLEdBQUMsS0FBSyxDQUFDO1lBQ3ZDLENBQUM7aUJBQU0sQ0FBQztnQkFDSix3Q0FBeUMsQ0FBQyxXQUFXLEdBQUcsRUFBRSxDQUFDO2dCQUMzRCxjQUFjLEVBQUUsZUFBZSxDQUFDLFVBQVUsQ0FBQyxDQUFDO2dCQUM1QyxjQUFjLENBQUMsS0FBSyxDQUFDLE9BQU8sR0FBQyxHQUFHLENBQUM7WUFDckMsQ0FBQztRQUNMLENBQUMsQ0FBQyxDQUFDO1FBRUgsY0FBYyxDQUFDLGdCQUFnQixDQUFDLE9BQU8sRUFBRSxLQUFLLEVBQUUsQ0FBQyxFQUFFLEVBQUU7WUFDakQsQ0FBQyxDQUFDLGNBQWMsRUFBRSxDQUFDO1lBQ25CLENBQUMsQ0FBQyxlQUFlLEVBQUUsQ0FBQztZQUVwQixVQUFVLENBQUMsSUFBSSxFQUFFLENBQUM7WUFDbEIsTUFBTSxhQUFhLEVBQUUsQ0FBQztZQUN0QixPQUFPO1lBQ1Asa0VBQWtFO1lBQ2xFLDREQUE0RDtZQUM1RCxFQUFFO1lBQ0YsOEJBQThCO1lBQzlCLDJCQUEyQjtZQUMzQiw2QkFBNkI7WUFDN0IsY0FBYztZQUNkLElBQUk7WUFDSixFQUFFO1lBQ0YseURBQXlEO1lBQ3pELHNCQUFzQjtZQUN0Qiw0QkFBNEI7WUFDNUIsNkNBQTZDO1lBQzdDLEtBQUs7WUFDTCxFQUFFO1lBQ0YsNENBQTRDO1lBQzVDLHdGQUF3RjtZQUN4RixlQUFlO1lBQ2YsRUFBRTtZQUNGLGdHQUFnRztZQUNoRyxpQkFBaUI7WUFDakIsNkJBQTZCO1lBQzdCLGNBQWM7WUFDZCxJQUFJO1lBQ0osRUFBRTtZQUNGLG9DQUFvQztRQUN4QyxDQUFDLENBQUMsQ0FBQztJQUNQLENBQUMsQ0FBQyxDQUFDO0FBQ1AsQ0FBQyxDQUFDLENBQUMiLCJzb3VyY2VzQ29udGVudCI6WyJpbXBvcnQgeyBQb3BvdmVyIH0gZnJvbSBcImJvb3RzdHJhcFwiO1xuaW1wb3J0IERPTVB1cmlmeSBmcm9tIFwiZG9tcHVyaWZ5XCI7XG5cbi8vIGltcG9ydCB0eXBlIHsgVXBkYXRlQ2FydFByb2R1Y3RRdHkgfSBmcm9tIFwiLi4vdHlwZXMvcHJvZHVjdHMudHNcIjtcbi8vXG4vLyBpbXBvcnQgeyB1cGRhdGVDYXJ0UHJvZHVjdFF0eSB9IGZyb20gXCIuLi9hcGkvY2FydHNTZXJ2aWNlLmpzXCI7XG5pbXBvcnQgeyBnZXRDdXJyUHJvZERldGFpbHMgfSBmcm9tIFwiLi4vYXBpL3Byb2R1Y3RzU2VydmljZS5qc1wiO1xuaW1wb3J0IHsgaXNWYWxpZFF0eSB9IGZyb20gXCIuLi91dGlscy9kYXRhVmFsaWRhdGlvblV0aWxzLmpzXCI7XG5pbXBvcnQgeyBzaG93RXJyb3JTd2FsIH0gZnJvbSBcIi4uL3V0aWxzL2RvbVV0aWxzLmpzXCI7XG5cbmNvbnN0IGNoYW5nZVF0eUJ1dHRvbkVscyA9IGRvY3VtZW50LnF1ZXJ5U2VsZWN0b3JBbGwoXCIuY2hhbmdlLXF0eS1idXR0b25cIik7XG5jb25zdCBwb3BvdmVyVGVtcGxhdGVFbCA9IGRvY3VtZW50LnF1ZXJ5U2VsZWN0b3I8SFRNTERpdkVsZW1lbnQ+KFwiLmNoYW5nZS1xdHktcG9wb3ZlclwiKSE7XG5cbmNoYW5nZVF0eUJ1dHRvbkVscy5mb3JFYWNoKGNoYW5nZVF0eUJ0biA9PiB7XG4gICAgY29uc3QgcG9wb3Zlck9iaiA9IG5ldyBQb3BvdmVyKGNoYW5nZVF0eUJ0biwge1xuICAgICAgICBjb250ZW50OiAoKSA9PiBwb3BvdmVyVGVtcGxhdGVFbC5pbm5lckhUTUwsXG4gICAgICAgIHBsYWNlbWVudDogXCJ0b3BcIixcbiAgICAgICAgY29udGFpbmVyOiBcIi5jYXJ0LWNvbnRhaW5lclwiLFxuICAgICAgICB0cmlnZ2VyOiBcIm1hbnVhbFwiLFxuICAgICAgICBodG1sOiB0cnVlLFxuICAgICAgICBzYW5pdGl6ZTogdHJ1ZSxcbiAgICAgICAgc2FuaXRpemVGbjogKChjb250ZW50OiBzdHJpbmcpID0+IERPTVB1cmlmeS5zYW5pdGl6ZShjb250ZW50KSkgYXMgdW5rbm93biBhcyAoKSA9PiB2b2lkLCAvLyA9PiBtaXNzbWF0Y2ggYmV0d2VlbiB0aGUgYm9vdHN0cmFwIGltcGwgYW5kIHRoZSB0eXBlcyBmb3IgYm9vdHN0cmFwLlxuICAgIH0pO1xuXG4gICAgY2hhbmdlUXR5QnRuLmFkZEV2ZW50TGlzdGVuZXIoXCJjbGlja1wiLCBhc3luYyAoZSkgPT4ge1xuICAgICAgICBlLnByZXZlbnREZWZhdWx0KCk7XG5cbiAgICAgICAgY29uc3QgcHJvZHVjdElkID0gY2hhbmdlUXR5QnRuLmdldEF0dHJpYnV0ZShcImRhdGEtcHJvZHVjdC1pZFwiKTtcbiAgICAgICAgaWYgKCFwcm9kdWN0SWQpIHtcbiAgICAgICAgICAgIGF3YWl0IHNob3dFcnJvclN3YWwoKTtcbiAgICAgICAgICAgIHJldHVybjtcbiAgICAgICAgfVxuXG4gICAgICAgIGNvbnN0IGN1cnJQcm9kRGV0YWlscyA9IGF3YWl0IGdldEN1cnJQcm9kRGV0YWlscyhwcm9kdWN0SWQpO1xuICAgICAgICBpZighY3VyclByb2REZXRhaWxzKSB7XG4gICAgICAgICAgICBhd2FpdCBzaG93RXJyb3JTd2FsKCk7XG4gICAgICAgICAgICByZXR1cm47XG4gICAgICAgIH1cblxuICAgICAgICBjb25zdCBjdXJyUHJvZHVjdFF0eVNwYW4gPSBwb3BvdmVyVGVtcGxhdGVFbC5xdWVyeVNlbGVjdG9yPEhUTUxTcGFuRWxlbWVudD4oXCIuY3Vyci1wcm9kLXF0eVwiKSE7XG4gICAgICAgIGN1cnJQcm9kdWN0UXR5U3Bhbi50ZXh0Q29udGVudCA9IGAke2N1cnJQcm9kRGV0YWlscy5xdWFudGl0eUluU3RvY2t9YDtcblxuICAgICAgICBwb3BvdmVyT2JqLnNob3coKTtcbiAgICB9KTtcblxuICAgIC8vIHNob3duLmJzLnBvcG92ZXIgPT4gYm9vdHN0cmFwIHBvcG92ZXIgZXZlbnQgdHJpZ2dlcmVkIG9uIHRoZSBwb3BvdmVyIGluc3RhbmNlIGJlaW5nIHNob3duXG4gICAgY2hhbmdlUXR5QnRuLmFkZEV2ZW50TGlzdGVuZXIoXCJzaG93bi5icy5wb3BvdmVyXCIsICgpID0+IHtcbiAgICAgICAgY29uc3QgcG9wb3ZlcklkID0gY2hhbmdlUXR5QnRuLmdldEF0dHJpYnV0ZShcImFyaWEtZGVzY3JpYmVkYnlcIik7IC8vID0+IGdlbmVyYXRlZCBwb3BvdmVyIGluc3RhbmNlIGlkIG9uIHRoZSBET01cbiAgICAgICAgY29uc3QgaW5qZWN0ZWRQb3BvdmVySW5zdGFuY2VFbCA9IGRvY3VtZW50LnF1ZXJ5U2VsZWN0b3I8SFRNTERpdkVsZW1lbnQ+KGAjJHtwb3BvdmVySWR9YCkhO1xuXG4gICAgICAgIGNvbnN0IGNsb3NlUG9wb3ZlckJ0biA9IGluamVjdGVkUG9wb3Zlckluc3RhbmNlRWxcbiAgICAgICAgICAgIC5xdWVyeVNlbGVjdG9yPEhUTUxCdXR0b25FbGVtZW50PihcIi5jbG9zZS1wb3BvdmVyXCIpITtcblxuICAgICAgICBjb25zdCBzYXZlQ2hhbmdlc0J0biA9IGluamVjdGVkUG9wb3Zlckluc3RhbmNlRWxcbiAgICAgICAgICAgIC5xdWVyeVNlbGVjdG9yPEhUTUxCdXR0b25FbGVtZW50PihcIi5zYXZlLWNoYW5nZXMtYnRuXCIpITtcblxuICAgICAgICBjb25zdCBxdHlJbnB1dEZpZWxkID0gaW5qZWN0ZWRQb3BvdmVySW5zdGFuY2VFbC5xdWVyeVNlbGVjdG9yPEhUTUxJbnB1dEVsZW1lbnQ+KFwiLnF0eS1pbnB1dFwiKSE7XG4gICAgICAgIHF0eUlucHV0RmllbGQudmFsdWUgPSBjaGFuZ2VRdHlCdG4uY2xvc2VzdChcIi5jYXJ0LXByb2R1Y3RcIikhXG4gICAgICAgICAgICAucXVlcnlTZWxlY3RvcihcIi5xdWFudGl0eS1hbHJlYWR5LWFkZGVkXCIpIS50ZXh0Q29udGVudFxuICAgICAgICAgICAgLnNwbGl0KFwiOiBcIikucG9wKCkhO1xuXG4gICAgICAgIGNvbnN0IHF0eUlucHV0RmllbGRWYWxpZGF0aW9uTWVzc2FnZXNDb250YWluZXIgPSBpbmplY3RlZFBvcG92ZXJJbnN0YW5jZUVsXG4gICAgICAgICAgICAucXVlcnlTZWxlY3RvcihcIi52YWxpZGF0aW9uLWVycm9yLWNvbnRhaW5lclwiKTtcblxuICAgICAgICBjb25zdCBjdXJyUHJvZHVjdFF0eSA9IE51bWJlcihpbmplY3RlZFBvcG92ZXJJbnN0YW5jZUVsXG4gICAgICAgICAgICAucXVlcnlTZWxlY3RvcihcIi5jdXJyLXByb2QtcXR5XCIpIS50ZXh0Q29udGVudCk7XG5cbiAgICAgICAgaWYoIWlzVmFsaWRRdHkoTnVtYmVyKHF0eUlucHV0RmllbGQudmFsdWUpLCBjdXJyUHJvZHVjdFF0eSkpIHtcbiAgICAgICAgICAgIHNhdmVDaGFuZ2VzQnRuPy5zZXRBdHRyaWJ1dGUoXCJkaXNhYmxlZFwiLCBcInRydWVcIik7XG4gICAgICAgICAgICBzYXZlQ2hhbmdlc0J0bi5zdHlsZS5vcGFjaXR5ID0gXCIwLjhcIjtcbiAgICAgICAgfVxuXG5cbiAgICAgICAgY2xvc2VQb3BvdmVyQnRuLmFkZEV2ZW50TGlzdGVuZXIoXCJjbGlja1wiLCAoKSA9PiBwb3BvdmVyT2JqLmhpZGUoKSk7XG5cbiAgICAgICAgcXR5SW5wdXRGaWVsZC5hZGRFdmVudExpc3RlbmVyKFwiaW5wdXRcIiwgKCkgPT4ge1xuICAgICAgICAgICAgaWYoIWlzVmFsaWRRdHkoTnVtYmVyKHF0eUlucHV0RmllbGQudmFsdWUpLCBjdXJyUHJvZHVjdFF0eSkpIHtcbiAgICAgICAgICAgICAgICBxdHlJbnB1dEZpZWxkVmFsaWRhdGlvbk1lc3NhZ2VzQ29udGFpbmVyIS50ZXh0Q29udGVudCA9IFwiSW52YWxpZCBxdWFudGl0eSFcIjtcbiAgICAgICAgICAgICAgICBzYXZlQ2hhbmdlc0J0bj8uc2V0QXR0cmlidXRlKFwiZGlzYWJsZWRcIiwgXCJ0cnVlXCIpO1xuICAgICAgICAgICAgICAgIHNhdmVDaGFuZ2VzQnRuLnN0eWxlLm9wYWNpdHk9XCIwLjhcIjtcbiAgICAgICAgICAgIH0gZWxzZSB7XG4gICAgICAgICAgICAgICAgcXR5SW5wdXRGaWVsZFZhbGlkYXRpb25NZXNzYWdlc0NvbnRhaW5lciEudGV4dENvbnRlbnQgPSBcIlwiO1xuICAgICAgICAgICAgICAgIHNhdmVDaGFuZ2VzQnRuPy5yZW1vdmVBdHRyaWJ1dGUoXCJkaXNhYmxlZFwiKTtcbiAgICAgICAgICAgICAgICBzYXZlQ2hhbmdlc0J0bi5zdHlsZS5vcGFjaXR5PVwiMVwiO1xuICAgICAgICAgICAgfVxuICAgICAgICB9KTtcblxuICAgICAgICBzYXZlQ2hhbmdlc0J0bi5hZGRFdmVudExpc3RlbmVyKFwiY2xpY2tcIiwgYXN5bmMgKGUpID0+IHtcbiAgICAgICAgICAgIGUucHJldmVudERlZmF1bHQoKTtcbiAgICAgICAgICAgIGUuc3RvcFByb3BhZ2F0aW9uKCk7XG5cbiAgICAgICAgICAgIHBvcG92ZXJPYmouaGlkZSgpO1xuICAgICAgICAgICAgYXdhaXQgc2hvd0Vycm9yU3dhbCgpO1xuICAgICAgICAgICAgcmV0dXJuO1xuICAgICAgICAgICAgLy8gY29uc3QgcHJvZHVjdElkID0gY2hhbmdlUXR5QnRuLmdldEF0dHJpYnV0ZShcImRhdGEtcHJvZHVjdC1pZFwiKTtcbiAgICAgICAgICAgIC8vIGNvbnN0IGNhcnRJZCA9IGNoYW5nZVF0eUJ0bi5nZXRBdHRyaWJ1dGUoXCJkYXRhLWNhcnQtaWRcIik7XG4gICAgICAgICAgICAvL1xuICAgICAgICAgICAgLy8gaWYoIXByb2R1Y3RJZCB8fCAhY2FydElkKSB7XG4gICAgICAgICAgICAvLyAgICAgcG9wb3Zlck9iai50b2dnbGUoKTtcbiAgICAgICAgICAgIC8vICAgICBhd2FpdCBzaG93RXJyb3JTd2FsKCk7XG4gICAgICAgICAgICAvLyAgICAgcmV0dXJuO1xuICAgICAgICAgICAgLy8gfVxuICAgICAgICAgICAgLy9cbiAgICAgICAgICAgIC8vIGNvbnN0IHVwZGF0ZWRDYXJ0UHJvZFBheWxvYWQ6IFVwZGF0ZUNhcnRQcm9kdWN0UXR5ID0ge1xuICAgICAgICAgICAgLy8gICAgIGNhcnRJZDogY2FydElkLFxuICAgICAgICAgICAgLy8gICAgIHByb2R1Y3RJZDogcHJvZHVjdElkLFxuICAgICAgICAgICAgLy8gICAgIHF1YW50aXR5OiBOdW1iZXIocXR5SW5wdXRGaWVsZC52YWx1ZSksXG4gICAgICAgICAgICAvLyB9O1xuICAgICAgICAgICAgLy9cbiAgICAgICAgICAgIC8vIGNvbnN0IHJlcXVlc3RWZXJpZmljYXRpb25Ub2tlbiA9IGRvY3VtZW50XG4gICAgICAgICAgICAvLyAgICAgLnF1ZXJ5U2VsZWN0b3JBbGw8SFRNTElucHV0RWxlbWVudD4oXCJpbnB1dFtuYW1lPV9fUmVxdWVzdFZlcmlmaWNhdGlvblRva2VuXVwiKVswXSFcbiAgICAgICAgICAgIC8vICAgICAudmFsdWUhO1xuICAgICAgICAgICAgLy9cbiAgICAgICAgICAgIC8vIGNvbnN0IHN1Y2Nlc3MgPSBhd2FpdCB1cGRhdGVDYXJ0UHJvZHVjdFF0eSh1cGRhdGVkQ2FydFByb2RQYXlsb2FkLCByZXF1ZXN0VmVyaWZpY2F0aW9uVG9rZW4pO1xuICAgICAgICAgICAgLy8gaWYoIXN1Y2Nlc3MpIHtcbiAgICAgICAgICAgIC8vICAgICBhd2FpdCBzaG93RXJyb3JTd2FsKCk7XG4gICAgICAgICAgICAvLyAgICAgcmV0dXJuO1xuICAgICAgICAgICAgLy8gfVxuICAgICAgICAgICAgLy9cbiAgICAgICAgICAgIC8vIGRvY3VtZW50LmxvY2F0aW9uLmhyZWYgPSBcIi9DYXJ0XCI7XG4gICAgICAgIH0pO1xuICAgIH0pO1xufSk7XG4iXX0=