const deleteModal = document.getElementById("deleteModal");
deleteModal?.addEventListener("show.bs.modal", (event) => {
    const modalTriggerBtn = event.relatedTarget;
    
    const productId = modalTriggerBtn?.getAttribute("data-product-id");
    const productName = modalTriggerBtn?.getAttribute("data-product-name");
    
    const modalBody = deleteModal.querySelector(".modal-body");
    modalBody.textContent = `Are you sure you want to delete ${productName} from your inventory?`;
    
    const submitFormEl = deleteModal.querySelector("#submit-delete");
    submitFormEl.setAttribute("asp-route-id", productId);
});
