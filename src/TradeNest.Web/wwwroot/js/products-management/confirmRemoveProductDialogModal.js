import * as bootstrap from "bootstrap";
import { html } from "lit-html";
import { showErrorSwal, showPlainSuccessSwal } from "../utils/domUtils.js";
import showProductsTable from "./productsTable.js";
export default function removeProductTemplate(product, context) {
    const deleteProductModalId = `remove-product-${product.id}`;
    return html `
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
    const modalEl = event.currentTarget
        .closest(`div#${deleteProductModalId}`);
    const modal = bootstrap.Modal.getInstance(modalEl);
    modal?.toggle();
    const removeProductResult = await context.removeProduct(product.id);
    if (!removeProductResult) {
        showErrorSwal()
            .then(async () => await showProductsTable(context));
    }
    else {
        showPlainSuccessSwal("Product removed successfully!")
            .then(async () => await showProductsTable(context));
    }
}
//# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJmaWxlIjoiY29uZmlybVJlbW92ZVByb2R1Y3REaWFsb2dNb2RhbC5qcyIsInNvdXJjZVJvb3QiOiIiLCJzb3VyY2VzIjpbIi4uLy4uLy4uL0Zyb250RW5kU2NyaXB0cy9wcm9kdWN0cy1tYW5hZ2VtZW50L2NvbmZpcm1SZW1vdmVQcm9kdWN0RGlhbG9nTW9kYWwudHMiXSwibmFtZXMiOltdLCJtYXBwaW5ncyI6IkFBQUEsT0FBTyxLQUFLLFNBQVMsTUFBTSxXQUFXLENBQUM7QUFDdkMsT0FBTyxFQUFFLElBQUksRUFBRSxNQUFNLFVBQVUsQ0FBQztBQUloQyxPQUFPLEVBQUUsYUFBYSxFQUFFLG9CQUFvQixFQUFFLE1BQU0sc0JBQXNCLENBQUM7QUFDM0UsT0FBTyxpQkFBaUIsTUFBTSxvQkFBb0IsQ0FBQztBQUVuRCxNQUFNLENBQUMsT0FBTyxVQUFVLHFCQUFxQixDQUFDLE9BQWdCLEVBQUUsT0FBcUI7SUFDakYsTUFBTSxvQkFBb0IsR0FBRyxrQkFBa0IsT0FBTyxDQUFDLEVBQUUsRUFBRSxDQUFDO0lBRTVELE9BQU8sSUFBSSxDQUFBO3NDQUN1QixvQkFBb0I7O2dDQUUxQixvQkFBb0I7Ozs7Ozs7Ozs7Ozs7OzBEQWNNLE9BQU8sQ0FBQyxJQUFJOzs7Ozs7d0NBTTlCLEtBQUssRUFBRSxLQUFZLEVBQUUsRUFBRSxDQUFDLE1BQU0sc0JBQXNCLENBQUMsS0FBSyxFQUFFLE9BQU8sRUFBRSxPQUFPLENBQUM7Ozs7Ozs7S0FPaEgsQ0FBQztBQUNOLENBQUM7QUFFRCxLQUFLLFVBQVUsc0JBQXNCLENBQUMsS0FBWSxFQUFFLE9BQWdCLEVBQUUsT0FBcUI7SUFDdkYsS0FBSyxDQUFDLGNBQWMsRUFBRSxDQUFDO0lBRXZCLE1BQU0sb0JBQW9CLEdBQUcsa0JBQWtCLE9BQU8sQ0FBQyxFQUFFLEVBQUUsQ0FBQztJQUM1RCxNQUFNLE9BQU8sR0FBSSxLQUFLLENBQUMsYUFBaUM7U0FDbkQsT0FBTyxDQUFDLE9BQU8sb0JBQW9CLEVBQUUsQ0FBRSxDQUFDO0lBQzdDLE1BQU0sS0FBSyxHQUFHLFNBQVMsQ0FBQyxLQUFLLENBQUMsV0FBVyxDQUFDLE9BQVEsQ0FBQyxDQUFDO0lBQ3BELEtBQUssRUFBRSxNQUFNLEVBQUUsQ0FBQztJQUVoQixNQUFNLG1CQUFtQixHQUFHLE1BQU0sT0FBTyxDQUFDLGFBQWEsQ0FBQyxPQUFPLENBQUMsRUFBRSxDQUFDLENBQUM7SUFDcEUsSUFBRyxDQUFDLG1CQUFtQixFQUFFLENBQUM7UUFDdEIsYUFBYSxFQUFFO2FBQ1YsSUFBSSxDQUFDLEtBQUssSUFBSSxFQUFFLENBQUMsTUFBTSxpQkFBaUIsQ0FBQyxPQUFPLENBQUMsQ0FBQyxDQUFDO0lBQzVELENBQUM7U0FBTSxDQUFDO1FBQ0osb0JBQW9CLENBQUMsK0JBQStCLENBQUM7YUFDaEQsSUFBSSxDQUFDLEtBQUssSUFBSSxFQUFFLENBQUMsTUFBTSxpQkFBaUIsQ0FBQyxPQUFPLENBQUMsQ0FBQyxDQUFDO0lBQzVELENBQUM7QUFDTCxDQUFDIiwic291cmNlc0NvbnRlbnQiOlsiaW1wb3J0ICogYXMgYm9vdHN0cmFwIGZyb20gXCJib290c3RyYXBcIjtcbmltcG9ydCB7IGh0bWwgfSBmcm9tIFwibGl0LWh0bWxcIjtcblxuaW1wb3J0IHR5cGUgeyBQcm9kdWN0IH0gZnJvbSBcIi4uL3R5cGVzL3Byb2R1Y3RzLnRzXCI7XG5pbXBvcnQgdHlwZSB7IFRhYmxlQ29udGV4dCB9IGZyb20gXCIuLi90eXBlcy90YWJsZUNvbnRleHQudHNcIjtcbmltcG9ydCB7IHNob3dFcnJvclN3YWwsIHNob3dQbGFpblN1Y2Nlc3NTd2FsIH0gZnJvbSBcIi4uL3V0aWxzL2RvbVV0aWxzLmpzXCI7XG5pbXBvcnQgc2hvd1Byb2R1Y3RzVGFibGUgZnJvbSBcIi4vcHJvZHVjdHNUYWJsZS5qc1wiO1xuXG5leHBvcnQgZGVmYXVsdCBmdW5jdGlvbiByZW1vdmVQcm9kdWN0VGVtcGxhdGUocHJvZHVjdDogUHJvZHVjdCwgY29udGV4dDogVGFibGVDb250ZXh0KSB7XG4gICAgY29uc3QgZGVsZXRlUHJvZHVjdE1vZGFsSWQgPSBgcmVtb3ZlLXByb2R1Y3QtJHtwcm9kdWN0LmlkfWA7XG5cbiAgICByZXR1cm4gaHRtbGBcbiAgICAgICAgPGRpdiBjbGFzcz1cIm1vZGFsIGZhZGVcIiBpZD1cIiR7ZGVsZXRlUHJvZHVjdE1vZGFsSWR9XCJcbiAgICAgICAgICAgICBkYXRhLWJzLWtleWJvYXJkPVwidHJ1ZVwiIHRhYmluZGV4PVwiLTFcIiBkYXRhLWJzLWJhY2tkcm9wPVwic3RhdGljXCJcbiAgICAgICAgICAgICBhcmlhLWxhYmVsbGVkYnk9XCIke2RlbGV0ZVByb2R1Y3RNb2RhbElkfS1kaWFsb2dcIlxuICAgICAgICAgICAgIGFyaWEtaGlkZGVuPVwidHJ1ZVwiPlxuICAgICAgICAgICAgPGRpdiBjbGFzcz1cIm1vZGFsLWRpYWxvZ1wiPlxuICAgICAgICAgICAgICAgIDxkaXYgY2xhc3M9XCJtb2RhbC1jb250ZW50XCI+XG4gICAgICAgICAgICAgICAgICAgIDxkaXYgY2xhc3M9XCJtb2RhbC1oZWFkZXJcIj5cbiAgICAgICAgICAgICAgICAgICAgICAgIDxoMSBjbGFzcz1cIm1vZGFsLXRpdGxlIGZzLTVcIiBpZD1cInN0YXRpY0JhY2tkcm9wTGFiZWxcIj5cbiAgICAgICAgICAgICAgICAgICAgICAgICAgICBDb25maXJtIERlbGV0aW9uXG4gICAgICAgICAgICAgICAgICAgICAgICA8L2gxPlxuICAgICAgICAgICAgICAgICAgICAgICAgPGJ1dHRvbiB0eXBlPVwiYnV0dG9uXCIgY2xhc3M9XCJidG4tY2xvc2VcIlxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICBkYXRhLWJzLWRpc21pc3M9XCJtb2RhbFwiXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIGFyaWEtbGFiZWw9XCJDbG9zZVwiPjwvYnV0dG9uPlxuICAgICAgICAgICAgICAgICAgICA8L2Rpdj5cblxuICAgICAgICAgICAgICAgICAgICA8ZGl2IGNsYXNzPVwibW9kYWwtYm9keVwiPlxuICAgICAgICAgICAgICAgICAgICAgICAgQXJlIHlvdSBzdXJlIHlvdSB3YW50IHRvIHJlbW92ZSAke3Byb2R1Y3QubmFtZX1cbiAgICAgICAgICAgICAgICAgICAgPC9kaXY+XG4gICAgICAgICAgICAgICAgICAgIDxkaXYgY2xhc3M9XCJtb2RhbC1mb290ZXJcIj5cbiAgICAgICAgICAgICAgICAgICAgICAgIDxidXR0b24gdHlwZT1cImJ1dHRvblwiIGNsYXNzPVwiYnRuIGJ0bi1zZWNvbmRhcnlcIlxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICBkYXRhLWJzLWRpc21pc3M9XCJtb2RhbFwiPkNhbmNlbDwvYnV0dG9uPlxuXG4gICAgICAgICAgICAgICAgICAgICAgICA8Zm9ybSBAc3VibWl0PSR7YXN5bmMgKGV2ZW50OiBFdmVudCkgPT4gYXdhaXQgb25Db25maXJtUmVtb3ZlUHJvZHVjdChldmVudCwgcHJvZHVjdCwgY29udGV4dCl9PlxuICAgICAgICAgICAgICAgICAgICAgICAgICAgIDxidXR0b24gdHlwZT1cInN1Ym1pdFwiIGNsYXNzPVwiYnRuIGJ0bi1kYW5nZXJcIj4gWWVzIDwvYnV0dG9uPlxuICAgICAgICAgICAgICAgICAgICAgICAgPC9mb3JtPlxuICAgICAgICAgICAgICAgICAgICA8L2Rpdj5cbiAgICAgICAgICAgICAgICA8L2Rpdj5cbiAgICAgICAgICAgIDwvZGl2PlxuICAgICAgICA8L2Rpdj5cbiAgICBgO1xufVxuXG5hc3luYyBmdW5jdGlvbiBvbkNvbmZpcm1SZW1vdmVQcm9kdWN0KGV2ZW50OiBFdmVudCwgcHJvZHVjdDogUHJvZHVjdCwgY29udGV4dDogVGFibGVDb250ZXh0KSB7XG4gICAgZXZlbnQucHJldmVudERlZmF1bHQoKTtcblxuICAgIGNvbnN0IGRlbGV0ZVByb2R1Y3RNb2RhbElkID0gYHJlbW92ZS1wcm9kdWN0LSR7cHJvZHVjdC5pZH1gO1xuICAgIGNvbnN0IG1vZGFsRWwgPSAoZXZlbnQuY3VycmVudFRhcmdldCBhcyBIVE1MRGl2RWxlbWVudCkhXG4gICAgICAgIC5jbG9zZXN0KGBkaXYjJHtkZWxldGVQcm9kdWN0TW9kYWxJZH1gKSE7XG4gICAgY29uc3QgbW9kYWwgPSBib290c3RyYXAuTW9kYWwuZ2V0SW5zdGFuY2UobW9kYWxFbCEpO1xuICAgIG1vZGFsPy50b2dnbGUoKTtcblxuICAgIGNvbnN0IHJlbW92ZVByb2R1Y3RSZXN1bHQgPSBhd2FpdCBjb250ZXh0LnJlbW92ZVByb2R1Y3QocHJvZHVjdC5pZCk7XG4gICAgaWYoIXJlbW92ZVByb2R1Y3RSZXN1bHQpIHtcbiAgICAgICAgc2hvd0Vycm9yU3dhbCgpXG4gICAgICAgICAgICAudGhlbihhc3luYyAoKSA9PiBhd2FpdCBzaG93UHJvZHVjdHNUYWJsZShjb250ZXh0KSk7XG4gICAgfSBlbHNlIHtcbiAgICAgICAgc2hvd1BsYWluU3VjY2Vzc1N3YWwoXCJQcm9kdWN0IHJlbW92ZWQgc3VjY2Vzc2Z1bGx5IVwiKVxuICAgICAgICAgICAgLnRoZW4oYXN5bmMgKCkgPT4gYXdhaXQgc2hvd1Byb2R1Y3RzVGFibGUoY29udGV4dCkpO1xuICAgIH1cbn0iXX0=