import { html } from "../../lib/lit/lit.js";

import showProductsTable from "./productsTable.js";
import { showErrorSwal, showPlainSuccessSwal } from "./domUtils.js";


export default async function getProductDetailsModalTemplate(productId, context) {
    const productDetailsModalId = `product-details-${productId}`;

    const productDetails = await context.getProductDetails(productId);
    if (!productDetails) {
        return errTemplate(productDetailsModalId);
    }

    return template(productDetails, productDetailsModalId, context);
}

function template(productDetails, productDetailsModalId, context) {
    return html`
        <div class="modal fade" id="${productDetailsModalId}" data-bs-backdrop="static"
             data-bs-keyboard="true" tabindex="-1" aria-labelledby="${productDetailsModalId}"
             aria-hidden="true">
            <div class="modal-dialog modal-dialog-centered modal-dialog-scrollable modal-xl modal-fullscreen-lg-down">
                <div class="modal-content">
                    <div class="modal-header">
                        <h1 class="modal-title fs-4 text-teal" id="staticBackdropLabel">
                            ${productDetails.name}
                        </h1>
                        <button type="button" class="btn-close"
                                data-bs-dismiss="modal" aria-label="Close"></button>
                    </div>

                    <div class="modal-body">
                        <div class="container-fluid">

                            <div class="row images-row mb-4">
                                <div class="col-12">
                                    <div class="d-flex overflow-x-auto pb-2 gap-2 horizontal-thumbnails">
                                        ${productDetails.imagesUrls.length > 0
                                                ? productDetails.imagesUrls.map(url => html`
                                                    <div class="thumbnail-wrapper">
                                                        <img src="${url}" class="img-thumbnail"
                                                             style="height: 200px; width: auto; object-fit: cover;"
                                                             alt="Product Image">
                                                    </div>`)
                                                : html`
                                                    <p class="text-muted italic">
                                                        No images available for this product.
                                                    </p>`
                                        }
                                    </div>
                                </div>
                            </div>

                            <div class="row product-details-row">
                                <div class="col-md-6 mb-3">
                                    <div class="card h-100 border-0 shadow-sm">
                                        <div class="card-body">
                                            <h5 class="card-title text-navy border-bottom pb-2 mb-3">
                                                Product Information
                                            </h5>
                                            <div class="row mb-2">
                                                <div class="col-5 fw-bold text-teal">Owner:</div>
                                                <div class="col-7">${productDetails.ownerName}</div>
                                            </div>
                                            <div class="row mb-2">
                                                <div class="col-5 fw-bold text-teal">Category:</div>
                                                <div class="col-7">
                                                    ${productDetails.categoryName}
                                                </div>
                                            </div>
                                            <div class="row mb-2">
                                                <div class="col-5 fw-bold text-teal">
                                                    Quantity in Stock:
                                                </div>
                                                <div class="col-7">
                                                    ${productDetails.quantityInStock}
                                                </div>
                                            </div>
                                            <div class="row">
                                                <div class="col-5 fw-bold text-teal">Price:</div>
                                                <div class="col-7 fs-5 fw-bold text-navy">
                                                        $${productDetails.sellingPrice.toFixed(2)}
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>

                                <div class="col-md-6 mb-3">
                                    <div class="card h-100 border-0 shadow-sm">
                                        <div class="card-body">
                                            <h5 class="card-title text-navy border-bottom pb-2 mb-3">Approval Status</h5>
                                            <div class="row mb-2">
                                                <div class="col-5 fw-bold text-teal">Status:</div>
                                                <div class="col-7">
                                                    <span class="badge ${getStatusBadgeClass(productDetails.approvalDecision.approvalStatus)}">
                                                        ${formatStatus(productDetails.approvalDecision.approvalStatus)}
                                                    </span>
                                                </div>
                                            </div>
                                            <div class="row mb-2">
                                                <div class="col-5 fw-bold text-teal">
                                                    Decision Maker:
                                                </div>
                                                <div class="col-7">
                                                    ${productDetails.approvalDecision.approvalDecisionMakerUsername || "N/A"}
                                                </div>
                                            </div>
                                            <div class="row">
                                                <div class="col-5 fw-bold text-teal">Time:</div>
                                                <div class="col-7">
                                                    ${productDetails.approvalDecision.timeOfDecision
                                                            ? new Date(productDetails.approvalDecision.timeOfDecision).toLocaleString()
                                                            : "N/A"}
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>

                            <div class="row description-row mt-2">
                                <div class="col-12">
                                    <div class="card border-0 shadow-sm">
                                        <div class="card-body">
                                            <h5 class="card-title text-navy border-bottom pb-2 mb-3">Description</h5>
                                            <p class="card-text text-muted" style="white-space: pre-line;">
                                                ${productDetails.description}
                                            </p>
                                        </div>
                                    </div>
                                </div>
                            </div>

                            <div class="row manage-approval-row mt-4">
                                <div class="col-12">
                                    <form @submit=${async (event) => onModifyApproval(event, productDetails.id, context)}>
                                        <div class="card border-0 shadow-sm border-top border-4 border-teal">
                                            <div class="card-body">
                                                <h5 class="card-title text-navy mb-4">Manage Approval Status</h5>

                                                <div class="mb-4">
                                                    <label class="form-label fw-bold text-teal">Select Status</label>
                                                    <div class="btn-group w-100" role="group" aria-label="Approval status selection">
                                                        <input type="radio" class="btn-check" name="approvalStatus-${productDetails.id}" id="statusWaiting-${productDetails.id}"
                                                               value="WaitingApproval" .checked=${productDetails.approvalDecision.approvalStatus === "WaitingApproval"}>
                                                        <label class="btn btn-outline-warning" for="statusWaiting-${productDetails.id}">Waiting Approval</label>

                                                        <input type="radio" class="btn-check" name="approvalStatus-${productDetails.id}" id="statusApproved-${productDetails.id}"
                                                               value="Approved" .checked=${productDetails.approvalDecision.approvalStatus === "Approved"}>
                                                        <label class="btn btn-outline-success" for="statusApproved-${productDetails.id}">Approved</label>

                                                        <input type="radio" class="btn-check" name="approvalStatus-${productDetails.id}" id="statusDisapproved-${productDetails.id}"
                                                               value="Disapproved" .checked=${productDetails.approvalDecision.approvalStatus === "Disapproved"}>
                                                        <label class="btn btn-outline-danger" for="statusDisapproved-${productDetails.id}">Disapproved</label>
                                                    </div>
                                                </div>

                                                <div class="mb-4">
                                                    <label for="decisionJustification-${productDetails.id}" class="form-label fw-bold text-teal">Decision Justification</label>
                                                    <textarea class="form-control" id="decisionJustification-${productDetails.id}" 
                                                              name="decision-justification-${productDetails.id}" rows="3"
                                                              placeholder="Provide a reason for the decision..."
                                                              .value=${productDetails.approvalDecision.decisionJustification ?? ""}></textarea>
                                                </div>

                                                <div class="d-grid">
                                                    <button type="submit" class="btn btn-teal btn-lg shadow-sm" id="save-approval-changes">
                                                        Save Approval Changes
                                                    </button>
                                                </div>
                                            </div>
                                        </div>
                                    </form>
                                </div>
                            </div>
                        </div>
                    </div>

                    <div class="modal-footer">
                        <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">
                            Close
                        </button>
                    </div>
                </div>
            </div>
        </div>`;
}

