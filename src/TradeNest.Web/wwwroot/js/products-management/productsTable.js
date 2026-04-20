import { html, render } from "../../lib/lit/lit.js";

import removeProductTemplate from "./confirmRemoveProductDialogModal.js";
import getProductDetailsModalTemplate from "./productDetailsModalTemplate.js";


const tableDivContainer = document.querySelector("div#table-container");
const dialogsSection = document.querySelector("div#dialogs-section");

export default async function showProductsTable(context) {
    const currPageProducts = await context.getCurrPageProducts();
    render(await template(currPageProducts, context), tableDivContainer);
}

async function template(currPageProducts, context) {
    const hoverEffect = "nav-link-border-radius-hover-effect-light"

    return html`
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
    
    return html`
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
                          @click=${async (event) => await onClearSearchForm(event, context)}>
                        x
                    </span>
                </a>
            </div>
        </div>`;
}

async function onClearSearchForm(event, context) {
    context.setSearchQuery("");
    await showProductsTable(context);
}

async function onSearchFormSubmitHandler(event, context) {
    event.preventDefault();

    const formData = new FormData(event.currentTarget);
    const searchQuery = formData.get("search");
    if(searchQuery.trim() === "" && context.getCurrSearchQuery() === "") {
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
    
    const pageNumbersOnScreen = calculatePageNumbers(currPageNumber, totalPagesCount)
    
    const [firstItemNumOnPage, lastItemNumOnPage] 
        = calculateItemsNumbers(currPageNumber, totalPagesCount, totalItemsCount, itemsCountOnPage);
    
    return html`
        <p class="text-navy text-muted fs-6 fst-italic d-inline">
            ${firstItemNumOnPage}-${lastItemNumOnPage} from ${totalItemsCount}
        </p>
        <nav aria-label="Table pagination control.">
            <ul class="pagination justify-content-center">
                ${currPageNumber <= 1
                        ? html`
                            <li class="page-item disabled">
                                <span class="page-link">Previous</span>
                            </li>`
                        : html`
                            <li class="page-item">
                                <a class="page-link"
                                   @click=${async () => await onPageNumBtnClick(context, currPageNumber - 1)}>
                                    Previous
                                </a>
                            </li>`
                }

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
                                </li>`
                })}

                ${currPageNumber === pageNumbersOnScreen.length
                        ? html`
                            <li class="page-item disabled">
                                <span class="page-link">Next</span>
                            </li>`
                        : html`
                            <li class="page-item">
                                <a class="page-link"
                                   @click=${async () => await onPageNumBtnClick(context, currPageNumber + 1)}>
                                    Next
                                </a>
                            </li>`
                }
            </ul>
        </nav>`;
}

async function onPageNumBtnClick(context, pageNumber) {
    context.setPageNumber(pageNumber);
    await showProductsTable(context);
}

function productTableRowTemplate(product, context) {
    const approvalStatusTdMap = {
        "Approved": () => [ "🟢", "Approved", "text-success fw-semibold" ],
        "WaitingApproval": () => [ "🟡", "Waiting Approval", "text-warning fw-semibold" ],
        "Disapproved": () => [ "🔴", "Disapproved", "text-danger fw-semibold" ],
    };
    const [dot, content, styles] = approvalStatusTdMap[product.approvalStatus]();

    return html `
        <tr class="text-center align-middle">
            <td>${product.name}</td>
            ${product.ownerName == null 
                    ? html`<td>Account Deleted</td>`
                    : html`<td>${product.ownerName}</td>`}
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
    const modalEl = dialogsSection.querySelector(`div#${productDetailsModalId}`);
    const modal = bootstrap.Modal.getOrCreateInstance(modalEl);

    modalEl.addEventListener('hidden.bs.modal', () => {
        render(html``, dialogsSection);
        modal.dispose();
    }, { once: true });
    
    modal.show();
}

async function onRemoveProductHandler(product, context) {
    render(removeProductTemplate(product, context), dialogsSection);
    
    const deleteProductModalId = `remove-product-${product.id}`;
    const modalEl = dialogsSection.querySelector(`div#${deleteProductModalId}`)
    const modal = bootstrap.Modal.getOrCreateInstance(modalEl);
    
    modalEl.addEventListener('hidden.bs.modal', () => {
        render(html``, dialogsSection);
        modal.dispose();
    }, { once: true });

    modal.show();
}

function calculatePageNumbers(currPageNumber, totalPagesCount) {
    const firstPageNumber = Math.max(1, currPageNumber - 3);
    const lastPageNumber = Math.min(totalPagesCount, currPageNumber + 3);
    
    const pageNumbersOnScreen = [];
    for(let i = firstPageNumber; i <= lastPageNumber; i++) {
        pageNumbersOnScreen.push(i);
    }

    return pageNumbersOnScreen;
}

function calculateItemsNumbers(currPageNumber, totalPagesCount, totalItemsCount, itemsCountOnPage) {
    const lastItemNumOnPage = Math.min(currPageNumber * itemsCountOnPage, totalItemsCount);
    
    let firstItemNumOnPage = lastItemNumOnPage - itemsCountOnPage + 1;
    if(currPageNumber === totalPagesCount) {
        if(totalItemsCount === 0)
            firstItemNumOnPage = 0;
        else if(currPageNumber === 1 && totalItemsCount !== 0)
            firstItemNumOnPage = 1;
        else
            firstItemNumOnPage = lastItemNumOnPage - (lastItemNumOnPage % itemsCountOnPage) + 1;
    }
    
    return [firstItemNumOnPage, lastItemNumOnPage];
}