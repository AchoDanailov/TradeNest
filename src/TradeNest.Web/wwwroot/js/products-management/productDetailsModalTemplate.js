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
    if (!newApprovalStatus || !decisionJustification) {
        showErrorSwal()
            .then(async () => await showProductsTable(context));
    }
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
        showErrorSwal().then(async () => await showProductsTable(context));
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
//# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJmaWxlIjoicHJvZHVjdERldGFpbHNNb2RhbFRlbXBsYXRlLmpzIiwic291cmNlUm9vdCI6IiIsInNvdXJjZXMiOlsiLi4vLi4vLi4vRnJvbnRFbmRTY3JpcHRzL3Byb2R1Y3RzLW1hbmFnZW1lbnQvcHJvZHVjdERldGFpbHNNb2RhbFRlbXBsYXRlLnRzIl0sIm5hbWVzIjpbXSwibWFwcGluZ3MiOiJBQUFBLE9BQU8sRUFBRSxLQUFLLEVBQUUsTUFBTSxXQUFXLENBQUM7QUFDbEMsT0FBTyxFQUFFLElBQUksRUFBdUIsTUFBTSxVQUFVLENBQUM7QUFPckQsT0FBTyxFQUFFLGFBQWEsRUFBRSxvQkFBb0IsRUFBRSxNQUFNLHNCQUFzQixDQUFDO0FBQzNFLE9BQU8saUJBQWlCLE1BQU0sb0JBQW9CLENBQUM7QUFHbkQsTUFBTSxDQUFDLE9BQU8sQ0FBQyxLQUFLLFVBQVUsOEJBQThCLENBQ3hELFNBQWlCLEVBQ2pCLE9BQXFCO0lBRXJCLE1BQU0scUJBQXFCLEdBQUcsbUJBQW1CLFNBQVMsRUFBRSxDQUFDO0lBRTdELE1BQU0sY0FBYyxHQUFHLE1BQU0sT0FBTyxDQUFDLGlCQUFpQixDQUFDLFNBQVMsQ0FBQyxDQUFDO0lBQ2xFLElBQUksQ0FBQyxjQUFjLEVBQUUsQ0FBQztRQUNsQixPQUFPLFdBQVcsQ0FBQyxxQkFBcUIsQ0FBQyxDQUFDO0lBQzlDLENBQUM7SUFFRCxPQUFPLFFBQVEsQ0FBQyxjQUFjLEVBQUUscUJBQXFCLEVBQUUsT0FBTyxDQUFDLENBQUM7QUFDcEUsQ0FBQztBQUVELFNBQVMsUUFBUSxDQUNiLGNBQThCLEVBQzlCLHFCQUE2QixFQUM3QixPQUFxQjtJQUVyQixPQUFPLElBQUksQ0FBQTtzQ0FDdUIscUJBQXFCO3NFQUNXLHFCQUFxQjs7Ozs7OzhCQU03RCxjQUFjLENBQUMsSUFBSTs7Ozs7Ozs7Ozs7OzBDQVlQLGNBQWMsQ0FBQyxVQUFVLENBQUMsTUFBTSxHQUFHLENBQUM7UUFDOUIsQ0FBQyxDQUFDLGNBQWMsQ0FBQyxVQUFVLENBQUMsR0FBRyxDQUFD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YSxDQUFDLENBQUMsT0FBTyxXQUFXLENBQUM7UUFDdkMsS0FBSyxpQkFBaUIsQ0FBQyxDQUFDLE9BQU8sc0JBQXNCLENBQUM7UUFDdEQsT0FBTyxDQUFDLENBQUMsT0FBTyxjQUFjLENBQUM7SUFDbkMsQ0FBQztBQUNMLENBQUM7QUFFRCxTQUFTLFlBQVksQ0FBQyxNQUE4QjtJQUNoRCxRQUFRLE1BQU0sRUFBRSxDQUFDO1FBQ2IsS0FBSyxVQUFVLENBQUMsQ0FBQyxPQUFPLFVBQVUsQ0FBQztRQUNuQyxLQUFLLGFBQWEsQ0FBQyxDQUFDLE9BQU8sYUFBYSxDQUFDO1FBQ3pDLEtBQUssaUJBQWlCLENBQUMsQ0FBQyxPQUFPLGtCQUFrQixDQUFDO1FBQ2xELE9BQU8sQ0FBQyxDQUFDLE9BQU8sTUFBTSxDQUFDO0lBQzNCLENBQUM7QUFDTCxDQUFDIiwic291cmNlc0NvbnRlbnQiOlsiaW1wb3J0IHsgTW9kYWwgfSBmcm9tIFwiYm9vdHN0cmFwXCI7XG5pbXBvcnQgeyBodG1sLCB0eXBlIFRlbXBsYXRlUmVzdWx0IH0gZnJvbSBcImxpdC1odG1sXCI7XG5cbmltcG9ydCB0eXBlIHtcbiAgICBQcm9kdWN0RGV0YWlscyxcbiAgICBQcm9kdWN0c0FwcHJvdmFsU3RhdHVzXG59IGZyb20gXCIuLi90eXBlcy9wcm9kdWN0cy50c1wiO1xuaW1wb3J0IHR5cGUgeyBUYWJsZUNvbnRleHQgfSBmcm9tIFwiLi4vdHlwZXMvdGFibGVDb250ZXh0LnRzXCI7XG5pbXBvcnQgeyBzaG93RXJyb3JTd2FsLCBzaG93UGxhaW5TdWNjZXNzU3dhbCB9IGZyb20gXCIuLi91dGlscy9kb21VdGlscy5qc1wiO1xuaW1wb3J0IHNob3dQcm9kdWN0c1RhYmxlIGZyb20gXCIuL3Byb2R1Y3RzVGFibGUuanNcIjtcblxuXG5leHBvcnQgZGVmYXVsdCBhc3luYyBmdW5jdGlvbiBnZXRQcm9kdWN0RGV0YWlsc01vZGFsVGVtcGxhdGUoXG4gICAgcHJvZHVjdElkOiBzdHJpbmcsXG4gICAgY29udGV4dDogVGFibGVDb250ZXh0XG4pOiBQcm9taXNlPFRlbXBsYXRlUmVzdWx0PiB7XG4gICAgY29uc3QgcHJvZHVjdERldGFpbHNNb2RhbElkID0gYHByb2R1Y3QtZGV0YWlscy0ke3Byb2R1Y3RJZH1gO1xuXG4gICAgY29uc3QgcHJvZHVjdERldGFpbHMgPSBhd2FpdCBjb250ZXh0LmdldFByb2R1Y3REZXRhaWxzKHByb2R1Y3RJZCk7XG4gICAgaWYgKCFwcm9kdWN0RGV0YWlscykge1xuICAgICAgICByZXR1cm4gZXJyVGVtcGxhdGUocHJvZHVjdERldGFpbHNNb2RhbElkKTtcbiAgICB9XG5cbiAgICByZXR1cm4gdGVtcGxhdGUocHJvZHVjdERldGFpbHMsIHByb2R1Y3REZXRhaWxzTW9kYWxJZCwgY29udGV4dCk7XG59XG5cbmZ1bmN0aW9uIHRlbXBsYXRlKFxuICAgIHByb2R1Y3REZXRhaWxzOiBQcm9kdWN0RGV0YWlscyxcbiAgICBwcm9kdWN0RGV0YWlsc01vZGFsSWQ6IHN0cmluZyxcbiAgICBjb250ZXh0OiBUYWJsZUNvbnRleHRcbik6IFRlbXBsYXRlUmVzdWx0IHtcbiAgICByZXR1cm4gaHRtbGBcbiAgICAgICAgPGRpdiBjbGFzcz1cIm1vZGFsIGZhZGVcIiBpZD1cIiR7cHJvZHVjdERldGFpbHNNb2RhbElkfVwiIGRhdGEtYnMtYmFja2Ryb3A9XCJzdGF0aWNcIlxuICAgICAgICAgICAgIGRhdGEtYnMta2V5Ym9hcmQ9XCJ0cnVlXCIgdGFiaW5kZXg9XCItMVwiIGFyaWEtbGFiZWxsZWRieT1cIiR7cHJvZHVjdERldGFpbHNNb2RhbElkfVwiXG4gICAgICAgICAgICAgYXJpYS1oaWRkZW49XCJ0cnVlXCI+XG4gICAgICAgICAgICA8ZGl2IGNsYXNzPVwibW9kYWwtZGlhbG9nIG1vZGFsLWRpYWxvZy1jZW50ZXJlZCBtb2RhbC1kaWFsb2ctc2Nyb2xsYWJsZSBtb2RhbC14bCBtb2RhbC1mdWxsc2NyZWVuLWxnLWRvd25cIj5cbiAgICAgICAgICAgICAgICA8ZGl2IGNsYXNzPVwibW9kYWwtY29udGVudFwiPlxuICAgICAgICAgICAgICAgICAgICA8ZGl2IGNsYXNzPVwibW9kYWwtaGVhZGVyXCI+XG4gICAgICAgICAgICAgICAgICAgICAgICA8aDEgY2xhc3M9XCJtb2RhbC10aXRsZSBmcy00IHRleHQtdGVhbFwiIGlkPVwic3RhdGljQmFja2Ryb3BMYWJlbFwiPlxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICR7cHJvZHVjdERldGFpbHMubmFtZX1cbiAgICAgICAgICAgICAgICAgICAgICAgIDwvaDE+XG4gICAgICAgICAgICAgICAgICAgICAgICA8YnV0dG9uIHR5cGU9XCJidXR0b25cIiBjbGFzcz1cImJ0bi1jbG9zZVwiXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIGRhdGEtYnMtZGlzbWlzcz1cIm1vZGFsXCIgYXJpYS1sYWJlbD1cIkNsb3NlXCI+PC9idXR0b24+XG4gICAgICAgICAgICAgICAgICAgIDwvZGl2PlxuXG4gICAgICAgICAgICAgICAgICAgIDxkaXYgY2xhc3M9XCJtb2RhbC1ib2R5XCI+XG4gICAgICAgICAgICAgICAgICAgICAgICA8ZGl2IGNsYXNzPVwiY29udGFpbmVyLWZsdWlkXCI+XG5cbiAgICAgICAgICAgICAgICAgICAgICAgICAgICA8ZGl2IGNsYXNzPVwicm93IGltYWdlcy1yb3cgbWItNFwiPlxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICA8ZGl2IGNsYXNzPVwiY29sLTEyXCI+XG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICA8ZGl2IGNsYXNzPVwiZC1mbGV4IG92ZXJmbG93LXgtYXV0byBwYi0yIGdhcC0yIGhvcml6b250YWwtdGh1bWJuYWlsc1wiPlxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICR7cHJvZHVjdERldGFpbHMuaW1hZ2VzVXJscy5sZW5ndGggPiAwXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICA/IHByb2R1Y3REZXRhaWxzLmltYWdlc1VybHMubWFwKHVybCA9PiBodG1sYFxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIDxkaXYgY2xhc3M9XCJ0aHVtYm5haWwtd3JhcHBlclwiPlxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICA8aW1nIHNyYz1cIiR7dXJsfVwiIGNsYXNzPVwiaW1nLXRodW1ibmFpbFwiXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgc3R5bGU9XCJoZWlnaHQ6IDIwMHB4OyB3aWR0aDogYXV0bzsgb2JqZWN0LWZpdDogY292ZXI7XCJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICBhbHQ9XCJQcm9kdWN0IEltYWdlXCI+XG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgPC9kaXY+YClcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIDogaHRtbGBcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICA8cCBjbGFzcz1cInRleHQtbXV0ZWQgaXRhbGljXCI+XG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIE5vIGltYWdlcyBhdmFpbGFibGUgZm9yIHRoaXMgcHJvZHVjdC5cbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICA8L3A+YFxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIH1cbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIDwvZGl2PlxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICA8L2Rpdj5cbiAgICAgICAgICAgICAgICAgICAgICAgICAgICA8L2Rpdj5cblxuICAgICAgICAgICAgICAgICAgICAgICAgICAgIDxkaXYgY2xhc3M9XCJyb3cgcHJvZHVjdC1kZXRhaWxzLXJvd1wiPlxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICA8ZGl2IGNsYXNzPVwiY29sLW1kLTYgbWItM1wiPlxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgPGRpdiBjbGFzcz1cImNhcmQgaC0xMDAgYm9yZGVyLTAgc2hhZG93LXNtXCI+XG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgPGRpdiBjbGFzcz1cImNhcmQtYm9keVwiPlxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICA8aDUgY2xhc3M9XCJjYXJkLXRpdGxlIHRleHQtbmF2eSBib3JkZXItYm90dG9tIHBiLTIgbWItM1wiPlxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgUHJvZHVjdCBJbmZvcm1hdGlvblxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICA8L2g1PlxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICA8ZGl2IGNsYXNzPVwicm93IG1iLTJcIj5cbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIDxkaXYgY2xhc3M9XCJjb2wtNSBmdy1ib2xkIHRleHQtdGVhbFwiPk93bmVyOjwvZGl2PlxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgPGRpdiBjbGFzcz1cImNvbC03XCI+JHtwcm9kdWN0RGV0YWlscy5vd25lck5hbWV9PC9kaXY+XG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIDwvZGl2PlxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICA8ZGl2IGNsYXNzPVwicm93IG1iLTJcIj5cbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIDxkaXYgY2xhc3M9XCJjb2wtNSBmdy1ib2xkIHRleHQtdGVhbFwiPkNhdGVnb3J5OjwvZGl2PlxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgPGRpdiBjbGFzcz1cImNvbC03XCI+XG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgJHtwcm9kdWN0RGV0YWlscy5jYXRlZ29yeU5hbWV9XG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICA8L2Rpdj5cbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgPC9kaXY+XG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIDxkaXYgY2xhc3M9XCJyb3cgbWItMlwiPlxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgPGRpdiBjbGFzcz1cImNvbC01IGZ3LWJvbGQgdGV4dC10ZWFsXCI+XG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgUXVhbnRpdHkgaW4gU3RvY2s6XG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICA8L2Rpdj5cbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIDxkaXYgY2xhc3M9XCJjb2wtN1wiPlxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICR7cHJvZHVjdERldGFpbHMucXVhbnRpdHlJblN0b2NrfVxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgPC9kaXY+XG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIDwvZGl2PlxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICA8ZGl2IGNsYXNzPVwicm93XCI+XG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICA8ZGl2IGNsYXNzPVwiY29sLTUgZnctYm9sZCB0ZXh0LXRlYWxcIj5QcmljZTo8L2Rpdj5cbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIDxkaXYgY2xhc3M9XCJjb2wtNyBmcy01IGZ3LWJvbGQgdGV4dC1uYXZ5XCI+XG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICQke3Byb2R1Y3REZXRhaWxzLnNlbGxpbmdQcmljZS50b0ZpeGVkKDIpfVxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgPC9kaXY+XG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIDwvZGl2PlxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIDwvZGl2PlxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgPC9kaXY+XG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIDwvZGl2PlxuXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIDxkaXYgY2xhc3M9XCJjb2wtbWQtNiBtYi0zXCI+XG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICA8ZGl2IGNsYXNzPVwiY2FyZCBoLTEwMCBib3JkZXItMCBzaGFkb3ctc21cIj5cbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICA8ZGl2IGNsYXNzPVwiY2FyZC1ib2R5XCI+XG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIDxoNSBjbGFzcz1cImNhcmQtdGl0bGUgdGV4dC1uYXZ5IGJvcmRlci1ib3R0b20gcGItMiBtYi0zXCI+QXBwcm92YWwgU3RhdHVzPC9oNT5cbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgPGRpdiBjbGFzcz1cInJvdyBtYi0yXCI+XG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICA8ZGl2IGNsYXNzPVwiY29sLTUgZnctYm9sZCB0ZXh0LXRlYWxcIj5TdGF0dXM6PC9kaXY+XG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICA8ZGl2IGNsYXNzPVwiY29sLTdcIj5cbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICA8c3BhbiBjbGFzcz1cImJhZGdlICR7Z2V0U3RhdHVzQmFkZ2VDbGFzcyhwcm9kdWN0RGV0YWlscy5hcHByb3ZhbERlY2lzaW9uLmFwcHJvdmFsU3RhdHVzKX1cIj5cbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgJHtmb3JtYXRTdGF0dXMocHJvZHVjdERldGFpbHMuYXBwcm92YWxEZWNpc2lvbi5hcHByb3ZhbFN0YXR1cyl9XG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgPC9zcGFuPlxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgPC9kaXY+XG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIDwvZGl2PlxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICA8ZGl2IGNsYXNzPVwicm93IG1iLTJcIj5cbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIDxkaXYgY2xhc3M9XCJjb2wtNSBmdy1ib2xkIHRleHQtdGVhbFwiPlxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIERlY2lzaW9uIE1ha2VyOlxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgPC9kaXY+XG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICA8ZGl2IGNsYXNzPVwiY29sLTdcIj5cbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAke3Byb2R1Y3REZXRhaWxzLmFwcHJvdmFsRGVjaXNpb24uYXBwcm92YWxEZWNpc2lvbk1ha2VyVXNlcm5hbWUgfHwgXCJOL0FcIn1cbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIDwvZGl2PlxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICA8L2Rpdj5cbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgPGRpdiBjbGFzcz1cInJvd1wiPlxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgPGRpdiBjbGFzcz1cImNvbC01IGZ3LWJvbGQgdGV4dC10ZWFsXCI+VGltZTo8L2Rpdj5cbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIDxkaXYgY2xhc3M9XCJjb2wtN1wiPlxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICR7cHJvZHVjdERldGFpbHMuYXBwcm92YWxEZWNpc2lvbi50aW1lT2ZEZWNpc2lvblxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgPyBuZXcgRGF0ZShwcm9kdWN0RGV0YWlscy5hcHByb3ZhbERlY2lzaW9uLnRpbWVPZkRlY2lzaW9uKS50b0xvY2FsZVN0cmluZygpXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICA6IFwiTi9BXCJ9XG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICA8L2Rpdj5cbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgPC9kaXY+XG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgPC9kaXY+XG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICA8L2Rpdj5cbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgPC9kaXY+XG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgPC9kaXY+XG5cbiAgICAgICAgICAgICAgICAgICAgICAgICAgICA8ZGl2IGNsYXNzPVwicm93IGRlc2NyaXB0aW9uLXJvdyBtdC0yXCI+XG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIDxkaXYgY2xhc3M9XCJjb2wtMTJcIj5cbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIDxkaXYgY2xhc3M9XCJjYXJkIGJvcmRlci0wIHNoYWRvdy1zbVwiPlxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIDxkaXYgY2xhc3M9XCJjYXJkLWJvZHlcIj5cbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgPGg1IGNsYXNzPVwiY2FyZC10aXRsZSB0ZXh0LW5hdnkgYm9yZGVyLWJvdHRvbSBwYi0yIG1iLTNcIj5EZXNjcmlwdGlvbjwvaDU+XG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIDxwIGNsYXNzPVwiY2FyZC10ZXh0IHRleHQtbXV0ZWRcIiBzdHlsZT1cIndoaXRlLXNwYWNlOiBwcmUtbGluZTtcIj5cbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICR7cHJvZHVjdERldGFpbHMuZGVzY3JpcHRpb259XG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIDwvcD5cbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICA8L2Rpdj5cbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIDwvZGl2PlxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICA8L2Rpdj5cbiAgICAgICAgICAgICAgICAgICAgICAgICAgICA8L2Rpdj5cblxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICR7cHJvZHVjdERldGFpbHMub3duZXJOYW1lICE9IG51bGxcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgID8gaHRtbGBcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICA8ZGl2IGNsYXNzPVwicm93IG1hbmFnZS1hcHByb3ZhbC1yb3cgbXQtNFwiPlxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICA8ZGl2IGNsYXNzPVwiY29sLTEyXCI+XG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICA8Zm9ybSBAc3VibWl0PSR7YXN5bmMgKGV2ZW50OiBFdmVudCkgPT4gb25Nb2RpZnlBcHByb3ZhbChldmVudCwgcHJvZHVjdERldGFpbHMsIGNvbnRleHQpfT5cbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICA8ZGl2IGNsYXNzPVwiY2FyZCBib3JkZXItMCBzaGFkb3ctc20gYm9yZGVyLXRvcCBib3JkZXItNCBib3JkZXItdGVhbFwiPlxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICA8ZGl2IGNsYXNzPVwiY2FyZC1ib2R5XCI+XG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICA8aDUgY2xhc3M9XCJjYXJkLXRpdGxlIHRleHQtbmF2eSBtYi00XCI+TWFuYWdlIEFwcHJvdmFsIFN0YXR1czwvaDU+XG5cbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIDxkaXYgY2xhc3M9XCJtYi00IHNlbGVjdC1zdGF0dXMtc2VjdGlvblwiPlxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIDxsYWJlbCBjbGFzcz1cImZvcm0tbGFiZWwgZnctYm9sZCB0ZXh0LXRlYWxcIj5TZWxlY3QgU3RhdHVzPC9sYWJlbD5cbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICA8ZGl2IGNsYXNzPVwiYnRuLWdyb3VwIHctMTAwXCIgcm9sZT1cImdyb3VwXCIgYXJpYS1sYWJlbD1cIkFwcHJvdmFsIHN0YXR1cyBzZWxlY3Rpb25cIlxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgQGNoYW5nZT0ke29uU3RhdHVzQ2hhbmdlVmFsaWRhdGlvbn0+XG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIDxpbnB1dCB0eXBlPVwicmFkaW9cIiBjbGFzcz1cImJ0bi1jaGVja1wiIG5hbWU9XCJhcHByb3ZhbFN0YXR1cy0ke3Byb2R1Y3REZXRhaWxzLmlkfVwiIGlkPVwic3RhdHVzQXBwcm92ZWQtJHtwcm9kdWN0RGV0YWlscy5pZH1cIlxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgdmFsdWU9XCJBcHByb3ZlZFwiIC5jaGVja2VkPSR7cHJvZHVjdERldGFpbHMuYXBwcm92YWxEZWNpc2lvbi5hcHByb3ZhbFN0YXR1cyA9PT0gXCJBcHByb3ZlZFwifT5cbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgPGxhYmVsIGNsYXNzPVwiYnRuIGJ0bi1vdXRsaW5lLXN1Y2Nlc3NcIiBmb3I9XCJzdGF0dXNBcHByb3ZlZC0ke3Byb2R1Y3REZXRhaWxzLmlkfVwiPkFwcHJvdmVkPC9sYWJlbD5cblxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICA8aW5wdXQgdHlwZT1cInJhZGlvXCIgY2xhc3M9XCJidG4tY2hlY2tcIiBuYW1lPVwiYXBwcm92YWxTdGF0dXMtJHtwcm9kdWN0RGV0YWlscy5pZH1cIiBpZD1cInN0YXR1c0Rpc2FwcHJvdmVkLSR7cHJvZHVjdERldGFpbHMuaWR9XCJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIHZhbHVlPVwiRGlzYXBwcm92ZWRcIiAuY2hlY2tlZD0ke3Byb2R1Y3REZXRhaWxzLmFwcHJvdmFsRGVjaXNpb24uYXBwcm92YWxTdGF0dXMgPT09IFwiRGlzYXBwcm92ZWRcIn0+XG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIDxsYWJlbCBjbGFzcz1cImJ0biBidG4tb3V0bGluZS1kYW5nZXJcIiBmb3I9XCJzdGF0dXNEaXNhcHByb3ZlZC0ke3Byb2R1Y3REZXRhaWxzLmlkfVwiPkRpc2FwcHJvdmVkPC9sYWJlbD5cbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICA8L2Rpdj5cbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICA8ZGl2IGNsYXNzPVwidGV4dC1kYW5nZXIgYXBwcm92YWwtdmFsaWRhdGlvbi1zZWN0aW9uXCI+PC9kaXY+XG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICA8L2Rpdj5cblxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgPGRpdiBjbGFzcz1cIm1iLTQgZGVjaXNpb24tanVzdGlmaWNhdGlvbi1zZWN0aW9uXCI+XG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgPGxhYmVsIGZvcj1cImRlY2lzaW9uSnVzdGlmaWNhdGlvbi0ke3Byb2R1Y3REZXRhaWxzLmlkfVwiIGNsYXNzPVwiZm9ybS1sYWJlbCBmdy1ib2xkIHRleHQtdGVhbFwiPkRlY2lzaW9uIEp1c3RpZmljYXRpb248L2xhYmVsPlxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIDx0ZXh0YXJlYSBjbGFzcz1cImZvcm0tY29udHJvbFwiIGlkPVwiZGVjaXNpb25KdXN0aWZpY2F0aW9uLSR7cHJvZHVjdERldGFpbHMuaWR9XCJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgbmFtZT1cImRlY2lzaW9uLWp1c3RpZmljYXRpb24tJHtwcm9kdWN0RGV0YWlscy5pZH1cIiByb3dzPVwiM1wiXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIHBsYWNlaG9sZGVyPVwiUHJvdmlkZSBhIHJlYXNvbiBmb3IgdGhlIGRlY2lzaW9uLi4uXCJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgLnZhbHVlIT0ke3Byb2R1Y3REZXRhaWxzLmFwcHJvdmFsRGVjaXNpb24uZGVjaXNpb25KdXN0aWZpY2F0aW9ufVxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICBAY2hhbmdlPSR7YXBwcm92YWxEZWNpc2lvbkp1c3RpZmljYXRpb25WYWxpZGF0aW9ufT48L3RleHRhcmVhPlxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIDxkaXYgY2xhc3M9XCJ0ZXh0LWRhbmdlciBqdXN0aWZpY2F0aW9uLXZhbGlkYXRpb24tc2VjdGlvblwiPjwvZGl2PlxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgPC9kaXY+XG5cbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIDxkaXYgY2xhc3M9XCJkLWdyaWRcIj5cbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICA8YnV0dG9uIHR5cGU9XCJzdWJtaXRcIiBjbGFzcz1cImJ0biBidG4tdGVhbCBidG4tbGcgc2hhZG93LXNtXCIgaWQ9XCJzYXZlLWFwcHJvdmFsLWNoYW5nZXNcIj5cbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgU2F2ZSBBcHByb3ZhbCBDaGFuZ2VzXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgPC9idXR0b24+XG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICA8L2Rpdj5cbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgPC9kaXY+XG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgPC9kaXY+XG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICA8L2Zvcm0+XG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIDwvZGl2PlxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIDwvZGl2PmBcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIDogaHRtbGBgXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgfVxuICAgICAgICAgICAgICAgICAgICAgICAgPC9kaXY+XG4gICAgICAgICAgICAgICAgICAgIDwvZGl2PlxuXG4gICAgICAgICAgICAgICAgICAgIDxkaXYgY2xhc3M9XCJtb2RhbC1mb290ZXJcIj5cbiAgICAgICAgICAgICAgICAgICAgICAgIDxidXR0b24gdHlwZT1cImJ1dHRvblwiIGNsYXNzPVwiYnRuIGJ0bi1zZWNvbmRhcnlcIiBkYXRhLWJzLWRpc21pc3M9XCJtb2RhbFwiPlxuICAgICAgICAgICAgICAgICAgICAgICAgICAgIENsb3NlXG4gICAgICAgICAgICAgICAgICAgICAgICA8L2J1dHRvbj5cbiAgICAgICAgICAgICAgICAgICAgPC9kaXY+XG4gICAgICAgICAgICAgICAgPC9kaXY+XG4gICAgICAgICAgICA8L2Rpdj5cbiAgICAgICAgPC9kaXY+YDtcbn1cblxuZnVuY3Rpb24gZXJyVGVtcGxhdGUocHJvZHVjdERldGFpbHNNb2RhbElkOiBzdHJpbmcpOiBUZW1wbGF0ZVJlc3VsdCB7XG4gICAgcmV0dXJuIGh0bWxgXG4gICAgICAgIDxkaXYgY2xhc3M9XCJtb2RhbCBmYWRlXCIgaWQ9XCIke3Byb2R1Y3REZXRhaWxzTW9kYWxJZH1cIlxuICAgICAgICAgICAgIGRhdGEtYnMta2V5Ym9hcmQ9XCJ0cnVlXCIgdGFiaW5kZXg9XCItMVwiXG4gICAgICAgICAgICAgYXJpYS1sYWJlbGxlZGJ5PVwicHJvZHVjdERldGFpbHNNb2RhbElkXCIgYXJpYS1oaWRkZW49XCJ0cnVlXCI+XG4gICAgICAgICAgICA8ZGl2IGNsYXNzPVwibW9kYWwtZGlhbG9nIG1vZGFsLWRpYWxvZy1jZW50ZXJlZCBtb2RhbC1zbVwiPlxuICAgICAgICAgICAgICAgIDxkaXYgY2xhc3M9XCJtb2RhbC1jb250ZW50XCI+XG4gICAgICAgICAgICAgICAgICAgIDxkaXYgY2xhc3M9XCJtb2RhbC1oZWFkZXJcIj5cbiAgICAgICAgICAgICAgICAgICAgICAgIDxoMSBjbGFzcz1cIm1vZGFsLXRpdGxlIGZzLTUgdGV4dC1kYW5nZXJcIiBpZD1cImVycm9yLW1vZGFsXCI+RXJyb3I8L2gxPlxuICAgICAgICAgICAgICAgICAgICAgICAgPGJ1dHRvbiB0eXBlPVwiYnV0dG9uXCIgY2xhc3M9XCJidG4tY2xvc2VcIiBkYXRhLWJzLWRpc21pc3M9XCJtb2RhbFwiIGFyaWEtbGFiZWw9XCJDbG9zZVwiPjwvYnV0dG9uPlxuICAgICAgICAgICAgICAgICAgICA8L2Rpdj5cblxuICAgICAgICAgICAgICAgICAgICA8ZGl2IGNsYXNzPVwibW9kYWwtYm9keVwiPlxuICAgICAgICAgICAgICAgICAgICAgICAgT29wcy4uLiBTb21ldGhpbmcgd2VudCB3cm9uZyEgUGxlYXNlIHRyeSBhZ2FpbiBsYXRlci5cbiAgICAgICAgICAgICAgICAgICAgPC9kaXY+XG5cbiAgICAgICAgICAgICAgICAgICAgPGRpdiBjbGFzcz1cIm1vZGFsLWZvb3RlclwiPlxuICAgICAgICAgICAgICAgICAgICAgICAgPGJ1dHRvbiB0eXBlPVwiYnV0dG9uXCIgY2xhc3M9XCJidG4gYnRuLXNlY29uZGFyeVwiIGRhdGEtYnMtZGlzbWlzcz1cIm1vZGFsXCI+XG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgT2tcbiAgICAgICAgICAgICAgICAgICAgICAgIDwvYnV0dG9uPlxuICAgICAgICAgICAgICAgICAgICA8L2Rpdj5cbiAgICAgICAgICAgICAgICA8L2Rpdj5cbiAgICAgICAgICAgIDwvZGl2PlxuICAgICAgICA8L2Rpdj5gO1xufVxuXG5hc3luYyBmdW5jdGlvbiBvbk1vZGlmeUFwcHJvdmFsKFxuICAgIGV2ZW50OiBFdmVudCxcbiAgICBwcm9kdWN0RGV0YWlsczogUHJvZHVjdERldGFpbHMsXG4gICAgY29udGV4dDogVGFibGVDb250ZXh0XG4pOiBQcm9taXNlPHZvaWQ+IHtcbiAgICBldmVudC5wcmV2ZW50RGVmYXVsdCgpO1xuXG4gICAgY29uc3QgZm9ybURhdGEgPSBuZXcgRm9ybURhdGEoZXZlbnQuY3VycmVudFRhcmdldCBhcyBIVE1MRm9ybUVsZW1lbnQpO1xuICAgIGNvbnN0IG5ld0FwcHJvdmFsU3RhdHVzID0gZm9ybURhdGEuZ2V0KGBhcHByb3ZhbFN0YXR1cy0ke3Byb2R1Y3REZXRhaWxzLmlkfWApIGFzIHN0cmluZztcbiAgICBjb25zdCBkZWNpc2lvbkp1c3RpZmljYXRpb24gPSBmb3JtRGF0YS5nZXQoYGRlY2lzaW9uLWp1c3RpZmljYXRpb24tJHtwcm9kdWN0RGV0YWlscy5pZH1gKSBhcyBzdHJpbmc7XG4gICAgXG4gICAgaWYgKCFuZXdBcHByb3ZhbFN0YXR1cyB8fCAhZGVjaXNpb25KdXN0aWZpY2F0aW9uKSB7XG4gICAgICAgIHNob3dFcnJvclN3YWwoKVxuICAgICAgICAgICAgLnRoZW4oYXN5bmMgKCkgPT4gYXdhaXQgc2hvd1Byb2R1Y3RzVGFibGUoY29udGV4dCkpO1xuICAgIH1cblxuICAgIGxldCBpc1NhbWVEYXRhID0gZmFsc2U7XG4gICAgaWYobmV3QXBwcm92YWxTdGF0dXMgPT09IHByb2R1Y3REZXRhaWxzLmFwcHJvdmFsRGVjaXNpb24uYXBwcm92YWxTdGF0dXMpIHtcbiAgICAgICAgKGV2ZW50LmN1cnJlbnRUYXJnZXQgYXMgSFRNTEZvcm1FbGVtZW50KVxuICAgICAgICAgICAgLnF1ZXJ5U2VsZWN0b3I8SFRNTERpdkVsZW1lbnQ+KFwiZGl2LmFwcHJvdmFsLXZhbGlkYXRpb24tc2VjdGlvblwiKSFcbiAgICAgICAgICAgIC50ZXh0Q29udGVudCA9IFwiQXBwcm92YWwgc3RhdHVzIHdhcyBub3QgY2hhbmdlZC5cIjtcblxuICAgICAgICBpc1NhbWVEYXRhID0gdHJ1ZTtcbiAgICB9XG4gICAgaWYoZGVjaXNpb25KdXN0aWZpY2F0aW9uID09PSBwcm9kdWN0RGV0YWlscy5hcHByb3ZhbERlY2lzaW9uLmRlY2lzaW9uSnVzdGlmaWNhdGlvbikge1xuICAgICAgICAoZXZlbnQuY3VycmVudFRhcmdldCBhcyBIVE1MRGl2RWxlbWVudClcbiAgICAgICAgICAgIC5xdWVyeVNlbGVjdG9yKFwiZGl2Lmp1c3RpZmljYXRpb24tdmFsaWRhdGlvbi1zZWN0aW9uXCIpIVxuICAgICAgICAgICAgLnRleHRDb250ZW50ID0gXCJQbGVhc2UgcHJvdmlkZSBhIG5ldyByZWFzb24gZm9yIHRoZSBuZXcgYXBwcm92YWwgZGVjaXNpb24uXCI7XG5cbiAgICAgICAgaXNTYW1lRGF0YSA9IHRydWU7XG4gICAgfVxuXG4gICAgaWYoaXNTYW1lRGF0YSkge1xuICAgICAgICByZXR1cm47XG4gICAgfVxuXG4gICAgY29uc3QgbW9kYWxFbCA9IChldmVudC5jdXJyZW50VGFyZ2V0IGFzIEhUTUxGb3JtRWxlbWVudClcbiAgICAgICAgLmNsb3Nlc3QoYGRpdi5tb2RhbCNwcm9kdWN0LWRldGFpbHMtJHtwcm9kdWN0RGV0YWlscy5pZH1gKSE7XG4gICAgY29uc3QgbW9kYWwgPSBNb2RhbC5nZXRJbnN0YW5jZShtb2RhbEVsKSE7XG4gICAgbW9kYWwudG9nZ2xlKCk7XG5cbiAgICBjb25zdCBtb2RpZnlBcHByb3ZhbFJlc3VsdCA9IGF3YWl0IGNvbnRleHQubW9kaWZ5UHJvZHVjdEFwcHJvdmFsKHtcbiAgICAgICAgcHJvZHVjdElkOiBwcm9kdWN0RGV0YWlscy5pZCxcbiAgICAgICAgYXBwcm92YWxTdGF0dXM6IG5ld0FwcHJvdmFsU3RhdHVzLFxuICAgICAgICBkZWNpc2lvbkp1c3RpZmljYXRpb246IGRlY2lzaW9uSnVzdGlmaWNhdGlvbixcbiAgICB9KTtcbiAgICBpZighbW9kaWZ5QXBwcm92YWxSZXN1bHQpIHtcbiAgICAgICAgc2hvd0Vycm9yU3dhbCgpLnRoZW4oYXN5bmMgKCkgPT4gYXdhaXQgc2hvd1Byb2R1Y3RzVGFibGUoY29udGV4dCkpO1xuICAgIH0gZWxzZSB7XG4gICAgICAgIHNob3dQbGFpblN1Y2Nlc3NTd2FsKFwiVGhlIHByb2R1Y3QgYXBwcm92YWwgc3RhdHVzIGhhcyBiZWVuIGNoYW5nZWQgc3VjY2Vzc2Z1bGx5LlwiKVxuICAgICAgICAgICAgLnRoZW4oYXN5bmMgKCkgPT4gYXdhaXQgc2hvd1Byb2R1Y3RzVGFibGUoY29udGV4dCkpO1xuICAgIH1cbn1cblxuZnVuY3Rpb24gb25TdGF0dXNDaGFuZ2VWYWxpZGF0aW9uKGV2ZW50OiBFdmVudCkge1xuICAgIGNvbnN0IHZhbGlkYXRpb25TZWN0aW9uID0gKGV2ZW50LmN1cnJlbnRUYXJnZXQgYXMgSFRNTERpdkVsZW1lbnQpXG4gICAgICAgIC5jbG9zZXN0KFwiZGl2LnNlbGVjdC1zdGF0dXMtc2VjdGlvblwiKSFcbiAgICAgICAgLnF1ZXJ5U2VsZWN0b3IoXCJkaXYuYXBwcm92YWwtdmFsaWRhdGlvbi1zZWN0aW9uXCIpO1xuXG4gICAgaWYodmFsaWRhdGlvblNlY3Rpb24/LnRleHRDb250ZW50ICE9PSBcIlwiKSB7XG4gICAgICAgIHZhbGlkYXRpb25TZWN0aW9uIS50ZXh0Q29udGVudCA9IFwiXCI7XG4gICAgfVxufVxuXG5mdW5jdGlvbiBhcHByb3ZhbERlY2lzaW9uSnVzdGlmaWNhdGlvblZhbGlkYXRpb24oZXZlbnQ6IEV2ZW50KSB7XG4gICAgY29uc3Qgc3VibWl0QnV0dG9uID0gZG9jdW1lbnQucXVlcnlTZWxlY3RvcihcIiNzYXZlLWFwcHJvdmFsLWNoYW5nZXNcIik7XG4gICAgY29uc3QgdGV4dEFyZWEgPSBldmVudC5jdXJyZW50VGFyZ2V0IGFzIEhUTUxUZXh0QXJlYUVsZW1lbnQ7XG4gICAgY29uc3QgdmFsaWRhdGlvblNlY3Rpb24gPSB0ZXh0QXJlYVxuICAgICAgICAuY2xvc2VzdChcImRpdi5kZWNpc2lvbi1qdXN0aWZpY2F0aW9uLXNlY3Rpb25cIikhXG4gICAgICAgIC5xdWVyeVNlbGVjdG9yKFwiZGl2Lmp1c3RpZmljYXRpb24tdmFsaWRhdGlvbi1zZWN0aW9uXCIpITtcblxuICAgIGlmKHRleHRBcmVhLnZhbHVlLmxlbmd0aCA8IDQgfHwgdGV4dEFyZWEudmFsdWUubGVuZ3RoID4gMzAwMCkge1xuICAgICAgICB2YWxpZGF0aW9uU2VjdGlvbi50ZXh0Q29udGVudFxuICAgICAgICAgICAgPSBcIlRoZSBkZWNpc2lvbiByZWFzb24gc2hvdWxkIGJlIGJldHdlZW4gNCBhbmQgMzAwMCBjaGFyYWN0ZXJzIGxvbmcuXCI7XG5cbiAgICAgICAgc3VibWl0QnV0dG9uPy5zZXRBdHRyaWJ1dGUoXCJkaXNhYmxlZFwiLCBcImRpc2FibGVkXCIpO1xuICAgIH0gZWxzZSB7XG4gICAgICAgIHZhbGlkYXRpb25TZWN0aW9uLnRleHRDb250ZW50ID0gXCJcIjtcbiAgICAgICAgc3VibWl0QnV0dG9uPy5yZW1vdmVBdHRyaWJ1dGUoXCJkaXNhYmxlZFwiKTtcbiAgICB9XG59XG5cbmZ1bmN0aW9uIGdldFN0YXR1c0JhZGdlQ2xhc3Moc3RhdHVzOiBQcm9kdWN0c0FwcHJvdmFsU3RhdHVzKSB7XG4gICAgc3dpdGNoIChzdGF0dXMpIHtcbiAgICAgICAgY2FzZSBcIkFwcHJvdmVkXCI6IHJldHVybiBcImJnLXN1Y2Nlc3NcIjtcbiAgICAgICAgY2FzZSBcIkRpc2FwcHJvdmVkXCI6IHJldHVybiBcImJnLWRhbmdlclwiO1xuICAgICAgICBjYXNlIFwiV2FpdGluZ0FwcHJvdmFsXCI6IHJldHVybiBcImJnLXdhcm5pbmcgdGV4dC1kYXJrXCI7XG4gICAgICAgIGRlZmF1bHQ6IHJldHVybiBcImJnLXNlY29uZGFyeVwiO1xuICAgIH1cbn1cblxuZnVuY3Rpb24gZm9ybWF0U3RhdHVzKHN0YXR1czogUHJvZHVjdHNBcHByb3ZhbFN0YXR1cykge1xuICAgIHN3aXRjaCAoc3RhdHVzKSB7XG4gICAgICAgIGNhc2UgXCJBcHByb3ZlZFwiOiByZXR1cm4gXCJBcHByb3ZlZFwiO1xuICAgICAgICBjYXNlIFwiRGlzYXBwcm92ZWRcIjogcmV0dXJuIFwiRGlzYXBwcm92ZWRcIjtcbiAgICAgICAgY2FzZSBcIldhaXRpbmdBcHByb3ZhbFwiOiByZXR1cm4gXCJXYWl0aW5nIEFwcHJvdmFsXCI7XG4gICAgICAgIGRlZmF1bHQ6IHJldHVybiBzdGF0dXM7XG4gICAgfVxufVxuIl19