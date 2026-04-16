import { html } from "../../lib/lit/lit.js"

import showProductsTable from "./productsTable.js";

export default function removeProductTemplate(product, context) {
    const deleteProductModalId = `remove-product-${product.id}`;
    
    return html`
        <div class="modal fade" id="${deleteProductModalId}"
             data-bs-keyboard="true" tabindex="-1" data-bs-backdrop="static"
             aria-labelledby="remove-product-${deleteProductModalId}-dialog"
             aria-hidden="true">
            <div class="modal-dialog">
                <div class="modal-content">
                    <div class="modal-header">
                        <h1 class="modal-title fs-5" id="staticBackdropLabel">
                            Confirm Deletion
                        </h1>
                        <button type="button" class="btn-close"
                                data-bs-dismiss="modal"
                                aria-label="Close"></button>
                    </div>
                    
                    <div class="modal-body">
                        Are you sure you want to remove ${product.name}
                    </div>
                    <div class="modal-footer">
                        <button type="button" class="btn btn-secondary"
                                data-bs-dismiss="modal">Cancel</button>

                        <form @submit=${async (event) => await onConfirmRemoveProduct(event, product, context)}>
                            <button type="submit" class="btn btn-danger"> Yes </button>
                        </form>
                    </div>
                </div>
            </div>
        </div>
    `;
}

async function onConfirmRemoveProduct(event, product, context) {
    event.preventDefault();
    
    const deleteProductModalId = `remove-product-${product.id}`;
    const modalEl = event.currentTarget.closest(`div#${deleteProductModalId}`);
    const modal = bootstrap.Modal.getInstance(modalEl);
    modal.toggle();
    
    const removeProductResult = await context.removeProduct(product.id);
    if(!removeProductResult) {
        Swal.fire({
            icon: "error",
            title: "Oops...",
            text: "Something went wrong! Please try again.",
            confirmButtonColor: "#0FAF9A",
            draggable: true,
            showClass: { popup: ` animate__animated animate__fadeInUp animate__faster ` },
            hideClass: { popup: ` animate__animated animate__fadeOutDown animate__faster ` }
        }).then(async () => {
            modal.dispose();
            await showProductsTable(context);
        });
    } else {
        Swal.fire({
            icon: "success",
            title: "Success",
            text: "Product removed successfully!",
            draggable: true,
            confirmButtonColor: "#0FAF9A",
            showClass: { popup: ` animate__animated animate__fadeInUp animate__faster ` },
            hideClass: { popup: ` animate__animated animate__fadeOutDown animate__faster ` },
        }).then(async () => {
            modal.dispose();
            await showProductsTable(context);
        });
    }
}