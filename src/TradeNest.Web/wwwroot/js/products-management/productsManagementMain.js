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
//# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJmaWxlIjoicHJvZHVjdHNNYW5hZ2VtZW50TWFpbi5qcyIsInNvdXJjZVJvb3QiOiIiLCJzb3VyY2VzIjpbIi4uLy4uLy4uL0Zyb250RW5kU2NyaXB0cy9wcm9kdWN0cy1tYW5hZ2VtZW50L3Byb2R1Y3RzTWFuYWdlbWVudE1haW4udHMiXSwibmFtZXMiOltdLCJtYXBwaW5ncyI6IkFBQUEsT0FBTyxFQUFFLElBQUksRUFBRSxNQUFNLEVBQXVCLE1BQU0sVUFBVSxDQUFDO0FBRzdELE9BQU8sRUFBRSxlQUFlLEVBQUUsTUFBTSxzQkFBc0IsQ0FBQztBQUN2RCxPQUFPLGlCQUFpQixNQUFNLG9CQUFvQixDQUFDO0FBQ25ELE9BQU8scUJBQXFCLE1BQU0sbUJBQW1CLENBQUM7QUFHdEQsTUFBTSxpQkFBaUIsR0FBRyxDQUFDLENBQUM7QUFDNUIsTUFBTSx1QkFBdUIsR0FBRyxDQUFDLENBQUM7QUFFbEMsTUFBTSxhQUFhLEdBQUcsUUFBUSxDQUFDLGFBQWEsQ0FBaUIsY0FBYyxDQUFFLENBQUM7QUFFOUUsUUFBUSxDQUFDLGdCQUFnQixDQUFDLGtCQUFrQixFQUFFLEtBQUssSUFBSSxFQUFFO0lBQ3JELE1BQU0sQ0FBQyxZQUFZLEVBQUUsRUFBRSxhQUFhLENBQUMsQ0FBQztJQUN0QyxNQUFNLFlBQVksQ0FBQyxVQUFVLENBQUMsQ0FBQztBQUNuQyxDQUFDLENBQUMsQ0FBQztBQUVILFNBQVMsWUFBWTtJQUNqQixPQUFPLElBQUksQ0FBQTs7Ozs2QkFJYyxLQUFLLElBQUksRUFBRSxDQUFDLE1BQU0sWUFBWSxDQUFDLFVBQVUsQ0FBQzs7Ozs7OzZCQU0xQyxLQUFLLElBQUksRUFBRSxDQUFDLE1BQU0sWUFBWSxDQUFDLGNBQWMsQ0FBQzs7Ozs7S0FLdEUsQ0FBQztBQUNOLENBQUM7QUFFRCxLQUFLLFVBQVUsWUFBWSxDQUFDLGNBQXNDO0lBQzlELGVBQWUsQ0FBQyxjQUFjLENBQUMsQ0FBQztJQUVoQyxNQUFNLFVBQVUsR0FBRyxxQkFBcUIsQ0FBQztRQUNyQyxzQkFBc0IsRUFBRSxjQUFjO1FBQ3RDLGVBQWUsRUFBRSxpQkFBaUI7UUFDbEMsb0JBQW9CLEVBQUUsdUJBQXVCO0tBQ2hELENBQUMsQ0FBQztJQUVILE1BQU0saUJBQWlCLENBQUMsVUFBVSxDQUFDLENBQUM7QUFDeEMsQ0FBQyIsInNvdXJjZXNDb250ZW50IjpbImltcG9ydCB7IGh0bWwsIHJlbmRlciwgdHlwZSBUZW1wbGF0ZVJlc3VsdCB9IGZyb20gXCJsaXQtaHRtbFwiO1xuXG5pbXBvcnQgdHlwZSB7IFByb2R1Y3RzQXBwcm92YWxTdGF0dXMgfSBmcm9tIFwiLi4vdHlwZXMvcHJvZHVjdHMudHNcIjtcbmltcG9ydCB7IHRvZ2dsZUhpZ2hsaWdodCB9IGZyb20gXCIuLi91dGlscy9kb21VdGlscy5qc1wiO1xuaW1wb3J0IHNob3dQcm9kdWN0c1RhYmxlIGZyb20gXCIuL3Byb2R1Y3RzVGFibGUuanNcIjtcbmltcG9ydCBnZXROZXdDb250ZXh0SW5zdGFuY2UgZnJvbSBcIi4vdGFibGVDb250ZXh0LmpzXCI7XG5cblxuY29uc3QgU1RBUlRfUEFHRV9OVU1CRVIgPSAxO1xuY29uc3QgUFJPRFVDVFNfUEVSX1BBR0VfQ09VTlQgPSA1O1xuXG5jb25zdCBjb250cm9sc0RpdkVsID0gZG9jdW1lbnQucXVlcnlTZWxlY3RvcjxIVE1MRGl2RWxlbWVudD4oXCJkaXYjY29udHJvbHNcIikhO1xuXG5kb2N1bWVudC5hZGRFdmVudExpc3RlbmVyKFwiRE9NQ29udGVudExvYWRlZFwiLCBhc3luYyAoKSA9PiB7XG4gICAgcmVuZGVyKG1haW5UZW1wbGF0ZSgpLCBjb250cm9sc0RpdkVsKTtcbiAgICBhd2FpdCBzaG93UHJvZHVjdHMoXCJBcHByb3ZlZFwiKTtcbn0pO1xuXG5mdW5jdGlvbiBtYWluVGVtcGxhdGUoKTogVGVtcGxhdGVSZXN1bHQge1xuICAgIHJldHVybiBodG1sYFxuICAgICAgICA8ZGl2IGNsYXNzPVwiYnRuLWdyb3VwIGJ0bi1ncm91cC1sZyB3LTEwMFwiIHJvbGU9XCJncm91cFwiXG4gICAgICAgICAgICAgYXJpYS1sYWJlbD1cIlRhYmxlIGRhdGEgY29udHJvbCBidXR0b25zLlwiPlxuICAgICAgICAgICAgPGlucHV0IHR5cGU9XCJyYWRpb1wiIGNsYXNzPVwiYnRuLWNoZWNrXCIgbmFtZT1cImJ0bnJhZGlvXCIgaWQ9XCJidG5yYWRpbzFcIlxuICAgICAgICAgICAgICAgICAgIEBjaGFuZ2U9JHthc3luYyAoKSA9PiBhd2FpdCBzaG93UHJvZHVjdHMoXCJBcHByb3ZlZFwiKX1cbiAgICAgICAgICAgICAgICAgICBhdXRvY29tcGxldGU9XCJvZmZcIj5cbiAgICAgICAgICAgIDxsYWJlbCBjbGFzcz1cImJ0biBidG4tdGVhbCBkLWZsZXgganVzdGlmeS1jb250ZW50LWNlbnRlciBhbGlnbi1pdGVtcy1jZW50ZXJcIlxuICAgICAgICAgICAgICAgICAgIHN0eWxlPVwid2lkdGg6IDI5N3B4O1wiIGZvcj1cImJ0bnJhZGlvMVwiPkFwcHJvdmVkPC9sYWJlbD5cblxuICAgICAgICAgICAgPGlucHV0IHR5cGU9XCJyYWRpb1wiIGNsYXNzPVwiYnRuLWNoZWNrXCIgbmFtZT1cImJ0bnJhZGlvXCIgaWQ9XCJidG5yYWRpbzJcIlxuICAgICAgICAgICAgICAgICAgIEBjaGFuZ2U9JHthc3luYyAoKSA9PiBhd2FpdCBzaG93UHJvZHVjdHMoXCJOb3QgQXBwcm92ZWRcIil9XG4gICAgICAgICAgICAgICAgICAgYXV0b2NvbXBsZXRlPVwib2ZmXCI+XG4gICAgICAgICAgICA8bGFiZWwgY2xhc3M9XCJidG4gYnRuLW91dGxpbmUtdGVhbCBkLWZsZXgganVzdGlmeS1jb250ZW50LWNlbnRlciBhbGlnbi1pdGVtcy1jZW50ZXJcIlxuICAgICAgICAgICAgICAgICAgIHN0eWxlPVwid2lkdGg6IDI5N3B4O1wiIGZvcj1cImJ0bnJhZGlvMlwiPk5vdCBBcHByb3ZlZDwvbGFiZWw+XG4gICAgICAgIDwvZGl2PlxuICAgIGA7XG59XG5cbmFzeW5jIGZ1bmN0aW9uIHNob3dQcm9kdWN0cyhhcHByb3ZhbFN0YXR1czogUHJvZHVjdHNBcHByb3ZhbFN0YXR1cyk6IFByb21pc2U8dm9pZD4ge1xuICAgIHRvZ2dsZUhpZ2hsaWdodChhcHByb3ZhbFN0YXR1cyk7XG5cbiAgICBjb25zdCBuZXdDb250ZXh0ID0gZ2V0TmV3Q29udGV4dEluc3RhbmNlKHtcbiAgICAgICAgcHJvZHVjdHNBcHByb3ZhbFN0YXR1czogYXBwcm92YWxTdGF0dXMsXG4gICAgICAgIHN0YXJ0UGFnZU51bWJlcjogU1RBUlRfUEFHRV9OVU1CRVIsXG4gICAgICAgIHByb2R1Y3RzUGVyUGFnZUNvdW50OiBQUk9EVUNUU19QRVJfUEFHRV9DT1VOVCxcbiAgICB9KTtcblxuICAgIGF3YWl0IHNob3dQcm9kdWN0c1RhYmxlKG5ld0NvbnRleHQpO1xufSJdfQ==