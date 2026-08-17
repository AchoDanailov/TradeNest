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
//# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJmaWxlIjoicHJvZHVjdHNUYWJsZS5qcyIsInNvdXJjZVJvb3QiOiIiLCJzb3VyY2VzIjpbIi4uLy4uLy4uL0Zyb250RW5kU2NyaXB0cy9wcm9kdWN0cy1tYW5hZ2VtZW50L3Byb2R1Y3RzVGFibGUudHMiXSwibmFtZXMiOltdLCJtYXBwaW5ncyI6IkFBQUEsT0FBTyxFQUFFLEtBQUssRUFBRSxNQUFNLFdBQVcsQ0FBQztBQUNsQyxPQUFPLEVBQUUsSUFBSSxFQUFFLE1BQU0sRUFBdUIsTUFBTSxVQUFVLENBQUM7QUFLN0QsT0FBTyxxQkFBcUIsTUFBTSxzQ0FBc0MsQ0FBQztBQUN6RSxPQUFPLDhCQUE4QixNQUFNLGtDQUFrQyxDQUFDO0FBRzlFLE1BQU0saUJBQWlCLEdBQUcsUUFBUSxDQUFDLGFBQWEsQ0FBaUIscUJBQXFCLENBQUMsQ0FBQztBQUN4RixNQUFNLGNBQWMsR0FBRyxRQUFRLENBQUMsYUFBYSxDQUFpQixxQkFBcUIsQ0FBQyxDQUFDO0FBRXJGLE1BQU0sQ0FBQyxPQUFPLENBQUMsS0FBSyxVQUFVLGlCQUFpQixDQUFDLE9BQXFCO0lBQ2pFLE1BQU0sZ0JBQWdCLEdBQUcsTUFBTSxPQUFPLENBQUMsbUJBQW1CLEVBQUUsQ0FBQztJQUM3RCxNQUFNLENBQUMsTUFBTSxRQUFRLENBQUMsZ0JBQWdCLEVBQUUsT0FBTyxDQUFDLEVBQUUsaUJBQWtCLENBQUMsQ0FBQztBQUMxRSxDQUFDO0FBRUQsS0FBSyxVQUFVLFFBQVEsQ0FDbkIsZ0JBQTJCLEVBQzNCLE9BQXFCO0lBRXJCLE1BQU0sV0FBVyxHQUFHLDJDQUEyQyxDQUFBO0lBRS9ELE9BQU8sSUFBSSxDQUFBOztjQUVELGtCQUFrQixDQUFDLE9BQU8sQ0FBQzs7Ozs7OztpQ0FPUixXQUFXO2lDQUNYLFdBQVc7aUNBQ1gsV0FBVztpQ0FDWCxXQUFXO2lDQUNYLFdBQVc7Ozs7O3NCQUt0QixnQkFBZ0IsQ0FBQyxHQUFHLENBQUMsQ0FBQyxDQUFDLEVBQUUsQ0FBQyx1QkFBdUIsQ0FBQyxDQUFDLEVBQUUsT0FBTyxD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YyxHQUFHLENBQUMsQ0FBQyxHQUFHLGdCQUFnQixHQUFHLENBQUMsQ0FBQztJQUV2RSxPQUFPLENBQUMsa0JBQWtCLEVBQUUsaUJBQWlCLENBQUMsQ0FBQztBQUNuRCxDQUFDIiwic291cmNlc0NvbnRlbnQiOlsiaW1wb3J0IHsgTW9kYWwgfSBmcm9tIFwiYm9vdHN0cmFwXCI7XG5pbXBvcnQgeyBodG1sLCByZW5kZXIsIHR5cGUgVGVtcGxhdGVSZXN1bHQgfSBmcm9tIFwibGl0LWh0bWxcIjtcblxuaW1wb3J0IHR5cGUgeyBQcm9kdWN0LCBQcm9kdWN0c0FwcHJvdmFsU3RhdHVzIH0gZnJvbSBcIi4uL3R5cGVzL3Byb2R1Y3RzLnRzXCI7XG5pbXBvcnQgdHlwZSB7IFRhYmxlQ29udGV4dCB9IGZyb20gXCIuLi90eXBlcy90YWJsZUNvbnRleHQudHNcIjtcblxuaW1wb3J0IHJlbW92ZVByb2R1Y3RUZW1wbGF0ZSBmcm9tIFwiLi9jb25maXJtUmVtb3ZlUHJvZHVjdERpYWxvZ01vZGFsLmpzXCI7XG5pbXBvcnQgZ2V0UHJvZHVjdERldGFpbHNNb2RhbFRlbXBsYXRlIGZyb20gXCIuL3Byb2R1Y3REZXRhaWxzTW9kYWxUZW1wbGF0ZS5qc1wiO1xuXG5cbmNvbnN0IHRhYmxlRGl2Q29udGFpbmVyID0gZG9jdW1lbnQucXVlcnlTZWxlY3RvcjxIVE1MRGl2RWxlbWVudD4oXCJkaXYjdGFibGUtY29udGFpbmVyXCIpO1xuY29uc3QgZGlhbG9nc1NlY3Rpb24gPSBkb2N1bWVudC5xdWVyeVNlbGVjdG9yPEhUTUxEaXZFbGVtZW50PihcImRpdiNkaWFsb2dzLXNlY3Rpb25cIik7XG5cbmV4cG9ydCBkZWZhdWx0IGFzeW5jIGZ1bmN0aW9uIHNob3dQcm9kdWN0c1RhYmxlKGNvbnRleHQ6IFRhYmxlQ29udGV4dCkge1xuICAgIGNvbnN0IGN1cnJQYWdlUHJvZHVjdHMgPSBhd2FpdCBjb250ZXh0LmdldEN1cnJQYWdlUHJvZHVjdHMoKTtcbiAgICByZW5kZXIoYXdhaXQgdGVtcGxhdGUoY3VyclBhZ2VQcm9kdWN0cywgY29udGV4dCksIHRhYmxlRGl2Q29udGFpbmVyISk7XG59XG5cbmFzeW5jIGZ1bmN0aW9uIHRlbXBsYXRlKFxuICAgIGN1cnJQYWdlUHJvZHVjdHM6IFByb2R1Y3RbXSxcbiAgICBjb250ZXh0OiBUYWJsZUNvbnRleHRcbik6IFByb21pc2U8VGVtcGxhdGVSZXN1bHQ+IHtcbiAgICBjb25zdCBob3ZlckVmZmVjdCA9IFwibmF2LWxpbmstYm9yZGVyLXJhZGl1cy1ob3Zlci1lZmZlY3QtbGlnaHRcIlxuXG4gICAgcmV0dXJuIGh0bWxgXG4gICAgICAgIDxkaXY+XG4gICAgICAgICAgICAke3NlYXJjaEZvcm1UZW1wbGF0ZShjb250ZXh0KX1cbiAgICAgICAgPC9kaXY+XG5cbiAgICAgICAgPGRpdiBpZD1cInRhYmxlLXdyYXBwZXJcIiBjbGFzcz1cIm10LTAgcHQtMCB3LTEwMFwiPlxuICAgICAgICAgICAgPHRhYmxlIGNsYXNzPVwidGFibGUgdGFibGUtaG92ZXIgdy0xMDBcIj5cbiAgICAgICAgICAgICAgICA8dGhlYWQgY2xhc3M9XCJzaXRlLXNlY3Rpb25zLWJnLXRlYWwgdGV4dC1jZW50ZXJcIj5cbiAgICAgICAgICAgICAgICA8dHIgY2xhc3M9XCJhbGlnbi1taWRkbGVcIj5cbiAgICAgICAgICAgICAgICAgICAgPHRoIGNsYXNzPVwiJHtob3ZlckVmZmVjdH1cIj4gUHJvZHVjdCA8L3RoPlxuICAgICAgICAgICAgICAgICAgICA8dGggY2xhc3M9XCIke2hvdmVyRWZmZWN0fVwiPiBPd25lciA8L3RoPlxuICAgICAgICAgICAgICAgICAgICA8dGggY2xhc3M9XCIke2hvdmVyRWZmZWN0fVwiPiBDYXRlZ29yeSBOYW1lIDwvdGg+XG4gICAgICAgICAgICAgICAgICAgIDx0aCBjbGFzcz1cIiR7aG92ZXJFZmZlY3R9XCI+IEFwcHJvdmFsIFN0YXR1cyA8L3RoPlxuICAgICAgICAgICAgICAgICAgICA8dGggY2xhc3M9XCIke2hvdmVyRWZmZWN0fVwiPiBBY3Rpb25zIDwvdGg+XG4gICAgICAgICAgICAgICAgPC90cj5cbiAgICAgICAgICAgICAgICA8L3RoZWFkPlxuXG4gICAgICAgICAgICAgICAgPHRib2R5IGNsYXNzPVwidGJvZHktdG9wLWJvcmRlclwiPlxuICAgICAgICAgICAgICAgICAgICAke2N1cnJQYWdlUHJvZHVjdHMubWFwKHAgPT4gcHJvZHVjdFRhYmxlUm93VGVtcGxhdGUocCwgY29udGV4dCkpfVxuICAgICAgICAgICAgICAgIDwvdGJvZHk+XG4gICAgICAgICAgICA8L3RhYmxlPlxuXG4gICAgICAgICAgICA8ZGl2IGNsYXNzPVwiZC1mbGV4IGp1c3RpZnktY29udGVudC1lbmQgYWxpZ24taXRlbXMtY2VudGVyIGdhcC0yIHBvc2l0aW9uLXJlbGF0aXZlIGJvdHRvbS0wIGVuZC0wXCI+XG4gICAgICAgICAgICAgICAgJHthd2FpdCBjb250cm9sc1RlbXBsYXRlKGNvbnRleHQpfVxuICAgICAgICAgICAgPC9kaXY+XG5cbiAgICAgICAgPC9kaXY+XG4gICAgYDtcbn1cblxuZnVuY3Rpb24gc2VhcmNoRm9ybVRlbXBsYXRlKGNvbnRleHQ6IFRhYmxlQ29udGV4dCkge1xuICAgIGNvbnN0IGRpc3BsYXlDbGFzcyA9IGNvbnRleHQuZ2V0Q3VyclNlYXJjaFF1ZXJ5KCkudHJpbSgpID09PSBcIlwiID8gXCJkLW5vbmVcIiA6IFwiXCI7XG5cbiAgICByZXR1cm4gaHRtbGBcbiAgICAgICAgPGRpdiBjbGFzcz1cImQtZmxleCBmbGV4LWNvbHVtbiBhbGlnbi1pdGVtcy1jZW50ZXIgbXQtNSBtYi0zXCIgaWQ9XCJzZWFyY2gtc2VjdGlvbi13cmFwcGVyXCI+XG4gICAgICAgICAgICA8bGFiZWwgZm9yPVwic2VhcmNoSW5wdXRcIiBjbGFzcz1cImZvcm0tbGFiZWwgdGV4dC1uYXZ5IHRleHQtY2VudGVyXCI+XG4gICAgICAgICAgICAgICAgU2VhcmNoIGZvciBwcm9kdWN0c1xuICAgICAgICAgICAgPC9sYWJlbD5cbiAgICAgICAgICAgIDxmb3JtIGlkPVwic2VhcmNoRm9ybVwiIGNsYXNzPVwibXgtbWQtM1wiXG4gICAgICAgICAgICAgICAgICBAc3VibWl0PSR7YXN5bmMgKGV2ZW50OiBFdmVudCkgPT4gYXdhaXQgb25TZWFyY2hGb3JtU3VibWl0SGFuZGxlcihldmVudCwgY29udGV4dCl9PlxuICAgICAgICAgICAgICAgIDxkaXYgY2xhc3M9XCJpbnB1dC1ncm91cC1zbSBkLWZsZXggZ2FwLTFcIj5cbiAgICAgICAgICAgICAgICAgICAgPGlucHV0IG5hbWU9XCJzZWFyY2hcIiB0eXBlPVwic2VhcmNoXCIgaWQ9XCJzZWFyY2hJbnB1dFwiXG4gICAgICAgICAgICAgICAgICAgICAgICAgICBjbGFzcz1cImZvcm0tY29udHJvbFwiIHBsYWNlaG9sZGVyPVwiU2VhcmNoLi4uXCJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgIGFyaWEtbGFiZWw9XCJTZWFyY2hcIiAudmFsdWU9JHtjb250ZXh0LmdldEN1cnJTZWFyY2hRdWVyeSgpfSAvPlxuICAgICAgICAgICAgICAgICAgICA8YnV0dG9uIGNsYXNzPVwiYnRuIGJ0bi1vdXRsaW5lLXRlYWxcIiB0eXBlPVwic3VibWl0XCI+U2VhcmNoPC9idXR0b24+XG4gICAgICAgICAgICAgICAgPC9kaXY+XG4gICAgICAgICAgICA8L2Zvcm0+XG4gICAgICAgICAgICA8ZGl2IGNsYXNzPVwibXktMiBwb3NpdGlvbi1yZWxhdGl2ZSAke2Rpc3BsYXlDbGFzc31cIiBzdHlsZT1cInJpZ2h0OiA1cHhcIj5cbiAgICAgICAgICAgICAgICA8c3Bhbj5yZXN1bHRzIGZvcjogJHtjb250ZXh0LmdldEN1cnJTZWFyY2hRdWVyeSgpfTwvc3Bhbj5cbiAgICAgICAgICAgICAgICA8YT5cbiAgICAgICAgICAgICAgICAgICAgPHNwYW4gY2xhc3M9XCJidG4tc20gYnRuLWRhbmdlclwiXG4gICAgICAgICAgICAgICAgICAgICAgICAgIEBjbGljaz0ke2FzeW5jICgpID0+IGF3YWl0IG9uQ2xlYXJTZWFyY2hGb3JtKGNvbnRleHQpfT5cbiAgICAgICAgICAgICAgICAgICAgICAgIHhcbiAgICAgICAgICAgICAgICAgICAgPC9zcGFuPlxuICAgICAgICAgICAgICAgIDwvYT5cbiAgICAgICAgICAgIDwvZGl2PlxuICAgICAgICA8L2Rpdj5gO1xufVxuXG5hc3luYyBmdW5jdGlvbiBvbkNsZWFyU2VhcmNoRm9ybShjb250ZXh0OiBUYWJsZUNvbnRleHQpIHtcbiAgICBjb250ZXh0LnNldFNlYXJjaFF1ZXJ5KFwiXCIpO1xuICAgIGF3YWl0IHNob3dQcm9kdWN0c1RhYmxlKGNvbnRleHQpO1xufVxuXG5hc3luYyBmdW5jdGlvbiBvblNlYXJjaEZvcm1TdWJtaXRIYW5kbGVyKGV2ZW50OiBFdmVudCwgY29udGV4dDogVGFibGVDb250ZXh0KSB7XG4gICAgZXZlbnQucHJldmVudERlZmF1bHQoKTtcblxuICAgIGNvbnN0IGZvcm1EYXRhID0gbmV3IEZvcm1EYXRhKGV2ZW50LmN1cnJlbnRUYXJnZXQgYXMgSFRNTEZvcm1FbGVtZW50KTtcbiAgICBjb25zdCBzZWFyY2hRdWVyeSA9IGZvcm1EYXRhLmdldChcInNlYXJjaFwiKSBhcyBzdHJpbmc7XG4gICAgaWYoc2VhcmNoUXVlcnkudHJpbSgpID09PSBcIlwiICYmIGNvbnRleHQuZ2V0Q3VyclNlYXJjaFF1ZXJ5KCkgPT09IFwiXCIpIHtcbiAgICAgICAgcmV0dXJuO1xuICAgIH1cblxuICAgIGNvbnRleHQuc2V0U2VhcmNoUXVlcnkoc2VhcmNoUXVlcnkpO1xuICAgIGF3YWl0IHNob3dQcm9kdWN0c1RhYmxlKGNvbnRleHQpO1xufVxuXG5hc3luYyBmdW5jdGlvbiBjb250cm9sc1RlbXBsYXRlKGNvbnRleHQ6IFRhYmxlQ29udGV4dCkge1xuICAgIGNvbnN0IHRvdGFsUGFnZXNDb3VudCA9IGNvbnRleHQuZ2V0UGFnZXNUb3RhbENvdW50KCk7XG4gICAgY29uc3QgY3VyclBhZ2VOdW1iZXIgPSBjb250ZXh0LmdldEN1cnJQYWdlTnVtYmVyKCk7XG4gICAgY29uc3QgdG90YWxJdGVtc0NvdW50ID0gY29udGV4dC5nZXRQcm9kdWN0c0NvdW50KCk7XG4gICAgY29uc3QgaXRlbXNDb3VudE9uUGFnZSA9IGNvbnRleHQuZ2V0Q3Vyckl0ZW1zT25QYWdlQ291bnQoKTtcblxuICAgIGNvbnN0IHBhZ2VOdW1iZXJzT25TY3JlZW4gPSBjYWxjdWxhdGVQYWdlTnVtYmVycyhjdXJyUGFnZU51bWJlciwgdG90YWxQYWdlc0NvdW50KVxuXG4gICAgY29uc3QgW2ZpcnN0SXRlbU51bU9uUGFnZSwgbGFzdEl0ZW1OdW1PblBhZ2VdXG4gICAgICAgID0gY2FsY3VsYXRlSXRlbXNOdW1iZXJzKGN1cnJQYWdlTnVtYmVyLCB0b3RhbEl0ZW1zQ291bnQsIGl0ZW1zQ291bnRPblBhZ2UpO1xuXG4gICAgcmV0dXJuIGh0bWxgXG4gICAgICAgIDxwIGNsYXNzPVwidGV4dC1uYXZ5IHRleHQtbXV0ZWQgZnMtNiBmc3QtaXRhbGljIGQtaW5saW5lXCI+XG4gICAgICAgICAgICAke2ZpcnN0SXRlbU51bU9uUGFnZX0tJHtsYXN0SXRlbU51bU9uUGFnZX0gZnJvbSAke3RvdGFsSXRlbXNDb3VudH1cbiAgICAgICAgPC9wPlxuICAgICAgICA8bmF2IGFyaWEtbGFiZWw9XCJUYWJsZSBwYWdpbmF0aW9uIGNvbnRyb2wuXCI+XG4gICAgICAgICAgICA8dWwgY2xhc3M9XCJwYWdpbmF0aW9uIGp1c3RpZnktY29udGVudC1jZW50ZXJcIj5cbiAgICAgICAgICAgICAgICAke2N1cnJQYWdlTnVtYmVyIDw9IDFcbiAgICAgICAgICAgICAgICAgICAgICAgID8gaHRtbGBcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICA8bGkgY2xhc3M9XCJwYWdlLWl0ZW0gZGlzYWJsZWRcIj5cbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgPHNwYW4gY2xhc3M9XCJwYWdlLWxpbmtcIj5QcmV2aW91czwvc3Bhbj5cbiAgICAgICAgICAgICAgICAgICAgICAgICAgICA8L2xpPmBcbiAgICAgICAgICAgICAgICAgICAgICAgIDogaHRtbGBcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICA8bGkgY2xhc3M9XCJwYWdlLWl0ZW1cIj5cbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgPGEgY2xhc3M9XCJwYWdlLWxpbmtcIlxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICBAY2xpY2s9JHthc3luYyAoKSA9PiBhd2FpdCBvblBhZ2VOdW1CdG5DbGljayhjb250ZXh0LCBjdXJyUGFnZU51bWJlciAtIDEpfT5cbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIFByZXZpb3VzXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIDwvYT5cbiAgICAgICAgICAgICAgICAgICAgICAgICAgICA8L2xpPmBcbiAgICAgICAgICAgICAgICB9XG5cbiAgICAgICAgICAgICAgICAke3BhZ2VOdW1iZXJzT25TY3JlZW4ubWFwKHBhZ2VOdW0gPT4ge1xuICAgICAgICAgICAgICAgICAgICByZXR1cm4gcGFnZU51bSA9PT0gY3VyclBhZ2VOdW1iZXJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICA/IGh0bWwgYFxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICA8bGkgY2xhc3M9XCJwYWdlLWl0ZW0gYWN0aXZlXCI+XG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIDxzcGFuIGNsYXNzPVwicGFnZS1saW5rXCI+XG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAke2N1cnJQYWdlTnVtYmVyfVxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICA8L3NwYW4+XG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIDwvbGk+YFxuICAgICAgICAgICAgICAgICAgICAgICAgICAgIDogaHRtbCBgXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIDxsaSBjbGFzcz1cInBhZ2UtaXRlbVwiPlxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgPGEgY2xhc3M9XCJwYWdlLWxpbmtcIlxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgQGNsaWNrPSR7YXN5bmMgKCkgPT4gYXdhaXQgb25QYWdlTnVtQnRuQ2xpY2soY29udGV4dCwgcGFnZU51bSl9PlxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICR7cGFnZU51bX1cbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIDwvYT5cbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgPC9saT5gXG4gICAgICAgICAgICAgICAgfSl9XG5cbiAgICAgICAgICAgICAgICAke2N1cnJQYWdlTnVtYmVyID09PSBwYWdlTnVtYmVyc09uU2NyZWVuLmxlbmd0aFxuICAgICAgICAgICAgICAgICAgICAgICAgPyBodG1sYFxuICAgICAgICAgICAgICAgICAgICAgICAgICAgIDxsaSBjbGFzcz1cInBhZ2UtaXRlbSBkaXNhYmxlZFwiPlxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICA8c3BhbiBjbGFzcz1cInBhZ2UtbGlua1wiPk5leHQ8L3NwYW4+XG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgPC9saT5gXG4gICAgICAgICAgICAgICAgICAgICAgICA6IGh0bWxgXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgPGxpIGNsYXNzPVwicGFnZS1pdGVtXCI+XG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIDxhIGNsYXNzPVwicGFnZS1saW5rXCJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgQGNsaWNrPSR7YXN5bmMgKCkgPT4gYXdhaXQgb25QYWdlTnVtQnRuQ2xpY2soY29udGV4dCwgY3VyclBhZ2VOdW1iZXIgKyAxKX0+XG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICBOZXh0XG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIDwvYT5cbiAgICAgICAgICAgICAgICAgICAgICAgICAgICA8L2xpPmBcbiAgICAgICAgICAgICAgICB9XG4gICAgICAgICAgICA8L3VsPlxuICAgICAgICA8L25hdj5gO1xufVxuXG5hc3luYyBmdW5jdGlvbiBvblBhZ2VOdW1CdG5DbGljayhjb250ZXh0OiBUYWJsZUNvbnRleHQsIHBhZ2VOdW1iZXI6IG51bWJlcikge1xuICAgIGNvbnRleHQuc2V0UGFnZU51bWJlcihwYWdlTnVtYmVyKTtcbiAgICBhd2FpdCBzaG93UHJvZHVjdHNUYWJsZShjb250ZXh0KTtcbn1cblxuZnVuY3Rpb24gcHJvZHVjdFRhYmxlUm93VGVtcGxhdGUocHJvZHVjdDogUHJvZHVjdCwgY29udGV4dDogVGFibGVDb250ZXh0KSB7XG4gICAgY29uc3QgYXBwcm92YWxTdGF0dXNUZE1hcDogUmVjb3JkPFByb2R1Y3RzQXBwcm92YWxTdGF0dXMsICgpID0+IHJlYWRvbmx5IFtkb3Q6IHN0cmluZywgY29udGVudDogc3RyaW5nLCBzdHlsZXM6IHN0cmluZ10+ID0ge1xuICAgICAgICBcIkFwcHJvdmVkXCI6ICgpID0+IFtcIvCfn6JcIiwgXCJBcHByb3ZlZFwiLCBcInRleHQtc3VjY2VzcyBmdy1zZW1pYm9sZFwiXSxcbiAgICAgICAgXCJXYWl0aW5nQXBwcm92YWxcIjogKCkgPT4gW1wi8J+foVwiLCBcIldhaXRpbmcgQXBwcm92YWxcIiwgXCJ0ZXh0LXdhcm5pbmcgZnctc2VtaWJvbGRcIl0sXG4gICAgICAgIFwiRGlzYXBwcm92ZWRcIjogKCkgPT4gW1wi8J+UtFwiLCBcIkRpc2FwcHJvdmVkXCIsIFwidGV4dC1kYW5nZXIgZnctc2VtaWJvbGRcIl0sXG4gICAgfTtcblxuICAgIGNvbnN0IFtkb3QsIGNvbnRlbnQsIHN0eWxlc10gPSBhcHByb3ZhbFN0YXR1c1RkTWFwW3Byb2R1Y3QhLmFwcHJvdmFsU3RhdHVzXSEoKTtcblxuICAgIHJldHVybiBodG1sYFxuICAgICAgICA8dHIgY2xhc3M9XCJ0ZXh0LWNlbnRlciBhbGlnbi1taWRkbGVcIj5cbiAgICAgICAgICAgIDx0ZD4ke3Byb2R1Y3QubmFtZX08L3RkPlxuICAgICAgICAgICAgPHRkPiR7cHJvZHVjdC5vd25lck5hbWV9PC90ZD5cbiAgICAgICAgICAgIDx0ZD4ke3Byb2R1Y3QuY2F0ZWdvcnlOYW1lfTwvdGQ+XG4gICAgICAgICAgICA8dGQgY2xhc3M9XCIke3N0eWxlc31cIj4ke2RvdH0gJHtjb250ZW50fTwvdGQ+XG4gICAgICAgICAgICA8dGQ+XG4gICAgICAgICAgICAgICAgPGRpdiBjbGFzcz1cImJ0bi1ncm91cC1zbSBkLWZsZXggZmxleC13cmFwIGp1c3RpZnktY29udGVudC1jZW50ZXIgZ2FwLTEgZ2FwLXNtLTIgZ2FwLW1kLTJcIj5cbiAgICAgICAgICAgICAgICAgICAgPGJ1dHRvbiBjbGFzcz1cImJ0biByb3VuZGVkLXBpbGwgYnRuLXRlYWwgYnRuLXNtIHctMTAwXCIgc3R5bGU9XCJtYXgtd2lkdGg6IDEyZW1cIlxuICAgICAgICAgICAgICAgICAgICAgICAgICAgIEBjbGljaz0ke2FzeW5jICgpID0+IGF3YWl0IG9uVmlld1Byb2R1Y3REZXRhaWxzSGFuZGxlcihwcm9kdWN0LmlkLCBjb250ZXh0KX0+XG4gICAgICAgICAgICAgICAgICAgICAgICBWaWV3IERldGFpbHNcbiAgICAgICAgICAgICAgICAgICAgPC9idXR0b24+XG5cbiAgICAgICAgICAgICAgICAgICAgPGJ1dHRvbiBjbGFzcz1cImJ0biByb3VuZGVkLXBpbGwgYnRuLW91dGxpbmUtZGFuZ2VyIGJ0bi1zbSB3LTEwMFwiIHN0eWxlPVwibWF4LXdpZHRoOiAxMmVtXCJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICBAY2xpY2s9JHthc3luYyAoKSA9PiBhd2FpdCBvblJlbW92ZVByb2R1Y3RIYW5kbGVyKHByb2R1Y3QsIGNvbnRleHQpfT5cbiAgICAgICAgICAgICAgICAgICAgICAgIFJlbW92ZSBQcm9kdWN0XG4gICAgICAgICAgICAgICAgICAgIDwvYnV0dG9uPlxuICAgICAgICAgICAgICAgIDwvZGl2PlxuICAgICAgICAgICAgPC90ZD5cbiAgICAgICAgPC90cj5cbiAgICBgO1xufVxuXG5hc3luYyBmdW5jdGlvbiBvblZpZXdQcm9kdWN0RGV0YWlsc0hhbmRsZXIocHJvZHVjdElkOiBzdHJpbmcsIGNvbnRleHQ6IFRhYmxlQ29udGV4dCkge1xuICAgIHJlbmRlcihhd2FpdCBnZXRQcm9kdWN0RGV0YWlsc01vZGFsVGVtcGxhdGUocHJvZHVjdElkLCBjb250ZXh0KSwgZGlhbG9nc1NlY3Rpb24hKTtcblxuICAgIGNvbnN0IHByb2R1Y3REZXRhaWxzTW9kYWxJZCA9IGBwcm9kdWN0LWRldGFpbHMtJHtwcm9kdWN0SWR9YDtcbiAgICBjb25zdCBtb2RhbEVsID0gZGlhbG9nc1NlY3Rpb24/LnF1ZXJ5U2VsZWN0b3I8SFRNTERpdkVsZW1lbnQ+KGBkaXYjJHtwcm9kdWN0RGV0YWlsc01vZGFsSWR9YCk7XG4gICAgY29uc3QgbW9kYWwgPSBNb2RhbC5nZXRPckNyZWF0ZUluc3RhbmNlKG1vZGFsRWwhKTtcblxuICAgIG1vZGFsRWw/LmFkZEV2ZW50TGlzdGVuZXIoJ2hpZGRlbi5icy5tb2RhbCcsICgpID0+IHtcbiAgICAgICAgcmVuZGVyKGh0bWxgYCwgZGlhbG9nc1NlY3Rpb24hKTtcbiAgICAgICAgbW9kYWwuZGlzcG9zZSgpO1xuICAgIH0sIHsgb25jZTogdHJ1ZSB9KTtcblxuICAgIG1vZGFsLnNob3coKTtcbn1cblxuYXN5bmMgZnVuY3Rpb24gb25SZW1vdmVQcm9kdWN0SGFuZGxlcihwcm9kdWN0OiBQcm9kdWN0LCBjb250ZXh0OiBUYWJsZUNvbnRleHQpIHtcbiAgICByZW5kZXIocmVtb3ZlUHJvZHVjdFRlbXBsYXRlKHByb2R1Y3QsIGNvbnRleHQpLCBkaWFsb2dzU2VjdGlvbiEpO1xuXG4gICAgY29uc3QgZGVsZXRlUHJvZHVjdE1vZGFsSWQgPSBgcmVtb3ZlLXByb2R1Y3QtJHtwcm9kdWN0LmlkfWA7XG4gICAgY29uc3QgbW9kYWxFbCA9IGRpYWxvZ3NTZWN0aW9uPy5xdWVyeVNlbGVjdG9yPEhUTUxEaXZFbGVtZW50PihgZGl2IyR7ZGVsZXRlUHJvZHVjdE1vZGFsSWR9YClcbiAgICBjb25zdCBtb2RhbCA9IE1vZGFsLmdldE9yQ3JlYXRlSW5zdGFuY2UobW9kYWxFbCEpO1xuXG4gICAgbW9kYWxFbD8uYWRkRXZlbnRMaXN0ZW5lcignaGlkZGVuLmJzLm1vZGFsJywgKCkgPT4ge1xuICAgICAgICByZW5kZXIoaHRtbGBgLCBkaWFsb2dzU2VjdGlvbiEpO1xuICAgICAgICBtb2RhbC5kaXNwb3NlKCk7XG4gICAgfSwgeyBvbmNlOiB0cnVlIH0pO1xuXG4gICAgbW9kYWwuc2hvdygpO1xufVxuXG5mdW5jdGlvbiBjYWxjdWxhdGVQYWdlTnVtYmVycyhjdXJyUGFnZU51bWJlcjogbnVtYmVyLCB0b3RhbFBhZ2VzQ291bnQ6IG51bWJlcikge1xuICAgIGNvbnN0IGZpcnN0UGFnZU51bWJlciA9IE1hdGgubWF4KDEsIGN1cnJQYWdlTnVtYmVyIC0gMyk7XG4gICAgY29uc3QgbGFzdFBhZ2VOdW1iZXIgPSBNYXRoLm1pbih0b3RhbFBhZ2VzQ291bnQsIGN1cnJQYWdlTnVtYmVyICsgMyk7XG5cbiAgICBjb25zdCBwYWdlTnVtYmVyc09uU2NyZWVuID0gW107XG4gICAgZm9yKGxldCBpID0gZmlyc3RQYWdlTnVtYmVyOyBpIDw9IGxhc3RQYWdlTnVtYmVyOyBpKyspIHtcbiAgICAgICAgcGFnZU51bWJlcnNPblNjcmVlbi5wdXNoKGkpO1xuICAgIH1cblxuICAgIHJldHVybiBwYWdlTnVtYmVyc09uU2NyZWVuO1xufVxuXG5mdW5jdGlvbiBjYWxjdWxhdGVJdGVtc051bWJlcnMoXG4gICAgY3VyclBhZ2VOdW1iZXI6IG51bWJlcixcbiAgICB0b3RhbEl0ZW1zQ291bnQ6IG51bWJlcixcbiAgICBpdGVtc0NvdW50T25QYWdlOiBudW1iZXJcbik6IHJlYWRvbmx5IFtmaXJzdEl0ZW1OdW1PblBhZ2U6IG51bWJlciwgbGFzdEl0ZW1OdW1PblBhZ2U6IG51bWJlcl0ge1xuICAgIGlmICh0b3RhbEl0ZW1zQ291bnQgPT09IDApIHtcbiAgICAgICAgcmV0dXJuIFswLCAwXTtcbiAgICB9XG5cbiAgICBjb25zdCBsYXN0SXRlbU51bU9uUGFnZSA9IE1hdGgubWluKGN1cnJQYWdlTnVtYmVyICogaXRlbXNDb3VudE9uUGFnZSwgdG90YWxJdGVtc0NvdW50KTtcbiAgICBjb25zdCBmaXJzdEl0ZW1OdW1PblBhZ2UgPSAoY3VyclBhZ2VOdW1iZXIgLSAxKSAqIGl0ZW1zQ291bnRPblBhZ2UgKyAxO1xuXG4gICAgcmV0dXJuIFtmaXJzdEl0ZW1OdW1PblBhZ2UsIGxhc3RJdGVtTnVtT25QYWdlXTtcbn1cbiJdfQ==