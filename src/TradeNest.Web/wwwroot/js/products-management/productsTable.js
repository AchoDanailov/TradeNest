import bootstrap from "bootstrap";
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
    const [firstItemNumOnPage, lastItemNumOnPage] = calculateItemsNumbers(currPageNumber, totalPagesCount, totalItemsCount, itemsCountOnPage);
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
    const modal = bootstrap.Modal.getOrCreateInstance(modalEl);
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
    const modal = bootstrap.Modal.getOrCreateInstance(modalEl);
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
function calculateItemsNumbers(currPageNumber, totalPagesCount, totalItemsCount, itemsCountOnPage) {
    const lastItemNumOnPage = Math.min(currPageNumber * itemsCountOnPage, totalItemsCount);
    let firstItemNumOnPage = lastItemNumOnPage - itemsCountOnPage + 1;
    if (currPageNumber === totalPagesCount) {
        if (totalItemsCount === 0)
            firstItemNumOnPage = 0;
        else if (currPageNumber === 1 && totalItemsCount !== 0)
            firstItemNumOnPage = 1;
        else
            firstItemNumOnPage = lastItemNumOnPage - (lastItemNumOnPage % itemsCountOnPage) + 1;
    }
    return [firstItemNumOnPage, lastItemNumOnPage];
}
//# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJmaWxlIjoicHJvZHVjdHNUYWJsZS5qcyIsInNvdXJjZVJvb3QiOiIiLCJzb3VyY2VzIjpbIi4uLy4uLy4uL0Zyb250RW5kU2NyaXB0cy9wcm9kdWN0cy1tYW5hZ2VtZW50L3Byb2R1Y3RzVGFibGUudHMiXSwibmFtZXMiOltdLCJtYXBwaW5ncyI6IkFBQUEsT0FBTyxTQUFTLE1BQU0sV0FBVyxDQUFDO0FBQ2xDLE9BQU8sRUFBRSxJQUFJLEVBQUUsTUFBTSxFQUF1QixNQUFNLFVBQVUsQ0FBQztBQUs3RCxPQUFPLHFCQUFxQixNQUFNLHNDQUFzQyxDQUFDO0FBQ3pFLE9BQU8sOEJBQThCLE1BQU0sa0NBQWtDLENBQUM7QUFHOUUsTUFBTSxpQkFBaUIsR0FBRyxRQUFRLENBQUMsYUFBYSxDQUFpQixxQkFBcUIsQ0FBQyxDQUFDO0FBQ3hGLE1BQU0sY0FBYyxHQUFHLFFBQVEsQ0FBQyxhQUFhLENBQWlCLHFCQUFxQixDQUFDLENBQUM7QUFFckYsTUFBTSxDQUFDLE9BQU8sQ0FBQyxLQUFLLFVBQVUsaUJBQWlCLENBQUMsT0FBcUI7SUFDakUsTUFBTSxnQkFBZ0IsR0FBRyxNQUFNLE9BQU8sQ0FBQyxtQkFBbUIsRUFBRSxDQUFDO0lBQzdELE1BQU0sQ0FBQyxNQUFNLFFBQVEsQ0FBQyxnQkFBZ0IsRUFBRSxPQUFPLENBQUMsRUFBRSxpQkFBa0IsQ0FBQyxDQUFDO0FBQzFFLENBQUM7QUFFRCxLQUFLLFVBQVUsUUFBUSxDQUNuQixnQkFBMkIsRUFDM0IsT0FBcUI7SUFFckIsTUFBTSxXQUFXLEdBQUcsMkNBQTJDLENBQUE7SUFFL0QsT0FBTyxJQUFJLENBQUE7O2NBRUQsa0JBQWtCLENBQUMsT0FBTyxDQUFDOzs7Ozs7O2lDQU9SLFdBQVc7aUNBQ1gsV0FBVztpQ0FDWCxXQUFXO2lDQUNYLFdBQVc7aUNBQ1gsV0FBVzs7Ozs7c0JBS3RCLGdCQUFnQixDQUFDLEdBQUcsQ0FBQyxDQUFDLENBQUMsRUFBRSxDQUFDLHVCQUF1QixDQUFDLENBQUMsRUFBRSxPQUFPLENBQUMsQ0FBQzs7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QUUsQ0FBQztRQUNyQyxJQUFJLGVBQWUsS0FBSyxDQUFDO1lBQ3JCLGtCQUFrQixHQUFHLENBQUMsQ0FBQzthQUN0QixJQUFJLGNBQWMsS0FBSyxDQUFDLElBQUksZUFBZSxLQUFLLENBQUM7WUFDbEQsa0JBQWtCLEdBQUcsQ0FBQyxDQUFDOztZQUV2QixrQkFBa0IsR0FBRyxpQkFBaUIsR0FBRyxDQUFDLGlCQUFpQixHQUFHLGdCQUFnQixDQUFDLEdBQUcsQ0FBQyxDQUFDO0lBQzVGLENBQUM7SUFFRCxPQUFPLENBQUMsa0JBQWtCLEVBQUUsaUJBQWlCLENBQUMsQ0FBQztBQUNuRCxDQUFDIiwic291cmNlc0NvbnRlbnQiOlsiaW1wb3J0IGJvb3RzdHJhcCBmcm9tIFwiYm9vdHN0cmFwXCI7XG5pbXBvcnQgeyBodG1sLCByZW5kZXIsIHR5cGUgVGVtcGxhdGVSZXN1bHQgfSBmcm9tIFwibGl0LWh0bWxcIjtcblxuaW1wb3J0IHR5cGUgeyBQcm9kdWN0LCBQcm9kdWN0c0FwcHJvdmFsU3RhdHVzIH0gZnJvbSBcIi4uL3R5cGVzL3Byb2R1Y3RzLnRzXCI7XG5pbXBvcnQgdHlwZSB7IFRhYmxlQ29udGV4dCB9IGZyb20gXCIuLi90eXBlcy90YWJsZUNvbnRleHQudHNcIjtcblxuaW1wb3J0IHJlbW92ZVByb2R1Y3RUZW1wbGF0ZSBmcm9tIFwiLi9jb25maXJtUmVtb3ZlUHJvZHVjdERpYWxvZ01vZGFsLmpzXCI7XG5pbXBvcnQgZ2V0UHJvZHVjdERldGFpbHNNb2RhbFRlbXBsYXRlIGZyb20gXCIuL3Byb2R1Y3REZXRhaWxzTW9kYWxUZW1wbGF0ZS5qc1wiO1xuXG5cbmNvbnN0IHRhYmxlRGl2Q29udGFpbmVyID0gZG9jdW1lbnQucXVlcnlTZWxlY3RvcjxIVE1MRGl2RWxlbWVudD4oXCJkaXYjdGFibGUtY29udGFpbmVyXCIpO1xuY29uc3QgZGlhbG9nc1NlY3Rpb24gPSBkb2N1bWVudC5xdWVyeVNlbGVjdG9yPEhUTUxEaXZFbGVtZW50PihcImRpdiNkaWFsb2dzLXNlY3Rpb25cIik7XG5cbmV4cG9ydCBkZWZhdWx0IGFzeW5jIGZ1bmN0aW9uIHNob3dQcm9kdWN0c1RhYmxlKGNvbnRleHQ6IFRhYmxlQ29udGV4dCkge1xuICAgIGNvbnN0IGN1cnJQYWdlUHJvZHVjdHMgPSBhd2FpdCBjb250ZXh0LmdldEN1cnJQYWdlUHJvZHVjdHMoKTtcbiAgICByZW5kZXIoYXdhaXQgdGVtcGxhdGUoY3VyclBhZ2VQcm9kdWN0cywgY29udGV4dCksIHRhYmxlRGl2Q29udGFpbmVyISk7XG59XG5cbmFzeW5jIGZ1bmN0aW9uIHRlbXBsYXRlKFxuICAgIGN1cnJQYWdlUHJvZHVjdHM6IFByb2R1Y3RbXSxcbiAgICBjb250ZXh0OiBUYWJsZUNvbnRleHRcbik6IFByb21pc2U8VGVtcGxhdGVSZXN1bHQ+IHtcbiAgICBjb25zdCBob3ZlckVmZmVjdCA9IFwibmF2LWxpbmstYm9yZGVyLXJhZGl1cy1ob3Zlci1lZmZlY3QtbGlnaHRcIlxuXG4gICAgcmV0dXJuIGh0bWxgXG4gICAgICAgIDxkaXY+XG4gICAgICAgICAgICAke3NlYXJjaEZvcm1UZW1wbGF0ZShjb250ZXh0KX1cbiAgICAgICAgPC9kaXY+XG5cbiAgICAgICAgPGRpdiBpZD1cInRhYmxlLXdyYXBwZXJcIiBjbGFzcz1cIm10LTAgcHQtMCB3LTEwMFwiPlxuICAgICAgICAgICAgPHRhYmxlIGNsYXNzPVwidGFibGUgdGFibGUtaG92ZXIgdy0xMDBcIj5cbiAgICAgICAgICAgICAgICA8dGhlYWQgY2xhc3M9XCJzaXRlLXNlY3Rpb25zLWJnLXRlYWwgdGV4dC1jZW50ZXJcIj5cbiAgICAgICAgICAgICAgICA8dHIgY2xhc3M9XCJhbGlnbi1taWRkbGVcIj5cbiAgICAgICAgICAgICAgICAgICAgPHRoIGNsYXNzPVwiJHtob3ZlckVmZmVjdH1cIj4gUHJvZHVjdCA8L3RoPlxuICAgICAgICAgICAgICAgICAgICA8dGggY2xhc3M9XCIke2hvdmVyRWZmZWN0fVwiPiBPd25lciA8L3RoPlxuICAgICAgICAgICAgICAgICAgICA8dGggY2xhc3M9XCIke2hvdmVyRWZmZWN0fVwiPiBDYXRlZ29yeSBOYW1lIDwvdGg+XG4gICAgICAgICAgICAgICAgICAgIDx0aCBjbGFzcz1cIiR7aG92ZXJFZmZlY3R9XCI+IEFwcHJvdmFsIFN0YXR1cyA8L3RoPlxuICAgICAgICAgICAgICAgICAgICA8dGggY2xhc3M9XCIke2hvdmVyRWZmZWN0fVwiPiBBY3Rpb25zIDwvdGg+XG4gICAgICAgICAgICAgICAgPC90cj5cbiAgICAgICAgICAgICAgICA8L3RoZWFkPlxuXG4gICAgICAgICAgICAgICAgPHRib2R5IGNsYXNzPVwidGJvZHktdG9wLWJvcmRlclwiPlxuICAgICAgICAgICAgICAgICAgICAke2N1cnJQYWdlUHJvZHVjdHMubWFwKHAgPT4gcHJvZHVjdFRhYmxlUm93VGVtcGxhdGUocCwgY29udGV4dCkpfVxuICAgICAgICAgICAgICAgIDwvdGJvZHk+XG4gICAgICAgICAgICA8L3RhYmxlPlxuXG4gICAgICAgICAgICA8ZGl2IGNsYXNzPVwiZC1mbGV4IGp1c3RpZnktY29udGVudC1lbmQgYWxpZ24taXRlbXMtY2VudGVyIGdhcC0yIHBvc2l0aW9uLXJlbGF0aXZlIGJvdHRvbS0wIGVuZC0wXCI+XG4gICAgICAgICAgICAgICAgJHthd2FpdCBjb250cm9sc1RlbXBsYXRlKGNvbnRleHQpfVxuICAgICAgICAgICAgPC9kaXY+XG5cbiAgICAgICAgPC9kaXY+XG4gICAgYDtcbn1cblxuZnVuY3Rpb24gc2VhcmNoRm9ybVRlbXBsYXRlKGNvbnRleHQ6IFRhYmxlQ29udGV4dCkge1xuICAgIGNvbnN0IGRpc3BsYXlDbGFzcyA9IGNvbnRleHQuZ2V0Q3VyclNlYXJjaFF1ZXJ5KCkudHJpbSgpID09PSBcIlwiID8gXCJkLW5vbmVcIiA6IFwiXCI7XG5cbiAgICByZXR1cm4gaHRtbGBcbiAgICAgICAgPGRpdiBjbGFzcz1cImQtZmxleCBmbGV4LWNvbHVtbiBhbGlnbi1pdGVtcy1jZW50ZXIgbXQtNSBtYi0zXCIgaWQ9XCJzZWFyY2gtc2VjdGlvbi13cmFwcGVyXCI+XG4gICAgICAgICAgICA8bGFiZWwgZm9yPVwic2VhcmNoSW5wdXRcIiBjbGFzcz1cImZvcm0tbGFiZWwgdGV4dC1uYXZ5IHRleHQtY2VudGVyXCI+XG4gICAgICAgICAgICAgICAgU2VhcmNoIGZvciBwcm9kdWN0c1xuICAgICAgICAgICAgPC9sYWJlbD5cbiAgICAgICAgICAgIDxmb3JtIGlkPVwic2VhcmNoRm9ybVwiIGNsYXNzPVwibXgtbWQtM1wiXG4gICAgICAgICAgICAgICAgICBAc3VibWl0PSR7YXN5bmMgKGV2ZW50OiBFdmVudCkgPT4gYXdhaXQgb25TZWFyY2hGb3JtU3VibWl0SGFuZGxlcihldmVudCwgY29udGV4dCl9PlxuICAgICAgICAgICAgICAgIDxkaXYgY2xhc3M9XCJpbnB1dC1ncm91cC1zbSBkLWZsZXggZ2FwLTFcIj5cbiAgICAgICAgICAgICAgICAgICAgPGlucHV0IG5hbWU9XCJzZWFyY2hcIiB0eXBlPVwic2VhcmNoXCIgaWQ9XCJzZWFyY2hJbnB1dFwiXG4gICAgICAgICAgICAgICAgICAgICAgICAgICBjbGFzcz1cImZvcm0tY29udHJvbFwiIHBsYWNlaG9sZGVyPVwiU2VhcmNoLi4uXCJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgIGFyaWEtbGFiZWw9XCJTZWFyY2hcIiAudmFsdWU9JHtjb250ZXh0LmdldEN1cnJTZWFyY2hRdWVyeSgpfSAvPlxuICAgICAgICAgICAgICAgICAgICA8YnV0dG9uIGNsYXNzPVwiYnRuIGJ0bi1vdXRsaW5lLXRlYWxcIiB0eXBlPVwic3VibWl0XCI+U2VhcmNoPC9idXR0b24+XG4gICAgICAgICAgICAgICAgPC9kaXY+XG4gICAgICAgICAgICA8L2Zvcm0+XG4gICAgICAgICAgICA8ZGl2IGNsYXNzPVwibXktMiBwb3NpdGlvbi1yZWxhdGl2ZSAke2Rpc3BsYXlDbGFzc31cIiBzdHlsZT1cInJpZ2h0OiA1cHhcIj5cbiAgICAgICAgICAgICAgICA8c3Bhbj5yZXN1bHRzIGZvcjogJHtjb250ZXh0LmdldEN1cnJTZWFyY2hRdWVyeSgpfTwvc3Bhbj5cbiAgICAgICAgICAgICAgICA8YT5cbiAgICAgICAgICAgICAgICAgICAgPHNwYW4gY2xhc3M9XCJidG4tc20gYnRuLWRhbmdlclwiXG4gICAgICAgICAgICAgICAgICAgICAgICAgIEBjbGljaz0ke2FzeW5jICgpID0+IGF3YWl0IG9uQ2xlYXJTZWFyY2hGb3JtKGNvbnRleHQpfT5cbiAgICAgICAgICAgICAgICAgICAgICAgIHhcbiAgICAgICAgICAgICAgICAgICAgPC9zcGFuPlxuICAgICAgICAgICAgICAgIDwvYT5cbiAgICAgICAgICAgIDwvZGl2PlxuICAgICAgICA8L2Rpdj5gO1xufVxuXG5hc3luYyBmdW5jdGlvbiBvbkNsZWFyU2VhcmNoRm9ybShjb250ZXh0OiBUYWJsZUNvbnRleHQpIHtcbiAgICBjb250ZXh0LnNldFNlYXJjaFF1ZXJ5KFwiXCIpO1xuICAgIGF3YWl0IHNob3dQcm9kdWN0c1RhYmxlKGNvbnRleHQpO1xufVxuXG5hc3luYyBmdW5jdGlvbiBvblNlYXJjaEZvcm1TdWJtaXRIYW5kbGVyKGV2ZW50OiBFdmVudCwgY29udGV4dDogVGFibGVDb250ZXh0KSB7XG4gICAgZXZlbnQucHJldmVudERlZmF1bHQoKTtcblxuICAgIGNvbnN0IGZvcm1EYXRhID0gbmV3IEZvcm1EYXRhKGV2ZW50LmN1cnJlbnRUYXJnZXQgYXMgSFRNTEZvcm1FbGVtZW50IHwgdW5kZWZpbmVkKTtcbiAgICBjb25zdCBzZWFyY2hRdWVyeSA9IGZvcm1EYXRhLmdldChcInNlYXJjaFwiKSBhcyBGb3JtRGF0YUVudHJ5VmFsdWUgYXMgc3RyaW5nO1xuICAgIGlmKHNlYXJjaFF1ZXJ5LnRyaW0oKSA9PT0gXCJcIiAmJiBjb250ZXh0LmdldEN1cnJTZWFyY2hRdWVyeSgpID09PSBcIlwiKSB7XG4gICAgICAgIHJldHVybjtcbiAgICB9XG5cbiAgICBjb250ZXh0LnNldFNlYXJjaFF1ZXJ5KHNlYXJjaFF1ZXJ5KTtcbiAgICBhd2FpdCBzaG93UHJvZHVjdHNUYWJsZShjb250ZXh0KTtcbn1cblxuYXN5bmMgZnVuY3Rpb24gY29udHJvbHNUZW1wbGF0ZShjb250ZXh0OiBUYWJsZUNvbnRleHQpIHtcbiAgICBjb25zdCB0b3RhbFBhZ2VzQ291bnQgPSBjb250ZXh0LmdldFBhZ2VzVG90YWxDb3VudCgpO1xuICAgIGNvbnN0IGN1cnJQYWdlTnVtYmVyID0gY29udGV4dC5nZXRDdXJyUGFnZU51bWJlcigpO1xuICAgIGNvbnN0IHRvdGFsSXRlbXNDb3VudCA9IGNvbnRleHQuZ2V0UHJvZHVjdHNDb3VudCgpO1xuICAgIGNvbnN0IGl0ZW1zQ291bnRPblBhZ2UgPSBjb250ZXh0LmdldEN1cnJJdGVtc09uUGFnZUNvdW50KCk7XG5cbiAgICBjb25zdCBwYWdlTnVtYmVyc09uU2NyZWVuID0gY2FsY3VsYXRlUGFnZU51bWJlcnMoY3VyclBhZ2VOdW1iZXIsIHRvdGFsUGFnZXNDb3VudClcblxuICAgIGNvbnN0IFtmaXJzdEl0ZW1OdW1PblBhZ2UsIGxhc3RJdGVtTnVtT25QYWdlXVxuICAgICAgICA9IGNhbGN1bGF0ZUl0ZW1zTnVtYmVycyhjdXJyUGFnZU51bWJlciwgdG90YWxQYWdlc0NvdW50LCB0b3RhbEl0ZW1zQ291bnQsIGl0ZW1zQ291bnRPblBhZ2UpO1xuXG4gICAgcmV0dXJuIGh0bWxgXG4gICAgICAgIDxwIGNsYXNzPVwidGV4dC1uYXZ5IHRleHQtbXV0ZWQgZnMtNiBmc3QtaXRhbGljIGQtaW5saW5lXCI+XG4gICAgICAgICAgICAke2ZpcnN0SXRlbU51bU9uUGFnZX0tJHtsYXN0SXRlbU51bU9uUGFnZX0gZnJvbSAke3RvdGFsSXRlbXNDb3VudH1cbiAgICAgICAgPC9wPlxuICAgICAgICA8bmF2IGFyaWEtbGFiZWw9XCJUYWJsZSBwYWdpbmF0aW9uIGNvbnRyb2wuXCI+XG4gICAgICAgICAgICA8dWwgY2xhc3M9XCJwYWdpbmF0aW9uIGp1c3RpZnktY29udGVudC1jZW50ZXJcIj5cbiAgICAgICAgICAgICAgICAke2N1cnJQYWdlTnVtYmVyIDw9IDFcbiAgICAgICAgICAgICAgICAgICAgICAgID8gaHRtbGBcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICA8bGkgY2xhc3M9XCJwYWdlLWl0ZW0gZGlzYWJsZWRcIj5cbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgPHNwYW4gY2xhc3M9XCJwYWdlLWxpbmtcIj5QcmV2aW91czwvc3Bhbj5cbiAgICAgICAgICAgICAgICAgICAgICAgICAgICA8L2xpPmBcbiAgICAgICAgICAgICAgICAgICAgICAgIDogaHRtbGBcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICA8bGkgY2xhc3M9XCJwYWdlLWl0ZW1cIj5cbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgPGEgY2xhc3M9XCJwYWdlLWxpbmtcIlxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICBAY2xpY2s9JHthc3luYyAoKSA9PiBhd2FpdCBvblBhZ2VOdW1CdG5DbGljayhjb250ZXh0LCBjdXJyUGFnZU51bWJlciAtIDEpfT5cbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIFByZXZpb3VzXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIDwvYT5cbiAgICAgICAgICAgICAgICAgICAgICAgICAgICA8L2xpPmBcbiAgICAgICAgICAgICAgICB9XG5cbiAgICAgICAgICAgICAgICAke3BhZ2VOdW1iZXJzT25TY3JlZW4ubWFwKHBhZ2VOdW0gPT4ge1xuICAgICAgICAgICAgICAgICAgICByZXR1cm4gcGFnZU51bSA9PT0gY3VyclBhZ2VOdW1iZXJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICA/IGh0bWwgYFxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICA8bGkgY2xhc3M9XCJwYWdlLWl0ZW0gYWN0aXZlXCI+XG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIDxzcGFuIGNsYXNzPVwicGFnZS1saW5rXCI+XG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAke2N1cnJQYWdlTnVtYmVyfVxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICA8L3NwYW4+XG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIDwvbGk+YFxuICAgICAgICAgICAgICAgICAgICAgICAgICAgIDogaHRtbCBgXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIDxsaSBjbGFzcz1cInBhZ2UtaXRlbVwiPlxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgPGEgY2xhc3M9XCJwYWdlLWxpbmtcIlxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgQGNsaWNrPSR7YXN5bmMgKCkgPT4gYXdhaXQgb25QYWdlTnVtQnRuQ2xpY2soY29udGV4dCwgcGFnZU51bSl9PlxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICR7cGFnZU51bX1cbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIDwvYT5cbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgPC9saT5gXG4gICAgICAgICAgICAgICAgfSl9XG5cbiAgICAgICAgICAgICAgICAke2N1cnJQYWdlTnVtYmVyID09PSBwYWdlTnVtYmVyc09uU2NyZWVuLmxlbmd0aFxuICAgICAgICAgICAgICAgICAgICAgICAgPyBodG1sYFxuICAgICAgICAgICAgICAgICAgICAgICAgICAgIDxsaSBjbGFzcz1cInBhZ2UtaXRlbSBkaXNhYmxlZFwiPlxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICA8c3BhbiBjbGFzcz1cInBhZ2UtbGlua1wiPk5leHQ8L3NwYW4+XG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgPC9saT5gXG4gICAgICAgICAgICAgICAgICAgICAgICA6IGh0bWxgXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgPGxpIGNsYXNzPVwicGFnZS1pdGVtXCI+XG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIDxhIGNsYXNzPVwicGFnZS1saW5rXCJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgQGNsaWNrPSR7YXN5bmMgKCkgPT4gYXdhaXQgb25QYWdlTnVtQnRuQ2xpY2soY29udGV4dCwgY3VyclBhZ2VOdW1iZXIgKyAxKX0+XG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICBOZXh0XG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIDwvYT5cbiAgICAgICAgICAgICAgICAgICAgICAgICAgICA8L2xpPmBcbiAgICAgICAgICAgICAgICB9XG4gICAgICAgICAgICA8L3VsPlxuICAgICAgICA8L25hdj5gO1xufVxuXG5hc3luYyBmdW5jdGlvbiBvblBhZ2VOdW1CdG5DbGljayhjb250ZXh0OiBUYWJsZUNvbnRleHQsIHBhZ2VOdW1iZXI6IG51bWJlcikge1xuICAgIGNvbnRleHQuc2V0UGFnZU51bWJlcihwYWdlTnVtYmVyKTtcbiAgICBhd2FpdCBzaG93UHJvZHVjdHNUYWJsZShjb250ZXh0KTtcbn1cblxuZnVuY3Rpb24gcHJvZHVjdFRhYmxlUm93VGVtcGxhdGUocHJvZHVjdDogUHJvZHVjdCwgY29udGV4dDogVGFibGVDb250ZXh0KSB7XG4gICAgY29uc3QgYXBwcm92YWxTdGF0dXNUZE1hcCA9IHtcbiAgICAgICAgXCJBcHByb3ZlZFwiOiAoKSA9PiBbXCLwn5+iXCIsIFwiQXBwcm92ZWRcIiwgXCJ0ZXh0LXN1Y2Nlc3MgZnctc2VtaWJvbGRcIl0sXG4gICAgICAgIFwiV2FpdGluZ0FwcHJvdmFsXCI6ICgpID0+IFtcIvCfn6FcIiwgXCJXYWl0aW5nIEFwcHJvdmFsXCIsIFwidGV4dC13YXJuaW5nIGZ3LXNlbWlib2xkXCJdLFxuICAgICAgICBcIkRpc2FwcHJvdmVkXCI6ICgpID0+IFtcIvCflLRcIiwgXCJEaXNhcHByb3ZlZFwiLCBcInRleHQtZGFuZ2VyIGZ3LXNlbWlib2xkXCJdLFxuICAgIH0gYXMgUmVjb3JkPFByb2R1Y3RzQXBwcm92YWxTdGF0dXMsICgpID0+IHJlYWRvbmx5IFtkb3Q6IHN0cmluZywgY29udGVudDogc3RyaW5nLCBzdHlsZXM6IHN0cmluZ10+O1xuXG4gICAgY29uc3QgW2RvdCwgY29udGVudCwgc3R5bGVzXSA9IGFwcHJvdmFsU3RhdHVzVGRNYXBbcHJvZHVjdCEuYXBwcm92YWxTdGF0dXNdISgpO1xuXG4gICAgcmV0dXJuIGh0bWxgXG4gICAgICAgIDx0ciBjbGFzcz1cInRleHQtY2VudGVyIGFsaWduLW1pZGRsZVwiPlxuICAgICAgICAgICAgPHRkPiR7cHJvZHVjdC5uYW1lfTwvdGQ+XG4gICAgICAgICAgICA8dGQ+JHtwcm9kdWN0Lm93bmVyTmFtZX08L3RkPlxuICAgICAgICAgICAgPHRkPiR7cHJvZHVjdC5jYXRlZ29yeU5hbWV9PC90ZD5cbiAgICAgICAgICAgIDx0ZCBjbGFzcz1cIiR7c3R5bGVzfVwiPiR7ZG90fSAke2NvbnRlbnR9PC90ZD5cbiAgICAgICAgICAgIDx0ZD5cbiAgICAgICAgICAgICAgICA8ZGl2IGNsYXNzPVwiYnRuLWdyb3VwLXNtIGQtZmxleCBmbGV4LXdyYXAganVzdGlmeS1jb250ZW50LWNlbnRlciBnYXAtMSBnYXAtc20tMiBnYXAtbWQtMlwiPlxuICAgICAgICAgICAgICAgICAgICA8YnV0dG9uIGNsYXNzPVwiYnRuIHJvdW5kZWQtcGlsbCBidG4tdGVhbCBidG4tc20gdy0xMDBcIiBzdHlsZT1cIm1heC13aWR0aDogMTJlbVwiXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgQGNsaWNrPSR7YXN5bmMgKCkgPT4gYXdhaXQgb25WaWV3UHJvZHVjdERldGFpbHNIYW5kbGVyKHByb2R1Y3QuaWQsIGNvbnRleHQpfT5cbiAgICAgICAgICAgICAgICAgICAgICAgIFZpZXcgRGV0YWlsc1xuICAgICAgICAgICAgICAgICAgICA8L2J1dHRvbj5cblxuICAgICAgICAgICAgICAgICAgICA8YnV0dG9uIGNsYXNzPVwiYnRuIHJvdW5kZWQtcGlsbCBidG4tb3V0bGluZS1kYW5nZXIgYnRuLXNtIHctMTAwXCIgc3R5bGU9XCJtYXgtd2lkdGg6IDEyZW1cIlxuICAgICAgICAgICAgICAgICAgICAgICAgICAgIEBjbGljaz0ke2FzeW5jICgpID0+IGF3YWl0IG9uUmVtb3ZlUHJvZHVjdEhhbmRsZXIocHJvZHVjdCwgY29udGV4dCl9PlxuICAgICAgICAgICAgICAgICAgICAgICAgUmVtb3ZlIFByb2R1Y3RcbiAgICAgICAgICAgICAgICAgICAgPC9idXR0b24+XG4gICAgICAgICAgICAgICAgPC9kaXY+XG4gICAgICAgICAgICA8L3RkPlxuICAgICAgICA8L3RyPlxuICAgIGA7XG59XG5cbmFzeW5jIGZ1bmN0aW9uIG9uVmlld1Byb2R1Y3REZXRhaWxzSGFuZGxlcihwcm9kdWN0SWQ6IHN0cmluZywgY29udGV4dDogVGFibGVDb250ZXh0KSB7XG4gICAgcmVuZGVyKGF3YWl0IGdldFByb2R1Y3REZXRhaWxzTW9kYWxUZW1wbGF0ZShwcm9kdWN0SWQsIGNvbnRleHQpLCBkaWFsb2dzU2VjdGlvbiEpO1xuXG4gICAgY29uc3QgcHJvZHVjdERldGFpbHNNb2RhbElkID0gYHByb2R1Y3QtZGV0YWlscy0ke3Byb2R1Y3RJZH1gO1xuICAgIGNvbnN0IG1vZGFsRWwgPSBkaWFsb2dzU2VjdGlvbj8ucXVlcnlTZWxlY3RvcjxIVE1MRGl2RWxlbWVudD4oYGRpdiMke3Byb2R1Y3REZXRhaWxzTW9kYWxJZH1gKTtcbiAgICBjb25zdCBtb2RhbCA9IGJvb3RzdHJhcC5Nb2RhbC5nZXRPckNyZWF0ZUluc3RhbmNlKG1vZGFsRWwhKTtcblxuICAgIG1vZGFsRWw/LmFkZEV2ZW50TGlzdGVuZXIoJ2hpZGRlbi5icy5tb2RhbCcsICgpID0+IHtcbiAgICAgICAgcmVuZGVyKGh0bWxgYCwgZGlhbG9nc1NlY3Rpb24hKTtcbiAgICAgICAgbW9kYWwuZGlzcG9zZSgpO1xuICAgIH0sIHsgb25jZTogdHJ1ZSB9KTtcblxuICAgIG1vZGFsLnNob3coKTtcbn1cblxuYXN5bmMgZnVuY3Rpb24gb25SZW1vdmVQcm9kdWN0SGFuZGxlcihwcm9kdWN0OiBQcm9kdWN0LCBjb250ZXh0OiBUYWJsZUNvbnRleHQpIHtcbiAgICByZW5kZXIocmVtb3ZlUHJvZHVjdFRlbXBsYXRlKHByb2R1Y3QsIGNvbnRleHQpLCBkaWFsb2dzU2VjdGlvbiEpO1xuXG4gICAgY29uc3QgZGVsZXRlUHJvZHVjdE1vZGFsSWQgPSBgcmVtb3ZlLXByb2R1Y3QtJHtwcm9kdWN0LmlkfWA7XG4gICAgY29uc3QgbW9kYWxFbCA9IGRpYWxvZ3NTZWN0aW9uPy5xdWVyeVNlbGVjdG9yPEhUTUxEaXZFbGVtZW50PihgZGl2IyR7ZGVsZXRlUHJvZHVjdE1vZGFsSWR9YClcbiAgICBjb25zdCBtb2RhbCA9IGJvb3RzdHJhcC5Nb2RhbC5nZXRPckNyZWF0ZUluc3RhbmNlKG1vZGFsRWwhKTtcblxuICAgIG1vZGFsRWw/LmFkZEV2ZW50TGlzdGVuZXIoJ2hpZGRlbi5icy5tb2RhbCcsICgpID0+IHtcbiAgICAgICAgcmVuZGVyKGh0bWxgYCwgZGlhbG9nc1NlY3Rpb24hKTtcbiAgICAgICAgbW9kYWwuZGlzcG9zZSgpO1xuICAgIH0sIHsgb25jZTogdHJ1ZSB9KTtcblxuICAgIG1vZGFsLnNob3coKTtcbn1cblxuZnVuY3Rpb24gY2FsY3VsYXRlUGFnZU51bWJlcnMoY3VyclBhZ2VOdW1iZXI6IG51bWJlciwgdG90YWxQYWdlc0NvdW50OiBudW1iZXIpIHtcbiAgICBjb25zdCBmaXJzdFBhZ2VOdW1iZXIgPSBNYXRoLm1heCgxLCBjdXJyUGFnZU51bWJlciAtIDMpO1xuICAgIGNvbnN0IGxhc3RQYWdlTnVtYmVyID0gTWF0aC5taW4odG90YWxQYWdlc0NvdW50LCBjdXJyUGFnZU51bWJlciArIDMpO1xuXG4gICAgY29uc3QgcGFnZU51bWJlcnNPblNjcmVlbiA9IFtdO1xuICAgIGZvcihsZXQgaSA9IGZpcnN0UGFnZU51bWJlcjsgaSA8PSBsYXN0UGFnZU51bWJlcjsgaSsrKSB7XG4gICAgICAgIHBhZ2VOdW1iZXJzT25TY3JlZW4ucHVzaChpKTtcbiAgICB9XG5cbiAgICByZXR1cm4gcGFnZU51bWJlcnNPblNjcmVlbjtcbn1cblxuZnVuY3Rpb24gY2FsY3VsYXRlSXRlbXNOdW1iZXJzKFxuICAgIGN1cnJQYWdlTnVtYmVyOiBudW1iZXIsXG4gICAgdG90YWxQYWdlc0NvdW50OiBudW1iZXIsXG4gICAgdG90YWxJdGVtc0NvdW50OiBudW1iZXIsXG4gICAgaXRlbXNDb3VudE9uUGFnZTogbnVtYmVyXG4pOiByZWFkb25seSBbZmlyc3RJdGVtTnVtT25QYWdlOiBudW1iZXIsIGxhc3RJdGVtTnVtT25QYWdlOiBudW1iZXJdIHtcbiAgICBjb25zdCBsYXN0SXRlbU51bU9uUGFnZSA9IE1hdGgubWluKGN1cnJQYWdlTnVtYmVyICogaXRlbXNDb3VudE9uUGFnZSwgdG90YWxJdGVtc0NvdW50KTtcblxuICAgIGxldCBmaXJzdEl0ZW1OdW1PblBhZ2UgPSBsYXN0SXRlbU51bU9uUGFnZSAtIGl0ZW1zQ291bnRPblBhZ2UgKyAxO1xuICAgIGlmIChjdXJyUGFnZU51bWJlciA9PT0gdG90YWxQYWdlc0NvdW50KSB7XG4gICAgICAgIGlmICh0b3RhbEl0ZW1zQ291bnQgPT09IDApXG4gICAgICAgICAgICBmaXJzdEl0ZW1OdW1PblBhZ2UgPSAwO1xuICAgICAgICBlbHNlIGlmIChjdXJyUGFnZU51bWJlciA9PT0gMSAmJiB0b3RhbEl0ZW1zQ291bnQgIT09IDApXG4gICAgICAgICAgICBmaXJzdEl0ZW1OdW1PblBhZ2UgPSAxO1xuICAgICAgICBlbHNlXG4gICAgICAgICAgICBmaXJzdEl0ZW1OdW1PblBhZ2UgPSBsYXN0SXRlbU51bU9uUGFnZSAtIChsYXN0SXRlbU51bU9uUGFnZSAlIGl0ZW1zQ291bnRPblBhZ2UpICsgMTtcbiAgICB9XG5cbiAgICByZXR1cm4gW2ZpcnN0SXRlbU51bU9uUGFnZSwgbGFzdEl0ZW1OdW1PblBhZ2VdO1xufSJdfQ==