import { Modal } from "bootstrap"
import { showErrorSwal } from "../utils/domUtils.js";

const deleteModal = document.querySelector<HTMLDivElement>("#deleteModal")!;

deleteModal.addEventListener("show.bs.modal", async (event: Event) => {
    const modalTriggerBtn = (event as Modal.Event).relatedTarget as HTMLButtonElement;

    const productId = modalTriggerBtn?.getAttribute("data-product-id");
    const productName = modalTriggerBtn?.getAttribute("data-product-name");
    if (!productId || !productName) {
        event.preventDefault(); // => stops the modal from showing
        Modal.getOrCreateInstance(deleteModal)?.dispose();
        await showErrorSwal();
        return;
    }

    const modalBody = deleteModal.querySelector<HTMLDivElement>(".modal-body")!;
    modalBody.textContent = `Are you sure you want to delete ${productName} from your inventory?`;

    const submitFormEl = deleteModal.querySelector<HTMLButtonElement>("#submit-delete")!;
    const formAction = submitFormEl.getAttribute("action");
    if(!formAction) {
        event.preventDefault(); // => stops the modal from showing
        Modal.getOrCreateInstance(deleteModal)?.dispose();
        await showErrorSwal();
        return;
    }

    const appendedFormActionWithProdId = formAction.concat(`/${productId}`);
    submitFormEl.setAttribute("action", appendedFormActionWithProdId);
});
