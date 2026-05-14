import * as bootstrap from "bootstrap";
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
    const modal = bootstrap.Modal.getInstance(modalEl);
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
//# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJmaWxlIjoicHJvZHVjdERldGFpbHNNb2RhbFRlbXBsYXRlLmpzIiwic291cmNlUm9vdCI6IiIsInNvdXJjZXMiOlsiLi4vLi4vLi4vRnJvbnRFbmRTY3JpcHRzL3Byb2R1Y3RzLW1hbmFnZW1lbnQvcHJvZHVjdERldGFpbHNNb2RhbFRlbXBsYXRlLnRzIl0sIm5hbWVzIjpbXSwibWFwcGluZ3MiOiJBQUFBLE9BQU8sS0FBSyxTQUFTLE1BQU0sV0FBVyxDQUFDO0FBQ3ZDLE9BQU8sRUFBRSxJQUFJLEVBQXVCLE1BQU0sVUFBVSxDQUFDO0FBUXJELE9BQU8sRUFBRSxhQUFhLEVBQUUsb0JBQW9CLEVBQUUsTUFBTSxzQkFBc0IsQ0FBQztBQUMzRSxPQUFPLGlCQUFpQixNQUFNLG9CQUFvQixDQUFDO0FBR25ELE1BQU0sQ0FBQyxPQUFPLENBQUMsS0FBSyxVQUFVLDhCQUE4QixDQUN4RCxTQUFpQixFQUNqQixPQUFxQjtJQUVyQixNQUFNLHFCQUFxQixHQUFHLG1CQUFtQixTQUFTLEVBQUUsQ0FBQztJQUU3RCxNQUFNLGNBQWMsR0FBRyxNQUFNLE9BQU8sQ0FBQyxpQkFBaUIsQ0FBQyxTQUFTLENBQUMsQ0FBQztJQUNsRSxJQUFJLENBQUMsY0FBYyxFQUFFLENBQUM7UUFDbEIsT0FBTyxXQUFXLENBQUMscUJBQXFCLENBQUMsQ0FBQztJQUM5QyxDQUFDO0lBRUQsT0FBTyxRQUFRLENBQUMsY0FBYyxFQUFFLHFCQUFxQixFQUFFLE9BQU8sQ0FBQyxDQUFDO0FBQ3BFLENBQUM7QUFFRCxTQUFTLFFBQVEsQ0FDYixjQUE4QixFQUM5QixxQkFBNkIsRUFDN0IsT0FBcUI7SUFFckIsT0FBTyxJQUFJLENBQUE7c0NBQ3VCLHFCQUFxQjtzRUFDVyxxQkFBcUI7Ozs7Ozs4QkFNN0QsY0FBYyxDQUFDLElBQUk7Ozs7Ozs7Ozs7OzswQ0FZUCxjQUFjLENBQUMsVUFBVSxDQUFDLE1BQU0sR0FBRyxDQUFDO1FBQzlCLENBQUMsQ0FBQyxjQUFjLENBQUMsVUFBVSxDQUFDLEdBQUcsQ0FBQyxHQUFH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WSxDQUFDLE1BQThCO0lBQ2hELFFBQVEsTUFBTSxFQUFFLENBQUM7UUFDYixLQUFLLFVBQVUsQ0FBQyxDQUFDLE9BQU8sVUFBVSxDQUFDO1FBQ25DLEtBQUssYUFBYSxDQUFDLENBQUMsT0FBTyxhQUFhLENBQUM7UUFDekMsS0FBSyxpQkFBaUIsQ0FBQyxDQUFDLE9BQU8sa0JBQWtCLENBQUM7UUFDbEQsT0FBTyxDQUFDLENBQUMsT0FBTyxNQUFNLENBQUM7SUFDM0IsQ0FBQztBQUNMLENBQUMiLCJzb3VyY2VzQ29udGVudCI6WyJpbXBvcnQgKiBhcyBib290c3RyYXAgZnJvbSBcImJvb3RzdHJhcFwiO1xuaW1wb3J0IHsgaHRtbCwgdHlwZSBUZW1wbGF0ZVJlc3VsdCB9IGZyb20gXCJsaXQtaHRtbFwiO1xuXG5pbXBvcnQgdHlwZSB7XG4gICAgRWRpdFByb2R1Y3RBcHByb3ZhbFN0YXR1cyxcbiAgICBQcm9kdWN0RGV0YWlscyxcbiAgICBQcm9kdWN0c0FwcHJvdmFsU3RhdHVzXG59IGZyb20gXCIuLi90eXBlcy9wcm9kdWN0cy50c1wiO1xuaW1wb3J0IHR5cGUgeyBUYWJsZUNvbnRleHQgfSBmcm9tIFwiLi4vdHlwZXMvdGFibGVDb250ZXh0LnRzXCI7XG5pbXBvcnQgeyBzaG93RXJyb3JTd2FsLCBzaG93UGxhaW5TdWNjZXNzU3dhbCB9IGZyb20gXCIuLi91dGlscy9kb21VdGlscy5qc1wiO1xuaW1wb3J0IHNob3dQcm9kdWN0c1RhYmxlIGZyb20gXCIuL3Byb2R1Y3RzVGFibGUuanNcIjtcblxuXG5leHBvcnQgZGVmYXVsdCBhc3luYyBmdW5jdGlvbiBnZXRQcm9kdWN0RGV0YWlsc01vZGFsVGVtcGxhdGUoXG4gICAgcHJvZHVjdElkOiBzdHJpbmcsXG4gICAgY29udGV4dDogVGFibGVDb250ZXh0XG4pOiBQcm9taXNlPFRlbXBsYXRlUmVzdWx0PiB7XG4gICAgY29uc3QgcHJvZHVjdERldGFpbHNNb2RhbElkID0gYHByb2R1Y3QtZGV0YWlscy0ke3Byb2R1Y3RJZH1gO1xuXG4gICAgY29uc3QgcHJvZHVjdERldGFpbHMgPSBhd2FpdCBjb250ZXh0LmdldFByb2R1Y3REZXRhaWxzKHByb2R1Y3RJZCk7XG4gICAgaWYgKCFwcm9kdWN0RGV0YWlscykge1xuICAgICAgICByZXR1cm4gZXJyVGVtcGxhdGUocHJvZHVjdERldGFpbHNNb2RhbElkKTtcbiAgICB9XG5cbiAgICByZXR1cm4gdGVtcGxhdGUocHJvZHVjdERldGFpbHMsIHByb2R1Y3REZXRhaWxzTW9kYWxJZCwgY29udGV4dCk7XG59XG5cbmZ1bmN0aW9uIHRlbXBsYXRlKFxuICAgIHByb2R1Y3REZXRhaWxzOiBQcm9kdWN0RGV0YWlscyxcbiAgICBwcm9kdWN0RGV0YWlsc01vZGFsSWQ6IHN0cmluZyxcbiAgICBjb250ZXh0OiBUYWJsZUNvbnRleHRcbik6IFRlbXBsYXRlUmVzdWx0IHtcbiAgICByZXR1cm4gaHRtbGBcbiAgICAgICAgPGRpdiBjbGFzcz1cIm1vZGFsIGZhZGVcIiBpZD1cIiR7cHJvZHVjdERldGFpbHNNb2RhbElkfVwiIGRhdGEtYnMtYmFja2Ryb3A9XCJzdGF0aWNcIlxuICAgICAgICAgICAgIGRhdGEtYnMta2V5Ym9hcmQ9XCJ0cnVlXCIgdGFiaW5kZXg9XCItMVwiIGFyaWEtbGFiZWxsZWRieT1cIiR7cHJvZHVjdERldGFpbHNNb2RhbElkfVwiXG4gICAgICAgICAgICAgYXJpYS1oaWRkZW49XCJ0cnVlXCI+XG4gICAgICAgICAgICA8ZGl2IGNsYXNzPVwibW9kYWwtZGlhbG9nIG1vZGFsLWRpYWxvZy1jZW50ZXJlZCBtb2RhbC1kaWFsb2ctc2Nyb2xsYWJsZSBtb2RhbC14bCBtb2RhbC1mdWxsc2NyZWVuLWxnLWRvd25cIj5cbiAgICAgICAgICAgICAgICA8ZGl2IGNsYXNzPVwibW9kYWwtY29udGVudFwiPlxuICAgICAgICAgICAgICAgICAgICA8ZGl2IGNsYXNzPVwibW9kYWwtaGVhZGVyXCI+XG4gICAgICAgICAgICAgICAgICAgICAgICA8aDEgY2xhc3M9XCJtb2RhbC10aXRsZSBmcy00IHRleHQtdGVhbFwiIGlkPVwic3RhdGljQmFja2Ryb3BMYWJlbFwiPlxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICR7cHJvZHVjdERldGFpbHMubmFtZX1cbiAgICAgICAgICAgICAgICAgICAgICAgIDwvaDE+XG4gICAgICAgICAgICAgICAgICAgICAgICA8YnV0dG9uIHR5cGU9XCJidXR0b25cIiBjbGFzcz1cImJ0bi1jbG9zZVwiXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIGRhdGEtYnMtZGlzbWlzcz1cIm1vZGFsXCIgYXJpYS1sYWJlbD1cIkNsb3NlXCI+PC9idXR0b24+XG4gICAgICAgICAgICAgICAgICAgIDwvZGl2PlxuXG4gICAgICAgICAgICAgICAgICAgIDxkaXYgY2xhc3M9XCJtb2RhbC1ib2R5XCI+XG4gICAgICAgICAgICAgICAgICAgICAgICA8ZGl2IGNsYXNzPVwiY29udGFpbmVyLWZsdWlkXCI+XG5cbiAgICAgICAgICAgICAgICAgICAgICAgICAgICA8ZGl2IGNsYXNzPVwicm93IGltYWdlcy1yb3cgbWItNFwiPlxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICA8ZGl2IGNsYXNzPVwiY29sLTEyXCI+XG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICA8ZGl2IGNsYXNzPVwiZC1mbGV4IG92ZXJmbG93LXgtYXV0byBwYi0yIGdhcC0yIGhvcml6b250YWwtdGh1bWJuYWlsc1wiPlxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICR7cHJvZHVjdERldGFpbHMuaW1hZ2VzVXJscy5sZW5ndGggPiAwXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICA/IHByb2R1Y3REZXRhaWxzLmltYWdlc1VybHMubWFwKHVybCA9PiBodG1sYFxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIDxkaXYgY2xhc3M9XCJ0aHVtYm5haWwtd3JhcHBlclwiPlxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICA8aW1nIHNyYz1cIiR7dXJsfVwiIGNsYXNzPVwiaW1nLXRodW1ibmFpbFwiXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgc3R5bGU9XCJoZWlnaHQ6IDIwMHB4OyB3aWR0aDogYXV0bzsgb2JqZWN0LWZpdDogY292ZXI7XCJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICBhbHQ9XCJQcm9kdWN0IEltYWdlXCI+XG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgPC9kaXY+YClcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIDogaHRtbGBcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICA8cCBjbGFzcz1cInRleHQtbXV0ZWQgaXRhbGljXCI+XG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIE5vIGltYWdlcyBhdmFpbGFibGUgZm9yIHRoaXMgcHJvZHVjdC5cbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICA8L3A+YFxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIH1cbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIDwvZGl2PlxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICA8L2Rpdj5cbiAgICAgICAgICAgICAgICAgICAgICAgICAgICA8L2Rpdj5cblxuICAgICAgICAgICAgICAgICAgICAgICAgICAgIDxkaXYgY2xhc3M9XCJyb3cgcHJvZHVjdC1kZXRhaWxzLXJvd1wiPlxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICA8ZGl2IGNsYXNzPVwiY29sLW1kLTYgbWItM1wiPlxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgPGRpdiBjbGFzcz1cImNhcmQgaC0xMDAgYm9yZGVyLTAgc2hhZG93LXNtXCI+XG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgPGRpdiBjbGFzcz1cImNhcmQtYm9keVwiPlxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICA8aDUgY2xhc3M9XCJjYXJkLXRpdGxlIHRleHQtbmF2eSBib3JkZXItYm90dG9tIHBiLTIgbWItM1wiPlxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgUHJvZHVjdCBJbmZvcm1hdGlvblxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICA8L2g1PlxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICA8ZGl2IGNsYXNzPVwicm93IG1iLTJcIj5cbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIDxkaXYgY2xhc3M9XCJjb2wtNSBmdy1ib2xkIHRleHQtdGVhbFwiPk93bmVyOjwvZGl2PlxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgPGRpdiBjbGFzcz1cImNvbC03XCI+JHtwcm9kdWN0RGV0YWlscy5vd25lck5hbWV9PC9kaXY+XG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIDwvZGl2PlxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICA8ZGl2IGNsYXNzPVwicm93IG1iLTJcIj5cbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIDxkaXYgY2xhc3M9XCJjb2wtNSBmdy1ib2xkIHRleHQtdGVhbFwiPkNhdGVnb3J5OjwvZGl2PlxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgPGRpdiBjbGFzcz1cImNvbC03XCI+XG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgJHtwcm9kdWN0RGV0YWlscy5jYXRlZ29yeU5hbWV9XG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICA8L2Rpdj5cbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgPC9kaXY+XG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIDxkaXYgY2xhc3M9XCJyb3cgbWItMlwiPlxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgPGRpdiBjbGFzcz1cImNvbC01IGZ3LWJvbGQgdGV4dC10ZWFsXCI+XG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgUXVhbnRpdHkgaW4gU3RvY2s6XG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICA8L2Rpdj5cbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIDxkaXYgY2xhc3M9XCJjb2wtN1wiPlxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICR7cHJvZHVjdERldGFpbHMucXVhbnRpdHlJblN0b2NrfVxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgPC9kaXY+XG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIDwvZGl2PlxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICA8ZGl2IGNsYXNzPVwicm93XCI+XG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICA8ZGl2IGNsYXNzPVwiY29sLTUgZnctYm9sZCB0ZXh0LXRlYWxcIj5QcmljZTo8L2Rpdj5cbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIDxkaXYgY2xhc3M9XCJjb2wtNyBmcy01IGZ3LWJvbGQgdGV4dC1uYXZ5XCI+XG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICQke3Byb2R1Y3REZXRhaWxzLnNlbGxpbmdQcmljZS50b0ZpeGVkKDIpfVxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgPC9kaXY+XG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIDwvZGl2PlxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIDwvZGl2PlxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgPC9kaXY+XG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIDwvZGl2PlxuXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIDxkaXYgY2xhc3M9XCJjb2wtbWQtNiBtYi0zXCI+XG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICA8ZGl2IGNsYXNzPVwiY2FyZCBoLTEwMCBib3JkZXItMCBzaGFkb3ctc21cIj5cbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICA8ZGl2IGNsYXNzPVwiY2FyZC1ib2R5XCI+XG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIDxoNSBjbGFzcz1cImNhcmQtdGl0bGUgdGV4dC1uYXZ5IGJvcmRlci1ib3R0b20gcGItMiBtYi0zXCI+QXBwcm92YWwgU3RhdHVzPC9oNT5cbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgPGRpdiBjbGFzcz1cInJvdyBtYi0yXCI+XG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICA8ZGl2IGNsYXNzPVwiY29sLTUgZnctYm9sZCB0ZXh0LXRlYWxcIj5TdGF0dXM6PC9kaXY+XG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICA8ZGl2IGNsYXNzPVwiY29sLTdcIj5cbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICA8c3BhbiBjbGFzcz1cImJhZGdlICR7Z2V0U3RhdHVzQmFkZ2VDbGFzcyhwcm9kdWN0RGV0YWlscy5hcHByb3ZhbERlY2lzaW9uLmFwcHJvdmFsU3RhdHVzKX1cIj5cbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgJHtmb3JtYXRTdGF0dXMocHJvZHVjdERldGFpbHMuYXBwcm92YWxEZWNpc2lvbi5hcHByb3ZhbFN0YXR1cyl9XG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgPC9zcGFuPlxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgPC9kaXY+XG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIDwvZGl2PlxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICA8ZGl2IGNsYXNzPVwicm93IG1iLTJcIj5cbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIDxkaXYgY2xhc3M9XCJjb2wtNSBmdy1ib2xkIHRleHQtdGVhbFwiPlxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIERlY2lzaW9uIE1ha2VyOlxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgPC9kaXY+XG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICA8ZGl2IGNsYXNzPVwiY29sLTdcIj5cbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAke3Byb2R1Y3REZXRhaWxzLmFwcHJvdmFsRGVjaXNpb24uYXBwcm92YWxEZWNpc2lvbk1ha2VyVXNlcm5hbWUgfHwgXCJOL0FcIn1cbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIDwvZGl2PlxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICA8L2Rpdj5cbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgPGRpdiBjbGFzcz1cInJvd1wiPlxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgPGRpdiBjbGFzcz1cImNvbC01IGZ3LWJvbGQgdGV4dC10ZWFsXCI+VGltZTo8L2Rpdj5cbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIDxkaXYgY2xhc3M9XCJjb2wtN1wiPlxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICR7cHJvZHVjdERldGFpbHMuYXBwcm92YWxEZWNpc2lvbi50aW1lT2ZEZWNpc2lvblxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgPyBuZXcgRGF0ZShwcm9kdWN0RGV0YWlscy5hcHByb3ZhbERlY2lzaW9uLnRpbWVPZkRlY2lzaW9uKS50b0xvY2FsZVN0cmluZygpXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICA6IFwiTi9BXCJ9XG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICA8L2Rpdj5cbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgPC9kaXY+XG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgPC9kaXY+XG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICA8L2Rpdj5cbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgPC9kaXY+XG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgPC9kaXY+XG5cbiAgICAgICAgICAgICAgICAgICAgICAgICAgICA8ZGl2IGNsYXNzPVwicm93IGRlc2NyaXB0aW9uLXJvdyBtdC0yXCI+XG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIDxkaXYgY2xhc3M9XCJjb2wtMTJcIj5cbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIDxkaXYgY2xhc3M9XCJjYXJkIGJvcmRlci0wIHNoYWRvdy1zbVwiPlxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIDxkaXYgY2xhc3M9XCJjYXJkLWJvZHlcIj5cbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgPGg1IGNsYXNzPVwiY2FyZC10aXRsZSB0ZXh0LW5hdnkgYm9yZGVyLWJvdHRvbSBwYi0yIG1iLTNcIj5EZXNjcmlwdGlvbjwvaDU+XG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIDxwIGNsYXNzPVwiY2FyZC10ZXh0IHRleHQtbXV0ZWRcIiBzdHlsZT1cIndoaXRlLXNwYWNlOiBwcmUtbGluZTtcIj5cbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICR7cHJvZHVjdERldGFpbHMuZGVzY3JpcHRpb259XG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIDwvcD5cbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICA8L2Rpdj5cbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIDwvZGl2PlxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICA8L2Rpdj5cbiAgICAgICAgICAgICAgICAgICAgICAgICAgICA8L2Rpdj5cblxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICR7cHJvZHVjdERldGFpbHMub3duZXJOYW1lICE9IG51bGxcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgID8gaHRtbGBcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICA8ZGl2IGNsYXNzPVwicm93IG1hbmFnZS1hcHByb3ZhbC1yb3cgbXQtNFwiPlxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICA8ZGl2IGNsYXNzPVwiY29sLTEyXCI+XG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICA8Zm9ybSBAc3VibWl0PSR7YXN5bmMgKGV2ZW50OiBFdmVudCkgPT4gb25Nb2RpZnlBcHByb3ZhbChldmVudCwgcHJvZHVjdERldGFpbHMsIGNvbnRleHQpfT5cbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICA8ZGl2IGNsYXNzPVwiY2FyZCBib3JkZXItMCBzaGFkb3ctc20gYm9yZGVyLXRvcCBib3JkZXItNCBib3JkZXItdGVhbFwiPlxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICA8ZGl2IGNsYXNzPVwiY2FyZC1ib2R5XCI+XG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICA8aDUgY2xhc3M9XCJjYXJkLXRpdGxlIHRleHQtbmF2eSBtYi00XCI+TWFuYWdlIEFwcHJvdmFsIFN0YXR1czwvaDU+XG5cbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIDxkaXYgY2xhc3M9XCJtYi00IHNlbGVjdC1zdGF0dXMtc2VjdGlvblwiPlxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIDxsYWJlbCBjbGFzcz1cImZvcm0tbGFiZWwgZnctYm9sZCB0ZXh0LXRlYWxcIj5TZWxlY3QgU3RhdHVzPC9sYWJlbD5cbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICA8ZGl2IGNsYXNzPVwiYnRuLWdyb3VwIHctMTAwXCIgcm9sZT1cImdyb3VwXCIgYXJpYS1sYWJlbD1cIkFwcHJvdmFsIHN0YXR1cyBzZWxlY3Rpb25cIlxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgQGNoYW5nZT0ke29uU3RhdHVzQ2hhbmdlVmFsaWRhdGlvbn0+XG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIDxpbnB1dCB0eXBlPVwicmFkaW9cIiBjbGFzcz1cImJ0bi1jaGVja1wiIG5hbWU9XCJhcHByb3ZhbFN0YXR1cy0ke3Byb2R1Y3REZXRhaWxzLmlkfVwiIGlkPVwic3RhdHVzQXBwcm92ZWQtJHtwcm9kdWN0RGV0YWlscy5pZH1cIlxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgdmFsdWU9XCJBcHByb3ZlZFwiIC5jaGVja2VkPSR7cHJvZHVjdERldGFpbHMuYXBwcm92YWxEZWNpc2lvbi5hcHByb3ZhbFN0YXR1cyA9PT0gXCJBcHByb3ZlZFwifT5cbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgPGxhYmVsIGNsYXNzPVwiYnRuIGJ0bi1vdXRsaW5lLXN1Y2Nlc3NcIiBmb3I9XCJzdGF0dXNBcHByb3ZlZC0ke3Byb2R1Y3REZXRhaWxzLmlkfVwiPkFwcHJvdmVkPC9sYWJlbD5cblxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICA8aW5wdXQgdHlwZT1cInJhZGlvXCIgY2xhc3M9XCJidG4tY2hlY2tcIiBuYW1lPVwiYXBwcm92YWxTdGF0dXMtJHtwcm9kdWN0RGV0YWlscy5pZH1cIiBpZD1cInN0YXR1c0Rpc2FwcHJvdmVkLSR7cHJvZHVjdERldGFpbHMuaWR9XCJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIHZhbHVlPVwiRGlzYXBwcm92ZWRcIiAuY2hlY2tlZD0ke3Byb2R1Y3REZXRhaWxzLmFwcHJvdmFsRGVjaXNpb24uYXBwcm92YWxTdGF0dXMgPT09IFwiRGlzYXBwcm92ZWRcIn0+XG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIDxsYWJlbCBjbGFzcz1cImJ0biBidG4tb3V0bGluZS1kYW5nZXJcIiBmb3I9XCJzdGF0dXNEaXNhcHByb3ZlZC0ke3Byb2R1Y3REZXRhaWxzLmlkfVwiPkRpc2FwcHJvdmVkPC9sYWJlbD5cbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICA8L2Rpdj5cbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICA8ZGl2IGNsYXNzPVwidGV4dC1kYW5nZXIgYXBwcm92YWwtdmFsaWRhdGlvbi1zZWN0aW9uXCI+PC9kaXY+XG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICA8L2Rpdj5cblxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgPGRpdiBjbGFzcz1cIm1iLTQgZGVjaXNpb24tanVzdGlmaWNhdGlvbi1zZWN0aW9uXCI+XG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgPGxhYmVsIGZvcj1cImRlY2lzaW9uSnVzdGlmaWNhdGlvbi0ke3Byb2R1Y3REZXRhaWxzLmlkfVwiIGNsYXNzPVwiZm9ybS1sYWJlbCBmdy1ib2xkIHRleHQtdGVhbFwiPkRlY2lzaW9uIEp1c3RpZmljYXRpb248L2xhYmVsPlxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIDx0ZXh0YXJlYSBjbGFzcz1cImZvcm0tY29udHJvbFwiIGlkPVwiZGVjaXNpb25KdXN0aWZpY2F0aW9uLSR7cHJvZHVjdERldGFpbHMuaWR9XCJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgbmFtZT1cImRlY2lzaW9uLWp1c3RpZmljYXRpb24tJHtwcm9kdWN0RGV0YWlscy5pZH1cIiByb3dzPVwiM1wiXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIHBsYWNlaG9sZGVyPVwiUHJvdmlkZSBhIHJlYXNvbiBmb3IgdGhlIGRlY2lzaW9uLi4uXCJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgLnZhbHVlIT0ke3Byb2R1Y3REZXRhaWxzLmFwcHJvdmFsRGVjaXNpb24uZGVjaXNpb25KdXN0aWZpY2F0aW9ufVxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICBAY2hhbmdlPSR7YXBwcm92YWxEZWNpc2lvbkp1c3RpZmljYXRpb25WYWxpZGF0aW9ufT48L3RleHRhcmVhPlxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIDxkaXYgY2xhc3M9XCJ0ZXh0LWRhbmdlciBqdXN0aWZpY2F0aW9uLXZhbGlkYXRpb24tc2VjdGlvblwiPjwvZGl2PlxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgPC9kaXY+XG5cbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIDxkaXYgY2xhc3M9XCJkLWdyaWRcIj5cbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICA8YnV0dG9uIHR5cGU9XCJzdWJtaXRcIiBjbGFzcz1cImJ0biBidG4tdGVhbCBidG4tbGcgc2hhZG93LXNtXCIgaWQ9XCJzYXZlLWFwcHJvdmFsLWNoYW5nZXNcIj5cbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgU2F2ZSBBcHByb3ZhbCBDaGFuZ2VzXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgPC9idXR0b24+XG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICA8L2Rpdj5cbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgPC9kaXY+XG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgPC9kaXY+XG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICA8L2Zvcm0+XG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIDwvZGl2PlxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIDwvZGl2PmBcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIDogaHRtbGBgXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgfVxuICAgICAgICAgICAgICAgICAgICAgICAgPC9kaXY+XG4gICAgICAgICAgICAgICAgICAgIDwvZGl2PlxuXG4gICAgICAgICAgICAgICAgICAgIDxkaXYgY2xhc3M9XCJtb2RhbC1mb290ZXJcIj5cbiAgICAgICAgICAgICAgICAgICAgICAgIDxidXR0b24gdHlwZT1cImJ1dHRvblwiIGNsYXNzPVwiYnRuIGJ0bi1zZWNvbmRhcnlcIiBkYXRhLWJzLWRpc21pc3M9XCJtb2RhbFwiPlxuICAgICAgICAgICAgICAgICAgICAgICAgICAgIENsb3NlXG4gICAgICAgICAgICAgICAgICAgICAgICA8L2J1dHRvbj5cbiAgICAgICAgICAgICAgICAgICAgPC9kaXY+XG4gICAgICAgICAgICAgICAgPC9kaXY+XG4gICAgICAgICAgICA8L2Rpdj5cbiAgICAgICAgPC9kaXY+YDtcbn1cblxuZnVuY3Rpb24gZXJyVGVtcGxhdGUocHJvZHVjdERldGFpbHNNb2RhbElkOiBzdHJpbmcpOiBUZW1wbGF0ZVJlc3VsdCB7XG4gICAgcmV0dXJuIGh0bWxgXG4gICAgICAgIDxkaXYgY2xhc3M9XCJtb2RhbCBmYWRlXCIgaWQ9XCIke3Byb2R1Y3REZXRhaWxzTW9kYWxJZH1cIlxuICAgICAgICAgICAgIGRhdGEtYnMta2V5Ym9hcmQ9XCJ0cnVlXCIgdGFiaW5kZXg9XCItMVwiXG4gICAgICAgICAgICAgYXJpYS1sYWJlbGxlZGJ5PVwicHJvZHVjdERldGFpbHNNb2RhbElkXCIgYXJpYS1oaWRkZW49XCJ0cnVlXCI+XG4gICAgICAgICAgICA8ZGl2IGNsYXNzPVwibW9kYWwtZGlhbG9nIG1vZGFsLWRpYWxvZy1jZW50ZXJlZCBtb2RhbC1zbVwiPlxuICAgICAgICAgICAgICAgIDxkaXYgY2xhc3M9XCJtb2RhbC1jb250ZW50XCI+XG4gICAgICAgICAgICAgICAgICAgIDxkaXYgY2xhc3M9XCJtb2RhbC1oZWFkZXJcIj5cbiAgICAgICAgICAgICAgICAgICAgICAgIDxoMSBjbGFzcz1cIm1vZGFsLXRpdGxlIGZzLTUgdGV4dC1kYW5nZXJcIiBpZD1cImVycm9yLW1vZGFsXCI+RXJyb3I8L2gxPlxuICAgICAgICAgICAgICAgICAgICAgICAgPGJ1dHRvbiB0eXBlPVwiYnV0dG9uXCIgY2xhc3M9XCJidG4tY2xvc2VcIiBkYXRhLWJzLWRpc21pc3M9XCJtb2RhbFwiIGFyaWEtbGFiZWw9XCJDbG9zZVwiPjwvYnV0dG9uPlxuICAgICAgICAgICAgICAgICAgICA8L2Rpdj5cblxuICAgICAgICAgICAgICAgICAgICA8ZGl2IGNsYXNzPVwibW9kYWwtYm9keVwiPlxuICAgICAgICAgICAgICAgICAgICAgICAgT29wcy4uLiBTb21ldGhpbmcgd2VudCB3cm9uZyEgUGxlYXNlIHRyeSBhZ2FpbiBsYXRlci5cbiAgICAgICAgICAgICAgICAgICAgPC9kaXY+XG5cbiAgICAgICAgICAgICAgICAgICAgPGRpdiBjbGFzcz1cIm1vZGFsLWZvb3RlclwiPlxuICAgICAgICAgICAgICAgICAgICAgICAgPGJ1dHRvbiB0eXBlPVwiYnV0dG9uXCIgY2xhc3M9XCJidG4gYnRuLXNlY29uZGFyeVwiIGRhdGEtYnMtZGlzbWlzcz1cIm1vZGFsXCI+XG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgT2tcbiAgICAgICAgICAgICAgICAgICAgICAgIDwvYnV0dG9uPlxuICAgICAgICAgICAgICAgICAgICA8L2Rpdj5cbiAgICAgICAgICAgICAgICA8L2Rpdj5cbiAgICAgICAgICAgIDwvZGl2PlxuICAgICAgICA8L2Rpdj5gO1xufVxuXG5hc3luYyBmdW5jdGlvbiBvbk1vZGlmeUFwcHJvdmFsKFxuICAgIGV2ZW50OiBFdmVudCxcbiAgICBwcm9kdWN0RGV0YWlsczogUHJvZHVjdERldGFpbHMsXG4gICAgY29udGV4dDogVGFibGVDb250ZXh0XG4pOiBQcm9taXNlPHZvaWQ+IHtcbiAgICBldmVudC5wcmV2ZW50RGVmYXVsdCgpO1xuXG4gICAgY29uc3QgZm9ybURhdGEgPSBuZXcgRm9ybURhdGEoZXZlbnQuY3VycmVudFRhcmdldCBhcyBIVE1MRm9ybUVsZW1lbnQgfCB1bmRlZmluZWQpO1xuICAgIGNvbnN0IG5ld0FwcHJvdmFsU3RhdHVzID0gZm9ybURhdGEuZ2V0KGBhcHByb3ZhbFN0YXR1cy0ke3Byb2R1Y3REZXRhaWxzLmlkfWApO1xuICAgIGNvbnN0IGRlY2lzaW9uSnVzdGlmaWNhdGlvbiA9IGZvcm1EYXRhLmdldChgZGVjaXNpb24tanVzdGlmaWNhdGlvbi0ke3Byb2R1Y3REZXRhaWxzLmlkfWApO1xuXG4gICAgbGV0IGlzU2FtZURhdGEgPSBmYWxzZTtcbiAgICBpZihuZXdBcHByb3ZhbFN0YXR1cyA9PT0gcHJvZHVjdERldGFpbHMuYXBwcm92YWxEZWNpc2lvbi5hcHByb3ZhbFN0YXR1cykge1xuICAgICAgICAoZXZlbnQuY3VycmVudFRhcmdldCBhcyBIVE1MRm9ybUVsZW1lbnQpXG4gICAgICAgICAgICAucXVlcnlTZWxlY3RvcjxIVE1MRGl2RWxlbWVudD4oXCJkaXYuYXBwcm92YWwtdmFsaWRhdGlvbi1zZWN0aW9uXCIpIVxuICAgICAgICAgICAgLnRleHRDb250ZW50ID0gXCJBcHByb3ZhbCBzdGF0dXMgd2FzIG5vdCBjaGFuZ2VkLlwiO1xuXG4gICAgICAgIGlzU2FtZURhdGEgPSB0cnVlO1xuICAgIH1cbiAgICBpZihkZWNpc2lvbkp1c3RpZmljYXRpb24gPT09IHByb2R1Y3REZXRhaWxzLmFwcHJvdmFsRGVjaXNpb24uZGVjaXNpb25KdXN0aWZpY2F0aW9uKSB7XG4gICAgICAgIChldmVudC5jdXJyZW50VGFyZ2V0IGFzIEhUTUxEaXZFbGVtZW50KVxuICAgICAgICAgICAgLnF1ZXJ5U2VsZWN0b3IoXCJkaXYuanVzdGlmaWNhdGlvbi12YWxpZGF0aW9uLXNlY3Rpb25cIikhXG4gICAgICAgICAgICAudGV4dENvbnRlbnQgPSBcIlBsZWFzZSBwcm92aWRlIGEgbmV3IHJlYXNvbiBmb3IgdGhlIG5ldyBhcHByb3ZhbCBkZWNpc2lvbi5cIjtcblxuICAgICAgICBpc1NhbWVEYXRhID0gdHJ1ZTtcbiAgICB9XG5cbiAgICBpZihpc1NhbWVEYXRhKSB7XG4gICAgICAgIHJldHVybjtcbiAgICB9XG5cbiAgICBjb25zdCBtb2RhbEVsID0gKGV2ZW50LmN1cnJlbnRUYXJnZXQgYXMgSFRNTEZvcm1FbGVtZW50KVxuICAgICAgICAuY2xvc2VzdChgZGl2Lm1vZGFsI3Byb2R1Y3QtZGV0YWlscy0ke3Byb2R1Y3REZXRhaWxzLmlkfWApITtcbiAgICBjb25zdCBtb2RhbCA9IGJvb3RzdHJhcC5Nb2RhbC5nZXRJbnN0YW5jZShtb2RhbEVsKSE7XG4gICAgbW9kYWwudG9nZ2xlKCk7XG5cbiAgICBjb25zdCBtb2RpZnlBcHByb3ZhbFJlc3VsdCA9IGF3YWl0IGNvbnRleHQubW9kaWZ5UHJvZHVjdEFwcHJvdmFsKHtcbiAgICAgICAgcHJvZHVjdElkOiBwcm9kdWN0RGV0YWlscy5pZCxcbiAgICAgICAgYXBwcm92YWxTdGF0dXM6IG5ld0FwcHJvdmFsU3RhdHVzLFxuICAgICAgICBkZWNpc2lvbkp1c3RpZmljYXRpb246IGRlY2lzaW9uSnVzdGlmaWNhdGlvbixcbiAgICB9IGFzIEVkaXRQcm9kdWN0QXBwcm92YWxTdGF0dXMpO1xuICAgIGlmKCFtb2RpZnlBcHByb3ZhbFJlc3VsdCkge1xuICAgICAgICBzaG93RXJyb3JTd2FsKClcbiAgICAgICAgICAgIC50aGVuKGFzeW5jICgpID0+IGF3YWl0IHNob3dQcm9kdWN0c1RhYmxlKGNvbnRleHQpKTtcbiAgICB9IGVsc2Uge1xuICAgICAgICBzaG93UGxhaW5TdWNjZXNzU3dhbChcIlRoZSBwcm9kdWN0IGFwcHJvdmFsIHN0YXR1cyBoYXMgYmVlbiBjaGFuZ2VkIHN1Y2Nlc3NmdWxseS5cIilcbiAgICAgICAgICAgIC50aGVuKGFzeW5jICgpID0+IGF3YWl0IHNob3dQcm9kdWN0c1RhYmxlKGNvbnRleHQpKTtcbiAgICB9XG59XG5cbmZ1bmN0aW9uIG9uU3RhdHVzQ2hhbmdlVmFsaWRhdGlvbihldmVudDogRXZlbnQpIHtcbiAgICBjb25zdCB2YWxpZGF0aW9uU2VjdGlvbiA9IChldmVudC5jdXJyZW50VGFyZ2V0IGFzIEhUTUxEaXZFbGVtZW50KVxuICAgICAgICAuY2xvc2VzdChcImRpdi5zZWxlY3Qtc3RhdHVzLXNlY3Rpb25cIikhXG4gICAgICAgIC5xdWVyeVNlbGVjdG9yKFwiZGl2LmFwcHJvdmFsLXZhbGlkYXRpb24tc2VjdGlvblwiKTtcblxuICAgIGlmKHZhbGlkYXRpb25TZWN0aW9uPy50ZXh0Q29udGVudCAhPT0gXCJcIikge1xuICAgICAgICB2YWxpZGF0aW9uU2VjdGlvbiEudGV4dENvbnRlbnQgPSBcIlwiO1xuICAgIH1cbn1cblxuZnVuY3Rpb24gYXBwcm92YWxEZWNpc2lvbkp1c3RpZmljYXRpb25WYWxpZGF0aW9uKGV2ZW50OiBFdmVudCkge1xuICAgIGNvbnN0IHN1Ym1pdEJ1dHRvbiA9IGRvY3VtZW50LnF1ZXJ5U2VsZWN0b3IoXCIjc2F2ZS1hcHByb3ZhbC1jaGFuZ2VzXCIpO1xuICAgIGNvbnN0IHRleHRBcmVhID0gZXZlbnQuY3VycmVudFRhcmdldCBhcyBIVE1MVGV4dEFyZWFFbGVtZW50O1xuICAgIGNvbnN0IHZhbGlkYXRpb25TZWN0aW9uID0gdGV4dEFyZWFcbiAgICAgICAgLmNsb3Nlc3QoXCJkaXYuZGVjaXNpb24tanVzdGlmaWNhdGlvbi1zZWN0aW9uXCIpIVxuICAgICAgICAucXVlcnlTZWxlY3RvcihcImRpdi5qdXN0aWZpY2F0aW9uLXZhbGlkYXRpb24tc2VjdGlvblwiKSE7XG5cbiAgICBpZih0ZXh0QXJlYS52YWx1ZS5sZW5ndGggPCA0IHx8IHRleHRBcmVhLnZhbHVlLmxlbmd0aCA+IDMwMDApIHtcbiAgICAgICAgdmFsaWRhdGlvblNlY3Rpb24udGV4dENvbnRlbnRcbiAgICAgICAgICAgID0gXCJUaGUgZGVjaXNpb24gcmVhc29uIHNob3VsZCBiZSBiZXR3ZWVuIDQgYW5kIDMwMDAgY2hhcmFjdGVycyBsb25nLlwiO1xuXG4gICAgICAgIHN1Ym1pdEJ1dHRvbj8uc2V0QXR0cmlidXRlKFwiZGlzYWJsZWRcIiwgXCJkaXNhYmxlZFwiKTtcbiAgICB9IGVsc2Uge1xuICAgICAgICB2YWxpZGF0aW9uU2VjdGlvbi50ZXh0Q29udGVudCA9IFwiXCI7XG4gICAgICAgIHN1Ym1pdEJ1dHRvbj8ucmVtb3ZlQXR0cmlidXRlKFwiZGlzYWJsZWRcIik7XG4gICAgfVxufVxuXG5mdW5jdGlvbiBnZXRTdGF0dXNCYWRnZUNsYXNzKHN0YXR1czogUHJvZHVjdHNBcHByb3ZhbFN0YXR1cykge1xuICAgIHN3aXRjaCAoc3RhdHVzKSB7XG4gICAgICAgIGNhc2UgXCJBcHByb3ZlZFwiOiByZXR1cm4gXCJiZy1zdWNjZXNzXCI7XG4gICAgICAgIGNhc2UgXCJEaXNhcHByb3ZlZFwiOiByZXR1cm4gXCJiZy1kYW5nZXJcIjtcbiAgICAgICAgY2FzZSBcIldhaXRpbmdBcHByb3ZhbFwiOiByZXR1cm4gXCJiZy13YXJuaW5nIHRleHQtZGFya1wiO1xuICAgICAgICBkZWZhdWx0OiByZXR1cm4gXCJiZy1zZWNvbmRhcnlcIjtcbiAgICB9XG59XG5cbmZ1bmN0aW9uIGZvcm1hdFN0YXR1cyhzdGF0dXM6IFByb2R1Y3RzQXBwcm92YWxTdGF0dXMpIHtcbiAgICBzd2l0Y2ggKHN0YXR1cykge1xuICAgICAgICBjYXNlIFwiQXBwcm92ZWRcIjogcmV0dXJuIFwiQXBwcm92ZWRcIjtcbiAgICAgICAgY2FzZSBcIkRpc2FwcHJvdmVkXCI6IHJldHVybiBcIkRpc2FwcHJvdmVkXCI7XG4gICAgICAgIGNhc2UgXCJXYWl0aW5nQXBwcm92YWxcIjogcmV0dXJuIFwiV2FpdGluZyBBcHByb3ZhbFwiO1xuICAgICAgICBkZWZhdWx0OiByZXR1cm4gc3RhdHVzO1xuICAgIH1cbn0iXX0=