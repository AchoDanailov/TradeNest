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
//# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJmaWxlIjoicHJvZHVjdHNUYWJsZS5qcyIsInNvdXJjZVJvb3QiOiIiLCJzb3VyY2VzIjpbIi4uLy4uLy4uL0Zyb250RW5kU2NyaXB0cy9wcm9kdWN0cy1tYW5hZ2VtZW50L3Byb2R1Y3RzVGFibGUudHMiXSwibmFtZXMiOltdLCJtYXBwaW5ncyI6IkFBQUEsT0FBTyxFQUFFLEtBQUssRUFBRSxNQUFNLFdBQVcsQ0FBQztBQUNsQyxPQUFPLEVBQUUsSUFBSSxFQUFFLE1BQU0sRUFBdUIsTUFBTSxVQUFVLENBQUM7QUFLN0QsT0FBTyxxQkFBcUIsTUFBTSxzQ0FBc0MsQ0FBQztBQUN6RSxPQUFPLDhCQUE4QixNQUFNLGtDQUFrQyxDQUFDO0FBRzlFLE1BQU0saUJBQWlCLEdBQUcsUUFBUSxDQUFDLGFBQWEsQ0FBaUIscUJBQXFCLENBQUMsQ0FBQztBQUN4RixNQUFNLGNBQWMsR0FBRyxRQUFRLENBQUMsYUFBYSxDQUFpQixxQkFBcUIsQ0FBQyxDQUFDO0FBRXJGLE1BQU0sQ0FBQyxPQUFPLENBQUMsS0FBSyxVQUFVLGlCQUFpQixDQUFDLE9BQXFCO0lBQ2pFLE1BQU0sZ0JBQWdCLEdBQUcsTUFBTSxPQUFPLENBQUMsbUJBQW1CLEVBQUUsQ0FBQztJQUM3RCxNQUFNLENBQUMsTUFBTSxRQUFRLENBQUMsZ0JBQWdCLEVBQUUsT0FBTyxDQUFDLEVBQUUsaUJBQWtCLENBQUMsQ0FBQztBQUMxRSxDQUFDO0FBRUQsS0FBSyxVQUFVLFFBQVEsQ0FDbkIsZ0JBQTJCLEVBQzNCLE9BQXFCO0lBRXJCLE1BQU0sV0FBVyxHQUFHLDJDQUEyQyxDQUFBO0lBRS9ELE9BQU8sSUFBSSxDQUFBOztjQUVELGtCQUFrQixDQUFDLE9BQU8sQ0FBQzs7Ozs7OztpQ0FPUixXQUFXO2lDQUNYLFdBQVc7aUNBQ1gsV0FBVztpQ0FDWCxXQUFXO2lDQUNYLFdBQVc7Ozs7O3NCQUt0QixnQkFBZ0IsQ0FBQyxHQUFHLENBQUMsQ0FBQyxDQUFDLEVBQUUsQ0FBQyx1QkFBdUIsQ0FBQyxDQUFDLEVBQUUsT0FBTyxD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Q3JDLElBQUksZUFBZSxLQUFLLENBQUM7WUFDckIsa0JBQWtCLEdBQUcsQ0FBQyxDQUFDO2FBQ3RCLElBQUksY0FBYyxLQUFLLENBQUMsSUFBSSxlQUFlLEtBQUssQ0FBQztZQUNsRCxrQkFBa0IsR0FBRyxDQUFDLENBQUM7O1lBRXZCLGtCQUFrQixHQUFHLGlCQUFpQixHQUFHLENBQUMsaUJBQWlCLEdBQUcsZ0JBQWdCLENBQUMsR0FBRyxDQUFDLENBQUM7SUFDNUYsQ0FBQztJQUVELE9BQU8sQ0FBQyxrQkFBa0IsRUFBRSxpQkFBaUIsQ0FBQyxDQUFDO0FBQ25ELENBQUMiLCJzb3VyY2VzQ29udGVudCI6WyJpbXBvcnQgeyBNb2RhbCB9IGZyb20gXCJib290c3RyYXBcIjtcbmltcG9ydCB7IGh0bWwsIHJlbmRlciwgdHlwZSBUZW1wbGF0ZVJlc3VsdCB9IGZyb20gXCJsaXQtaHRtbFwiO1xuXG5pbXBvcnQgdHlwZSB7IFByb2R1Y3QsIFByb2R1Y3RzQXBwcm92YWxTdGF0dXMgfSBmcm9tIFwiLi4vdHlwZXMvcHJvZHVjdHMudHNcIjtcbmltcG9ydCB0eXBlIHsgVGFibGVDb250ZXh0IH0gZnJvbSBcIi4uL3R5cGVzL3RhYmxlQ29udGV4dC50c1wiO1xuXG5pbXBvcnQgcmVtb3ZlUHJvZHVjdFRlbXBsYXRlIGZyb20gXCIuL2NvbmZpcm1SZW1vdmVQcm9kdWN0RGlhbG9nTW9kYWwuanNcIjtcbmltcG9ydCBnZXRQcm9kdWN0RGV0YWlsc01vZGFsVGVtcGxhdGUgZnJvbSBcIi4vcHJvZHVjdERldGFpbHNNb2RhbFRlbXBsYXRlLmpzXCI7XG5cblxuY29uc3QgdGFibGVEaXZDb250YWluZXIgPSBkb2N1bWVudC5xdWVyeVNlbGVjdG9yPEhUTUxEaXZFbGVtZW50PihcImRpdiN0YWJsZS1jb250YWluZXJcIik7XG5jb25zdCBkaWFsb2dzU2VjdGlvbiA9IGRvY3VtZW50LnF1ZXJ5U2VsZWN0b3I8SFRNTERpdkVsZW1lbnQ+KFwiZGl2I2RpYWxvZ3Mtc2VjdGlvblwiKTtcblxuZXhwb3J0IGRlZmF1bHQgYXN5bmMgZnVuY3Rpb24gc2hvd1Byb2R1Y3RzVGFibGUoY29udGV4dDogVGFibGVDb250ZXh0KSB7XG4gICAgY29uc3QgY3VyclBhZ2VQcm9kdWN0cyA9IGF3YWl0IGNvbnRleHQuZ2V0Q3VyclBhZ2VQcm9kdWN0cygpO1xuICAgIHJlbmRlcihhd2FpdCB0ZW1wbGF0ZShjdXJyUGFnZVByb2R1Y3RzLCBjb250ZXh0KSwgdGFibGVEaXZDb250YWluZXIhKTtcbn1cblxuYXN5bmMgZnVuY3Rpb24gdGVtcGxhdGUoXG4gICAgY3VyclBhZ2VQcm9kdWN0czogUHJvZHVjdFtdLFxuICAgIGNvbnRleHQ6IFRhYmxlQ29udGV4dFxuKTogUHJvbWlzZTxUZW1wbGF0ZVJlc3VsdD4ge1xuICAgIGNvbnN0IGhvdmVyRWZmZWN0ID0gXCJuYXYtbGluay1ib3JkZXItcmFkaXVzLWhvdmVyLWVmZmVjdC1saWdodFwiXG5cbiAgICByZXR1cm4gaHRtbGBcbiAgICAgICAgPGRpdj5cbiAgICAgICAgICAgICR7c2VhcmNoRm9ybVRlbXBsYXRlKGNvbnRleHQpfVxuICAgICAgICA8L2Rpdj5cblxuICAgICAgICA8ZGl2IGlkPVwidGFibGUtd3JhcHBlclwiIGNsYXNzPVwibXQtMCBwdC0wIHctMTAwXCI+XG4gICAgICAgICAgICA8dGFibGUgY2xhc3M9XCJ0YWJsZSB0YWJsZS1ob3ZlciB3LTEwMFwiPlxuICAgICAgICAgICAgICAgIDx0aGVhZCBjbGFzcz1cInNpdGUtc2VjdGlvbnMtYmctdGVhbCB0ZXh0LWNlbnRlclwiPlxuICAgICAgICAgICAgICAgIDx0ciBjbGFzcz1cImFsaWduLW1pZGRsZVwiPlxuICAgICAgICAgICAgICAgICAgICA8dGggY2xhc3M9XCIke2hvdmVyRWZmZWN0fVwiPiBQcm9kdWN0IDwvdGg+XG4gICAgICAgICAgICAgICAgICAgIDx0aCBjbGFzcz1cIiR7aG92ZXJFZmZlY3R9XCI+IE93bmVyIDwvdGg+XG4gICAgICAgICAgICAgICAgICAgIDx0aCBjbGFzcz1cIiR7aG92ZXJFZmZlY3R9XCI+IENhdGVnb3J5IE5hbWUgPC90aD5cbiAgICAgICAgICAgICAgICAgICAgPHRoIGNsYXNzPVwiJHtob3ZlckVmZmVjdH1cIj4gQXBwcm92YWwgU3RhdHVzIDwvdGg+XG4gICAgICAgICAgICAgICAgICAgIDx0aCBjbGFzcz1cIiR7aG92ZXJFZmZlY3R9XCI+IEFjdGlvbnMgPC90aD5cbiAgICAgICAgICAgICAgICA8L3RyPlxuICAgICAgICAgICAgICAgIDwvdGhlYWQ+XG5cbiAgICAgICAgICAgICAgICA8dGJvZHkgY2xhc3M9XCJ0Ym9keS10b3AtYm9yZGVyXCI+XG4gICAgICAgICAgICAgICAgICAgICR7Y3VyclBhZ2VQcm9kdWN0cy5tYXAocCA9PiBwcm9kdWN0VGFibGVSb3dUZW1wbGF0ZShwLCBjb250ZXh0KSl9XG4gICAgICAgICAgICAgICAgPC90Ym9keT5cbiAgICAgICAgICAgIDwvdGFibGU+XG5cbiAgICAgICAgICAgIDxkaXYgY2xhc3M9XCJkLWZsZXgganVzdGlmeS1jb250ZW50LWVuZCBhbGlnbi1pdGVtcy1jZW50ZXIgZ2FwLTIgcG9zaXRpb24tcmVsYXRpdmUgYm90dG9tLTAgZW5kLTBcIj5cbiAgICAgICAgICAgICAgICAke2F3YWl0IGNvbnRyb2xzVGVtcGxhdGUoY29udGV4dCl9XG4gICAgICAgICAgICA8L2Rpdj5cblxuICAgICAgICA8L2Rpdj5cbiAgICBgO1xufVxuXG5mdW5jdGlvbiBzZWFyY2hGb3JtVGVtcGxhdGUoY29udGV4dDogVGFibGVDb250ZXh0KSB7XG4gICAgY29uc3QgZGlzcGxheUNsYXNzID0gY29udGV4dC5nZXRDdXJyU2VhcmNoUXVlcnkoKS50cmltKCkgPT09IFwiXCIgPyBcImQtbm9uZVwiIDogXCJcIjtcblxuICAgIHJldHVybiBodG1sYFxuICAgICAgICA8ZGl2IGNsYXNzPVwiZC1mbGV4IGZsZXgtY29sdW1uIGFsaWduLWl0ZW1zLWNlbnRlciBtdC01IG1iLTNcIiBpZD1cInNlYXJjaC1zZWN0aW9uLXdyYXBwZXJcIj5cbiAgICAgICAgICAgIDxsYWJlbCBmb3I9XCJzZWFyY2hJbnB1dFwiIGNsYXNzPVwiZm9ybS1sYWJlbCB0ZXh0LW5hdnkgdGV4dC1jZW50ZXJcIj5cbiAgICAgICAgICAgICAgICBTZWFyY2ggZm9yIHByb2R1Y3RzXG4gICAgICAgICAgICA8L2xhYmVsPlxuICAgICAgICAgICAgPGZvcm0gaWQ9XCJzZWFyY2hGb3JtXCIgY2xhc3M9XCJteC1tZC0zXCJcbiAgICAgICAgICAgICAgICAgIEBzdWJtaXQ9JHthc3luYyAoZXZlbnQ6IEV2ZW50KSA9PiBhd2FpdCBvblNlYXJjaEZvcm1TdWJtaXRIYW5kbGVyKGV2ZW50LCBjb250ZXh0KX0+XG4gICAgICAgICAgICAgICAgPGRpdiBjbGFzcz1cImlucHV0LWdyb3VwLXNtIGQtZmxleCBnYXAtMVwiPlxuICAgICAgICAgICAgICAgICAgICA8aW5wdXQgbmFtZT1cInNlYXJjaFwiIHR5cGU9XCJzZWFyY2hcIiBpZD1cInNlYXJjaElucHV0XCJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgIGNsYXNzPVwiZm9ybS1jb250cm9sXCIgcGxhY2Vob2xkZXI9XCJTZWFyY2guLi5cIlxuICAgICAgICAgICAgICAgICAgICAgICAgICAgYXJpYS1sYWJlbD1cIlNlYXJjaFwiIC52YWx1ZT0ke2NvbnRleHQuZ2V0Q3VyclNlYXJjaFF1ZXJ5KCl9IC8+XG4gICAgICAgICAgICAgICAgICAgIDxidXR0b24gY2xhc3M9XCJidG4gYnRuLW91dGxpbmUtdGVhbFwiIHR5cGU9XCJzdWJtaXRcIj5TZWFyY2g8L2J1dHRvbj5cbiAgICAgICAgICAgICAgICA8L2Rpdj5cbiAgICAgICAgICAgIDwvZm9ybT5cbiAgICAgICAgICAgIDxkaXYgY2xhc3M9XCJteS0yIHBvc2l0aW9uLXJlbGF0aXZlICR7ZGlzcGxheUNsYXNzfVwiIHN0eWxlPVwicmlnaHQ6IDVweFwiPlxuICAgICAgICAgICAgICAgIDxzcGFuPnJlc3VsdHMgZm9yOiAke2NvbnRleHQuZ2V0Q3VyclNlYXJjaFF1ZXJ5KCl9PC9zcGFuPlxuICAgICAgICAgICAgICAgIDxhPlxuICAgICAgICAgICAgICAgICAgICA8c3BhbiBjbGFzcz1cImJ0bi1zbSBidG4tZGFuZ2VyXCJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgQGNsaWNrPSR7YXN5bmMgKCkgPT4gYXdhaXQgb25DbGVhclNlYXJjaEZvcm0oY29udGV4dCl9PlxuICAgICAgICAgICAgICAgICAgICAgICAgeFxuICAgICAgICAgICAgICAgICAgICA8L3NwYW4+XG4gICAgICAgICAgICAgICAgPC9hPlxuICAgICAgICAgICAgPC9kaXY+XG4gICAgICAgIDwvZGl2PmA7XG59XG5cbmFzeW5jIGZ1bmN0aW9uIG9uQ2xlYXJTZWFyY2hGb3JtKGNvbnRleHQ6IFRhYmxlQ29udGV4dCkge1xuICAgIGNvbnRleHQuc2V0U2VhcmNoUXVlcnkoXCJcIik7XG4gICAgYXdhaXQgc2hvd1Byb2R1Y3RzVGFibGUoY29udGV4dCk7XG59XG5cbmFzeW5jIGZ1bmN0aW9uIG9uU2VhcmNoRm9ybVN1Ym1pdEhhbmRsZXIoZXZlbnQ6IEV2ZW50LCBjb250ZXh0OiBUYWJsZUNvbnRleHQpIHtcbiAgICBldmVudC5wcmV2ZW50RGVmYXVsdCgpO1xuXG4gICAgY29uc3QgZm9ybURhdGEgPSBuZXcgRm9ybURhdGEoZXZlbnQuY3VycmVudFRhcmdldCBhcyBIVE1MRm9ybUVsZW1lbnQgfCB1bmRlZmluZWQpO1xuICAgIGNvbnN0IHNlYXJjaFF1ZXJ5ID0gZm9ybURhdGEuZ2V0KFwic2VhcmNoXCIpIGFzIEZvcm1EYXRhRW50cnlWYWx1ZSBhcyBzdHJpbmc7XG4gICAgaWYoc2VhcmNoUXVlcnkudHJpbSgpID09PSBcIlwiICYmIGNvbnRleHQuZ2V0Q3VyclNlYXJjaFF1ZXJ5KCkgPT09IFwiXCIpIHtcbiAgICAgICAgcmV0dXJuO1xuICAgIH1cblxuICAgIGNvbnRleHQuc2V0U2VhcmNoUXVlcnkoc2VhcmNoUXVlcnkpO1xuICAgIGF3YWl0IHNob3dQcm9kdWN0c1RhYmxlKGNvbnRleHQpO1xufVxuXG5hc3luYyBmdW5jdGlvbiBjb250cm9sc1RlbXBsYXRlKGNvbnRleHQ6IFRhYmxlQ29udGV4dCkge1xuICAgIGNvbnN0IHRvdGFsUGFnZXNDb3VudCA9IGNvbnRleHQuZ2V0UGFnZXNUb3RhbENvdW50KCk7XG4gICAgY29uc3QgY3VyclBhZ2VOdW1iZXIgPSBjb250ZXh0LmdldEN1cnJQYWdlTnVtYmVyKCk7XG4gICAgY29uc3QgdG90YWxJdGVtc0NvdW50ID0gY29udGV4dC5nZXRQcm9kdWN0c0NvdW50KCk7XG4gICAgY29uc3QgaXRlbXNDb3VudE9uUGFnZSA9IGNvbnRleHQuZ2V0Q3Vyckl0ZW1zT25QYWdlQ291bnQoKTtcblxuICAgIGNvbnN0IHBhZ2VOdW1iZXJzT25TY3JlZW4gPSBjYWxjdWxhdGVQYWdlTnVtYmVycyhjdXJyUGFnZU51bWJlciwgdG90YWxQYWdlc0NvdW50KVxuXG4gICAgY29uc3QgW2ZpcnN0SXRlbU51bU9uUGFnZSwgbGFzdEl0ZW1OdW1PblBhZ2VdXG4gICAgICAgID0gY2FsY3VsYXRlSXRlbXNOdW1iZXJzKGN1cnJQYWdlTnVtYmVyLCB0b3RhbFBhZ2VzQ291bnQsIHRvdGFsSXRlbXNDb3VudCwgaXRlbXNDb3VudE9uUGFnZSk7XG5cbiAgICByZXR1cm4gaHRtbGBcbiAgICAgICAgPHAgY2xhc3M9XCJ0ZXh0LW5hdnkgdGV4dC1tdXRlZCBmcy02IGZzdC1pdGFsaWMgZC1pbmxpbmVcIj5cbiAgICAgICAgICAgICR7Zmlyc3RJdGVtTnVtT25QYWdlfS0ke2xhc3RJdGVtTnVtT25QYWdlfSBmcm9tICR7dG90YWxJdGVtc0NvdW50fVxuICAgICAgICA8L3A+XG4gICAgICAgIDxuYXYgYXJpYS1sYWJlbD1cIlRhYmxlIHBhZ2luYXRpb24gY29udHJvbC5cIj5cbiAgICAgICAgICAgIDx1bCBjbGFzcz1cInBhZ2luYXRpb24ganVzdGlmeS1jb250ZW50LWNlbnRlclwiPlxuICAgICAgICAgICAgICAgICR7Y3VyclBhZ2VOdW1iZXIgPD0gMVxuICAgICAgICAgICAgICAgICAgICAgICAgPyBodG1sYFxuICAgICAgICAgICAgICAgICAgICAgICAgICAgIDxsaSBjbGFzcz1cInBhZ2UtaXRlbSBkaXNhYmxlZFwiPlxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICA8c3BhbiBjbGFzcz1cInBhZ2UtbGlua1wiPlByZXZpb3VzPC9zcGFuPlxuICAgICAgICAgICAgICAgICAgICAgICAgICAgIDwvbGk+YFxuICAgICAgICAgICAgICAgICAgICAgICAgOiBodG1sYFxuICAgICAgICAgICAgICAgICAgICAgICAgICAgIDxsaSBjbGFzcz1cInBhZ2UtaXRlbVwiPlxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICA8YSBjbGFzcz1cInBhZ2UtbGlua1wiXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIEBjbGljaz0ke2FzeW5jICgpID0+IGF3YWl0IG9uUGFnZU51bUJ0bkNsaWNrKGNvbnRleHQsIGN1cnJQYWdlTnVtYmVyIC0gMSl9PlxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgUHJldmlvdXNcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgPC9hPlxuICAgICAgICAgICAgICAgICAgICAgICAgICAgIDwvbGk+YFxuICAgICAgICAgICAgICAgIH1cblxuICAgICAgICAgICAgICAgICR7cGFnZU51bWJlcnNPblNjcmVlbi5tYXAocGFnZU51bSA9PiB7XG4gICAgICAgICAgICAgICAgICAgIHJldHVybiBwYWdlTnVtID09PSBjdXJyUGFnZU51bWJlclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgID8gaHRtbCBgXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIDxsaSBjbGFzcz1cInBhZ2UtaXRlbSBhY3RpdmVcIj5cbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgPHNwYW4gY2xhc3M9XCJwYWdlLWxpbmtcIj5cbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICR7Y3VyclBhZ2VOdW1iZXJ9XG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIDwvc3Bhbj5cbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgPC9saT5gXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgOiBodG1sIGBcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgPGxpIGNsYXNzPVwicGFnZS1pdGVtXCI+XG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICA8YSBjbGFzcz1cInBhZ2UtbGlua1wiXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICBAY2xpY2s9JHthc3luYyAoKSA9PiBhd2FpdCBvblBhZ2VOdW1CdG5DbGljayhjb250ZXh0LCBwYWdlTnVtKX0+XG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgJHtwYWdlTnVtfVxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgPC9hPlxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICA8L2xpPmBcbiAgICAgICAgICAgICAgICB9KX1cblxuICAgICAgICAgICAgICAgICR7Y3VyclBhZ2VOdW1iZXIgPT09IHBhZ2VOdW1iZXJzT25TY3JlZW4ubGVuZ3RoXG4gICAgICAgICAgICAgICAgICAgICAgICA/IGh0bWxgXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgPGxpIGNsYXNzPVwicGFnZS1pdGVtIGRpc2FibGVkXCI+XG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIDxzcGFuIGNsYXNzPVwicGFnZS1saW5rXCI+TmV4dDwvc3Bhbj5cbiAgICAgICAgICAgICAgICAgICAgICAgICAgICA8L2xpPmBcbiAgICAgICAgICAgICAgICAgICAgICAgIDogaHRtbGBcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICA8bGkgY2xhc3M9XCJwYWdlLWl0ZW1cIj5cbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgPGEgY2xhc3M9XCJwYWdlLWxpbmtcIlxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICBAY2xpY2s9JHthc3luYyAoKSA9PiBhd2FpdCBvblBhZ2VOdW1CdG5DbGljayhjb250ZXh0LCBjdXJyUGFnZU51bWJlciArIDEpfT5cbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIE5leHRcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgPC9hPlxuICAgICAgICAgICAgICAgICAgICAgICAgICAgIDwvbGk+YFxuICAgICAgICAgICAgICAgIH1cbiAgICAgICAgICAgIDwvdWw+XG4gICAgICAgIDwvbmF2PmA7XG59XG5cbmFzeW5jIGZ1bmN0aW9uIG9uUGFnZU51bUJ0bkNsaWNrKGNvbnRleHQ6IFRhYmxlQ29udGV4dCwgcGFnZU51bWJlcjogbnVtYmVyKSB7XG4gICAgY29udGV4dC5zZXRQYWdlTnVtYmVyKHBhZ2VOdW1iZXIpO1xuICAgIGF3YWl0IHNob3dQcm9kdWN0c1RhYmxlKGNvbnRleHQpO1xufVxuXG5mdW5jdGlvbiBwcm9kdWN0VGFibGVSb3dUZW1wbGF0ZShwcm9kdWN0OiBQcm9kdWN0LCBjb250ZXh0OiBUYWJsZUNvbnRleHQpIHtcbiAgICBjb25zdCBhcHByb3ZhbFN0YXR1c1RkTWFwID0ge1xuICAgICAgICBcIkFwcHJvdmVkXCI6ICgpID0+IFtcIvCfn6JcIiwgXCJBcHByb3ZlZFwiLCBcInRleHQtc3VjY2VzcyBmdy1zZW1pYm9sZFwiXSxcbiAgICAgICAgXCJXYWl0aW5nQXBwcm92YWxcIjogKCkgPT4gW1wi8J+foVwiLCBcIldhaXRpbmcgQXBwcm92YWxcIiwgXCJ0ZXh0LXdhcm5pbmcgZnctc2VtaWJvbGRcIl0sXG4gICAgICAgIFwiRGlzYXBwcm92ZWRcIjogKCkgPT4gW1wi8J+UtFwiLCBcIkRpc2FwcHJvdmVkXCIsIFwidGV4dC1kYW5nZXIgZnctc2VtaWJvbGRcIl0sXG4gICAgfSBhcyBSZWNvcmQ8UHJvZHVjdHNBcHByb3ZhbFN0YXR1cywgKCkgPT4gcmVhZG9ubHkgW2RvdDogc3RyaW5nLCBjb250ZW50OiBzdHJpbmcsIHN0eWxlczogc3RyaW5nXT47XG5cbiAgICBjb25zdCBbZG90LCBjb250ZW50LCBzdHlsZXNdID0gYXBwcm92YWxTdGF0dXNUZE1hcFtwcm9kdWN0IS5hcHByb3ZhbFN0YXR1c10hKCk7XG5cbiAgICByZXR1cm4gaHRtbGBcbiAgICAgICAgPHRyIGNsYXNzPVwidGV4dC1jZW50ZXIgYWxpZ24tbWlkZGxlXCI+XG4gICAgICAgICAgICA8dGQ+JHtwcm9kdWN0Lm5hbWV9PC90ZD5cbiAgICAgICAgICAgIDx0ZD4ke3Byb2R1Y3Qub3duZXJOYW1lfTwvdGQ+XG4gICAgICAgICAgICA8dGQ+JHtwcm9kdWN0LmNhdGVnb3J5TmFtZX08L3RkPlxuICAgICAgICAgICAgPHRkIGNsYXNzPVwiJHtzdHlsZXN9XCI+JHtkb3R9ICR7Y29udGVudH08L3RkPlxuICAgICAgICAgICAgPHRkPlxuICAgICAgICAgICAgICAgIDxkaXYgY2xhc3M9XCJidG4tZ3JvdXAtc20gZC1mbGV4IGZsZXgtd3JhcCBqdXN0aWZ5LWNvbnRlbnQtY2VudGVyIGdhcC0xIGdhcC1zbS0yIGdhcC1tZC0yXCI+XG4gICAgICAgICAgICAgICAgICAgIDxidXR0b24gY2xhc3M9XCJidG4gcm91bmRlZC1waWxsIGJ0bi10ZWFsIGJ0bi1zbSB3LTEwMFwiIHN0eWxlPVwibWF4LXdpZHRoOiAxMmVtXCJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICBAY2xpY2s9JHthc3luYyAoKSA9PiBhd2FpdCBvblZpZXdQcm9kdWN0RGV0YWlsc0hhbmRsZXIocHJvZHVjdC5pZCwgY29udGV4dCl9PlxuICAgICAgICAgICAgICAgICAgICAgICAgVmlldyBEZXRhaWxzXG4gICAgICAgICAgICAgICAgICAgIDwvYnV0dG9uPlxuXG4gICAgICAgICAgICAgICAgICAgIDxidXR0b24gY2xhc3M9XCJidG4gcm91bmRlZC1waWxsIGJ0bi1vdXRsaW5lLWRhbmdlciBidG4tc20gdy0xMDBcIiBzdHlsZT1cIm1heC13aWR0aDogMTJlbVwiXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgQGNsaWNrPSR7YXN5bmMgKCkgPT4gYXdhaXQgb25SZW1vdmVQcm9kdWN0SGFuZGxlcihwcm9kdWN0LCBjb250ZXh0KX0+XG4gICAgICAgICAgICAgICAgICAgICAgICBSZW1vdmUgUHJvZHVjdFxuICAgICAgICAgICAgICAgICAgICA8L2J1dHRvbj5cbiAgICAgICAgICAgICAgICA8L2Rpdj5cbiAgICAgICAgICAgIDwvdGQ+XG4gICAgICAgIDwvdHI+XG4gICAgYDtcbn1cblxuYXN5bmMgZnVuY3Rpb24gb25WaWV3UHJvZHVjdERldGFpbHNIYW5kbGVyKHByb2R1Y3RJZDogc3RyaW5nLCBjb250ZXh0OiBUYWJsZUNvbnRleHQpIHtcbiAgICByZW5kZXIoYXdhaXQgZ2V0UHJvZHVjdERldGFpbHNNb2RhbFRlbXBsYXRlKHByb2R1Y3RJZCwgY29udGV4dCksIGRpYWxvZ3NTZWN0aW9uISk7XG5cbiAgICBjb25zdCBwcm9kdWN0RGV0YWlsc01vZGFsSWQgPSBgcHJvZHVjdC1kZXRhaWxzLSR7cHJvZHVjdElkfWA7XG4gICAgY29uc3QgbW9kYWxFbCA9IGRpYWxvZ3NTZWN0aW9uPy5xdWVyeVNlbGVjdG9yPEhUTUxEaXZFbGVtZW50PihgZGl2IyR7cHJvZHVjdERldGFpbHNNb2RhbElkfWApO1xuICAgIGNvbnN0IG1vZGFsID0gTW9kYWwuZ2V0T3JDcmVhdGVJbnN0YW5jZShtb2RhbEVsISk7XG5cbiAgICBtb2RhbEVsPy5hZGRFdmVudExpc3RlbmVyKCdoaWRkZW4uYnMubW9kYWwnLCAoKSA9PiB7XG4gICAgICAgIHJlbmRlcihodG1sYGAsIGRpYWxvZ3NTZWN0aW9uISk7XG4gICAgICAgIG1vZGFsLmRpc3Bvc2UoKTtcbiAgICB9LCB7IG9uY2U6IHRydWUgfSk7XG5cbiAgICBtb2RhbC5zaG93KCk7XG59XG5cbmFzeW5jIGZ1bmN0aW9uIG9uUmVtb3ZlUHJvZHVjdEhhbmRsZXIocHJvZHVjdDogUHJvZHVjdCwgY29udGV4dDogVGFibGVDb250ZXh0KSB7XG4gICAgcmVuZGVyKHJlbW92ZVByb2R1Y3RUZW1wbGF0ZShwcm9kdWN0LCBjb250ZXh0KSwgZGlhbG9nc1NlY3Rpb24hKTtcblxuICAgIGNvbnN0IGRlbGV0ZVByb2R1Y3RNb2RhbElkID0gYHJlbW92ZS1wcm9kdWN0LSR7cHJvZHVjdC5pZH1gO1xuICAgIGNvbnN0IG1vZGFsRWwgPSBkaWFsb2dzU2VjdGlvbj8ucXVlcnlTZWxlY3RvcjxIVE1MRGl2RWxlbWVudD4oYGRpdiMke2RlbGV0ZVByb2R1Y3RNb2RhbElkfWApXG4gICAgY29uc3QgbW9kYWwgPSBNb2RhbC5nZXRPckNyZWF0ZUluc3RhbmNlKG1vZGFsRWwhKTtcblxuICAgIG1vZGFsRWw/LmFkZEV2ZW50TGlzdGVuZXIoJ2hpZGRlbi5icy5tb2RhbCcsICgpID0+IHtcbiAgICAgICAgcmVuZGVyKGh0bWxgYCwgZGlhbG9nc1NlY3Rpb24hKTtcbiAgICAgICAgbW9kYWwuZGlzcG9zZSgpO1xuICAgIH0sIHsgb25jZTogdHJ1ZSB9KTtcblxuICAgIG1vZGFsLnNob3coKTtcbn1cblxuZnVuY3Rpb24gY2FsY3VsYXRlUGFnZU51bWJlcnMoY3VyclBhZ2VOdW1iZXI6IG51bWJlciwgdG90YWxQYWdlc0NvdW50OiBudW1iZXIpIHtcbiAgICBjb25zdCBmaXJzdFBhZ2VOdW1iZXIgPSBNYXRoLm1heCgxLCBjdXJyUGFnZU51bWJlciAtIDMpO1xuICAgIGNvbnN0IGxhc3RQYWdlTnVtYmVyID0gTWF0aC5taW4odG90YWxQYWdlc0NvdW50LCBjdXJyUGFnZU51bWJlciArIDMpO1xuXG4gICAgY29uc3QgcGFnZU51bWJlcnNPblNjcmVlbiA9IFtdO1xuICAgIGZvcihsZXQgaSA9IGZpcnN0UGFnZU51bWJlcjsgaSA8PSBsYXN0UGFnZU51bWJlcjsgaSsrKSB7XG4gICAgICAgIHBhZ2VOdW1iZXJzT25TY3JlZW4ucHVzaChpKTtcbiAgICB9XG5cbiAgICByZXR1cm4gcGFnZU51bWJlcnNPblNjcmVlbjtcbn1cblxuZnVuY3Rpb24gY2FsY3VsYXRlSXRlbXNOdW1iZXJzKFxuICAgIGN1cnJQYWdlTnVtYmVyOiBudW1iZXIsXG4gICAgdG90YWxQYWdlc0NvdW50OiBudW1iZXIsXG4gICAgdG90YWxJdGVtc0NvdW50OiBudW1iZXIsXG4gICAgaXRlbXNDb3VudE9uUGFnZTogbnVtYmVyXG4pOiByZWFkb25seSBbZmlyc3RJdGVtTnVtT25QYWdlOiBudW1iZXIsIGxhc3RJdGVtTnVtT25QYWdlOiBudW1iZXJdIHtcbiAgICBjb25zdCBsYXN0SXRlbU51bU9uUGFnZSA9IE1hdGgubWluKGN1cnJQYWdlTnVtYmVyICogaXRlbXNDb3VudE9uUGFnZSwgdG90YWxJdGVtc0NvdW50KTtcblxuICAgIGxldCBmaXJzdEl0ZW1OdW1PblBhZ2UgPSBsYXN0SXRlbU51bU9uUGFnZSAtIGl0ZW1zQ291bnRPblBhZ2UgKyAxO1xuICAgIGlmIChjdXJyUGFnZU51bWJlciA9PT0gdG90YWxQYWdlc0NvdW50KSB7XG4gICAgICAgIGlmICh0b3RhbEl0ZW1zQ291bnQgPT09IDApXG4gICAgICAgICAgICBmaXJzdEl0ZW1OdW1PblBhZ2UgPSAwO1xuICAgICAgICBlbHNlIGlmIChjdXJyUGFnZU51bWJlciA9PT0gMSAmJiB0b3RhbEl0ZW1zQ291bnQgIT09IDApXG4gICAgICAgICAgICBmaXJzdEl0ZW1OdW1PblBhZ2UgPSAxO1xuICAgICAgICBlbHNlXG4gICAgICAgICAgICBmaXJzdEl0ZW1OdW1PblBhZ2UgPSBsYXN0SXRlbU51bU9uUGFnZSAtIChsYXN0SXRlbU51bU9uUGFnZSAlIGl0ZW1zQ291bnRPblBhZ2UpICsgMTtcbiAgICB9XG5cbiAgICByZXR1cm4gW2ZpcnN0SXRlbU51bU9uUGFnZSwgbGFzdEl0ZW1OdW1PblBhZ2VdO1xufVxuIl19