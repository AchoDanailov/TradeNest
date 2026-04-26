import bootstrap from "bootstrap";
import { html } from "lit-html";

import type { Product } from "../types/products.ts";
import type { TableContext } from "../types/tableContext.ts";
import { showErrorSwal, showPlainSuccessSwal } from "../utils/domUtils.js";
import showProductsTable from "./productsTable.js";

export default function removeProductTemplate(product: Product, context: TableContext) {
    const deleteProductModalId = `remove-product-${product.id}`;

    return html`
        <div class="modal fade" id="${deleteProductModalId}"
             data-bs-keyboard="true" tabindex="-1" data-bs-backdrop="static"
             aria-labelledby="${deleteProductModalId}-dialog"
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

                        <form @submit=${async (event: Event) => await onConfirmRemoveProduct(event, product, context)}>
                            <button type="submit" class="btn btn-danger"> Yes </button>
                        </form>
                    </div>
                </div>
            </div>
        </div>
    `;
}

async function onConfirmRemoveProduct(event: Event, product: Product, context: TableContext) {
    event.preventDefault();

    const deleteProductModalId = `remove-product-${product.id}`;
    const modalEl = (event.currentTarget as HTMLDivElement)!
        .closest(`div#${deleteProductModalId}`)!;
    const modal = bootstrap.Modal.getInstance(modalEl!);
    modal?.toggle();

    const removeProductResult = await context.removeProduct(product.id);
    if(!removeProductResult) {
        showErrorSwal()
            .then(async () => await showProductsTable(context));
    } else {
        showPlainSuccessSwal("Product removed successfully!")
            .then(async () => await showProductsTable(context));
    }
}