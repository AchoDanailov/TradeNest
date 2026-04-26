import bootstrap from "bootstrap";
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
//# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJmaWxlIjoiY29uZmlybVJlbW92ZVByb2R1Y3REaWFsb2dNb2RhbC5qcyIsInNvdXJjZVJvb3QiOiIiLCJzb3VyY2VzIjpbIi4uLy4uLy4uL0Zyb250RW5kU2NyaXB0cy9wcm9kdWN0cy1tYW5hZ2VtZW50L2NvbmZpcm1SZW1vdmVQcm9kdWN0RGlhbG9nTW9kYWwudHMiXSwibmFtZXMiOltdLCJtYXBwaW5ncyI6IkFBQUEsT0FBTyxTQUFTLE1BQU0sV0FBVyxDQUFDO0FBQ2xDLE9BQU8sRUFBRSxJQUFJLEVBQUUsTUFBTSxVQUFVLENBQUM7QUFJaEMsT0FBTyxFQUFFLGFBQWEsRUFBRSxvQkFBb0IsRUFBRSxNQUFNLHNCQUFzQixDQUFDO0FBQzNFLE9BQU8saUJBQWlCLE1BQU0sb0JBQW9CLENBQUM7QUFFbkQsTUFBTSxDQUFDLE9BQU8sVUFBVSxxQkFBcUIsQ0FBQyxPQUFnQixFQUFFLE9BQXFCO0lBQ2pGLE1BQU0sb0JBQW9CLEdBQUcsa0JBQWtCLE9BQU8sQ0FBQyxFQUFFLEVBQUUsQ0FBQztJQUU1RCxPQUFPLElBQUksQ0FBQTtzQ0FDdUIsb0JBQW9COztnQ0FFMUIsb0JBQW9COzs7Ozs7Ozs7Ozs7OzswREFjTSxPQUFPLENBQUMsSUFBSTs7Ozs7O3dDQU05QixLQUFLLEVBQUUsS0FBWSxFQUFFLEVBQUUsQ0FBQyxNQUFNLHNCQUFzQixDQUFDLEtBQUssRUFBRSxPQUFPLEVBQUUsT0FBTyxDQUFDOzs7Ozs7O0tBT2hILENBQUM7QUFDTixDQUFDO0FBRUQsS0FBSyxVQUFVLHNCQUFzQixDQUFDLEtBQVksRUFBRSxPQUFnQixFQUFFLE9BQXFCO0lBQ3ZGLEtBQUssQ0FBQyxjQUFjLEVBQUUsQ0FBQztJQUV2QixNQUFNLG9CQUFvQixHQUFHLGtCQUFrQixPQUFPLENBQUMsRUFBRSxFQUFFLENBQUM7SUFDNUQsTUFBTSxPQUFPLEdBQUksS0FBSyxDQUFDLGFBQWlDO1NBQ25ELE9BQU8sQ0FBQyxPQUFPLG9CQUFvQixFQUFFLENBQUUsQ0FBQztJQUM3QyxNQUFNLEtBQUssR0FBRyxTQUFTLENBQUMsS0FBSyxDQUFDLFdBQVcsQ0FBQyxPQUFRLENBQUMsQ0FBQztJQUNwRCxLQUFLLEVBQUUsTUFBTSxFQUFFLENBQUM7SUFFaEIsTUFBTSxtQkFBbUIsR0FBRyxNQUFNLE9BQU8sQ0FBQyxhQUFhLENBQUMsT0FBTyxDQUFDLEVBQUUsQ0FBQyxDQUFDO0lBQ3BFLElBQUcsQ0FBQyxtQkFBbUIsRUFBRSxDQUFDO1FBQ3RCLGFBQWEsRUFBRTthQUNWLElBQUksQ0FBQyxLQUFLLElBQUksRUFBRSxDQUFDLE1BQU0saUJBQWlCLENBQUMsT0FBTyxDQUFDLENBQUMsQ0FBQztJQUM1RCxDQUFDO1NBQU0sQ0FBQztRQUNKLG9CQUFvQixDQUFDLCtCQUErQixDQUFDO2FBQ2hELElBQUksQ0FBQyxLQUFLLElBQUksRUFBRSxDQUFDLE1BQU0saUJBQWlCLENBQUMsT0FBTyxDQUFDLENBQUMsQ0FBQztJQUM1RCxDQUFDO0FBQ0wsQ0FBQyIsInNvdXJjZXNDb250ZW50IjpbImltcG9ydCBib290c3RyYXAgZnJvbSBcImJvb3RzdHJhcFwiO1xuaW1wb3J0IHsgaHRtbCB9IGZyb20gXCJsaXQtaHRtbFwiO1xuXG5pbXBvcnQgdHlwZSB7IFByb2R1Y3QgfSBmcm9tIFwiLi4vdHlwZXMvcHJvZHVjdHMudHNcIjtcbmltcG9ydCB0eXBlIHsgVGFibGVDb250ZXh0IH0gZnJvbSBcIi4uL3R5cGVzL3RhYmxlQ29udGV4dC50c1wiO1xuaW1wb3J0IHsgc2hvd0Vycm9yU3dhbCwgc2hvd1BsYWluU3VjY2Vzc1N3YWwgfSBmcm9tIFwiLi4vdXRpbHMvZG9tVXRpbHMuanNcIjtcbmltcG9ydCBzaG93UHJvZHVjdHNUYWJsZSBmcm9tIFwiLi9wcm9kdWN0c1RhYmxlLmpzXCI7XG5cbmV4cG9ydCBkZWZhdWx0IGZ1bmN0aW9uIHJlbW92ZVByb2R1Y3RUZW1wbGF0ZShwcm9kdWN0OiBQcm9kdWN0LCBjb250ZXh0OiBUYWJsZUNvbnRleHQpIHtcbiAgICBjb25zdCBkZWxldGVQcm9kdWN0TW9kYWxJZCA9IGByZW1vdmUtcHJvZHVjdC0ke3Byb2R1Y3QuaWR9YDtcblxuICAgIHJldHVybiBodG1sYFxuICAgICAgICA8ZGl2IGNsYXNzPVwibW9kYWwgZmFkZVwiIGlkPVwiJHtkZWxldGVQcm9kdWN0TW9kYWxJZH1cIlxuICAgICAgICAgICAgIGRhdGEtYnMta2V5Ym9hcmQ9XCJ0cnVlXCIgdGFiaW5kZXg9XCItMVwiIGRhdGEtYnMtYmFja2Ryb3A9XCJzdGF0aWNcIlxuICAgICAgICAgICAgIGFyaWEtbGFiZWxsZWRieT1cIiR7ZGVsZXRlUHJvZHVjdE1vZGFsSWR9LWRpYWxvZ1wiXG4gICAgICAgICAgICAgYXJpYS1oaWRkZW49XCJ0cnVlXCI+XG4gICAgICAgICAgICA8ZGl2IGNsYXNzPVwibW9kYWwtZGlhbG9nXCI+XG4gICAgICAgICAgICAgICAgPGRpdiBjbGFzcz1cIm1vZGFsLWNvbnRlbnRcIj5cbiAgICAgICAgICAgICAgICAgICAgPGRpdiBjbGFzcz1cIm1vZGFsLWhlYWRlclwiPlxuICAgICAgICAgICAgICAgICAgICAgICAgPGgxIGNsYXNzPVwibW9kYWwtdGl0bGUgZnMtNVwiIGlkPVwic3RhdGljQmFja2Ryb3BMYWJlbFwiPlxuICAgICAgICAgICAgICAgICAgICAgICAgICAgIENvbmZpcm0gRGVsZXRpb25cbiAgICAgICAgICAgICAgICAgICAgICAgIDwvaDE+XG4gICAgICAgICAgICAgICAgICAgICAgICA8YnV0dG9uIHR5cGU9XCJidXR0b25cIiBjbGFzcz1cImJ0bi1jbG9zZVwiXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIGRhdGEtYnMtZGlzbWlzcz1cIm1vZGFsXCJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgYXJpYS1sYWJlbD1cIkNsb3NlXCI+PC9idXR0b24+XG4gICAgICAgICAgICAgICAgICAgIDwvZGl2PlxuXG4gICAgICAgICAgICAgICAgICAgIDxkaXYgY2xhc3M9XCJtb2RhbC1ib2R5XCI+XG4gICAgICAgICAgICAgICAgICAgICAgICBBcmUgeW91IHN1cmUgeW91IHdhbnQgdG8gcmVtb3ZlICR7cHJvZHVjdC5uYW1lfVxuICAgICAgICAgICAgICAgICAgICA8L2Rpdj5cbiAgICAgICAgICAgICAgICAgICAgPGRpdiBjbGFzcz1cIm1vZGFsLWZvb3RlclwiPlxuICAgICAgICAgICAgICAgICAgICAgICAgPGJ1dHRvbiB0eXBlPVwiYnV0dG9uXCIgY2xhc3M9XCJidG4gYnRuLXNlY29uZGFyeVwiXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIGRhdGEtYnMtZGlzbWlzcz1cIm1vZGFsXCI+Q2FuY2VsPC9idXR0b24+XG5cbiAgICAgICAgICAgICAgICAgICAgICAgIDxmb3JtIEBzdWJtaXQ9JHthc3luYyAoZXZlbnQ6IEV2ZW50KSA9PiBhd2FpdCBvbkNvbmZpcm1SZW1vdmVQcm9kdWN0KGV2ZW50LCBwcm9kdWN0LCBjb250ZXh0KX0+XG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgPGJ1dHRvbiB0eXBlPVwic3VibWl0XCIgY2xhc3M9XCJidG4gYnRuLWRhbmdlclwiPiBZZXMgPC9idXR0b24+XG4gICAgICAgICAgICAgICAgICAgICAgICA8L2Zvcm0+XG4gICAgICAgICAgICAgICAgICAgIDwvZGl2PlxuICAgICAgICAgICAgICAgIDwvZGl2PlxuICAgICAgICAgICAgPC9kaXY+XG4gICAgICAgIDwvZGl2PlxuICAgIGA7XG59XG5cbmFzeW5jIGZ1bmN0aW9uIG9uQ29uZmlybVJlbW92ZVByb2R1Y3QoZXZlbnQ6IEV2ZW50LCBwcm9kdWN0OiBQcm9kdWN0LCBjb250ZXh0OiBUYWJsZUNvbnRleHQpIHtcbiAgICBldmVudC5wcmV2ZW50RGVmYXVsdCgpO1xuXG4gICAgY29uc3QgZGVsZXRlUHJvZHVjdE1vZGFsSWQgPSBgcmVtb3ZlLXByb2R1Y3QtJHtwcm9kdWN0LmlkfWA7XG4gICAgY29uc3QgbW9kYWxFbCA9IChldmVudC5jdXJyZW50VGFyZ2V0IGFzIEhUTUxEaXZFbGVtZW50KSFcbiAgICAgICAgLmNsb3Nlc3QoYGRpdiMke2RlbGV0ZVByb2R1Y3RNb2RhbElkfWApITtcbiAgICBjb25zdCBtb2RhbCA9IGJvb3RzdHJhcC5Nb2RhbC5nZXRJbnN0YW5jZShtb2RhbEVsISk7XG4gICAgbW9kYWw/LnRvZ2dsZSgpO1xuXG4gICAgY29uc3QgcmVtb3ZlUHJvZHVjdFJlc3VsdCA9IGF3YWl0IGNvbnRleHQucmVtb3ZlUHJvZHVjdChwcm9kdWN0LmlkKTtcbiAgICBpZighcmVtb3ZlUHJvZHVjdFJlc3VsdCkge1xuICAgICAgICBzaG93RXJyb3JTd2FsKClcbiAgICAgICAgICAgIC50aGVuKGFzeW5jICgpID0+IGF3YWl0IHNob3dQcm9kdWN0c1RhYmxlKGNvbnRleHQpKTtcbiAgICB9IGVsc2Uge1xuICAgICAgICBzaG93UGxhaW5TdWNjZXNzU3dhbChcIlByb2R1Y3QgcmVtb3ZlZCBzdWNjZXNzZnVsbHkhXCIpXG4gICAgICAgICAgICAudGhlbihhc3luYyAoKSA9PiBhd2FpdCBzaG93UHJvZHVjdHNUYWJsZShjb250ZXh0KSk7XG4gICAgfVxufSJdfQ==