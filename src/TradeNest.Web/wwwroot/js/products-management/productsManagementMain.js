import { html, render } from "lit-html";
import { toggleHighlight } from "../utils/domUtils.js";
import showProductsTable from "./productsTable.js";
import getNewContextInstance from "./tableContext.js";
const START_PAGE_NUMBER = 1;
const PRODUCTS_PER_PAGE_COUNT = 5;
const controlsDivEl = document.querySelector("div#controls");
document.addEventListener("DOMContentLoaded", async () => {
    render(mainTemplate(), controlsDivEl);
    await showProducts("Approved");
});
function mainTemplate() {
    return html `
        <div class="btn-group btn-group-lg w-100" role="group"
             aria-label="Table data control buttons.">
            <input type="radio" class="btn-check" name="btnradio" id="btnradio1"
                   @change=${async () => await showProducts("Approved")}
                   autocomplete="off">
            <label class="btn btn-teal d-flex justify-content-center align-items-center"
                   style="width: 297px;" for="btnradio1">Approved</label>

            <input type="radio" class="btn-check" name="btnradio" id="btnradio2"
                   @change=${async () => await showProducts("Not Approved")}
                   autocomplete="off">
            <label class="btn btn-outline-teal d-flex justify-content-center align-items-center"
                   style="width: 297px;" for="btnradio2">Not Approved</label>
        </div>
    `;
}
async function showProducts(approvalStatus) {
    toggleHighlight(approvalStatus);
    const newContext = getNewContextInstance({
        productsApprovalStatus: approvalStatus,
        startPageNumber: START_PAGE_NUMBER,
        productsPerPageCount: PRODUCTS_PER_PAGE_COUNT,
    });
    await showProductsTable(newContext);
}
//# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJmaWxlIjoicHJvZHVjdHNNYW5hZ2VtZW50TWFpbi5qcyIsInNvdXJjZVJvb3QiOiIiLCJzb3VyY2VzIjpbIi4uLy4uLy4uL0Zyb250RW5kU2NyaXB0cy9wcm9kdWN0cy1tYW5hZ2VtZW50L3Byb2R1Y3RzTWFuYWdlbWVudE1haW4udHMiXSwibmFtZXMiOltdLCJtYXBwaW5ncyI6IkFBQUEsT0FBTyxFQUFFLElBQUksRUFBRSxNQUFNLEVBQXVCLE1BQU0sVUFBVSxDQUFDO0FBSTdELE9BQU8sRUFBRSxlQUFlLEVBQUUsTUFBTSxzQkFBc0IsQ0FBQztBQUN2RCxPQUFPLGlCQUFpQixNQUFNLG9CQUFvQixDQUFDO0FBQ25ELE9BQU8scUJBQXFCLE1BQU0sbUJBQW1CLENBQUM7QUFHdEQsTUFBTSxpQkFBaUIsR0FBRyxDQUFDLENBQUM7QUFDNUIsTUFBTSx1QkFBdUIsR0FBRyxDQUFDLENBQUM7QUFFbEMsTUFBTSxhQUFhLEdBQUcsUUFBUSxDQUFDLGFBQWEsQ0FBaUIsY0FBYyxDQUFFLENBQUM7QUFFOUUsUUFBUSxDQUFDLGdCQUFnQixDQUFDLGtCQUFrQixFQUFFLEtBQUssSUFBSSxFQUFFO0lBQ3JELE1BQU0sQ0FBQyxZQUFZLEVBQUUsRUFBRSxhQUFhLENBQUMsQ0FBQztJQUN0QyxNQUFNLFlBQVksQ0FBQyxVQUFVLENBQUMsQ0FBQztBQUNuQyxDQUFDLENBQUMsQ0FBQztBQUVILFNBQVMsWUFBWTtJQUNqQixPQUFPLElBQUksQ0FBQTs7Ozs2QkFJYyxLQUFLLElBQUksRUFBRSxDQUFDLE1BQU0sWUFBWSxDQUFDLFVBQVUsQ0FBQzs7Ozs7OzZCQU0xQyxLQUFLLElBQUksRUFBRSxDQUFDLE1BQU0sWUFBWSxDQUFDLGNBQWMsQ0FBQzs7Ozs7S0FLdEUsQ0FBQztBQUNOLENBQUM7QUFFRCxLQUFLLFVBQVUsWUFBWSxDQUFDLGNBQXNDO0lBQzlELGVBQWUsQ0FBQyxjQUFjLENBQUMsQ0FBQztJQUVoQyxNQUFNLFVBQVUsR0FBRyxxQkFBcUIsQ0FBQztRQUNyQyxzQkFBc0IsRUFBRSxjQUFjO1FBQ3RDLGVBQWUsRUFBRSxpQkFBaUI7UUFDbEMsb0JBQW9CLEVBQUUsdUJBQXVCO0tBQ2pDLENBQUMsQ0FBQztJQUVsQixNQUFNLGlCQUFpQixDQUFDLFVBQVUsQ0FBQyxDQUFDO0FBQ3hDLENBQUMiLCJzb3VyY2VzQ29udGVudCI6WyJpbXBvcnQgeyBodG1sLCByZW5kZXIsIHR5cGUgVGVtcGxhdGVSZXN1bHQgfSBmcm9tIFwibGl0LWh0bWxcIjtcblxuaW1wb3J0IHR5cGUgeyBQcm9kdWN0c0FwcHJvdmFsU3RhdHVzIH0gZnJvbSBcIi4uL3R5cGVzL3Byb2R1Y3RzLnRzXCI7XG5pbXBvcnQgdHlwZSB7IFN0YXRlQ29uZmlnIH0gZnJvbSBcIi4uL3R5cGVzL3RhYmxlQ29udGV4dC50c1wiO1xuaW1wb3J0IHsgdG9nZ2xlSGlnaGxpZ2h0IH0gZnJvbSBcIi4uL3V0aWxzL2RvbVV0aWxzLmpzXCI7XG5pbXBvcnQgc2hvd1Byb2R1Y3RzVGFibGUgZnJvbSBcIi4vcHJvZHVjdHNUYWJsZS5qc1wiO1xuaW1wb3J0IGdldE5ld0NvbnRleHRJbnN0YW5jZSBmcm9tIFwiLi90YWJsZUNvbnRleHQuanNcIjtcblxuXG5jb25zdCBTVEFSVF9QQUdFX05VTUJFUiA9IDE7XG5jb25zdCBQUk9EVUNUU19QRVJfUEFHRV9DT1VOVCA9IDU7XG5cbmNvbnN0IGNvbnRyb2xzRGl2RWwgPSBkb2N1bWVudC5xdWVyeVNlbGVjdG9yPEhUTUxEaXZFbGVtZW50PihcImRpdiNjb250cm9sc1wiKSE7XG5cbmRvY3VtZW50LmFkZEV2ZW50TGlzdGVuZXIoXCJET01Db250ZW50TG9hZGVkXCIsIGFzeW5jICgpID0+IHtcbiAgICByZW5kZXIobWFpblRlbXBsYXRlKCksIGNvbnRyb2xzRGl2RWwpO1xuICAgIGF3YWl0IHNob3dQcm9kdWN0cyhcIkFwcHJvdmVkXCIpO1xufSk7XG5cbmZ1bmN0aW9uIG1haW5UZW1wbGF0ZSgpOiBUZW1wbGF0ZVJlc3VsdCB7XG4gICAgcmV0dXJuIGh0bWxgXG4gICAgICAgIDxkaXYgY2xhc3M9XCJidG4tZ3JvdXAgYnRuLWdyb3VwLWxnIHctMTAwXCIgcm9sZT1cImdyb3VwXCJcbiAgICAgICAgICAgICBhcmlhLWxhYmVsPVwiVGFibGUgZGF0YSBjb250cm9sIGJ1dHRvbnMuXCI+XG4gICAgICAgICAgICA8aW5wdXQgdHlwZT1cInJhZGlvXCIgY2xhc3M9XCJidG4tY2hlY2tcIiBuYW1lPVwiYnRucmFkaW9cIiBpZD1cImJ0bnJhZGlvMVwiXG4gICAgICAgICAgICAgICAgICAgQGNoYW5nZT0ke2FzeW5jICgpID0+IGF3YWl0IHNob3dQcm9kdWN0cyhcIkFwcHJvdmVkXCIpfVxuICAgICAgICAgICAgICAgICAgIGF1dG9jb21wbGV0ZT1cIm9mZlwiPlxuICAgICAgICAgICAgPGxhYmVsIGNsYXNzPVwiYnRuIGJ0bi10ZWFsIGQtZmxleCBqdXN0aWZ5LWNvbnRlbnQtY2VudGVyIGFsaWduLWl0ZW1zLWNlbnRlclwiXG4gICAgICAgICAgICAgICAgICAgc3R5bGU9XCJ3aWR0aDogMjk3cHg7XCIgZm9yPVwiYnRucmFkaW8xXCI+QXBwcm92ZWQ8L2xhYmVsPlxuXG4gICAgICAgICAgICA8aW5wdXQgdHlwZT1cInJhZGlvXCIgY2xhc3M9XCJidG4tY2hlY2tcIiBuYW1lPVwiYnRucmFkaW9cIiBpZD1cImJ0bnJhZGlvMlwiXG4gICAgICAgICAgICAgICAgICAgQGNoYW5nZT0ke2FzeW5jICgpID0+IGF3YWl0IHNob3dQcm9kdWN0cyhcIk5vdCBBcHByb3ZlZFwiKX1cbiAgICAgICAgICAgICAgICAgICBhdXRvY29tcGxldGU9XCJvZmZcIj5cbiAgICAgICAgICAgIDxsYWJlbCBjbGFzcz1cImJ0biBidG4tb3V0bGluZS10ZWFsIGQtZmxleCBqdXN0aWZ5LWNvbnRlbnQtY2VudGVyIGFsaWduLWl0ZW1zLWNlbnRlclwiXG4gICAgICAgICAgICAgICAgICAgc3R5bGU9XCJ3aWR0aDogMjk3cHg7XCIgZm9yPVwiYnRucmFkaW8yXCI+Tm90IEFwcHJvdmVkPC9sYWJlbD5cbiAgICAgICAgPC9kaXY+XG4gICAgYDtcbn1cblxuYXN5bmMgZnVuY3Rpb24gc2hvd1Byb2R1Y3RzKGFwcHJvdmFsU3RhdHVzOiBQcm9kdWN0c0FwcHJvdmFsU3RhdHVzKTogUHJvbWlzZTx2b2lkPiB7XG4gICAgdG9nZ2xlSGlnaGxpZ2h0KGFwcHJvdmFsU3RhdHVzKTtcblxuICAgIGNvbnN0IG5ld0NvbnRleHQgPSBnZXROZXdDb250ZXh0SW5zdGFuY2Uoe1xuICAgICAgICBwcm9kdWN0c0FwcHJvdmFsU3RhdHVzOiBhcHByb3ZhbFN0YXR1cyxcbiAgICAgICAgc3RhcnRQYWdlTnVtYmVyOiBTVEFSVF9QQUdFX05VTUJFUixcbiAgICAgICAgcHJvZHVjdHNQZXJQYWdlQ291bnQ6IFBST0RVQ1RTX1BFUl9QQUdFX0NPVU5ULFxuICAgIH0gYXMgU3RhdGVDb25maWcpO1xuXG4gICAgYXdhaXQgc2hvd1Byb2R1Y3RzVGFibGUobmV3Q29udGV4dCk7XG59Il19