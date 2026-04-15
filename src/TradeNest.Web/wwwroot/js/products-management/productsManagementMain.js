import { html, render } from "../../lib/lit/lit.js";

import { toggleHighlight } from "./domUtils.js";
import showProductsTable from "./productsTable.js";
import getNewPaginatorInstance from "./tablePaginator.js";


const START_PAGE_NUMBER = 1;
const PRODUCTS_PER_PAGE_COUNT = 5;

const controlsDivEl = document.querySelector("div#controls");

document.addEventListener("DOMContentLoaded", async () => {
    render(mainTemplate(), controlsDivEl);
    await showProducts("Approved");
});

function mainTemplate() {
    return html`
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

    const newPaginator = getNewPaginatorInstance({
        productsApprovalStatus: approvalStatus,
        startPageNumber: START_PAGE_NUMBER,
        productsPerPageCount: PRODUCTS_PER_PAGE_COUNT,
    });
    
    await showProductsTable(newPaginator);
}