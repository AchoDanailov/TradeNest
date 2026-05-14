import * as bootstrap from "bootstrap";
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
//# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJmaWxlIjoicHJvZHVjdHNUYWJsZS5qcyIsInNvdXJjZVJvb3QiOiIiLCJzb3VyY2VzIjpbIi4uLy4uLy4uL0Zyb250RW5kU2NyaXB0cy9wcm9kdWN0cy1tYW5hZ2VtZW50L3Byb2R1Y3RzVGFibGUudHMiXSwibmFtZXMiOltdLCJtYXBwaW5ncyI6IkFBQUEsT0FBTyxLQUFLLFNBQVMsTUFBTSxXQUFXLENBQUM7QUFDdkMsT0FBTyxFQUFFLElBQUksRUFBRSxNQUFNLEVBQXVCLE1BQU0sVUFBVSxDQUFDO0FBSzdELE9BQU8scUJBQXFCLE1BQU0sc0NBQXNDLENBQUM7QUFDekUsT0FBTyw4QkFBOEIsTUFBTSxrQ0FBa0MsQ0FBQztBQUc5RSxNQUFNLGlCQUFpQixHQUFHLFFBQVEsQ0FBQyxhQUFhLENBQWlCLHFCQUFxQixDQUFDLENBQUM7QUFDeEYsTUFBTSxjQUFjLEdBQUcsUUFBUSxDQUFDLGFBQWEsQ0FBaUIscUJBQXFCLENBQUMsQ0FBQztBQUVyRixNQUFNLENBQUMsT0FBTyxDQUFDLEtBQUssVUFBVSxpQkFBaUIsQ0FBQyxPQUFxQjtJQUNqRSxNQUFNLGdCQUFnQixHQUFHLE1BQU0sT0FBTyxDQUFDLG1CQUFtQixFQUFFLENBQUM7SUFDN0QsTUFBTSxDQUFDLE1BQU0sUUFBUSxDQUFDLGdCQUFnQixFQUFFLE9BQU8sQ0FBQyxFQUFFLGlCQUFrQixDQUFDLENBQUM7QUFDMUUsQ0FBQztBQUVELEtBQUssVUFBVSxRQUFRLENBQ25CLGdCQUEyQixFQUMzQixPQUFxQjtJQUVyQixNQUFNLFdBQVcsR0FBRywyQ0FBMkMsQ0FBQTtJQUUvRCxPQUFPLElBQUksQ0FBQTs7Y0FFRCxrQkFBa0IsQ0FBQyxPQUFPLENBQUM7Ozs7Ozs7aUNBT1IsV0FBVztpQ0FDWCxXQUFXO2lDQUNYLFdBQVc7aUNBQ1gsV0FBVztpQ0FDWCxXQUFXOzs7OztzQkFLdEIsZ0JBQWdCLENBQUMsR0FBRyxDQUFDLENBQUMsQ0FBQyxFQUFFLENBQUMsdUJBQXVCLENBQUMsQ0FBQyxFQUFFLE9BQU8sQ0FBQyxDQUFDOzs7OztrQkFLbEUsTUFBTSxnQkFBZ0IsQ0FBQyxPQUFPLENBQUM7Ozs7S0FJNUMsQ0FBQztBQUNOLENBQUM7QUFFRCxTQUFTLGtCQUFrQixDQUFDLE9BQXFCO0lBQzdDLE1BQU0sWUFBWSxHQUFHLE9BQU8sQ0FBQyxrQkFBa0IsRUFBRSxDQUFDLElBQUksRUFBRSxLQUFLLEVBQUUsQ0FBQyxDQUFDLENBQUMsUUFBUSxDQUFDLENBQUMsQ0FBQyxFQUFFLENBQUM7SUFFaEYsT0FBTyxJQUFJLENBQUE7Ozs7Ozs0QkFNYSxLQUFLLEVBQUUsS0FBWSxFQUFFLEVBQUUsQ0FBQyxNQUFNLHlCQUF5QixDQUFDLEtBQUssRUFBRSxPQUFPLENBQUM7Ozs7d0RBSTNDLE9BQU8sQ0FBQyxrQkFBa0IsRUFBRTs7OztpREFJbkMsWUFBWTtxQ0FDeEIsT0FBTyxDQUFDLGtCQUFrQixFQUFFOzs7bUNBRzlCLEtBQUssSUFBSSxFQUFFLENBQUMsTUFBTSxpQkFBaUIsQ0FBQyxPQUFPLENBQUM7Ozs7O2VBS2hFLENBQUM7QUFDaEIsQ0FBQztBQUVELEtBQUssVUFBVSxpQkFBaUIsQ0FBQyxPQUFxQjtJQUNsRCxPQUFPLENBQUMsY0FBYyxDQUFDLEVBQUUsQ0FBQyxDQUFDO0lBQzNCLE1BQU0saUJBQWlCLENBQUMsT0FBTyxDQUFDLENBQUM7QUFDckMsQ0FBQztBQUVELEtBQUssVUFBVSx5QkFBeUIsQ0FBQyxLQUFZLEVBQUUsT0FBcUI7SUFDeEUsS0FBSyxDQUFDLGNBQWMsRUFBRSxDQUFDO0lBRXZCLE1BQU0sUUFBUSxHQUFHLElBQUksUUFBUSxDQUFDLEtBQUssQ0FBQyxhQUE0QyxDQUFDLENBQUM7SUFDbEYsTUFBTSxXQUFXLEdBQUcsUUFBUSxDQUFDLEdBQUcsQ0FBQyxRQUFRLENBQWlDLENBQUM7SUFDM0UsSUFBRyxXQUFXLENBQUMsSUFBSSxFQUFFLEtBQUssRUFBRSxJQUFJLE9BQU8sQ0FBQyxrQkFBa0IsRUFBRSxLQUFLLEVBQUUsRUFBRSxDQUFDO1FBQ2xFLE9BQU87SUFDWCxDQUFDO0lBRUQsT0FBTyxDQUFDLGNBQWMsQ0FBQyxXQUFXLENBQUMsQ0FBQztJQUNwQyxNQUFNLGlCQUFpQixDQUFDLE9BQU8sQ0FBQyxDQUFDO0FBQ3JDLENBQUM7QUFFRCxLQUFLLFVBQVUsZ0JBQWdCLENBQUMsT0FBcUI7SUFDakQsTUFBTSxlQUFlLEdBQUcsT0FBTyxDQUFDLGtCQUFrQixFQUFFLENBQUM7SUFDckQsTUFBTSxjQUFjLEdBQUcsT0FBTyxDQUFDLGlCQUFpQixFQUFFLENBQUM7SUFDbkQsTUFBTSxlQUFlLEdBQUcsT0FBTyxDQUFDLGdCQUFnQixFQUFFLENBQUM7SUFDbkQsTUFBTSxnQkFBZ0IsR0FBRyxPQUFPLENBQUMsdUJBQXVCLEVBQUUsQ0FBQztJQUUzRCxNQUFNLG1CQUFtQixHQUFHLG9CQUFvQixDQUFDLGNBQWMsRUFBRSxlQUFlLENBQUMsQ0FBQTtJQUVqRixNQUFNLENBQUMsa0JBQWtCLEVBQUUsaUJBQWlCLENBQUMsR0FDdkMscUJBQXFCLENBQUMsY0FBYyxFQUFFLGVBQWUsRUFBRSxlQUFlLEVBQUUsZ0JBQWdCLENBQUMsQ0FBQztJQUVoRyxPQUFPLElBQUksQ0FBQTs7Y0FFRCxrQkFBa0IsSUFBSSxpQkFBaUIsU0FBUyxlQUFlOzs7O2tCQUkzRCxjQUFjLElBQUksQ0FBQztRQUNiLENBQUMsQ0FBQyxJQUFJLENBQUE7OztrQ0FHSTtRQUNWLENBQUMsQ0FBQyxJQUFJLENBQUE7Ozs0Q0FHYyxLQUFLLElBQUksRUFBRSxDQUFDLE1BQU0saUJBQWlCLENBQUMsT0FBTyxFQUFFLGNBQWMsR0FBRyxDQUFDLENBQUM7OztrQ0FJNUY7O2tCQUVFLG1CQUFtQixDQUFDLEdBQUcsQ0FBQyxPQUFPLENBQUMsRUFBRTtRQUNoQyxPQUFPLE9BQU8sS0FBSyxjQUFjO1lBQ3pCLENBQUMsQ0FBQyxJQUFJLENBQUM7Ozt5Q0FHTSxjQUFjOztzQ0FFakI7WUFDVixDQUFDLENBQUMsSUFBSSxDQUFDOzs7Z0RBR2EsS0FBSyxJQUFJLEVBQUUsQ0FBQyxNQUFNLGlCQUFpQixDQUFDLE9BQU8sRUFBRSxPQUFPLENBQUM7MENBQzNELE9BQU87O3NDQUVYLENBQUE7SUFDdEIsQ0FBQyxDQUFDOztrQkFFQSxjQUFjLEtBQUssbUJBQW1CLENBQUMsTUFBTTtRQUN2QyxDQUFDLENBQUMsSUFBSSxDQUFBOzs7a0NBR0k7UUFDVixDQUFDLENBQUMsSUFBSSxDQUFBOzs7NENBR2MsS0FBSyxJQUFJLEVBQUUsQ0FBQyxNQUFNLGlCQUFpQixDQUFDLE9BQU8sRUFBRSxjQUFjLEdBQUcsQ0FBQyxDQUFDOzs7a0NBSTVGOztlQUVELENBQUM7QUFDaEIsQ0FBQztBQUVELEtBQUssVUFBVSxpQkFBaUIsQ0FBQyxPQUFxQixFQUFFLFVBQWtCO0lBQ3RFLE9BQU8sQ0FBQyxhQUFhLENBQUMsVUFBVSxDQUFDLENBQUM7SUFDbEMsTUFBTSxpQkFBaUIsQ0FBQyxPQUFPLENBQUMsQ0FBQztBQUNyQyxDQUFDO0FBRUQsU0FBUyx1QkFBdUIsQ0FBQyxPQUFnQixFQUFFLE9BQXFCO0lBQ3BFLE1BQU0sbUJBQW1CLEdBQUc7UUFDeEIsVUFBVSxFQUFFLEdBQUcsRUFBRSxDQUFDLENBQUMsSUFBSSxFQUFFLFVBQVUsRUFBRSwwQkFBMEIsQ0FBQztRQUNoRSxpQkFBaUIsRUFBRSxHQUFHLEVBQUUsQ0FBQyxDQUFDLElBQUksRUFBRSxrQkFBa0IsRUFBRSwwQkFBMEIsQ0FBQztRQUMvRSxhQUFhLEVBQUUsR0FBRyxFQUFFLENBQUMsQ0FBQyxJQUFJLEVBQUUsYUFBYSxFQUFFLHlCQUF5QixDQUFDO0tBQ3lCLENBQUM7SUFFbkcsTUFBTSxDQUFDLEdBQUcsRUFBRSxPQUFPLEVBQUUsTUFBTSxDQUFDLEdBQUcsbUJBQW1CLENBQUMsT0FBUSxDQUFDLGNBQWMsQ0FBRSxFQUFFLENBQUM7SUFFL0UsT0FBTyxJQUFJLENBQUE7O2tCQUVHLE9BQU8sQ0FBQyxJQUFJO2tCQUNaLE9BQU8sQ0FBQyxTQUFTO2tCQUNqQixPQUFPLENBQUMsWUFBWTt5QkFDYixNQUFNLEtBQUssR0FBRyxJQUFJLE9BQU87Ozs7cUNBSWIsS0FBSyxJQUFJLEVBQUUsQ0FBQyxNQUFNLDJCQUEyQixDQUFDLE9BQU8sQ0FBQyxFQUFFLEVBQUUsT0FBTyxDQUFDOzs7OztxQ0FLbEUsS0FBSyxJQUFJLEVBQUUsQ0FBQyxNQUFNLHNCQUFzQixDQUFDLE9BQU8sRUFBRSxPQUFPLENBQUM7Ozs7OztLQU0xRixDQUFDO0FBQ04sQ0FBQztBQUVELEtBQUssVUFBVSwyQkFBMkIsQ0FBQyxTQUFpQixFQUFFLE9BQXFCO0lBQy9FLE1BQU0sQ0FBQyxNQUFNLDhCQUE4QixDQUFDLFNBQVMsRUFBRSxPQUFPLENBQUMsRUFBRSxjQUFlLENBQUMsQ0FBQztJQUVsRixNQUFNLHFCQUFxQixHQUFHLG1CQUFtQixTQUFTLEVBQUUsQ0FBQztJQUM3RCxNQUFNLE9BQU8sR0FBRyxjQUFjLEVBQUUsYUFBYSxDQUFpQixPQUFPLHFCQUFxQixFQUFFLENBQUMsQ0FBQztJQUM5RixNQUFNLEtBQUssR0FBRyxTQUFTLENBQUMsS0FBSyxDQUFDLG1CQUFtQixDQUFDLE9BQVEsQ0FBQyxDQUFDO0lBRTVELE9BQU8sRUFBRSxnQkFBZ0IsQ0FBQyxpQkFBaUIsRUFBRSxHQUFHLEVBQUU7UUFDOUMsTUFBTSxDQUFDLElBQUksQ0FBQSxFQUFFLEVBQUUsY0FBZSxDQUFDLENBQUM7UUFDaEMsS0FBSyxDQUFDLE9BQU8sRUFBRSxDQUFDO0lBQ3BCLENBQUMsRUFBRSxFQUFFLElBQUksRUFBRSxJQUFJLEVBQUUsQ0FBQyxDQUFDO0lBRW5CLEtBQUssQ0FBQyxJQUFJLEVBQUUsQ0FBQztBQUNqQixDQUFDO0FBRUQsS0FBSyxVQUFVLHNCQUFzQixDQUFDLE9BQWdCLEVBQUUsT0FBcUI7SUFDekUsTUFBTSxDQUFDLHFCQUFxQixDQUFDLE9BQU8sRUFBRSxPQUFPLENBQUMsRUFBRSxjQUFlLENBQUMsQ0FBQztJQUVqRSxNQUFNLG9CQUFvQixHQUFHLGtCQUFrQixPQUFPLENBQUMsRUFBRSxFQUFFLENBQUM7SUFDNUQsTUFBTSxPQUFPLEdBQUcsY0FBYyxFQUFFLGFBQWEsQ0FBaUIsT0FBTyxvQkFBb0IsRUFBRSxDQUFDLENBQUE7SUFDNUYsTUFBTSxLQUFLLEdBQUcsU0FBUyxDQUFDLEtBQUssQ0FBQyxtQkFBbUIsQ0FBQyxPQUFRLENBQUMsQ0FBQztJQUU1RCxPQUFPLEVBQUUsZ0JBQWdCLENBQUMsaUJBQWlCLEVBQUUsR0FBRyxFQUFFO1FBQzlDLE1BQU0sQ0FBQyxJQUFJLENBQUEsRUFBRSxFQUFFLGNBQWUsQ0FBQyxDQUFDO1FBQ2hDLEtBQUssQ0FBQyxPQUFPLEVBQUUsQ0FBQztJQUNwQixDQUFDLEVBQUUsRUFBRSxJQUFJLEVBQUUsSUFBSSxFQUFFLENBQUMsQ0FBQztJQUVuQixLQUFLLENBQUMsSUFBSSxFQUFFLENBQUM7QUFDakIsQ0FBQztBQUVELFNBQVMsb0JBQW9CLENBQUMsY0FBc0IsRUFBRSxlQUF1QjtJQUN6RSxNQUFNLGVBQWUsR0FBRyxJQUFJLENBQUMsR0FBRyxDQUFDLENBQUMsRUFBRSxjQUFjLEdBQUcsQ0FBQyxDQUFDLENBQUM7SUFDeEQsTUFBTSxjQUFjLEdBQUcsSUFBSSxDQUFDLEdBQUcsQ0FBQyxlQUFlLEVBQUUsY0FBYyxHQUFHLENBQUMsQ0FBQyxDQUFDO0lBRXJFLE1BQU0sbUJBQW1CLEdBQUcsRUFBRSxDQUFDO0lBQy9CLEtBQUksSUFBSSxDQUFDLEdBQUcsZUFBZSxFQUFFLENBQUMsSUFBSSxjQUFjLEVBQUUsQ0FBQyxFQUFFLEVBQUUsQ0FBQztRQUNwRCxtQkFBbUIsQ0FBQyxJQUFJLENBQUMsQ0FBQyxDQUFDLENBQUM7SUFDaEMsQ0FBQztJQUVELE9BQU8sbUJBQW1CLENBQUM7QUFDL0IsQ0FBQztBQUVELFNBQVMscUJBQXFCLENBQzFCLGNBQXNCLEVBQ3RCLGVBQXVCLEVBQ3ZCLGVBQXVCLEVBQ3ZCLGdCQUF3QjtJQUV4QixNQUFNLGlCQUFpQixHQUFHLElBQUksQ0FBQyxHQUFHLENBQUMsY0FBYyxHQUFHLGdCQUFnQixFQUFFLGVBQWUsQ0FBQyxDQUFDO0lBRXZGLElBQUksa0JBQWtCLEdBQUcsaUJBQWlCLEdBQUcsZ0JBQWdCLEdBQUcsQ0FBQyxDQUFDO0lBQ2xFLElBQUksY0FBYyxLQUFLLGVBQWUsRUFBRSxDQUFDO1FBQ3JDLElBQUksZUFBZSxLQUFLLENBQUM7WUFDckIsa0JBQWtCLEdBQUcsQ0FBQyxDQUFDO2FBQ3RCLElBQUksY0FBYyxLQUFLLENBQUMsSUFBSSxlQUFlLEtBQUssQ0FBQztZQUNsRCxrQkFBa0IsR0FBRyxDQUFDLENBQUM7O1lBRXZCLGtCQUFrQixHQUFHLGlCQUFpQixHQUFHLENBQUMsaUJBQWlCLEdBQUcsZ0JBQWdCLENBQUMsR0FBRyxDQUFDLENBQUM7SUFDNUYsQ0FBQztJQUVELE9BQU8sQ0FBQyxrQkFBa0IsRUFBRSxpQkFBaUIsQ0FBQyxDQUFDO0FBQ25ELENBQUMiLCJzb3VyY2VzQ29udGVudCI6WyJpbXBvcnQgKiBhcyBib290c3RyYXAgZnJvbSBcImJvb3RzdHJhcFwiO1xuaW1wb3J0IHsgaHRtbCwgcmVuZGVyLCB0eXBlIFRlbXBsYXRlUmVzdWx0IH0gZnJvbSBcImxpdC1odG1sXCI7XG5cbmltcG9ydCB0eXBlIHsgUHJvZHVjdCwgUHJvZHVjdHNBcHByb3ZhbFN0YXR1cyB9IGZyb20gXCIuLi90eXBlcy9wcm9kdWN0cy50c1wiO1xuaW1wb3J0IHR5cGUgeyBUYWJsZUNvbnRleHQgfSBmcm9tIFwiLi4vdHlwZXMvdGFibGVDb250ZXh0LnRzXCI7XG5cbmltcG9ydCByZW1vdmVQcm9kdWN0VGVtcGxhdGUgZnJvbSBcIi4vY29uZmlybVJlbW92ZVByb2R1Y3REaWFsb2dNb2RhbC5qc1wiO1xuaW1wb3J0IGdldFByb2R1Y3REZXRhaWxzTW9kYWxUZW1wbGF0ZSBmcm9tIFwiLi9wcm9kdWN0RGV0YWlsc01vZGFsVGVtcGxhdGUuanNcIjtcblxuXG5jb25zdCB0YWJsZURpdkNvbnRhaW5lciA9IGRvY3VtZW50LnF1ZXJ5U2VsZWN0b3I8SFRNTERpdkVsZW1lbnQ+KFwiZGl2I3RhYmxlLWNvbnRhaW5lclwiKTtcbmNvbnN0IGRpYWxvZ3NTZWN0aW9uID0gZG9jdW1lbnQucXVlcnlTZWxlY3RvcjxIVE1MRGl2RWxlbWVudD4oXCJkaXYjZGlhbG9ncy1zZWN0aW9uXCIpO1xuXG5leHBvcnQgZGVmYXVsdCBhc3luYyBmdW5jdGlvbiBzaG93UHJvZHVjdHNUYWJsZShjb250ZXh0OiBUYWJsZUNvbnRleHQpIHtcbiAgICBjb25zdCBjdXJyUGFnZVByb2R1Y3RzID0gYXdhaXQgY29udGV4dC5nZXRDdXJyUGFnZVByb2R1Y3RzKCk7XG4gICAgcmVuZGVyKGF3YWl0IHRlbXBsYXRlKGN1cnJQYWdlUHJvZHVjdHMsIGNvbnRleHQpLCB0YWJsZURpdkNvbnRhaW5lciEpO1xufVxuXG5hc3luYyBmdW5jdGlvbiB0ZW1wbGF0ZShcbiAgICBjdXJyUGFnZVByb2R1Y3RzOiBQcm9kdWN0W10sXG4gICAgY29udGV4dDogVGFibGVDb250ZXh0XG4pOiBQcm9taXNlPFRlbXBsYXRlUmVzdWx0PiB7XG4gICAgY29uc3QgaG92ZXJFZmZlY3QgPSBcIm5hdi1saW5rLWJvcmRlci1yYWRpdXMtaG92ZXItZWZmZWN0LWxpZ2h0XCJcblxuICAgIHJldHVybiBodG1sYFxuICAgICAgICA8ZGl2PlxuICAgICAgICAgICAgJHtzZWFyY2hGb3JtVGVtcGxhdGUoY29udGV4dCl9XG4gICAgICAgIDwvZGl2PlxuXG4gICAgICAgIDxkaXYgaWQ9XCJ0YWJsZS13cmFwcGVyXCIgY2xhc3M9XCJtdC0wIHB0LTAgdy0xMDBcIj5cbiAgICAgICAgICAgIDx0YWJsZSBjbGFzcz1cInRhYmxlIHRhYmxlLWhvdmVyIHctMTAwXCI+XG4gICAgICAgICAgICAgICAgPHRoZWFkIGNsYXNzPVwic2l0ZS1zZWN0aW9ucy1iZy10ZWFsIHRleHQtY2VudGVyXCI+XG4gICAgICAgICAgICAgICAgPHRyIGNsYXNzPVwiYWxpZ24tbWlkZGxlXCI+XG4gICAgICAgICAgICAgICAgICAgIDx0aCBjbGFzcz1cIiR7aG92ZXJFZmZlY3R9XCI+IFByb2R1Y3QgPC90aD5cbiAgICAgICAgICAgICAgICAgICAgPHRoIGNsYXNzPVwiJHtob3ZlckVmZmVjdH1cIj4gT3duZXIgPC90aD5cbiAgICAgICAgICAgICAgICAgICAgPHRoIGNsYXNzPVwiJHtob3ZlckVmZmVjdH1cIj4gQ2F0ZWdvcnkgTmFtZSA8L3RoPlxuICAgICAgICAgICAgICAgICAgICA8dGggY2xhc3M9XCIke2hvdmVyRWZmZWN0fVwiPiBBcHByb3ZhbCBTdGF0dXMgPC90aD5cbiAgICAgICAgICAgICAgICAgICAgPHRoIGNsYXNzPVwiJHtob3ZlckVmZmVjdH1cIj4gQWN0aW9ucyA8L3RoPlxuICAgICAgICAgICAgICAgIDwvdHI+XG4gICAgICAgICAgICAgICAgPC90aGVhZD5cblxuICAgICAgICAgICAgICAgIDx0Ym9keSBjbGFzcz1cInRib2R5LXRvcC1ib3JkZXJcIj5cbiAgICAgICAgICAgICAgICAgICAgJHtjdXJyUGFnZVByb2R1Y3RzLm1hcChwID0+IHByb2R1Y3RUYWJsZVJvd1RlbXBsYXRlKHAsIGNvbnRleHQpKX1cbiAgICAgICAgICAgICAgICA8L3Rib2R5PlxuICAgICAgICAgICAgPC90YWJsZT5cblxuICAgICAgICAgICAgPGRpdiBjbGFzcz1cImQtZmxleCBqdXN0aWZ5LWNvbnRlbnQtZW5kIGFsaWduLWl0ZW1zLWNlbnRlciBnYXAtMiBwb3NpdGlvbi1yZWxhdGl2ZSBib3R0b20tMCBlbmQtMFwiPlxuICAgICAgICAgICAgICAgICR7YXdhaXQgY29udHJvbHNUZW1wbGF0ZShjb250ZXh0KX1cbiAgICAgICAgICAgIDwvZGl2PlxuXG4gICAgICAgIDwvZGl2PlxuICAgIGA7XG59XG5cbmZ1bmN0aW9uIHNlYXJjaEZvcm1UZW1wbGF0ZShjb250ZXh0OiBUYWJsZUNvbnRleHQpIHtcbiAgICBjb25zdCBkaXNwbGF5Q2xhc3MgPSBjb250ZXh0LmdldEN1cnJTZWFyY2hRdWVyeSgpLnRyaW0oKSA9PT0gXCJcIiA/IFwiZC1ub25lXCIgOiBcIlwiO1xuXG4gICAgcmV0dXJuIGh0bWxgXG4gICAgICAgIDxkaXYgY2xhc3M9XCJkLWZsZXggZmxleC1jb2x1bW4gYWxpZ24taXRlbXMtY2VudGVyIG10LTUgbWItM1wiIGlkPVwic2VhcmNoLXNlY3Rpb24td3JhcHBlclwiPlxuICAgICAgICAgICAgPGxhYmVsIGZvcj1cInNlYXJjaElucHV0XCIgY2xhc3M9XCJmb3JtLWxhYmVsIHRleHQtbmF2eSB0ZXh0LWNlbnRlclwiPlxuICAgICAgICAgICAgICAgIFNlYXJjaCBmb3IgcHJvZHVjdHNcbiAgICAgICAgICAgIDwvbGFiZWw+XG4gICAgICAgICAgICA8Zm9ybSBpZD1cInNlYXJjaEZvcm1cIiBjbGFzcz1cIm14LW1kLTNcIlxuICAgICAgICAgICAgICAgICAgQHN1Ym1pdD0ke2FzeW5jIChldmVudDogRXZlbnQpID0+IGF3YWl0IG9uU2VhcmNoRm9ybVN1Ym1pdEhhbmRsZXIoZXZlbnQsIGNvbnRleHQpfT5cbiAgICAgICAgICAgICAgICA8ZGl2IGNsYXNzPVwiaW5wdXQtZ3JvdXAtc20gZC1mbGV4IGdhcC0xXCI+XG4gICAgICAgICAgICAgICAgICAgIDxpbnB1dCBuYW1lPVwic2VhcmNoXCIgdHlwZT1cInNlYXJjaFwiIGlkPVwic2VhcmNoSW5wdXRcIlxuICAgICAgICAgICAgICAgICAgICAgICAgICAgY2xhc3M9XCJmb3JtLWNvbnRyb2xcIiBwbGFjZWhvbGRlcj1cIlNlYXJjaC4uLlwiXG4gICAgICAgICAgICAgICAgICAgICAgICAgICBhcmlhLWxhYmVsPVwiU2VhcmNoXCIgLnZhbHVlPSR7Y29udGV4dC5nZXRDdXJyU2VhcmNoUXVlcnkoKX0gLz5cbiAgICAgICAgICAgICAgICAgICAgPGJ1dHRvbiBjbGFzcz1cImJ0biBidG4tb3V0bGluZS10ZWFsXCIgdHlwZT1cInN1Ym1pdFwiPlNlYXJjaDwvYnV0dG9uPlxuICAgICAgICAgICAgICAgIDwvZGl2PlxuICAgICAgICAgICAgPC9mb3JtPlxuICAgICAgICAgICAgPGRpdiBjbGFzcz1cIm15LTIgcG9zaXRpb24tcmVsYXRpdmUgJHtkaXNwbGF5Q2xhc3N9XCIgc3R5bGU9XCJyaWdodDogNXB4XCI+XG4gICAgICAgICAgICAgICAgPHNwYW4+cmVzdWx0cyBmb3I6ICR7Y29udGV4dC5nZXRDdXJyU2VhcmNoUXVlcnkoKX08L3NwYW4+XG4gICAgICAgICAgICAgICAgPGE+XG4gICAgICAgICAgICAgICAgICAgIDxzcGFuIGNsYXNzPVwiYnRuLXNtIGJ0bi1kYW5nZXJcIlxuICAgICAgICAgICAgICAgICAgICAgICAgICBAY2xpY2s9JHthc3luYyAoKSA9PiBhd2FpdCBvbkNsZWFyU2VhcmNoRm9ybShjb250ZXh0KX0+XG4gICAgICAgICAgICAgICAgICAgICAgICB4XG4gICAgICAgICAgICAgICAgICAgIDwvc3Bhbj5cbiAgICAgICAgICAgICAgICA8L2E+XG4gICAgICAgICAgICA8L2Rpdj5cbiAgICAgICAgPC9kaXY+YDtcbn1cblxuYXN5bmMgZnVuY3Rpb24gb25DbGVhclNlYXJjaEZvcm0oY29udGV4dDogVGFibGVDb250ZXh0KSB7XG4gICAgY29udGV4dC5zZXRTZWFyY2hRdWVyeShcIlwiKTtcbiAgICBhd2FpdCBzaG93UHJvZHVjdHNUYWJsZShjb250ZXh0KTtcbn1cblxuYXN5bmMgZnVuY3Rpb24gb25TZWFyY2hGb3JtU3VibWl0SGFuZGxlcihldmVudDogRXZlbnQsIGNvbnRleHQ6IFRhYmxlQ29udGV4dCkge1xuICAgIGV2ZW50LnByZXZlbnREZWZhdWx0KCk7XG5cbiAgICBjb25zdCBmb3JtRGF0YSA9IG5ldyBGb3JtRGF0YShldmVudC5jdXJyZW50VGFyZ2V0IGFzIEhUTUxGb3JtRWxlbWVudCB8IHVuZGVmaW5lZCk7XG4gICAgY29uc3Qgc2VhcmNoUXVlcnkgPSBmb3JtRGF0YS5nZXQoXCJzZWFyY2hcIikgYXMgRm9ybURhdGFFbnRyeVZhbHVlIGFzIHN0cmluZztcbiAgICBpZihzZWFyY2hRdWVyeS50cmltKCkgPT09IFwiXCIgJiYgY29udGV4dC5nZXRDdXJyU2VhcmNoUXVlcnkoKSA9PT0gXCJcIikge1xuICAgICAgICByZXR1cm47XG4gICAgfVxuXG4gICAgY29udGV4dC5zZXRTZWFyY2hRdWVyeShzZWFyY2hRdWVyeSk7XG4gICAgYXdhaXQgc2hvd1Byb2R1Y3RzVGFibGUoY29udGV4dCk7XG59XG5cbmFzeW5jIGZ1bmN0aW9uIGNvbnRyb2xzVGVtcGxhdGUoY29udGV4dDogVGFibGVDb250ZXh0KSB7XG4gICAgY29uc3QgdG90YWxQYWdlc0NvdW50ID0gY29udGV4dC5nZXRQYWdlc1RvdGFsQ291bnQoKTtcbiAgICBjb25zdCBjdXJyUGFnZU51bWJlciA9IGNvbnRleHQuZ2V0Q3VyclBhZ2VOdW1iZXIoKTtcbiAgICBjb25zdCB0b3RhbEl0ZW1zQ291bnQgPSBjb250ZXh0LmdldFByb2R1Y3RzQ291bnQoKTtcbiAgICBjb25zdCBpdGVtc0NvdW50T25QYWdlID0gY29udGV4dC5nZXRDdXJySXRlbXNPblBhZ2VDb3VudCgpO1xuXG4gICAgY29uc3QgcGFnZU51bWJlcnNPblNjcmVlbiA9IGNhbGN1bGF0ZVBhZ2VOdW1iZXJzKGN1cnJQYWdlTnVtYmVyLCB0b3RhbFBhZ2VzQ291bnQpXG5cbiAgICBjb25zdCBbZmlyc3RJdGVtTnVtT25QYWdlLCBsYXN0SXRlbU51bU9uUGFnZV1cbiAgICAgICAgPSBjYWxjdWxhdGVJdGVtc051bWJlcnMoY3VyclBhZ2VOdW1iZXIsIHRvdGFsUGFnZXNDb3VudCwgdG90YWxJdGVtc0NvdW50LCBpdGVtc0NvdW50T25QYWdlKTtcblxuICAgIHJldHVybiBodG1sYFxuICAgICAgICA8cCBjbGFzcz1cInRleHQtbmF2eSB0ZXh0LW11dGVkIGZzLTYgZnN0LWl0YWxpYyBkLWlubGluZVwiPlxuICAgICAgICAgICAgJHtmaXJzdEl0ZW1OdW1PblBhZ2V9LSR7bGFzdEl0ZW1OdW1PblBhZ2V9IGZyb20gJHt0b3RhbEl0ZW1zQ291bnR9XG4gICAgICAgIDwvcD5cbiAgICAgICAgPG5hdiBhcmlhLWxhYmVsPVwiVGFibGUgcGFnaW5hdGlvbiBjb250cm9sLlwiPlxuICAgICAgICAgICAgPHVsIGNsYXNzPVwicGFnaW5hdGlvbiBqdXN0aWZ5LWNvbnRlbnQtY2VudGVyXCI+XG4gICAgICAgICAgICAgICAgJHtjdXJyUGFnZU51bWJlciA8PSAxXG4gICAgICAgICAgICAgICAgICAgICAgICA/IGh0bWxgXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgPGxpIGNsYXNzPVwicGFnZS1pdGVtIGRpc2FibGVkXCI+XG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIDxzcGFuIGNsYXNzPVwicGFnZS1saW5rXCI+UHJldmlvdXM8L3NwYW4+XG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgPC9saT5gXG4gICAgICAgICAgICAgICAgICAgICAgICA6IGh0bWxgXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgPGxpIGNsYXNzPVwicGFnZS1pdGVtXCI+XG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIDxhIGNsYXNzPVwicGFnZS1saW5rXCJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgQGNsaWNrPSR7YXN5bmMgKCkgPT4gYXdhaXQgb25QYWdlTnVtQnRuQ2xpY2soY29udGV4dCwgY3VyclBhZ2VOdW1iZXIgLSAxKX0+XG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICBQcmV2aW91c1xuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICA8L2E+XG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgPC9saT5gXG4gICAgICAgICAgICAgICAgfVxuXG4gICAgICAgICAgICAgICAgJHtwYWdlTnVtYmVyc09uU2NyZWVuLm1hcChwYWdlTnVtID0+IHtcbiAgICAgICAgICAgICAgICAgICAgcmV0dXJuIHBhZ2VOdW0gPT09IGN1cnJQYWdlTnVtYmVyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgPyBodG1sIGBcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgPGxpIGNsYXNzPVwicGFnZS1pdGVtIGFjdGl2ZVwiPlxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICA8c3BhbiBjbGFzcz1cInBhZ2UtbGlua1wiPlxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgJHtjdXJyUGFnZU51bWJlcn1cbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgPC9zcGFuPlxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICA8L2xpPmBcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICA6IGh0bWwgYFxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICA8bGkgY2xhc3M9XCJwYWdlLWl0ZW1cIj5cbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIDxhIGNsYXNzPVwicGFnZS1saW5rXCJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIEBjbGljaz0ke2FzeW5jICgpID0+IGF3YWl0IG9uUGFnZU51bUJ0bkNsaWNrKGNvbnRleHQsIHBhZ2VOdW0pfT5cbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAke3BhZ2VOdW19XG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICA8L2E+XG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIDwvbGk+YFxuICAgICAgICAgICAgICAgIH0pfVxuXG4gICAgICAgICAgICAgICAgJHtjdXJyUGFnZU51bWJlciA9PT0gcGFnZU51bWJlcnNPblNjcmVlbi5sZW5ndGhcbiAgICAgICAgICAgICAgICAgICAgICAgID8gaHRtbGBcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICA8bGkgY2xhc3M9XCJwYWdlLWl0ZW0gZGlzYWJsZWRcIj5cbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgPHNwYW4gY2xhc3M9XCJwYWdlLWxpbmtcIj5OZXh0PC9zcGFuPlxuICAgICAgICAgICAgICAgICAgICAgICAgICAgIDwvbGk+YFxuICAgICAgICAgICAgICAgICAgICAgICAgOiBodG1sYFxuICAgICAgICAgICAgICAgICAgICAgICAgICAgIDxsaSBjbGFzcz1cInBhZ2UtaXRlbVwiPlxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICA8YSBjbGFzcz1cInBhZ2UtbGlua1wiXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIEBjbGljaz0ke2FzeW5jICgpID0+IGF3YWl0IG9uUGFnZU51bUJ0bkNsaWNrKGNvbnRleHQsIGN1cnJQYWdlTnVtYmVyICsgMSl9PlxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgTmV4dFxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICA8L2E+XG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgPC9saT5gXG4gICAgICAgICAgICAgICAgfVxuICAgICAgICAgICAgPC91bD5cbiAgICAgICAgPC9uYXY+YDtcbn1cblxuYXN5bmMgZnVuY3Rpb24gb25QYWdlTnVtQnRuQ2xpY2soY29udGV4dDogVGFibGVDb250ZXh0LCBwYWdlTnVtYmVyOiBudW1iZXIpIHtcbiAgICBjb250ZXh0LnNldFBhZ2VOdW1iZXIocGFnZU51bWJlcik7XG4gICAgYXdhaXQgc2hvd1Byb2R1Y3RzVGFibGUoY29udGV4dCk7XG59XG5cbmZ1bmN0aW9uIHByb2R1Y3RUYWJsZVJvd1RlbXBsYXRlKHByb2R1Y3Q6IFByb2R1Y3QsIGNvbnRleHQ6IFRhYmxlQ29udGV4dCkge1xuICAgIGNvbnN0IGFwcHJvdmFsU3RhdHVzVGRNYXAgPSB7XG4gICAgICAgIFwiQXBwcm92ZWRcIjogKCkgPT4gW1wi8J+folwiLCBcIkFwcHJvdmVkXCIsIFwidGV4dC1zdWNjZXNzIGZ3LXNlbWlib2xkXCJdLFxuICAgICAgICBcIldhaXRpbmdBcHByb3ZhbFwiOiAoKSA9PiBbXCLwn5+hXCIsIFwiV2FpdGluZyBBcHByb3ZhbFwiLCBcInRleHQtd2FybmluZyBmdy1zZW1pYm9sZFwiXSxcbiAgICAgICAgXCJEaXNhcHByb3ZlZFwiOiAoKSA9PiBbXCLwn5S0XCIsIFwiRGlzYXBwcm92ZWRcIiwgXCJ0ZXh0LWRhbmdlciBmdy1zZW1pYm9sZFwiXSxcbiAgICB9IGFzIFJlY29yZDxQcm9kdWN0c0FwcHJvdmFsU3RhdHVzLCAoKSA9PiByZWFkb25seSBbZG90OiBzdHJpbmcsIGNvbnRlbnQ6IHN0cmluZywgc3R5bGVzOiBzdHJpbmddPjtcblxuICAgIGNvbnN0IFtkb3QsIGNvbnRlbnQsIHN0eWxlc10gPSBhcHByb3ZhbFN0YXR1c1RkTWFwW3Byb2R1Y3QhLmFwcHJvdmFsU3RhdHVzXSEoKTtcblxuICAgIHJldHVybiBodG1sYFxuICAgICAgICA8dHIgY2xhc3M9XCJ0ZXh0LWNlbnRlciBhbGlnbi1taWRkbGVcIj5cbiAgICAgICAgICAgIDx0ZD4ke3Byb2R1Y3QubmFtZX08L3RkPlxuICAgICAgICAgICAgPHRkPiR7cHJvZHVjdC5vd25lck5hbWV9PC90ZD5cbiAgICAgICAgICAgIDx0ZD4ke3Byb2R1Y3QuY2F0ZWdvcnlOYW1lfTwvdGQ+XG4gICAgICAgICAgICA8dGQgY2xhc3M9XCIke3N0eWxlc31cIj4ke2RvdH0gJHtjb250ZW50fTwvdGQ+XG4gICAgICAgICAgICA8dGQ+XG4gICAgICAgICAgICAgICAgPGRpdiBjbGFzcz1cImJ0bi1ncm91cC1zbSBkLWZsZXggZmxleC13cmFwIGp1c3RpZnktY29udGVudC1jZW50ZXIgZ2FwLTEgZ2FwLXNtLTIgZ2FwLW1kLTJcIj5cbiAgICAgICAgICAgICAgICAgICAgPGJ1dHRvbiBjbGFzcz1cImJ0biByb3VuZGVkLXBpbGwgYnRuLXRlYWwgYnRuLXNtIHctMTAwXCIgc3R5bGU9XCJtYXgtd2lkdGg6IDEyZW1cIlxuICAgICAgICAgICAgICAgICAgICAgICAgICAgIEBjbGljaz0ke2FzeW5jICgpID0+IGF3YWl0IG9uVmlld1Byb2R1Y3REZXRhaWxzSGFuZGxlcihwcm9kdWN0LmlkLCBjb250ZXh0KX0+XG4gICAgICAgICAgICAgICAgICAgICAgICBWaWV3IERldGFpbHNcbiAgICAgICAgICAgICAgICAgICAgPC9idXR0b24+XG5cbiAgICAgICAgICAgICAgICAgICAgPGJ1dHRvbiBjbGFzcz1cImJ0biByb3VuZGVkLXBpbGwgYnRuLW91dGxpbmUtZGFuZ2VyIGJ0bi1zbSB3LTEwMFwiIHN0eWxlPVwibWF4LXdpZHRoOiAxMmVtXCJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICBAY2xpY2s9JHthc3luYyAoKSA9PiBhd2FpdCBvblJlbW92ZVByb2R1Y3RIYW5kbGVyKHByb2R1Y3QsIGNvbnRleHQpfT5cbiAgICAgICAgICAgICAgICAgICAgICAgIFJlbW92ZSBQcm9kdWN0XG4gICAgICAgICAgICAgICAgICAgIDwvYnV0dG9uPlxuICAgICAgICAgICAgICAgIDwvZGl2PlxuICAgICAgICAgICAgPC90ZD5cbiAgICAgICAgPC90cj5cbiAgICBgO1xufVxuXG5hc3luYyBmdW5jdGlvbiBvblZpZXdQcm9kdWN0RGV0YWlsc0hhbmRsZXIocHJvZHVjdElkOiBzdHJpbmcsIGNvbnRleHQ6IFRhYmxlQ29udGV4dCkge1xuICAgIHJlbmRlcihhd2FpdCBnZXRQcm9kdWN0RGV0YWlsc01vZGFsVGVtcGxhdGUocHJvZHVjdElkLCBjb250ZXh0KSwgZGlhbG9nc1NlY3Rpb24hKTtcblxuICAgIGNvbnN0IHByb2R1Y3REZXRhaWxzTW9kYWxJZCA9IGBwcm9kdWN0LWRldGFpbHMtJHtwcm9kdWN0SWR9YDtcbiAgICBjb25zdCBtb2RhbEVsID0gZGlhbG9nc1NlY3Rpb24/LnF1ZXJ5U2VsZWN0b3I8SFRNTERpdkVsZW1lbnQ+KGBkaXYjJHtwcm9kdWN0RGV0YWlsc01vZGFsSWR9YCk7XG4gICAgY29uc3QgbW9kYWwgPSBib290c3RyYXAuTW9kYWwuZ2V0T3JDcmVhdGVJbnN0YW5jZShtb2RhbEVsISk7XG5cbiAgICBtb2RhbEVsPy5hZGRFdmVudExpc3RlbmVyKCdoaWRkZW4uYnMubW9kYWwnLCAoKSA9PiB7XG4gICAgICAgIHJlbmRlcihodG1sYGAsIGRpYWxvZ3NTZWN0aW9uISk7XG4gICAgICAgIG1vZGFsLmRpc3Bvc2UoKTtcbiAgICB9LCB7IG9uY2U6IHRydWUgfSk7XG5cbiAgICBtb2RhbC5zaG93KCk7XG59XG5cbmFzeW5jIGZ1bmN0aW9uIG9uUmVtb3ZlUHJvZHVjdEhhbmRsZXIocHJvZHVjdDogUHJvZHVjdCwgY29udGV4dDogVGFibGVDb250ZXh0KSB7XG4gICAgcmVuZGVyKHJlbW92ZVByb2R1Y3RUZW1wbGF0ZShwcm9kdWN0LCBjb250ZXh0KSwgZGlhbG9nc1NlY3Rpb24hKTtcblxuICAgIGNvbnN0IGRlbGV0ZVByb2R1Y3RNb2RhbElkID0gYHJlbW92ZS1wcm9kdWN0LSR7cHJvZHVjdC5pZH1gO1xuICAgIGNvbnN0IG1vZGFsRWwgPSBkaWFsb2dzU2VjdGlvbj8ucXVlcnlTZWxlY3RvcjxIVE1MRGl2RWxlbWVudD4oYGRpdiMke2RlbGV0ZVByb2R1Y3RNb2RhbElkfWApXG4gICAgY29uc3QgbW9kYWwgPSBib290c3RyYXAuTW9kYWwuZ2V0T3JDcmVhdGVJbnN0YW5jZShtb2RhbEVsISk7XG5cbiAgICBtb2RhbEVsPy5hZGRFdmVudExpc3RlbmVyKCdoaWRkZW4uYnMubW9kYWwnLCAoKSA9PiB7XG4gICAgICAgIHJlbmRlcihodG1sYGAsIGRpYWxvZ3NTZWN0aW9uISk7XG4gICAgICAgIG1vZGFsLmRpc3Bvc2UoKTtcbiAgICB9LCB7IG9uY2U6IHRydWUgfSk7XG5cbiAgICBtb2RhbC5zaG93KCk7XG59XG5cbmZ1bmN0aW9uIGNhbGN1bGF0ZVBhZ2VOdW1iZXJzKGN1cnJQYWdlTnVtYmVyOiBudW1iZXIsIHRvdGFsUGFnZXNDb3VudDogbnVtYmVyKSB7XG4gICAgY29uc3QgZmlyc3RQYWdlTnVtYmVyID0gTWF0aC5tYXgoMSwgY3VyclBhZ2VOdW1iZXIgLSAzKTtcbiAgICBjb25zdCBsYXN0UGFnZU51bWJlciA9IE1hdGgubWluKHRvdGFsUGFnZXNDb3VudCwgY3VyclBhZ2VOdW1iZXIgKyAzKTtcblxuICAgIGNvbnN0IHBhZ2VOdW1iZXJzT25TY3JlZW4gPSBbXTtcbiAgICBmb3IobGV0IGkgPSBmaXJzdFBhZ2VOdW1iZXI7IGkgPD0gbGFzdFBhZ2VOdW1iZXI7IGkrKykge1xuICAgICAgICBwYWdlTnVtYmVyc09uU2NyZWVuLnB1c2goaSk7XG4gICAgfVxuXG4gICAgcmV0dXJuIHBhZ2VOdW1iZXJzT25TY3JlZW47XG59XG5cbmZ1bmN0aW9uIGNhbGN1bGF0ZUl0ZW1zTnVtYmVycyhcbiAgICBjdXJyUGFnZU51bWJlcjogbnVtYmVyLFxuICAgIHRvdGFsUGFnZXNDb3VudDogbnVtYmVyLFxuICAgIHRvdGFsSXRlbXNDb3VudDogbnVtYmVyLFxuICAgIGl0ZW1zQ291bnRPblBhZ2U6IG51bWJlclxuKTogcmVhZG9ubHkgW2ZpcnN0SXRlbU51bU9uUGFnZTogbnVtYmVyLCBsYXN0SXRlbU51bU9uUGFnZTogbnVtYmVyXSB7XG4gICAgY29uc3QgbGFzdEl0ZW1OdW1PblBhZ2UgPSBNYXRoLm1pbihjdXJyUGFnZU51bWJlciAqIGl0ZW1zQ291bnRPblBhZ2UsIHRvdGFsSXRlbXNDb3VudCk7XG5cbiAgICBsZXQgZmlyc3RJdGVtTnVtT25QYWdlID0gbGFzdEl0ZW1OdW1PblBhZ2UgLSBpdGVtc0NvdW50T25QYWdlICsgMTtcbiAgICBpZiAoY3VyclBhZ2VOdW1iZXIgPT09IHRvdGFsUGFnZXNDb3VudCkge1xuICAgICAgICBpZiAodG90YWxJdGVtc0NvdW50ID09PSAwKVxuICAgICAgICAgICAgZmlyc3RJdGVtTnVtT25QYWdlID0gMDtcbiAgICAgICAgZWxzZSBpZiAoY3VyclBhZ2VOdW1iZXIgPT09IDEgJiYgdG90YWxJdGVtc0NvdW50ICE9PSAwKVxuICAgICAgICAgICAgZmlyc3RJdGVtTnVtT25QYWdlID0gMTtcbiAgICAgICAgZWxzZVxuICAgICAgICAgICAgZmlyc3RJdGVtTnVtT25QYWdlID0gbGFzdEl0ZW1OdW1PblBhZ2UgLSAobGFzdEl0ZW1OdW1PblBhZ2UgJSBpdGVtc0NvdW50T25QYWdlKSArIDE7XG4gICAgfVxuXG4gICAgcmV0dXJuIFtmaXJzdEl0ZW1OdW1PblBhZ2UsIGxhc3RJdGVtTnVtT25QYWdlXTtcbn0iXX0=