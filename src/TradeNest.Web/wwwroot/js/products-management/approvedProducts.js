import { html, render } from "../../lib/lit/lit.js";
import getNewPaginatorInstance from "./tablePaginator.js";

const tableDivContainer = document.querySelector("div#table-container");

export default async function showApprovedProductsTable(paginator) {
    paginator ??= getNewPaginatorInstance({
        productsApprovalStatus: "Approved",
        startPageNumber: 1,
        productsPerPageCount: 5,
    });

    const currPageProducts = await paginator.getCurrPageProducts();
    render(await template(currPageProducts, paginator), tableDivContainer);
}

async function template(currPageProducts, paginator) {
    const hoverEffect = "nav-link-border-radius-hover-effect-light"

    return html`
        ${searchFormTemplate(paginator)}
        ${await controlsTemplate(paginator)}
        <div id="table-wrapper">
            <table class="table table-hover caption-top">
                <caption>List of Approved Products</caption>

                <thead class="site-sections-bg-teal text-center">
                <tr>
                    <th class="${hoverEffect}"> Product </th>
                    <th class="${hoverEffect}"> Owner </th>
                    <th class="${hoverEffect}"> Unit Price </th>
                    <th class="${hoverEffect}"> Sales Count </th>
                    <th class="${hoverEffect}"> Actions </th>
                </tr>
                </thead>

                <tbody class="tbody-top-border">
                    ${currPageProducts.map(p => approvedProductTableRowTemplate(p))}
                </tbody>
            </table>
        </div>
    `;
}

function approvedProductTableRowTemplate(product) {
    return html `
        <tr class="text-center">
            <td>${product.name}</td>
            <td>${product.ownerUsername}</td>
            <td>${product.unitPrice}</td>
            <td>${product.salesCount}</td>
            <td>
                <div class="btn-group-sm d-flex flex-wrap justify-content-center gap-1 gap-sm-2 gap-md-3">

                    <button class="btn rounded-pill btn-teal btn-sm">
                        View Details
                    </button>

                    <button class="btn rounded-pill btn-danger btn-sm">
                        Disapprove Product
                    </button>

                    <button class="btn rounded-pill btn-outline-danger btn-sm">
                        Remove Product
                    </button>

                </div>
            </td>
        </tr>
    `;
}

function searchFormTemplate(paginator) {
    return html`
        <div class="d-flex flex-column align-items-center">
            <label for="searchInput" class="form-label text-navy text-center">
                Search for products
            </label>
            <form id="searchForm" 
                  @submit=${async (event) => await onSearchFormSubmitHandler(event, paginator)}>
                <div class="input-group">
                    <input name="search" type="search" id="searchInput"
                           class="form-control" placeholder="Search..."
                           aria-label="Search" value=${paginator.getCurrSearchQuery()} />
                    <button class="btn btn-outline-teal" type="submit">Search</button>
                </div>
            </form>
        </div>`;
}

async function onSearchFormSubmitHandler(event, paginator) {
    event.preventDefault();

    const formData = new FormData(event.currentTarget);
    const searchQuery = formData.get("search");
    if(searchQuery.trim() === "" && paginator.getCurrSearchQuery() === "") {
        return;
    }

    paginator.setSearchQuery(searchQuery);
    await showApprovedProductsTable(paginator);
}

async function controlsTemplate(paginator) {
    const currPageNumber = paginator.getCurrPageNumber();
    const pagesTotalCount = await paginator.getPagesTotalCount()

    const firstPageNumber = Math.max(1, currPageNumber - 3);
    const lastPageNumber = Math.min(pagesTotalCount, currPageNumber + 3);
    const pageNumbersOnScreen = buildArrOfPageNumbers(firstPageNumber, lastPageNumber)

    return html`
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

                ${currPageNumber === lastPageNumber
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
    await showApprovedProductsTable(paginator);
}

// function onViewProductDetailsHandler(productId) {
//
// }
//
// function onDisapproveProductHandler(productId) {
//
// } 
//
// function onRemoveProductHandler(productId) {
//
// }

function buildArrOfPageNumbers(firstPageNumber, lastPageNumber) {
    const pageNumbersOnScreen = [];
    for(let i = firstPageNumber; i <= lastPageNumber; i++) {
        pageNumbersOnScreen.push(i);
    }

    return pageNumbersOnScreen;
}