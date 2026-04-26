import { Modal } from "bootstrap"

const deleteModal = document.querySelector<HTMLDivElement>("#deleteModal")!;
deleteModal.addEventListener("show.bs.modal", (event: Event) => {
    const modalTriggerBtn = (event as Modal.Event).relatedTarget as HTMLButtonElement;

    const productId = modalTriggerBtn.getAttribute("data-product-id");
    const productName = modalTriggerBtn?.getAttribute("data-product-name");

    const modalBody = deleteModal.querySelector<HTMLDivElement>(".modal-body")!;
    modalBody.textContent = `Are you sure you want to delete ${productName} from your inventory?`;

    const submitFormEl = deleteModal.querySelector<HTMLButtonElement>("#submit-delete")!;
    const formAction = submitFormEl.getAttribute("action") as string;
    const appendedFormActionWithProdId = formAction.concat(`/${productId}`);
    submitFormEl.setAttribute("action", appendedFormActionWithProdId);
});