function errTemplate(productDetailsModalId) {
    return html`
        <div class="modal fade" id="${productDetailsModalId}"
             data-bs-keyboard="true" tabindex="-1"
             aria-labelledby="productDetailsModalId" aria-hidden="true">
            <div class="modal-dialog modal-dialog-centered modal-sm">
                <div class="modal-content">
                    <div class="modal-header">
                        <h1 class="modal-title fs-5 text-danger" id="error-modal">Error</h1>
                        <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                    </div>

                    <div class="modal-body">
                        Oops... Something went wrong! Please try again later.
                    </div>

                    <div class="modal-footer">
                        <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">
                            Ok
                        </button>
                    </div>
                </div>
            </div>
        </div>`;
}

async function onModifyApproval(event, productId, context) {
    event.preventDefault();
    
    const modalEl = event.currentTarget.closest(`div.modal#product-details-${productId}`);
    const modal = bootstrap.Modal.getInstance(modalEl);
    modal.toggle();

    const formData = new FormData(event.currentTarget);
    const newApprovalStatus = formData.get(`approvalStatus-${productId}`);
    const decisionJustification = formData.get(`decision-justification-${productId}`);

    const modifyApprovalResult = await context.modifyProductApproval({
        productId: productId,
        approvalStatus: newApprovalStatus,
        decisionJustification: decisionJustification,
    });
    if(!modifyApprovalResult) {
        showErrorSwal()
            .then(async () => {
                modal.dispose();
                await showProductsTable(context);
            });
    } else {
        showPlainSuccessSwal("The product approval status has been changed successfully.")
            .then(async () => {
                modal.dispose();
                await showProductsTable(context);
            });
    }
}

function getStatusBadgeClass(status) {
    switch (status) {
        case "Approved": return "bg-success";
        case "Disapproved": return "bg-danger";
        case "WaitingApproval": return "bg-warning text-dark";
        default: return "bg-secondary";
    }
}

function formatStatus(status) {
    switch (status) {
        case "Approved": return "Approved";
        case "Disapproved": return "Disapproved";
        case "WaitingApproval": return "Waiting Approval";
        default: return status;
    }
}
