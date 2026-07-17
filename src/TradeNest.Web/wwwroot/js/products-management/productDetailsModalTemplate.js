import { Modal } from "bootstrap";
import { html } from "lit-html";
import { showErrorSwal, showPlainSuccessSwal } from "../utils/domUtils.js";
import showProductsTable from "./productsTable.js";
export default async function getProductDetailsModalTemplate(productId, context) {
    const productDetailsModalId = `product-details-${productId}`;
    const productDetails = await context.getProductDetails(productId);
    if (!productDetails) {
        return errTemplate(productDetailsModalId);
    }
    return template(productDetails, productDetailsModalId, context);
}
function template(productDetails, productDetailsModalId, context) {
    return html `
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
        ? productDetails.imagesUrls.map(url => html `
                                                    <div class="thumbnail-wrapper">
                                                        <img src="${url}" class="img-thumbnail"
                                                             style="height: 200px; width: auto; object-fit: cover;"
                                                             alt="Product Image">
                                                    </div>`)
        : html `
                                                    <p class="text-muted italic">
                                                        No images available for this product.
                                                    </p>`}
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

                            ${productDetails.ownerName != null
        ? html `
                                        <div class="row manage-approval-row mt-4">
                                            <div class="col-12">
                                                <form @submit=${async (event) => onModifyApproval(event, productDetails, context)}>
                                                    <div class="card border-0 shadow-sm border-top border-4 border-teal">
                                                        <div class="card-body">
                                                            <h5 class="card-title text-navy mb-4">Manage Approval Status</h5>

                                                            <div class="mb-4 select-status-section">
                                                                <label class="form-label fw-bold text-teal">Select Status</label>
                                                                <div class="btn-group w-100" role="group" aria-label="Approval status selection"
                                                                     @change=${onStatusChangeValidation}>
                                                                    <input type="radio" class="btn-check" name="approvalStatus-${productDetails.id}" id="statusApproved-${productDetails.id}"
                                                                           value="Approved" .checked=${productDetails.approvalDecision.approvalStatus === "Approved"}>
                                                                    <label class="btn btn-outline-success" for="statusApproved-${productDetails.id}">Approved</label>

                                                                    <input type="radio" class="btn-check" name="approvalStatus-${productDetails.id}" id="statusDisapproved-${productDetails.id}"
                                                                           value="Disapproved" .checked=${productDetails.approvalDecision.approvalStatus === "Disapproved"}>
                                                                    <label class="btn btn-outline-danger" for="statusDisapproved-${productDetails.id}">Disapproved</label>
                                                                </div>
                                                                <div class="text-danger approval-validation-section"></div>
                                                            </div>

                                                            <div class="mb-4 decision-justification-section">
                                                                <label for="decisionJustification-${productDetails.id}" class="form-label fw-bold text-teal">Decision Justification</label>
                                                                <textarea class="form-control" id="decisionJustification-${productDetails.id}"
                                                                          name="decision-justification-${productDetails.id}" rows="3"
                                                                          placeholder="Provide a reason for the decision..."
                                                                          .value!=${productDetails.approvalDecision.decisionJustification}
                                                                          @change=${approvalDecisionJustificationValidation}></textarea>
                                                                <div class="text-danger justification-validation-section"></div>
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
                                        </div>`
        : html ``}
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
    return html `
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
async function onModifyApproval(event, productDetails, context) {
    event.preventDefault();
    const formData = new FormData(event.currentTarget);
    const newApprovalStatus = formData.get(`approvalStatus-${productDetails.id}`);
    const decisionJustification = formData.get(`decision-justification-${productDetails.id}`);
    let isSameData = false;
    if (newApprovalStatus === productDetails.approvalDecision.approvalStatus) {
        event.currentTarget
            .querySelector("div.approval-validation-section")
            .textContent = "Approval status was not changed.";
        isSameData = true;
    }
    if (decisionJustification === productDetails.approvalDecision.decisionJustification) {
        event.currentTarget
            .querySelector("div.justification-validation-section")
            .textContent = "Please provide a new reason for the new approval decision.";
        isSameData = true;
    }
    if (isSameData) {
        return;
    }
    const modalEl = event.currentTarget
        .closest(`div.modal#product-details-${productDetails.id}`);
    const modal = Modal.getInstance(modalEl);
    modal.toggle();
    const modifyApprovalResult = await context.modifyProductApproval({
        productId: productDetails.id,
        approvalStatus: newApprovalStatus,
        decisionJustification: decisionJustification,
    });
    if (!modifyApprovalResult) {
        showErrorSwal()
            .then(async () => await showProductsTable(context));
    }
    else {
        showPlainSuccessSwal("The product approval status has been changed successfully.")
            .then(async () => await showProductsTable(context));
    }
}
function onStatusChangeValidation(event) {
    const validationSection = event.currentTarget
        .closest("div.select-status-section")
        .querySelector("div.approval-validation-section");
    if (validationSection?.textContent !== "") {
        validationSection.textContent = "";
    }
}
function approvalDecisionJustificationValidation(event) {
    const submitButton = document.querySelector("#save-approval-changes");
    const textArea = event.currentTarget;
    const validationSection = textArea
        .closest("div.decision-justification-section")
        .querySelector("div.justification-validation-section");
    if (textArea.value.length < 4 || textArea.value.length > 3000) {
        validationSection.textContent
            = "The decision reason should be between 4 and 3000 characters long.";
        submitButton?.setAttribute("disabled", "disabled");
    }
    else {
        validationSection.textContent = "";
        submitButton?.removeAttribute("disabled");
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
//# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJmaWxlIjoicHJvZHVjdERldGFpbHNNb2RhbFRlbXBsYXRlLmpzIiwic291cmNlUm9vdCI6IiIsInNvdXJjZXMiOlsiLi4vLi4vLi4vRnJvbnRFbmRTY3JpcHRzL3Byb2R1Y3RzLW1hbmFnZW1lbnQvcHJvZHVjdERldGFpbHNNb2RhbFRlbXBsYXRlLnRzIl0sIm5hbWVzIjpbXSwibWFwcGluZ3MiOiJBQUFBLE9BQU8sRUFBRSxLQUFLLEVBQUUsTUFBTSxXQUFXLENBQUM7QUFDbEMsT0FBTyxFQUFFLElBQUksRUFBdUIsTUFBTSxVQUFVLENBQUM7QUFRckQsT0FBTyxFQUFFLGFBQWEsRUFBRSxvQkFBb0IsRUFBRSxNQUFNLHNCQUFzQixDQUFDO0FBQzNFLE9BQU8saUJBQWlCLE1BQU0sb0JBQW9CLENBQUM7QUFHbkQsTUFBTSxDQUFDLE9BQU8sQ0FBQyxLQUFLLFVBQVUsOEJBQThCLENBQ3hELFNBQWlCLEVBQ2pCLE9BQXFCO0lBRXJCLE1BQU0scUJBQXFCLEdBQUcsbUJBQW1CLFNBQVMsRUFBRSxDQUFDO0lBRTdELE1BQU0sY0FBYyxHQUFHLE1BQU0sT0FBTyxDQUFDLGlCQUFpQixDQUFDLFNBQVMsQ0FBQyxDQUFDO0lBQ2xFLElBQUksQ0FBQyxjQUFjLEVBQUUsQ0FBQztRQUNsQixPQUFPLFdBQVcsQ0FBQyxxQkFBcUIsQ0FBQyxDQUFDO0lBQzlDLENBQUM7SUFFRCxPQUFPLFFBQVEsQ0FBQyxjQUFjLEVBQUUscUJBQXFCLEVBQUUsT0FBTyxDQUFDLENBQUM7QUFDcEUsQ0FBQztBQUVELFNBQVMsUUFBUSxDQUNiLGNBQThCLEVBQzlCLHFCQUE2QixFQUM3QixPQUFxQjtJQUVyQixPQUFPLElBQUksQ0FBQTtzQ0FDdUIscUJBQXFCO3NFQUNXLHFCQUFxQjs7Ozs7OzhCQU03RCxjQUFjLENBQUMsSUFBSTs7Ozs7Ozs7Ozs7OzBDQVlQLGNBQWMsQ0FBQyxVQUFVLENBQUMsTUFBTSxHQUFHLENBQUM7UUFDOUIsQ0FBQyxDQUFDLGNBQWMsQ0FBQyxVQUFVLENBQUMsR0FBRyxDQUFD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QyxNQUE4QjtJQUNoRCxRQUFRLE1BQU0sRUFBRSxDQUFDO1FBQ2IsS0FBSyxVQUFVLENBQUMsQ0FBQyxPQUFPLFVBQVUsQ0FBQztRQUNuQyxLQUFLLGFBQWEsQ0FBQyxDQUFDLE9BQU8sYUFBYSxDQUFDO1FBQ3pDLEtBQUssaUJBQWlCLENBQUMsQ0FBQyxPQUFPLGtCQUFrQixDQUFDO1FBQ2xELE9BQU8sQ0FBQyxDQUFDLE9BQU8sTUFBTSxDQUFDO0lBQzNCLENBQUM7QUFDTCxDQUFDIiwic291cmNlc0NvbnRlbnQiOlsiaW1wb3J0IHsgTW9kYWwgfSBmcm9tIFwiYm9vdHN0cmFwXCI7XG5pbXBvcnQgeyBodG1sLCB0eXBlIFRlbXBsYXRlUmVzdWx0IH0gZnJvbSBcImxpdC1odG1sXCI7XG5cbmltcG9ydCB0eXBlIHtcbiAgICBFZGl0UHJvZHVjdEFwcHJvdmFsU3RhdHVzLFxuICAgIFByb2R1Y3REZXRhaWxzLFxuICAgIFByb2R1Y3RzQXBwcm92YWxTdGF0dXNcbn0gZnJvbSBcIi4uL3R5cGVzL3Byb2R1Y3RzLnRzXCI7XG5pbXBvcnQgdHlwZSB7IFRhYmxlQ29udGV4dCB9IGZyb20gXCIuLi90eXBlcy90YWJsZUNvbnRleHQudHNcIjtcbmltcG9ydCB7IHNob3dFcnJvclN3YWwsIHNob3dQbGFpblN1Y2Nlc3NTd2FsIH0gZnJvbSBcIi4uL3V0aWxzL2RvbVV0aWxzLmpzXCI7XG5pbXBvcnQgc2hvd1Byb2R1Y3RzVGFibGUgZnJvbSBcIi4vcHJvZHVjdHNUYWJsZS5qc1wiO1xuXG5cbmV4cG9ydCBkZWZhdWx0IGFzeW5jIGZ1bmN0aW9uIGdldFByb2R1Y3REZXRhaWxzTW9kYWxUZW1wbGF0ZShcbiAgICBwcm9kdWN0SWQ6IHN0cmluZyxcbiAgICBjb250ZXh0OiBUYWJsZUNvbnRleHRcbik6IFByb21pc2U8VGVtcGxhdGVSZXN1bHQ+IHtcbiAgICBjb25zdCBwcm9kdWN0RGV0YWlsc01vZGFsSWQgPSBgcHJvZHVjdC1kZXRhaWxzLSR7cHJvZHVjdElkfWA7XG5cbiAgICBjb25zdCBwcm9kdWN0RGV0YWlscyA9IGF3YWl0IGNvbnRleHQuZ2V0UHJvZHVjdERldGFpbHMocHJvZHVjdElkKTtcbiAgICBpZiAoIXByb2R1Y3REZXRhaWxzKSB7XG4gICAgICAgIHJldHVybiBlcnJUZW1wbGF0ZShwcm9kdWN0RGV0YWlsc01vZGFsSWQpO1xuICAgIH1cblxuICAgIHJldHVybiB0ZW1wbGF0ZShwcm9kdWN0RGV0YWlscywgcHJvZHVjdERldGFpbHNNb2RhbElkLCBjb250ZXh0KTtcbn1cblxuZnVuY3Rpb24gdGVtcGxhdGUoXG4gICAgcHJvZHVjdERldGFpbHM6IFByb2R1Y3REZXRhaWxzLFxuICAgIHByb2R1Y3REZXRhaWxzTW9kYWxJZDogc3RyaW5nLFxuICAgIGNvbnRleHQ6IFRhYmxlQ29udGV4dFxuKTogVGVtcGxhdGVSZXN1bHQge1xuICAgIHJldHVybiBodG1sYFxuICAgICAgICA8ZGl2IGNsYXNzPVwibW9kYWwgZmFkZVwiIGlkPVwiJHtwcm9kdWN0RGV0YWlsc01vZGFsSWR9XCIgZGF0YS1icy1iYWNrZHJvcD1cInN0YXRpY1wiXG4gICAgICAgICAgICAgZGF0YS1icy1rZXlib2FyZD1cInRydWVcIiB0YWJpbmRleD1cIi0xXCIgYXJpYS1sYWJlbGxlZGJ5PVwiJHtwcm9kdWN0RGV0YWlsc01vZGFsSWR9XCJcbiAgICAgICAgICAgICBhcmlhLWhpZGRlbj1cInRydWVcIj5cbiAgICAgICAgICAgIDxkaXYgY2xhc3M9XCJtb2RhbC1kaWFsb2cgbW9kYWwtZGlhbG9nLWNlbnRlcmVkIG1vZGFsLWRpYWxvZy1zY3JvbGxhYmxlIG1vZGFsLXhsIG1vZGFsLWZ1bGxzY3JlZW4tbGctZG93blwiPlxuICAgICAgICAgICAgICAgIDxkaXYgY2xhc3M9XCJtb2RhbC1jb250ZW50XCI+XG4gICAgICAgICAgICAgICAgICAgIDxkaXYgY2xhc3M9XCJtb2RhbC1oZWFkZXJcIj5cbiAgICAgICAgICAgICAgICAgICAgICAgIDxoMSBjbGFzcz1cIm1vZGFsLXRpdGxlIGZzLTQgdGV4dC10ZWFsXCIgaWQ9XCJzdGF0aWNCYWNrZHJvcExhYmVsXCI+XG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgJHtwcm9kdWN0RGV0YWlscy5uYW1lfVxuICAgICAgICAgICAgICAgICAgICAgICAgPC9oMT5cbiAgICAgICAgICAgICAgICAgICAgICAgIDxidXR0b24gdHlwZT1cImJ1dHRvblwiIGNsYXNzPVwiYnRuLWNsb3NlXCJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgZGF0YS1icy1kaXNtaXNzPVwibW9kYWxcIiBhcmlhLWxhYmVsPVwiQ2xvc2VcIj48L2J1dHRvbj5cbiAgICAgICAgICAgICAgICAgICAgPC9kaXY+XG5cbiAgICAgICAgICAgICAgICAgICAgPGRpdiBjbGFzcz1cIm1vZGFsLWJvZHlcIj5cbiAgICAgICAgICAgICAgICAgICAgICAgIDxkaXYgY2xhc3M9XCJjb250YWluZXItZmx1aWRcIj5cblxuICAgICAgICAgICAgICAgICAgICAgICAgICAgIDxkaXYgY2xhc3M9XCJyb3cgaW1hZ2VzLXJvdyBtYi00XCI+XG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIDxkaXYgY2xhc3M9XCJjb2wtMTJcIj5cbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIDxkaXYgY2xhc3M9XCJkLWZsZXggb3ZlcmZsb3cteC1hdXRvIHBiLTIgZ2FwLTIgaG9yaXpvbnRhbC10aHVtYm5haWxzXCI+XG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgJHtwcm9kdWN0RGV0YWlscy5pbWFnZXNVcmxzLmxlbmd0aCA+IDBcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgID8gcHJvZHVjdERldGFpbHMuaW1hZ2VzVXJscy5tYXAodXJsID0+IGh0bWxgXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgPGRpdiBjbGFzcz1cInRodW1ibmFpbC13cmFwcGVyXCI+XG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIDxpbWcgc3JjPVwiJHt1cmx9XCIgY2xhc3M9XCJpbWctdGh1bWJuYWlsXCJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICBzdHlsZT1cImhlaWdodDogMjAwcHg7IHdpZHRoOiBhdXRvOyBvYmplY3QtZml0OiBjb3ZlcjtcIlxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIGFsdD1cIlByb2R1Y3QgSW1hZ2VcIj5cbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICA8L2Rpdj5gKVxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgOiBodG1sYFxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIDxwIGNsYXNzPVwidGV4dC1tdXRlZCBpdGFsaWNcIj5cbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgTm8gaW1hZ2VzIGF2YWlsYWJsZSBmb3IgdGhpcyBwcm9kdWN0LlxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIDwvcD5gXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgfVxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgPC9kaXY+XG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIDwvZGl2PlxuICAgICAgICAgICAgICAgICAgICAgICAgICAgIDwvZGl2PlxuXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgPGRpdiBjbGFzcz1cInJvdyBwcm9kdWN0LWRldGFpbHMtcm93XCI+XG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIDxkaXYgY2xhc3M9XCJjb2wtbWQtNiBtYi0zXCI+XG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICA8ZGl2IGNsYXNzPVwiY2FyZCBoLTEwMCBib3JkZXItMCBzaGFkb3ctc21cIj5cbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICA8ZGl2IGNsYXNzPVwiY2FyZC1ib2R5XCI+XG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIDxoNSBjbGFzcz1cImNhcmQtdGl0bGUgdGV4dC1uYXZ5IGJvcmRlci1ib3R0b20gcGItMiBtYi0zXCI+XG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICBQcm9kdWN0IEluZm9ybWF0aW9uXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIDwvaDU+XG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIDxkaXYgY2xhc3M9XCJyb3cgbWItMlwiPlxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgPGRpdiBjbGFzcz1cImNvbC01IGZ3LWJvbGQgdGV4dC10ZWFsXCI+T3duZXI6PC9kaXY+XG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICA8ZGl2IGNsYXNzPVwiY29sLTdcIj4ke3Byb2R1Y3REZXRhaWxzLm93bmVyTmFtZX08L2Rpdj5cbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgPC9kaXY+XG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIDxkaXYgY2xhc3M9XCJyb3cgbWItMlwiPlxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgPGRpdiBjbGFzcz1cImNvbC01IGZ3LWJvbGQgdGV4dC10ZWFsXCI+Q2F0ZWdvcnk6PC9kaXY+XG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICA8ZGl2IGNsYXNzPVwiY29sLTdcIj5cbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAke3Byb2R1Y3REZXRhaWxzLmNhdGVnb3J5TmFtZX1cbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIDwvZGl2PlxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICA8L2Rpdj5cbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgPGRpdiBjbGFzcz1cInJvdyBtYi0yXCI+XG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICA8ZGl2IGNsYXNzPVwiY29sLTUgZnctYm9sZCB0ZXh0LXRlYWxcIj5cbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICBRdWFudGl0eSBpbiBTdG9jazpcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIDwvZGl2PlxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgPGRpdiBjbGFzcz1cImNvbC03XCI+XG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgJHtwcm9kdWN0RGV0YWlscy5xdWFudGl0eUluU3RvY2t9XG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICA8L2Rpdj5cbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgPC9kaXY+XG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIDxkaXYgY2xhc3M9XCJyb3dcIj5cbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIDxkaXYgY2xhc3M9XCJjb2wtNSBmdy1ib2xkIHRleHQtdGVhbFwiPlByaWNlOjwvZGl2PlxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgPGRpdiBjbGFzcz1cImNvbC03IGZzLTUgZnctYm9sZCB0ZXh0LW5hdnlcIj5cbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgJCR7cHJvZHVjdERldGFpbHMuc2VsbGluZ1ByaWNlLnRvRml4ZWQoMil9XG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICA8L2Rpdj5cbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgPC9kaXY+XG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgPC9kaXY+XG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICA8L2Rpdj5cbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgPC9kaXY+XG5cbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgPGRpdiBjbGFzcz1cImNvbC1tZC02IG1iLTNcIj5cbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIDxkaXYgY2xhc3M9XCJjYXJkIGgtMTAwIGJvcmRlci0wIHNoYWRvdy1zbVwiPlxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIDxkaXYgY2xhc3M9XCJjYXJkLWJvZHlcIj5cbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgPGg1IGNsYXNzPVwiY2FyZC10aXRsZSB0ZXh0LW5hdnkgYm9yZGVyLWJvdHRvbSBwYi0yIG1iLTNcIj5BcHByb3ZhbCBTdGF0dXM8L2g1PlxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICA8ZGl2IGNsYXNzPVwicm93IG1iLTJcIj5cbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIDxkaXYgY2xhc3M9XCJjb2wtNSBmdy1ib2xkIHRleHQtdGVhbFwiPlN0YXR1czo8L2Rpdj5cbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIDxkaXYgY2xhc3M9XCJjb2wtN1wiPlxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIDxzcGFuIGNsYXNzPVwiYmFkZ2UgJHtnZXRTdGF0dXNCYWRnZUNsYXNzKHByb2R1Y3REZXRhaWxzLmFwcHJvdmFsRGVjaXNpb24uYXBwcm92YWxTdGF0dXMpfVwiPlxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAke2Zvcm1hdFN0YXR1cyhwcm9kdWN0RGV0YWlscy5hcHByb3ZhbERlY2lzaW9uLmFwcHJvdmFsU3RhdHVzKX1cbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICA8L3NwYW4+XG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICA8L2Rpdj5cbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgPC9kaXY+XG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIDxkaXYgY2xhc3M9XCJyb3cgbWItMlwiPlxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgPGRpdiBjbGFzcz1cImNvbC01IGZ3LWJvbGQgdGV4dC10ZWFsXCI+XG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgRGVjaXNpb24gTWFrZXI6XG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICA8L2Rpdj5cbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIDxkaXYgY2xhc3M9XCJjb2wtN1wiPlxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICR7cHJvZHVjdERldGFpbHMuYXBwcm92YWxEZWNpc2lvbi5hcHByb3ZhbERlY2lzaW9uTWFrZXJVc2VybmFtZSB8fCBcIk4vQVwifVxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgPC9kaXY+XG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIDwvZGl2PlxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICA8ZGl2IGNsYXNzPVwicm93XCI+XG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICA8ZGl2IGNsYXNzPVwiY29sLTUgZnctYm9sZCB0ZXh0LXRlYWxcIj5UaW1lOjwvZGl2PlxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgPGRpdiBjbGFzcz1cImNvbC03XCI+XG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgJHtwcm9kdWN0RGV0YWlscy5hcHByb3ZhbERlY2lzaW9uLnRpbWVPZkRlY2lzaW9uXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICA/IG5ldyBEYXRlKHByb2R1Y3REZXRhaWxzLmFwcHJvdmFsRGVjaXNpb24udGltZU9mRGVjaXNpb24pLnRvTG9jYWxlU3RyaW5nKClcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIDogXCJOL0FcIn1cbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIDwvZGl2PlxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICA8L2Rpdj5cbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICA8L2Rpdj5cbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIDwvZGl2PlxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICA8L2Rpdj5cbiAgICAgICAgICAgICAgICAgICAgICAgICAgICA8L2Rpdj5cblxuICAgICAgICAgICAgICAgICAgICAgICAgICAgIDxkaXYgY2xhc3M9XCJyb3cgZGVzY3JpcHRpb24tcm93IG10LTJcIj5cbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgPGRpdiBjbGFzcz1cImNvbC0xMlwiPlxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgPGRpdiBjbGFzcz1cImNhcmQgYm9yZGVyLTAgc2hhZG93LXNtXCI+XG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgPGRpdiBjbGFzcz1cImNhcmQtYm9keVwiPlxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICA8aDUgY2xhc3M9XCJjYXJkLXRpdGxlIHRleHQtbmF2eSBib3JkZXItYm90dG9tIHBiLTIgbWItM1wiPkRlc2NyaXB0aW9uPC9oNT5cbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgPHAgY2xhc3M9XCJjYXJkLXRleHQgdGV4dC1tdXRlZFwiIHN0eWxlPVwid2hpdGUtc3BhY2U6IHByZS1saW5lO1wiPlxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgJHtwcm9kdWN0RGV0YWlscy5kZXNjcmlwdGlvbn1cbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgPC9wPlxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIDwvZGl2PlxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgPC9kaXY+XG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIDwvZGl2PlxuICAgICAgICAgICAgICAgICAgICAgICAgICAgIDwvZGl2PlxuXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgJHtwcm9kdWN0RGV0YWlscy5vd25lck5hbWUgIT0gbnVsbFxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgPyBodG1sYFxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIDxkaXYgY2xhc3M9XCJyb3cgbWFuYWdlLWFwcHJvdmFsLXJvdyBtdC00XCI+XG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIDxkaXYgY2xhc3M9XCJjb2wtMTJcIj5cbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIDxmb3JtIEBzdWJtaXQ9JHthc3luYyAoZXZlbnQ6IEV2ZW50KSA9PiBvbk1vZGlmeUFwcHJvdmFsKGV2ZW50LCBwcm9kdWN0RGV0YWlscywgY29udGV4dCl9PlxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIDxkaXYgY2xhc3M9XCJjYXJkIGJvcmRlci0wIHNoYWRvdy1zbSBib3JkZXItdG9wIGJvcmRlci00IGJvcmRlci10ZWFsXCI+XG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIDxkaXYgY2xhc3M9XCJjYXJkLWJvZHlcIj5cbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIDxoNSBjbGFzcz1cImNhcmQtdGl0bGUgdGV4dC1uYXZ5IG1iLTRcIj5NYW5hZ2UgQXBwcm92YWwgU3RhdHVzPC9oNT5cblxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgPGRpdiBjbGFzcz1cIm1iLTQgc2VsZWN0LXN0YXR1cy1zZWN0aW9uXCI+XG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgPGxhYmVsIGNsYXNzPVwiZm9ybS1sYWJlbCBmdy1ib2xkIHRleHQtdGVhbFwiPlNlbGVjdCBTdGF0dXM8L2xhYmVsPlxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIDxkaXYgY2xhc3M9XCJidG4tZ3JvdXAgdy0xMDBcIiByb2xlPVwiZ3JvdXBcIiBhcmlhLWxhYmVsPVwiQXBwcm92YWwgc3RhdHVzIHNlbGVjdGlvblwiXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICBAY2hhbmdlPSR7b25TdGF0dXNDaGFuZ2VWYWxpZGF0aW9ufT5cbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgPGlucHV0IHR5cGU9XCJyYWRpb1wiIGNsYXNzPVwiYnRuLWNoZWNrXCIgbmFtZT1cImFwcHJvdmFsU3RhdHVzLSR7cHJvZHVjdERldGFpbHMuaWR9XCIgaWQ9XCJzdGF0dXNBcHByb3ZlZC0ke3Byb2R1Y3REZXRhaWxzLmlkfVwiXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICB2YWx1ZT1cIkFwcHJvdmVkXCIgLmNoZWNrZWQ9JHtwcm9kdWN0RGV0YWlscy5hcHByb3ZhbERlY2lzaW9uLmFwcHJvdmFsU3RhdHVzID09PSBcIkFwcHJvdmVkXCJ9PlxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICA8bGFiZWwgY2xhc3M9XCJidG4gYnRuLW91dGxpbmUtc3VjY2Vzc1wiIGZvcj1cInN0YXR1c0FwcHJvdmVkLSR7cHJvZHVjdERldGFpbHMuaWR9XCI+QXBwcm92ZWQ8L2xhYmVsPlxuXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIDxpbnB1dCB0eXBlPVwicmFkaW9cIiBjbGFzcz1cImJ0bi1jaGVja1wiIG5hbWU9XCJhcHByb3ZhbFN0YXR1cy0ke3Byb2R1Y3REZXRhaWxzLmlkfVwiIGlkPVwic3RhdHVzRGlzYXBwcm92ZWQtJHtwcm9kdWN0RGV0YWlscy5pZH1cIlxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgdmFsdWU9XCJEaXNhcHByb3ZlZFwiIC5jaGVja2VkPSR7cHJvZHVjdERldGFpbHMuYXBwcm92YWxEZWNpc2lvbi5hcHByb3ZhbFN0YXR1cyA9PT0gXCJEaXNhcHByb3ZlZFwifT5cbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgPGxhYmVsIGNsYXNzPVwiYnRuIGJ0bi1vdXRsaW5lLWRhbmdlclwiIGZvcj1cInN0YXR1c0Rpc2FwcHJvdmVkLSR7cHJvZHVjdERldGFpbHMuaWR9XCI+RGlzYXBwcm92ZWQ8L2xhYmVsPlxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIDwvZGl2PlxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIDxkaXYgY2xhc3M9XCJ0ZXh0LWRhbmdlciBhcHByb3ZhbC12YWxpZGF0aW9uLXNlY3Rpb25cIj48L2Rpdj5cbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIDwvZGl2PlxuXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICA8ZGl2IGNsYXNzPVwibWItNCBkZWNpc2lvbi1qdXN0aWZpY2F0aW9uLXNlY3Rpb25cIj5cbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICA8bGFiZWwgZm9yPVwiZGVjaXNpb25KdXN0aWZpY2F0aW9uLSR7cHJvZHVjdERldGFpbHMuaWR9XCIgY2xhc3M9XCJmb3JtLWxhYmVsIGZ3LWJvbGQgdGV4dC10ZWFsXCI+RGVjaXNpb24gSnVzdGlmaWNhdGlvbjwvbGFiZWw+XG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgPHRleHRhcmVhIGNsYXNzPVwiZm9ybS1jb250cm9sXCIgaWQ9XCJkZWNpc2lvbkp1c3RpZmljYXRpb24tJHtwcm9kdWN0RGV0YWlscy5pZH1cIlxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICBuYW1lPVwiZGVjaXNpb24tanVzdGlmaWNhdGlvbi0ke3Byb2R1Y3REZXRhaWxzLmlkfVwiIHJvd3M9XCIzXCJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgcGxhY2Vob2xkZXI9XCJQcm92aWRlIGEgcmVhc29uIGZvciB0aGUgZGVjaXNpb24uLi5cIlxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAudmFsdWUhPSR7cHJvZHVjdERldGFpbHMuYXBwcm92YWxEZWNpc2lvbi5kZWNpc2lvbkp1c3RpZmljYXRpb259XG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIEBjaGFuZ2U9JHthcHByb3ZhbERlY2lzaW9uSnVzdGlmaWNhdGlvblZhbGlkYXRpb259PjwvdGV4dGFyZWE+XG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgPGRpdiBjbGFzcz1cInRleHQtZGFuZ2VyIGp1c3RpZmljYXRpb24tdmFsaWRhdGlvbi1zZWN0aW9uXCI+PC9kaXY+XG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICA8L2Rpdj5cblxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgPGRpdiBjbGFzcz1cImQtZ3JpZFwiPlxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIDxidXR0b24gdHlwZT1cInN1Ym1pdFwiIGNsYXNzPVwiYnRuIGJ0bi10ZWFsIGJ0bi1sZyBzaGFkb3ctc21cIiBpZD1cInNhdmUtYXBwcm92YWwtY2hhbmdlc1wiPlxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICBTYXZlIEFwcHJvdmFsIENoYW5nZXNcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICA8L2J1dHRvbj5cbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIDwvZGl2PlxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICA8L2Rpdj5cbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICA8L2Rpdj5cbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIDwvZm9ybT5cbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgPC9kaXY+XG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgPC9kaXY+YFxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgOiBodG1sYGBcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICB9XG4gICAgICAgICAgICAgICAgICAgICAgICA8L2Rpdj5cbiAgICAgICAgICAgICAgICAgICAgPC9kaXY+XG5cbiAgICAgICAgICAgICAgICAgICAgPGRpdiBjbGFzcz1cIm1vZGFsLWZvb3RlclwiPlxuICAgICAgICAgICAgICAgICAgICAgICAgPGJ1dHRvbiB0eXBlPVwiYnV0dG9uXCIgY2xhc3M9XCJidG4gYnRuLXNlY29uZGFyeVwiIGRhdGEtYnMtZGlzbWlzcz1cIm1vZGFsXCI+XG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgQ2xvc2VcbiAgICAgICAgICAgICAgICAgICAgICAgIDwvYnV0dG9uPlxuICAgICAgICAgICAgICAgICAgICA8L2Rpdj5cbiAgICAgICAgICAgICAgICA8L2Rpdj5cbiAgICAgICAgICAgIDwvZGl2PlxuICAgICAgICA8L2Rpdj5gO1xufVxuXG5mdW5jdGlvbiBlcnJUZW1wbGF0ZShwcm9kdWN0RGV0YWlsc01vZGFsSWQ6IHN0cmluZyk6IFRlbXBsYXRlUmVzdWx0IHtcbiAgICByZXR1cm4gaHRtbGBcbiAgICAgICAgPGRpdiBjbGFzcz1cIm1vZGFsIGZhZGVcIiBpZD1cIiR7cHJvZHVjdERldGFpbHNNb2RhbElkfVwiXG4gICAgICAgICAgICAgZGF0YS1icy1rZXlib2FyZD1cInRydWVcIiB0YWJpbmRleD1cIi0xXCJcbiAgICAgICAgICAgICBhcmlhLWxhYmVsbGVkYnk9XCJwcm9kdWN0RGV0YWlsc01vZGFsSWRcIiBhcmlhLWhpZGRlbj1cInRydWVcIj5cbiAgICAgICAgICAgIDxkaXYgY2xhc3M9XCJtb2RhbC1kaWFsb2cgbW9kYWwtZGlhbG9nLWNlbnRlcmVkIG1vZGFsLXNtXCI+XG4gICAgICAgICAgICAgICAgPGRpdiBjbGFzcz1cIm1vZGFsLWNvbnRlbnRcIj5cbiAgICAgICAgICAgICAgICAgICAgPGRpdiBjbGFzcz1cIm1vZGFsLWhlYWRlclwiPlxuICAgICAgICAgICAgICAgICAgICAgICAgPGgxIGNsYXNzPVwibW9kYWwtdGl0bGUgZnMtNSB0ZXh0LWRhbmdlclwiIGlkPVwiZXJyb3ItbW9kYWxcIj5FcnJvcjwvaDE+XG4gICAgICAgICAgICAgICAgICAgICAgICA8YnV0dG9uIHR5cGU9XCJidXR0b25cIiBjbGFzcz1cImJ0bi1jbG9zZVwiIGRhdGEtYnMtZGlzbWlzcz1cIm1vZGFsXCIgYXJpYS1sYWJlbD1cIkNsb3NlXCI+PC9idXR0b24+XG4gICAgICAgICAgICAgICAgICAgIDwvZGl2PlxuXG4gICAgICAgICAgICAgICAgICAgIDxkaXYgY2xhc3M9XCJtb2RhbC1ib2R5XCI+XG4gICAgICAgICAgICAgICAgICAgICAgICBPb3BzLi4uIFNvbWV0aGluZyB3ZW50IHdyb25nISBQbGVhc2UgdHJ5IGFnYWluIGxhdGVyLlxuICAgICAgICAgICAgICAgICAgICA8L2Rpdj5cblxuICAgICAgICAgICAgICAgICAgICA8ZGl2IGNsYXNzPVwibW9kYWwtZm9vdGVyXCI+XG4gICAgICAgICAgICAgICAgICAgICAgICA8YnV0dG9uIHR5cGU9XCJidXR0b25cIiBjbGFzcz1cImJ0biBidG4tc2Vjb25kYXJ5XCIgZGF0YS1icy1kaXNtaXNzPVwibW9kYWxcIj5cbiAgICAgICAgICAgICAgICAgICAgICAgICAgICBPa1xuICAgICAgICAgICAgICAgICAgICAgICAgPC9idXR0b24+XG4gICAgICAgICAgICAgICAgICAgIDwvZGl2PlxuICAgICAgICAgICAgICAgIDwvZGl2PlxuICAgICAgICAgICAgPC9kaXY+XG4gICAgICAgIDwvZGl2PmA7XG59XG5cbmFzeW5jIGZ1bmN0aW9uIG9uTW9kaWZ5QXBwcm92YWwoXG4gICAgZXZlbnQ6IEV2ZW50LFxuICAgIHByb2R1Y3REZXRhaWxzOiBQcm9kdWN0RGV0YWlscyxcbiAgICBjb250ZXh0OiBUYWJsZUNvbnRleHRcbik6IFByb21pc2U8dm9pZD4ge1xuICAgIGV2ZW50LnByZXZlbnREZWZhdWx0KCk7XG5cbiAgICBjb25zdCBmb3JtRGF0YSA9IG5ldyBGb3JtRGF0YShldmVudC5jdXJyZW50VGFyZ2V0IGFzIEhUTUxGb3JtRWxlbWVudCB8IHVuZGVmaW5lZCk7XG4gICAgY29uc3QgbmV3QXBwcm92YWxTdGF0dXMgPSBmb3JtRGF0YS5nZXQoYGFwcHJvdmFsU3RhdHVzLSR7cHJvZHVjdERldGFpbHMuaWR9YCk7XG4gICAgY29uc3QgZGVjaXNpb25KdXN0aWZpY2F0aW9uID0gZm9ybURhdGEuZ2V0KGBkZWNpc2lvbi1qdXN0aWZpY2F0aW9uLSR7cHJvZHVjdERldGFpbHMuaWR9YCk7XG5cbiAgICBsZXQgaXNTYW1lRGF0YSA9IGZhbHNlO1xuICAgIGlmKG5ld0FwcHJvdmFsU3RhdHVzID09PSBwcm9kdWN0RGV0YWlscy5hcHByb3ZhbERlY2lzaW9uLmFwcHJvdmFsU3RhdHVzKSB7XG4gICAgICAgIChldmVudC5jdXJyZW50VGFyZ2V0IGFzIEhUTUxGb3JtRWxlbWVudClcbiAgICAgICAgICAgIC5xdWVyeVNlbGVjdG9yPEhUTUxEaXZFbGVtZW50PihcImRpdi5hcHByb3ZhbC12YWxpZGF0aW9uLXNlY3Rpb25cIikhXG4gICAgICAgICAgICAudGV4dENvbnRlbnQgPSBcIkFwcHJvdmFsIHN0YXR1cyB3YXMgbm90IGNoYW5nZWQuXCI7XG5cbiAgICAgICAgaXNTYW1lRGF0YSA9IHRydWU7XG4gICAgfVxuICAgIGlmKGRlY2lzaW9uSnVzdGlmaWNhdGlvbiA9PT0gcHJvZHVjdERldGFpbHMuYXBwcm92YWxEZWNpc2lvbi5kZWNpc2lvbkp1c3RpZmljYXRpb24pIHtcbiAgICAgICAgKGV2ZW50LmN1cnJlbnRUYXJnZXQgYXMgSFRNTERpdkVsZW1lbnQpXG4gICAgICAgICAgICAucXVlcnlTZWxlY3RvcihcImRpdi5qdXN0aWZpY2F0aW9uLXZhbGlkYXRpb24tc2VjdGlvblwiKSFcbiAgICAgICAgICAgIC50ZXh0Q29udGVudCA9IFwiUGxlYXNlIHByb3ZpZGUgYSBuZXcgcmVhc29uIGZvciB0aGUgbmV3IGFwcHJvdmFsIGRlY2lzaW9uLlwiO1xuXG4gICAgICAgIGlzU2FtZURhdGEgPSB0cnVlO1xuICAgIH1cblxuICAgIGlmKGlzU2FtZURhdGEpIHtcbiAgICAgICAgcmV0dXJuO1xuICAgIH1cblxuICAgIGNvbnN0IG1vZGFsRWwgPSAoZXZlbnQuY3VycmVudFRhcmdldCBhcyBIVE1MRm9ybUVsZW1lbnQpXG4gICAgICAgIC5jbG9zZXN0KGBkaXYubW9kYWwjcHJvZHVjdC1kZXRhaWxzLSR7cHJvZHVjdERldGFpbHMuaWR9YCkhO1xuICAgIGNvbnN0IG1vZGFsID0gTW9kYWwuZ2V0SW5zdGFuY2UobW9kYWxFbCkhO1xuICAgIG1vZGFsLnRvZ2dsZSgpO1xuXG4gICAgY29uc3QgbW9kaWZ5QXBwcm92YWxSZXN1bHQgPSBhd2FpdCBjb250ZXh0Lm1vZGlmeVByb2R1Y3RBcHByb3ZhbCh7XG4gICAgICAgIHByb2R1Y3RJZDogcHJvZHVjdERldGFpbHMuaWQsXG4gICAgICAgIGFwcHJvdmFsU3RhdHVzOiBuZXdBcHByb3ZhbFN0YXR1cyxcbiAgICAgICAgZGVjaXNpb25KdXN0aWZpY2F0aW9uOiBkZWNpc2lvbkp1c3RpZmljYXRpb24sXG4gICAgfSBhcyBFZGl0UHJvZHVjdEFwcHJvdmFsU3RhdHVzKTtcbiAgICBpZighbW9kaWZ5QXBwcm92YWxSZXN1bHQpIHtcbiAgICAgICAgc2hvd0Vycm9yU3dhbCgpXG4gICAgICAgICAgICAudGhlbihhc3luYyAoKSA9PiBhd2FpdCBzaG93UHJvZHVjdHNUYWJsZShjb250ZXh0KSk7XG4gICAgfSBlbHNlIHtcbiAgICAgICAgc2hvd1BsYWluU3VjY2Vzc1N3YWwoXCJUaGUgcHJvZHVjdCBhcHByb3ZhbCBzdGF0dXMgaGFzIGJlZW4gY2hhbmdlZCBzdWNjZXNzZnVsbHkuXCIpXG4gICAgICAgICAgICAudGhlbihhc3luYyAoKSA9PiBhd2FpdCBzaG93UHJvZHVjdHNUYWJsZShjb250ZXh0KSk7XG4gICAgfVxufVxuXG5mdW5jdGlvbiBvblN0YXR1c0NoYW5nZVZhbGlkYXRpb24oZXZlbnQ6IEV2ZW50KSB7XG4gICAgY29uc3QgdmFsaWRhdGlvblNlY3Rpb24gPSAoZXZlbnQuY3VycmVudFRhcmdldCBhcyBIVE1MRGl2RWxlbWVudClcbiAgICAgICAgLmNsb3Nlc3QoXCJkaXYuc2VsZWN0LXN0YXR1cy1zZWN0aW9uXCIpIVxuICAgICAgICAucXVlcnlTZWxlY3RvcihcImRpdi5hcHByb3ZhbC12YWxpZGF0aW9uLXNlY3Rpb25cIik7XG5cbiAgICBpZih2YWxpZGF0aW9uU2VjdGlvbj8udGV4dENvbnRlbnQgIT09IFwiXCIpIHtcbiAgICAgICAgdmFsaWRhdGlvblNlY3Rpb24hLnRleHRDb250ZW50ID0gXCJcIjtcbiAgICB9XG59XG5cbmZ1bmN0aW9uIGFwcHJvdmFsRGVjaXNpb25KdXN0aWZpY2F0aW9uVmFsaWRhdGlvbihldmVudDogRXZlbnQpIHtcbiAgICBjb25zdCBzdWJtaXRCdXR0b24gPSBkb2N1bWVudC5xdWVyeVNlbGVjdG9yKFwiI3NhdmUtYXBwcm92YWwtY2hhbmdlc1wiKTtcbiAgICBjb25zdCB0ZXh0QXJlYSA9IGV2ZW50LmN1cnJlbnRUYXJnZXQgYXMgSFRNTFRleHRBcmVhRWxlbWVudDtcbiAgICBjb25zdCB2YWxpZGF0aW9uU2VjdGlvbiA9IHRleHRBcmVhXG4gICAgICAgIC5jbG9zZXN0KFwiZGl2LmRlY2lzaW9uLWp1c3RpZmljYXRpb24tc2VjdGlvblwiKSFcbiAgICAgICAgLnF1ZXJ5U2VsZWN0b3IoXCJkaXYuanVzdGlmaWNhdGlvbi12YWxpZGF0aW9uLXNlY3Rpb25cIikhO1xuXG4gICAgaWYodGV4dEFyZWEudmFsdWUubGVuZ3RoIDwgNCB8fCB0ZXh0QXJlYS52YWx1ZS5sZW5ndGggPiAzMDAwKSB7XG4gICAgICAgIHZhbGlkYXRpb25TZWN0aW9uLnRleHRDb250ZW50XG4gICAgICAgICAgICA9IFwiVGhlIGRlY2lzaW9uIHJlYXNvbiBzaG91bGQgYmUgYmV0d2VlbiA0IGFuZCAzMDAwIGNoYXJhY3RlcnMgbG9uZy5cIjtcblxuICAgICAgICBzdWJtaXRCdXR0b24/LnNldEF0dHJpYnV0ZShcImRpc2FibGVkXCIsIFwiZGlzYWJsZWRcIik7XG4gICAgfSBlbHNlIHtcbiAgICAgICAgdmFsaWRhdGlvblNlY3Rpb24udGV4dENvbnRlbnQgPSBcIlwiO1xuICAgICAgICBzdWJtaXRCdXR0b24/LnJlbW92ZUF0dHJpYnV0ZShcImRpc2FibGVkXCIpO1xuICAgIH1cbn1cblxuZnVuY3Rpb24gZ2V0U3RhdHVzQmFkZ2VDbGFzcyhzdGF0dXM6IFByb2R1Y3RzQXBwcm92YWxTdGF0dXMpIHtcbiAgICBzd2l0Y2ggKHN0YXR1cykge1xuICAgICAgICBjYXNlIFwiQXBwcm92ZWRcIjogcmV0dXJuIFwiYmctc3VjY2Vzc1wiO1xuICAgICAgICBjYXNlIFwiRGlzYXBwcm92ZWRcIjogcmV0dXJuIFwiYmctZGFuZ2VyXCI7XG4gICAgICAgIGNhc2UgXCJXYWl0aW5nQXBwcm92YWxcIjogcmV0dXJuIFwiYmctd2FybmluZyB0ZXh0LWRhcmtcIjtcbiAgICAgICAgZGVmYXVsdDogcmV0dXJuIFwiYmctc2Vjb25kYXJ5XCI7XG4gICAgfVxufVxuXG5mdW5jdGlvbiBmb3JtYXRTdGF0dXMoc3RhdHVzOiBQcm9kdWN0c0FwcHJvdmFsU3RhdHVzKSB7XG4gICAgc3dpdGNoIChzdGF0dXMpIHtcbiAgICAgICAgY2FzZSBcIkFwcHJvdmVkXCI6IHJldHVybiBcIkFwcHJvdmVkXCI7XG4gICAgICAgIGNhc2UgXCJEaXNhcHByb3ZlZFwiOiByZXR1cm4gXCJEaXNhcHByb3ZlZFwiO1xuICAgICAgICBjYXNlIFwiV2FpdGluZ0FwcHJvdmFsXCI6IHJldHVybiBcIldhaXRpbmcgQXBwcm92YWxcIjtcbiAgICAgICAgZGVmYXVsdDogcmV0dXJuIHN0YXR1cztcbiAgICB9XG59XG4iXX0=
