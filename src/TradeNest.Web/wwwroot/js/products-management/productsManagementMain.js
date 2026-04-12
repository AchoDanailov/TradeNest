import { html, render } from "../../lib/lit/lit.js";

import { toggleHighlight } from "./domUtils.js";
import showApprovedProductsTable from "./approvedProducts.js";
// import showNotApprovedProductsTable from "./notApprovedProducts.js";


const controlsDivEl = document.querySelector("div#controls");

document.addEventListener("DOMContentLoaded", async () => {
    render(mainTemplate(), controlsDivEl);
    await viewApprovedProductsHandler();
});

function mainTemplate() {
    return html`
        <div class="btn-group btn-group-lg" id="chart-controller" role="group" aria-label="Chart data control buttons.">
            <input type="radio" class="btn-check" name="btnradio" id="btnradio1" 
                   @change=${async () => await viewApprovedProductsHandler()}
                   autocomplete="off">
            <label class="btn btn-teal" for="btnradio1">Approved</label>

            <input type="radio" class="btn-check" name="btnradio" id="btnradio2"
                   @change=${async () => await viewNotApprovedProductsHandler()}
                   autocomplete="off">
            <label class="btn btn-outline-teal" for="btnradio2">Not Approved</label>
        </div>
    `;
}

async function viewApprovedProductsHandler() {
    toggleHighlight("Approved");
    await showApprovedProductsTable();
}

async function viewNotApprovedProductsHandler() {
    toggleHighlight("Not Approved");
    render(html`<div>works</div>`, document.querySelector("div#table-container"));
    // await showNotApprovedProductsTable();
}
