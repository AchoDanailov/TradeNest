import { Modal } from "bootstrap";
import { html, render } from "lit-html";
import removeProductTemplate from "./confirmRemoveProductDialogModal.js";
import getProductDetailsModalTemplate from "./productDetailsModalTemplate.js";
const tableDivContainer = document.querySelector("div#table-container");
const dialogsSection = document.querySelector("div#dialogs-section");
export default async function showProductsTable(context) {
    const currPageProducts = await context.getCurrPageProducts();
    render(await template(currPageProducts, context), tableDivContainer);
}
async function template(currPageProducts, context) {
    const hoverEffect = "nav-link-border-radius-hover-effect-light";
    return html `
        <div>
            ${searchFormTemplate(context)}
        </div>

        <div id="table-wrapper" class="mt-0 pt-0 w-100">
            <table class="table table-hover w-100">
                <thead class="site-sections-bg-teal text-center">
                <tr class="align-middle">
                    <th class="${hoverEffect}"> Product </th>
                    <th class="${hoverEffect}"> Owner </th>
                    <th class="${hoverEffect}"> Category Name </th>
                    <th class="${hoverEffect}"> Approval Status </th>
                    <th class="${hoverEffect}"> Actions </th>
                </tr>
                </thead>

                <tbody class="tbody-top-border">
                    ${currPageProducts.map(p => productTableRowTemplate(p, context))}
                </tbody>
            </table>

            <div class="d-flex justify-content-end align-items-center gap-2 position-relative bottom-0 end-0">
                ${await controlsTemplate(context)}
            </div>

        </div>
    `;
}
function searchFormTemplate(context) {
    const displayClass = context.getCurrSearchQuery().trim() === "" ? "d-none" : "";
    return html `
        <div class="d-flex flex-column align-items-center mt-5 mb-3" id="search-section-wrapper">
            <label for="searchInput" class="form-label text-navy text-center">
                Search for products
            </label>
            <form id="searchForm" class="mx-md-3"
                  @submit=${async (event) => await onSearchFormSubmitHandler(event, context)}>
                <div class="input-group-sm d-flex gap-1">
                    <input name="search" type="search" id="searchInput"
                           class="form-control" placeholder="Search..."
                           aria-label="Search" .value=${context.getCurrSearchQuery()} />
                    <button class="btn btn-outline-teal" type="submit">Search</button>
                </div>
            </form>
            <div class="my-2 position-relative ${displayClass}" style="right: 5px">
                <span>results for: ${context.getCurrSearchQuery()}</span>
                <a>
                    <span class="btn-sm btn-danger"
                          @click=${async () => await onClearSearchForm(context)}>
                        x
                    </span>
                </a>
            </div>
        </div>`;
}
async function onClearSearchForm(context) {
    context.setSearchQuery("");
    await showProductsTable(context);
}
async function onSearchFormSubmitHandler(event, context) {
    event.preventDefault();
    const formData = new FormData(event.currentTarget);
    const searchQuery = formData.get("search");
    if (searchQuery.trim() === "" && context.getCurrSearchQuery() === "") {
        return;
    }
    context.setSearchQuery(searchQuery);
    await showProductsTable(context);
}
async function controlsTemplate(context) {
    const totalPagesCount = context.getPagesTotalCount();
    const currPageNumber = context.getCurrPageNumber();
    const totalItemsCount = context.getProductsCount();
    const itemsCountOnPage = context.getCurrItemsOnPageCount();
    const pageNumbersOnScreen = calculatePageNumbers(currPageNumber, totalPagesCount);
    const [firstItemNumOnPage, lastItemNumOnPage] = calculateItemsNumbers(currPageNumber, totalItemsCount, itemsCountOnPage);
    return html `
        <p class="text-navy text-muted fs-6 fst-italic d-inline">
            ${firstItemNumOnPage}-${lastItemNumOnPage} from ${totalItemsCount}
        </p>
        <nav aria-label="Table pagination control.">
            <ul class="pagination justify-content-center">
                ${currPageNumber <= 1
        ? html `
                            <li class="page-item disabled">
                                <span class="page-link">Previous</span>
                            </li>`
        : html `
                            <li class="page-item">
                                <a class="page-link"
                                   @click=${async () => await onPageNumBtnClick(context, currPageNumber - 1)}>
                                    Previous
                                </a>
                            </li>`}

                ${pageNumbersOnScreen.map(pageNum => {
        return pageNum === currPageNumber
            ? html `
                                <li class="page-item active">
                                   <span class="page-link">
                                       ${currPageNumber}
                                   </span>
                                </li>`
            : html `
                                <li class="page-item">
                                    <a class="page-link"
                                       @click=${async () => await onPageNumBtnClick(context, pageNum)}>
                                        ${pageNum}
                                    </a>
                                </li>`;
    })}

                ${currPageNumber === pageNumbersOnScreen.length
        ? html `
                            <li class="page-item disabled">
                                <span class="page-link">Next</span>
                            </li>`
        : html `
                            <li class="page-item">
                                <a class="page-link"
                                   @click=${async () => await onPageNumBtnClick(context, currPageNumber + 1)}>
                                    Next
                                </a>
                            </li>`}
            </ul>
        </nav>`;
}
async function onPageNumBtnClick(context, pageNumber) {
    context.setPageNumber(pageNumber);
    await showProductsTable(context);
}
function productTableRowTemplate(product, context) {
    const approvalStatusTdMap = {
        "Approved": () => ["🟢", "Approved", "text-success fw-semibold"],
        "WaitingApproval": () => ["🟡", "Waiting Approval", "text-warning fw-semibold"],
        "Disapproved": () => ["🔴", "Disapproved", "text-danger fw-semibold"],
    };
    const [dot, content, styles] = approvalStatusTdMap[product.approvalStatus]();
    return html `
        <tr class="text-center align-middle">
            <td>${product.name}</td>
            <td>${product.ownerName}</td>
            <td>${product.categoryName}</td>
            <td class="${styles}">${dot} ${content}</td>
            <td>
                <div class="btn-group-sm d-flex flex-wrap justify-content-center gap-1 gap-sm-2 gap-md-2">
                    <button class="btn rounded-pill btn-teal btn-sm w-100" style="max-width: 12em"
                            @click=${async () => await onViewProductDetailsHandler(product.id, context)}>
                        View Details
                    </button>

                    <button class="btn rounded-pill btn-outline-danger btn-sm w-100" style="max-width: 12em"
                            @click=${async () => await onRemoveProductHandler(product, context)}>
                        Remove Product
                    </button>
                </div>
            </td>
        </tr>
    `;
}
async function onViewProductDetailsHandler(productId, context) {
    render(await getProductDetailsModalTemplate(productId, context), dialogsSection);
    const productDetailsModalId = `product-details-${productId}`;
    const modalEl = dialogsSection?.querySelector(`div#${productDetailsModalId}`);
    const modal = Modal.getOrCreateInstance(modalEl);
    modalEl?.addEventListener('hidden.bs.modal', () => {
        render(html ``, dialogsSection);
        modal.dispose();
    }, { once: true });
    modal.show();
}
async function onRemoveProductHandler(product, context) {
    render(removeProductTemplate(product, context), dialogsSection);
    const deleteProductModalId = `remove-product-${product.id}`;
    const modalEl = dialogsSection?.querySelector(`div#${deleteProductModalId}`);
    const modal = Modal.getOrCreateInstance(modalEl);
    modalEl?.addEventListener('hidden.bs.modal', () => {
        render(html ``, dialogsSection);
        modal.dispose();
    }, { once: true });
    modal.show();
}
function calculatePageNumbers(currPageNumber, totalPagesCount) {
    const firstPageNumber = Math.max(1, currPageNumber - 3);
    const lastPageNumber = Math.min(totalPagesCount, currPageNumber + 3);
    const pageNumbersOnScreen = [];
    for (let i = firstPageNumber; i <= lastPageNumber; i++) {
        pageNumbersOnScreen.push(i);
    }
    return pageNumbersOnScreen;
}
function calculateItemsNumbers(currPageNumber, totalItemsCount, itemsCountOnPage) {
    if (totalItemsCount === 0) {
        return [0, 0];
    }
    const lastItemNumOnPage = Math.min(currPageNumber * itemsCountOnPage, totalItemsCount);
    const firstItemNumOnPage = (currPageNumber - 1) * itemsCountOnPage + 1;
    return [firstItemNumOnPage, lastItemNumOnPage];
}
//# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJmaWxlIjoicHJvZHVjdHNUYWJsZS5qcyIsInNvdXJjZVJvb3QiOiIiLCJzb3VyY2VzIjpbIi4uLy4uLy4uL0Zyb250RW5kU2NyaXB0cy9wcm9kdWN0cy1tYW5hZ2VtZW50L3Byb2R1Y3RzVGFibGUudHMiXSwibmFtZXMiOltdLCJtYXBwaW5ncyI6IkFBQUEsT0FBTyxFQUFFLEtBQUssRUFBRSxNQUFNLFdBQVcsQ0FBQztBQUNsQyxPQUFPLEVBQUUsSUFBSSxFQUFFLE1BQU0sRUFBdUIsTUFBTSxVQUFVLENBQUM7QUFLN0QsT0FBTyxxQkFBcUIsTUFBTSxzQ0FBc0MsQ0FBQztBQUN6RSxPQUFPLDhCQUE4QixNQUFNLGtDQUFrQyxDQUFDO0FBRzlFLE1BQU0saUJBQWlCLEdBQUcsUUFBUSxDQUFDLGFBQWEsQ0FBaUIscUJBQXFCLENBQUMsQ0FBQztBQUN4RixNQUFNLGNBQWMsR0FBRyxRQUFRLENBQUMsYUFBYSxDQUFpQixxQkFBcUIsQ0FBQyxDQUFDO0FBRXJGLE1BQU0sQ0FBQyxPQUFPLENBQUMsS0FBSyxVQUFVLGlCQUFpQixDQUFDLE9BQXFCO0lBQ2pFLE1BQU0sZ0JBQWdCLEdBQUcsTUFBTSxPQUFPLENBQUMsbUJBQW1CLEVBQUUsQ0FBQztJQUM3RCxNQUFNLENBQUMsTUFBTSxRQUFRLENBQUMsZ0JBQWdCLEVBQUUsT0FBTyxDQUFDLEVBQUUsaUJBQWtCLENBQUMsQ0FBQztBQUMxRSxDQUFDO0FBRUQsS0FBSyxVQUFVLFFBQVEsQ0FDbkIsZ0JBQTJCLEVBQzNCLE9BQXFCO0lBRXJCLE1BQU0sV0FBVyxHQUFHLDJDQUEyQyxDQUFBO0lBRS9ELE9BQU8sSUFBSSxDQUFBOztjQUVELGtCQUFrQixDQUFDLE9BQU8sQ0FBQzs7Ozs7OztpQ0FPUixXQUFXO2lDQUNYLFdBQVc7aUNBQ1gsV0FBVztpQ0FDWCxXQUFXO2lDQUNYLFdBQVc7Ozs7O3NCQUt0QixnQkFBZ0IsQ0FBQyxHQUFHLENBQUMsQ0FBQyxDQUFDLEVBQUUsQ0FBQyx1QkFBdUIsQ0FBQyxDQUFDLEVBQUUsT0FBTyxD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QWMsR0FBRyxDQUFDLENBQUMsR0FBRyxnQkFBZ0IsR0FBRyxDQUFDLENBQUM7SUFFdkUsT0FBTyxDQUFDLGtCQUFrQixFQUFFLGlCQUFpQixDQUFDLENBQUM7QUFDbkQsQ0FBQyIsInNvdXJjZXNDb250ZW50IjpbImltcG9ydCB7IE1vZGFsIH0gZnJvbSBcImJvb3RzdHJhcFwiO1xuaW1wb3J0IHsgaHRtbCwgcmVuZGVyLCB0eXBlIFRlbXBsYXRlUmVzdWx0IH0gZnJvbSBcImxpdC1odG1sXCI7XG5cbmltcG9ydCB0eXBlIHsgUHJvZHVjdCwgUHJvZHVjdHNBcHByb3ZhbFN0YXR1cyB9IGZyb20gXCIuLi90eXBlcy9wcm9kdWN0cy50c1wiO1xuaW1wb3J0IHR5cGUgeyBUYWJsZUNvbnRleHQgfSBmcm9tIFwiLi4vdHlwZXMvdGFibGVDb250ZXh0LnRzXCI7XG5cbmltcG9ydCByZW1vdmVQcm9kdWN0VGVtcGxhdGUgZnJvbSBcIi4vY29uZmlybVJlbW92ZVByb2R1Y3REaWFsb2dNb2RhbC5qc1wiO1xuaW1wb3J0IGdldFByb2R1Y3REZXRhaWxzTW9kYWxUZW1wbGF0ZSBmcm9tIFwiLi9wcm9kdWN0RGV0YWlsc01vZGFsVGVtcGxhdGUuanNcIjtcblxuXG5jb25zdCB0YWJsZURpdkNvbnRhaW5lciA9IGRvY3VtZW50LnF1ZXJ5U2VsZWN0b3I8SFRNTERpdkVsZW1lbnQ+KFwiZGl2I3RhYmxlLWNvbnRhaW5lclwiKTtcbmNvbnN0IGRpYWxvZ3NTZWN0aW9uID0gZG9jdW1lbnQucXVlcnlTZWxlY3RvcjxIVE1MRGl2RWxlbWVudD4oXCJkaXYjZGlhbG9ncy1zZWN0aW9uXCIpO1xuXG5leHBvcnQgZGVmYXVsdCBhc3luYyBmdW5jdGlvbiBzaG93UHJvZHVjdHNUYWJsZShjb250ZXh0OiBUYWJsZUNvbnRleHQpIHtcbiAgICBjb25zdCBjdXJyUGFnZVByb2R1Y3RzID0gYXdhaXQgY29udGV4dC5nZXRDdXJyUGFnZVByb2R1Y3RzKCk7XG4gICAgcmVuZGVyKGF3YWl0IHRlbXBsYXRlKGN1cnJQYWdlUHJvZHVjdHMsIGNvbnRleHQpLCB0YWJsZURpdkNvbnRhaW5lciEpO1xufVxuXG5hc3luYyBmdW5jdGlvbiB0ZW1wbGF0ZShcbiAgICBjdXJyUGFnZVByb2R1Y3RzOiBQcm9kdWN0W10sXG4gICAgY29udGV4dDogVGFibGVDb250ZXh0XG4pOiBQcm9taXNlPFRlbXBsYXRlUmVzdWx0PiB7XG4gICAgY29uc3QgaG92ZXJFZmZlY3QgPSBcIm5hdi1saW5rLWJvcmRlci1yYWRpdXMtaG92ZXItZWZmZWN0LWxpZ2h0XCJcblxuICAgIHJldHVybiBodG1sYFxuICAgICAgICA8ZGl2PlxuICAgICAgICAgICAgJHtzZWFyY2hGb3JtVGVtcGxhdGUoY29udGV4dCl9XG4gICAgICAgIDwvZGl2PlxuXG4gICAgICAgIDxkaXYgaWQ9XCJ0YWJsZS13cmFwcGVyXCIgY2xhc3M9XCJtdC0wIHB0LTAgdy0xMDBcIj5cbiAgICAgICAgICAgIDx0YWJsZSBjbGFzcz1cInRhYmxlIHRhYmxlLWhvdmVyIHctMTAwXCI+XG4gICAgICAgICAgICAgICAgPHRoZWFkIGNsYXNzPVwic2l0ZS1zZWN0aW9ucy1iZy10ZWFsIHRleHQtY2VudGVyXCI+XG4gICAgICAgICAgICAgICAgPHRyIGNsYXNzPVwiYWxpZ24tbWlkZGxlXCI+XG4gICAgICAgICAgICAgICAgICAgIDx0aCBjbGFzcz1cIiR7aG92ZXJFZmZlY3R9XCI+IFByb2R1Y3QgPC90aD5cbiAgICAgICAgICAgICAgICAgICAgPHRoIGNsYXNzPVwiJHtob3ZlckVmZmVjdH1cIj4gT3duZXIgPC90aD5cbiAgICAgICAgICAgICAgICAgICAgPHRoIGNsYXNzPVwiJHtob3ZlckVmZmVjdH1cIj4gQ2F0ZWdvcnkgTmFtZSA8L3RoPlxuICAgICAgICAgICAgICAgICAgICA8dGggY2xhc3M9XCIke2hvdmVyRWZmZWN0fVwiPiBBcHByb3ZhbCBTdGF0dXMgPC90aD5cbiAgICAgICAgICAgICAgICAgICAgPHRoIGNsYXNzPVwiJHtob3ZlckVmZmVjdH1cIj4gQWN0aW9ucyA8L3RoPlxuICAgICAgICAgICAgICAgIDwvdHI+XG4gICAgICAgICAgICAgICAgPC90aGVhZD5cblxuICAgICAgICAgICAgICAgIDx0Ym9keSBjbGFzcz1cInRib2R5LXRvcC1ib3JkZXJcIj5cbiAgICAgICAgICAgICAgICAgICAgJHtjdXJyUGFnZVByb2R1Y3RzLm1hcChwID0+IHByb2R1Y3RUYWJsZVJvd1RlbXBsYXRlKHAsIGNvbnRleHQpKX1cbiAgICAgICAgICAgICAgICA8L3Rib2R5PlxuICAgICAgICAgICAgPC90YWJsZT5cblxuICAgICAgICAgICAgPGRpdiBjbGFzcz1cImQtZmxleCBqdXN0aWZ5LWNvbnRlbnQtZW5kIGFsaWduLWl0ZW1zLWNlbnRlciBnYXAtMiBwb3NpdGlvbi1yZWxhdGl2ZSBib3R0b20tMCBlbmQtMFwiPlxuICAgICAgICAgICAgICAgICR7YXdhaXQgY29udHJvbHNUZW1wbGF0ZShjb250ZXh0KX1cbiAgICAgICAgICAgIDwvZGl2PlxuXG4gICAgICAgIDwvZGl2PlxuICAgIGA7XG59XG5cbmZ1bmN0aW9uIHNlYXJjaEZvcm1UZW1wbGF0ZShjb250ZXh0OiBUYWJsZUNvbnRleHQpIHtcbiAgICBjb25zdCBkaXNwbGF5Q2xhc3MgPSBjb250ZXh0LmdldEN1cnJTZWFyY2hRdWVyeSgpLnRyaW0oKSA9PT0gXCJcIiA/IFwiZC1ub25lXCIgOiBcIlwiO1xuXG4gICAgcmV0dXJuIGh0bWxgXG4gICAgICAgIDxkaXYgY2xhc3M9XCJkLWZsZXggZmxleC1jb2x1bW4gYWxpZ24taXRlbXMtY2VudGVyIG10LTUgbWItM1wiIGlkPVwic2VhcmNoLXNlY3Rpb24td3JhcHBlclwiPlxuICAgICAgICAgICAgPGxhYmVsIGZvcj1cInNlYXJjaElucHV0XCIgY2xhc3M9XCJmb3JtLWxhYmVsIHRleHQtbmF2eSB0ZXh0LWNlbnRlclwiPlxuICAgICAgICAgICAgICAgIFNlYXJjaCBmb3IgcHJvZHVjdHNcbiAgICAgICAgICAgIDwvbGFiZWw+XG4gICAgICAgICAgICA8Zm9ybSBpZD1cInNlYXJjaEZvcm1cIiBjbGFzcz1cIm14LW1kLTNcIlxuICAgICAgICAgICAgICAgICAgQHN1Ym1pdD0ke2FzeW5jIChldmVudDogRXZlbnQpID0+IGF3YWl0IG9uU2VhcmNoRm9ybVN1Ym1pdEhhbmRsZXIoZXZlbnQsIGNvbnRleHQpfT5cbiAgICAgICAgICAgICAgICA8ZGl2IGNsYXNzPVwiaW5wdXQtZ3JvdXAtc20gZC1mbGV4IGdhcC0xXCI+XG4gICAgICAgICAgICAgICAgICAgIDxpbnB1dCBuYW1lPVwic2VhcmNoXCIgdHlwZT1cInNlYXJjaFwiIGlkPVwic2VhcmNoSW5wdXRcIlxuICAgICAgICAgICAgICAgICAgICAgICAgICAgY2xhc3M9XCJmb3JtLWNvbnRyb2xcIiBwbGFjZWhvbGRlcj1cIlNlYXJjaC4uLlwiXG4gICAgICAgICAgICAgICAgICAgICAgICAgICBhcmlhLWxhYmVsPVwiU2VhcmNoXCIgLnZhbHVlPSR7Y29udGV4dC5nZXRDdXJyU2VhcmNoUXVlcnkoKX0gLz5cbiAgICAgICAgICAgICAgICAgICAgPGJ1dHRvbiBjbGFzcz1cImJ0biBidG4tb3V0bGluZS10ZWFsXCIgdHlwZT1cInN1Ym1pdFwiPlNlYXJjaDwvYnV0dG9uPlxuICAgICAgICAgICAgICAgIDwvZGl2PlxuICAgICAgICAgICAgPC9mb3JtPlxuICAgICAgICAgICAgPGRpdiBjbGFzcz1cIm15LTIgcG9zaXRpb24tcmVsYXRpdmUgJHtkaXNwbGF5Q2xhc3N9XCIgc3R5bGU9XCJyaWdodDogNXB4XCI+XG4gICAgICAgICAgICAgICAgPHNwYW4+cmVzdWx0cyBmb3I6ICR7Y29udGV4dC5nZXRDdXJyU2VhcmNoUXVlcnkoKX08L3NwYW4+XG4gICAgICAgICAgICAgICAgPGE+XG4gICAgICAgICAgICAgICAgICAgIDxzcGFuIGNsYXNzPVwiYnRuLXNtIGJ0bi1kYW5nZXJcIlxuICAgICAgICAgICAgICAgICAgICAgICAgICBAY2xpY2s9JHthc3luYyAoKSA9PiBhd2FpdCBvbkNsZWFyU2VhcmNoRm9ybShjb250ZXh0KX0+XG4gICAgICAgICAgICAgICAgICAgICAgICB4XG4gICAgICAgICAgICAgICAgICAgIDwvc3Bhbj5cbiAgICAgICAgICAgICAgICA8L2E+XG4gICAgICAgICAgICA8L2Rpdj5cbiAgICAgICAgPC9kaXY+YDtcbn1cblxuYXN5bmMgZnVuY3Rpb24gb25DbGVhclNlYXJjaEZvcm0oY29udGV4dDogVGFibGVDb250ZXh0KSB7XG4gICAgY29udGV4dC5zZXRTZWFyY2hRdWVyeShcIlwiKTtcbiAgICBhd2FpdCBzaG93UHJvZHVjdHNUYWJsZShjb250ZXh0KTtcbn1cblxuYXN5bmMgZnVuY3Rpb24gb25TZWFyY2hGb3JtU3VibWl0SGFuZGxlcihldmVudDogRXZlbnQsIGNvbnRleHQ6IFRhYmxlQ29udGV4dCkge1xuICAgIGV2ZW50LnByZXZlbnREZWZhdWx0KCk7XG5cbiAgICBjb25zdCBmb3JtRGF0YSA9IG5ldyBGb3JtRGF0YShldmVudC5jdXJyZW50VGFyZ2V0IGFzIEhUTUxGb3JtRWxlbWVudCB8IHVuZGVmaW5lZCk7XG4gICAgY29uc3Qgc2VhcmNoUXVlcnkgPSBmb3JtRGF0YS5nZXQoXCJzZWFyY2hcIikgYXMgRm9ybURhdGFFbnRyeVZhbHVlIGFzIHN0cmluZztcbiAgICBpZihzZWFyY2hRdWVyeS50cmltKCkgPT09IFwiXCIgJiYgY29udGV4dC5nZXRDdXJyU2VhcmNoUXVlcnkoKSA9PT0gXCJcIikge1xuICAgICAgICByZXR1cm47XG4gICAgfVxuXG4gICAgY29udGV4dC5zZXRTZWFyY2hRdWVyeShzZWFyY2hRdWVyeSk7XG4gICAgYXdhaXQgc2hvd1Byb2R1Y3RzVGFibGUoY29udGV4dCk7XG59XG5cbmFzeW5jIGZ1bmN0aW9uIGNvbnRyb2xzVGVtcGxhdGUoY29udGV4dDogVGFibGVDb250ZXh0KSB7XG4gICAgY29uc3QgdG90YWxQYWdlc0NvdW50ID0gY29udGV4dC5nZXRQYWdlc1RvdGFsQ291bnQoKTtcbiAgICBjb25zdCBjdXJyUGFnZU51bWJlciA9IGNvbnRleHQuZ2V0Q3VyclBhZ2VOdW1iZXIoKTtcbiAgICBjb25zdCB0b3RhbEl0ZW1zQ291bnQgPSBjb250ZXh0LmdldFByb2R1Y3RzQ291bnQoKTtcbiAgICBjb25zdCBpdGVtc0NvdW50T25QYWdlID0gY29udGV4dC5nZXRDdXJySXRlbXNPblBhZ2VDb3VudCgpO1xuXG4gICAgY29uc3QgcGFnZU51bWJlcnNPblNjcmVlbiA9IGNhbGN1bGF0ZVBhZ2VOdW1iZXJzKGN1cnJQYWdlTnVtYmVyLCB0b3RhbFBhZ2VzQ291bnQpXG5cbiAgICBjb25zdCBbZmlyc3RJdGVtTnVtT25QYWdlLCBsYXN0SXRlbU51bU9uUGFnZV1cbiAgICAgICAgPSBjYWxjdWxhdGVJdGVtc051bWJlcnMoY3VyclBhZ2VOdW1iZXIsIHRvdGFsSXRlbXNDb3VudCwgaXRlbXNDb3VudE9uUGFnZSk7XG5cbiAgICByZXR1cm4gaHRtbGBcbiAgICAgICAgPHAgY2xhc3M9XCJ0ZXh0LW5hdnkgdGV4dC1tdXRlZCBmcy02IGZzdC1pdGFsaWMgZC1pbmxpbmVcIj5cbiAgICAgICAgICAgICR7Zmlyc3RJdGVtTnVtT25QYWdlfS0ke2xhc3RJdGVtTnVtT25QYWdlfSBmcm9tICR7dG90YWxJdGVtc0NvdW50fVxuICAgICAgICA8L3A+XG4gICAgICAgIDxuYXYgYXJpYS1sYWJlbD1cIlRhYmxlIHBhZ2luYXRpb24gY29udHJvbC5cIj5cbiAgICAgICAgICAgIDx1bCBjbGFzcz1cInBhZ2luYXRpb24ganVzdGlmeS1jb250ZW50LWNlbnRlclwiPlxuICAgICAgICAgICAgICAgICR7Y3VyclBhZ2VOdW1iZXIgPD0gMVxuICAgICAgICAgICAgICAgICAgICAgICAgPyBodG1sYFxuICAgICAgICAgICAgICAgICAgICAgICAgICAgIDxsaSBjbGFzcz1cInBhZ2UtaXRlbSBkaXNhYmxlZFwiPlxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICA8c3BhbiBjbGFzcz1cInBhZ2UtbGlua1wiPlByZXZpb3VzPC9zcGFuPlxuICAgICAgICAgICAgICAgICAgICAgICAgICAgIDwvbGk+YFxuICAgICAgICAgICAgICAgICAgICAgICAgOiBodG1sYFxuICAgICAgICAgICAgICAgICAgICAgICAgICAgIDxsaSBjbGFzcz1cInBhZ2UtaXRlbVwiPlxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICA8YSBjbGFzcz1cInBhZ2UtbGlua1wiXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIEBjbGljaz0ke2FzeW5jICgpID0+IGF3YWl0IG9uUGFnZU51bUJ0bkNsaWNrKGNvbnRleHQsIGN1cnJQYWdlTnVtYmVyIC0gMSl9PlxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgUHJldmlvdXNcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgPC9hPlxuICAgICAgICAgICAgICAgICAgICAgICAgICAgIDwvbGk+YFxuICAgICAgICAgICAgICAgIH1cblxuICAgICAgICAgICAgICAgICR7cGFnZU51bWJlcnNPblNjcmVlbi5tYXAocGFnZU51bSA9PiB7XG4gICAgICAgICAgICAgICAgICAgIHJldHVybiBwYWdlTnVtID09PSBjdXJyUGFnZU51bWJlclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgID8gaHRtbCBgXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIDxsaSBjbGFzcz1cInBhZ2UtaXRlbSBhY3RpdmVcIj5cbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgPHNwYW4gY2xhc3M9XCJwYWdlLWxpbmtcIj5cbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICR7Y3VyclBhZ2VOdW1iZXJ9XG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIDwvc3Bhbj5cbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgPC9saT5gXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgOiBodG1sIGBcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgPGxpIGNsYXNzPVwicGFnZS1pdGVtXCI+XG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICA8YSBjbGFzcz1cInBhZ2UtbGlua1wiXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICBAY2xpY2s9JHthc3luYyAoKSA9PiBhd2FpdCBvblBhZ2VOdW1CdG5DbGljayhjb250ZXh0LCBwYWdlTnVtKX0+XG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgJHtwYWdlTnVtfVxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgPC9hPlxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICA8L2xpPmBcbiAgICAgICAgICAgICAgICB9KX1cblxuICAgICAgICAgICAgICAgICR7Y3VyclBhZ2VOdW1iZXIgPT09IHBhZ2VOdW1iZXJzT25TY3JlZW4ubGVuZ3RoXG4gICAgICAgICAgICAgICAgICAgICAgICA/IGh0bWxgXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgPGxpIGNsYXNzPVwicGFnZS1pdGVtIGRpc2FibGVkXCI+XG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIDxzcGFuIGNsYXNzPVwicGFnZS1saW5rXCI+TmV4dDwvc3Bhbj5cbiAgICAgICAgICAgICAgICAgICAgICAgICAgICA8L2xpPmBcbiAgICAgICAgICAgICAgICAgICAgICAgIDogaHRtbGBcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICA8bGkgY2xhc3M9XCJwYWdlLWl0ZW1cIj5cbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgPGEgY2xhc3M9XCJwYWdlLWxpbmtcIlxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICBAY2xpY2s9JHthc3luYyAoKSA9PiBhd2FpdCBvblBhZ2VOdW1CdG5DbGljayhjb250ZXh0LCBjdXJyUGFnZU51bWJlciArIDEpfT5cbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIE5leHRcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgPC9hPlxuICAgICAgICAgICAgICAgICAgICAgICAgICAgIDwvbGk+YFxuICAgICAgICAgICAgICAgIH1cbiAgICAgICAgICAgIDwvdWw+XG4gICAgICAgIDwvbmF2PmA7XG59XG5cbmFzeW5jIGZ1bmN0aW9uIG9uUGFnZU51bUJ0bkNsaWNrKGNvbnRleHQ6IFRhYmxlQ29udGV4dCwgcGFnZU51bWJlcjogbnVtYmVyKSB7XG4gICAgY29udGV4dC5zZXRQYWdlTnVtYmVyKHBhZ2VOdW1iZXIpO1xuICAgIGF3YWl0IHNob3dQcm9kdWN0c1RhYmxlKGNvbnRleHQpO1xufVxuXG5mdW5jdGlvbiBwcm9kdWN0VGFibGVSb3dUZW1wbGF0ZShwcm9kdWN0OiBQcm9kdWN0LCBjb250ZXh0OiBUYWJsZUNvbnRleHQpIHtcbiAgICBjb25zdCBhcHByb3ZhbFN0YXR1c1RkTWFwID0ge1xuICAgICAgICBcIkFwcHJvdmVkXCI6ICgpID0+IFtcIvCfn6JcIiwgXCJBcHByb3ZlZFwiLCBcInRleHQtc3VjY2VzcyBmdy1zZW1pYm9sZFwiXSxcbiAgICAgICAgXCJXYWl0aW5nQXBwcm92YWxcIjogKCkgPT4gW1wi8J+foVwiLCBcIldhaXRpbmcgQXBwcm92YWxcIiwgXCJ0ZXh0LXdhcm5pbmcgZnctc2VtaWJvbGRcIl0sXG4gICAgICAgIFwiRGlzYXBwcm92ZWRcIjogKCkgPT4gW1wi8J+UtFwiLCBcIkRpc2FwcHJvdmVkXCIsIFwidGV4dC1kYW5nZXIgZnctc2VtaWJvbGRcIl0sXG4gICAgfSBhcyBSZWNvcmQ8UHJvZHVjdHNBcHByb3ZhbFN0YXR1cywgKCkgPT4gcmVhZG9ubHkgW2RvdDogc3RyaW5nLCBjb250ZW50OiBzdHJpbmcsIHN0eWxlczogc3RyaW5nXT47XG5cbiAgICBjb25zdCBbZG90LCBjb250ZW50LCBzdHlsZXNdID0gYXBwcm92YWxTdGF0dXNUZE1hcFtwcm9kdWN0IS5hcHByb3ZhbFN0YXR1c10hKCk7XG5cbiAgICByZXR1cm4gaHRtbGBcbiAgICAgICAgPHRyIGNsYXNzPVwidGV4dC1jZW50ZXIgYWxpZ24tbWlkZGxlXCI+XG4gICAgICAgICAgICA8dGQ+JHtwcm9kdWN0Lm5hbWV9PC90ZD5cbiAgICAgICAgICAgIDx0ZD4ke3Byb2R1Y3Qub3duZXJOYW1lfTwvdGQ+XG4gICAgICAgICAgICA8dGQ+JHtwcm9kdWN0LmNhdGVnb3J5TmFtZX08L3RkPlxuICAgICAgICAgICAgPHRkIGNsYXNzPVwiJHtzdHlsZXN9XCI+JHtkb3R9ICR7Y29udGVudH08L3RkPlxuICAgICAgICAgICAgPHRkPlxuICAgICAgICAgICAgICAgIDxkaXYgY2xhc3M9XCJidG4tZ3JvdXAtc20gZC1mbGV4IGZsZXgtd3JhcCBqdXN0aWZ5LWNvbnRlbnQtY2VudGVyIGdhcC0xIGdhcC1zbS0yIGdhcC1tZC0yXCI+XG4gICAgICAgICAgICAgICAgICAgIDxidXR0b24gY2xhc3M9XCJidG4gcm91bmRlZC1waWxsIGJ0bi10ZWFsIGJ0bi1zbSB3LTEwMFwiIHN0eWxlPVwibWF4LXdpZHRoOiAxMmVtXCJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICBAY2xpY2s9JHthc3luYyAoKSA9PiBhd2FpdCBvblZpZXdQcm9kdWN0RGV0YWlsc0hhbmRsZXIocHJvZHVjdC5pZCwgY29udGV4dCl9PlxuICAgICAgICAgICAgICAgICAgICAgICAgVmlldyBEZXRhaWxzXG4gICAgICAgICAgICAgICAgICAgIDwvYnV0dG9uPlxuXG4gICAgICAgICAgICAgICAgICAgIDxidXR0b24gY2xhc3M9XCJidG4gcm91bmRlZC1waWxsIGJ0bi1vdXRsaW5lLWRhbmdlciBidG4tc20gdy0xMDBcIiBzdHlsZT1cIm1heC13aWR0aDogMTJlbVwiXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgQGNsaWNrPSR7YXN5bmMgKCkgPT4gYXdhaXQgb25SZW1vdmVQcm9kdWN0SGFuZGxlcihwcm9kdWN0LCBjb250ZXh0KX0+XG4gICAgICAgICAgICAgICAgICAgICAgICBSZW1vdmUgUHJvZHVjdFxuICAgICAgICAgICAgICAgICAgICA8L2J1dHRvbj5cbiAgICAgICAgICAgICAgICA8L2Rpdj5cbiAgICAgICAgICAgIDwvdGQ+XG4gICAgICAgIDwvdHI+XG4gICAgYDtcbn1cblxuYXN5bmMgZnVuY3Rpb24gb25WaWV3UHJvZHVjdERldGFpbHNIYW5kbGVyKHByb2R1Y3RJZDogc3RyaW5nLCBjb250ZXh0OiBUYWJsZUNvbnRleHQpIHtcbiAgICByZW5kZXIoYXdhaXQgZ2V0UHJvZHVjdERldGFpbHNNb2RhbFRlbXBsYXRlKHByb2R1Y3RJZCwgY29udGV4dCksIGRpYWxvZ3NTZWN0aW9uISk7XG5cbiAgICBjb25zdCBwcm9kdWN0RGV0YWlsc01vZGFsSWQgPSBgcHJvZHVjdC1kZXRhaWxzLSR7cHJvZHVjdElkfWA7XG4gICAgY29uc3QgbW9kYWxFbCA9IGRpYWxvZ3NTZWN0aW9uPy5xdWVyeVNlbGVjdG9yPEhUTUxEaXZFbGVtZW50PihgZGl2IyR7cHJvZHVjdERldGFpbHNNb2RhbElkfWApO1xuICAgIGNvbnN0IG1vZGFsID0gTW9kYWwuZ2V0T3JDcmVhdGVJbnN0YW5jZShtb2RhbEVsISk7XG5cbiAgICBtb2RhbEVsPy5hZGRFdmVudExpc3RlbmVyKCdoaWRkZW4uYnMubW9kYWwnLCAoKSA9PiB7XG4gICAgICAgIHJlbmRlcihodG1sYGAsIGRpYWxvZ3NTZWN0aW9uISk7XG4gICAgICAgIG1vZGFsLmRpc3Bvc2UoKTtcbiAgICB9LCB7IG9uY2U6IHRydWUgfSk7XG5cbiAgICBtb2RhbC5zaG93KCk7XG59XG5cbmFzeW5jIGZ1bmN0aW9uIG9uUmVtb3ZlUHJvZHVjdEhhbmRsZXIocHJvZHVjdDogUHJvZHVjdCwgY29udGV4dDogVGFibGVDb250ZXh0KSB7XG4gICAgcmVuZGVyKHJlbW92ZVByb2R1Y3RUZW1wbGF0ZShwcm9kdWN0LCBjb250ZXh0KSwgZGlhbG9nc1NlY3Rpb24hKTtcblxuICAgIGNvbnN0IGRlbGV0ZVByb2R1Y3RNb2RhbElkID0gYHJlbW92ZS1wcm9kdWN0LSR7cHJvZHVjdC5pZH1gO1xuICAgIGNvbnN0IG1vZGFsRWwgPSBkaWFsb2dzU2VjdGlvbj8ucXVlcnlTZWxlY3RvcjxIVE1MRGl2RWxlbWVudD4oYGRpdiMke2RlbGV0ZVByb2R1Y3RNb2RhbElkfWApXG4gICAgY29uc3QgbW9kYWwgPSBNb2RhbC5nZXRPckNyZWF0ZUluc3RhbmNlKG1vZGFsRWwhKTtcblxuICAgIG1vZGFsRWw/LmFkZEV2ZW50TGlzdGVuZXIoJ2hpZGRlbi5icy5tb2RhbCcsICgpID0+IHtcbiAgICAgICAgcmVuZGVyKGh0bWxgYCwgZGlhbG9nc1NlY3Rpb24hKTtcbiAgICAgICAgbW9kYWwuZGlzcG9zZSgpO1xuICAgIH0sIHsgb25jZTogdHJ1ZSB9KTtcblxuICAgIG1vZGFsLnNob3coKTtcbn1cblxuZnVuY3Rpb24gY2FsY3VsYXRlUGFnZU51bWJlcnMoY3VyclBhZ2VOdW1iZXI6IG51bWJlciwgdG90YWxQYWdlc0NvdW50OiBudW1iZXIpIHtcbiAgICBjb25zdCBmaXJzdFBhZ2VOdW1iZXIgPSBNYXRoLm1heCgxLCBjdXJyUGFnZU51bWJlciAtIDMpO1xuICAgIGNvbnN0IGxhc3RQYWdlTnVtYmVyID0gTWF0aC5taW4odG90YWxQYWdlc0NvdW50LCBjdXJyUGFnZU51bWJlciArIDMpO1xuXG4gICAgY29uc3QgcGFnZU51bWJlcnNPblNjcmVlbiA9IFtdO1xuICAgIGZvcihsZXQgaSA9IGZpcnN0UGFnZU51bWJlcjsgaSA8PSBsYXN0UGFnZU51bWJlcjsgaSsrKSB7XG4gICAgICAgIHBhZ2VOdW1iZXJzT25TY3JlZW4ucHVzaChpKTtcbiAgICB9XG5cbiAgICByZXR1cm4gcGFnZU51bWJlcnNPblNjcmVlbjtcbn1cblxuZnVuY3Rpb24gY2FsY3VsYXRlSXRlbXNOdW1iZXJzKFxuICAgIGN1cnJQYWdlTnVtYmVyOiBudW1iZXIsXG4gICAgdG90YWxJdGVtc0NvdW50OiBudW1iZXIsXG4gICAgaXRlbXNDb3VudE9uUGFnZTogbnVtYmVyXG4pOiByZWFkb25seSBbZmlyc3RJdGVtTnVtT25QYWdlOiBudW1iZXIsIGxhc3RJdGVtTnVtT25QYWdlOiBudW1iZXJdIHtcbiAgICBpZiAodG90YWxJdGVtc0NvdW50ID09PSAwKSB7XG4gICAgICAgIHJldHVybiBbMCwgMF07XG4gICAgfVxuXG4gICAgY29uc3QgbGFzdEl0ZW1OdW1PblBhZ2UgPSBNYXRoLm1pbihjdXJyUGFnZU51bWJlciAqIGl0ZW1zQ291bnRPblBhZ2UsIHRvdGFsSXRlbXNDb3VudCk7XG4gICAgY29uc3QgZmlyc3RJdGVtTnVtT25QYWdlID0gKGN1cnJQYWdlTnVtYmVyIC0gMSkgKiBpdGVtc0NvdW50T25QYWdlICsgMTtcblxuICAgIHJldHVybiBbZmlyc3RJdGVtTnVtT25QYWdlLCBsYXN0SXRlbU51bU9uUGFnZV07XG59XG4iXX0=