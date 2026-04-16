import { html, render } from "../../lib/lit/lit.js";

import removeProductTemplate from "./confirmRemoveProductDialogModal.js";


const tableDivContainer = document.querySelector("div#table-container");
const dialogsSection = document.querySelector("div#dialogs-section");

export default async function showProductsTable(paginator) {
    const currPageProducts = await paginator.getCurrPageProducts();
    render(await template(currPageProducts, paginator), tableDivContainer);
}

async function template(currPageProducts, paginator) {
    const hoverEffect = "nav-link-border-radius-hover-effect-light"

    return html`
        <div>
            ${searchFormTemplate(paginator)}
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
                    ${currPageProducts.map(p => productTableRowTemplate(p, paginator))}
                </tbody>
            </table>
            
            <div class="d-flex justify-content-end align-items-center gap-2 position-relative bottom-0 end-0">
                ${await controlsTemplate(paginator)}
            </div>
            
        </div>
    `;
}

function searchFormTemplate(paginator) {
    const displayClass = paginator.getCurrSearchQuery().trim() === "" ? "d-none" : "";
    
    return html`
        <div class="d-flex flex-column align-items-center mt-5 mb-3" id="search-section-wrapper">
            <label for="searchInput" class="form-label text-navy text-center">
                Search for products
            </label>
            <form id="searchForm" class="mx-md-3"
                  @submit=${async (event) => await onSearchFormSubmitHandler(event, paginator)}>
                <div class="input-group-sm d-flex gap-1">
                    <input name="search" type="search" id="searchInput"
                           class="form-control" placeholder="Search..."
                           aria-label="Search" .value=${paginator.getCurrSearchQuery()} />
                    <button class="btn btn-outline-teal" type="submit">Search</button>
                </div>
            </form>
            <div class="my-2 position-relative ${displayClass}" style="right: 5px">
                <span>results for: ${paginator.getCurrSearchQuery()}</span>
                <a>
                    <span class="btn-sm btn-danger"
                          @click=${async (event) => await onClearSearchForm(event, paginator)}>
                        x
                    </span>
                </a>
            </div>
        </div>`;
}

async function onClearSearchForm(event, paginator) {
    paginator.setSearchQuery("");
    await showProductsTable(paginator);
}

async function onSearchFormSubmitHandler(event, paginator) {
    event.preventDefault();

    const formData = new FormData(event.currentTarget);
    const searchQuery = formData.get("search");
    if(searchQuery.trim() === "" && paginator.getCurrSearchQuery() === "") {
        return;
    }

    paginator.setSearchQuery(searchQuery);
    await showProductsTable(paginator);
}

async function controlsTemplate(paginator) {
    const totalPagesCount = await paginator.getPagesTotalCount();
    const currPageNumber = paginator.getCurrPageNumber();
    const totalItemsCount = paginator.getProductsCount();
    const itemsCountOnPage = paginator.getCurrItemsOnPageCount();
    
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
                                   @click=${async () => await onPageNumBtnClick(paginator, currPageNumber - 1)}>
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
                                       @click=${async () => await onPageNumBtnClick(paginator, pageNum)}>
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
                                   @click=${async () => await onPageNumBtnClick(paginator, currPageNumber + 1)}>
                                    Next
                                </a>
                            </li>`
                }
            </ul>
        </nav>`;
}

async function onPageNumBtnClick(paginator, pageNumber) {
    paginator.setPageNumber(pageNumber);
    await showProductsTable(paginator);
}

function productTableRowTemplate(product, paginator) {
    const approvalStatusTdMap = {
        "Approved": () => [ "🟢", "Approved", "text-success fw-semibold" ],
        "WaitingApproval": () => [ "🟡", "Waiting Approval", "text-warning fw-semibold" ],
        "Disapproved": () => [ "🔴", "Disapproved", "text-danger fw-semibold" ],
    };
    const [dot, content, styles] = approvalStatusTdMap[product.approvalStatus]();

    return html `
        <tr class="text-center align-middle">
            <td>${product.name}</td>
            <td>${product.ownerName}</td>
            <td>${product.categoryName}</td>
            <td class="${styles}">${dot} ${content}</td>
            <td>
                <div class="btn-group-sm d-flex flex-wrap justify-content-center gap-1 gap-sm-2 gap-md-3">
                    <button class="btn rounded-pill btn-teal btn-sm w-100" style="max-width: 12em"
                            @click=${async () => await onViewProductDetailsHandler(product.id)}>
                        View Details
                    </button>

                    <button class="btn rounded-pill btn-outline-danger btn-sm w-100" style="max-width: 12em"
                            @click=${async () => await onRemoveProductHandler(product, paginator)}>
                        Remove Product
                    </button>
                </div>
            </td>
        </tr>
    `;
}

async function onViewProductDetailsHandler(productId) {
    alert(`View product details. id:${productId}`)
}

async function onRemoveProductHandler(product, paginator) {
    render(removeProductTemplate(product, paginator), dialogsSection);
    
    const deleteProductModalId = `remove-product-${product.id}`;
    const modalEl = dialogsSection.querySelector(`div#${deleteProductModalId}`)
    const modal = bootstrap.Modal.getOrCreateInstance(modalEl);
    
    modal.toggle();
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
        else
            firstItemNumOnPage = lastItemNumOnPage - (lastItemNumOnPage % itemsCountOnPage) + 1;
    }
    
    return [firstItemNumOnPage, lastItemNumOnPage];
}