import productsService from "../api/productsService.js";

const DEFAULT_PRODUCTS_APPROVAL_STATUS = "Approved";
const DEFAULT_START_PAGE_NUMBER = 1;
const DEFAULT_PRODUCTS_PER_PAGE_COUNT = 5;

export default function getNewPaginatorInstance(stateConfig) {
    const state = {
        productsApprovalStatus: stateConfig?.productsApprovalStatus ?? DEFAULT_PRODUCTS_APPROVAL_STATUS,
        currPageNumber: stateConfig?.startPageNumber ?? DEFAULT_START_PAGE_NUMBER,
        productsPerPageCount: stateConfig?.productsPerPageCount ?? DEFAULT_PRODUCTS_PER_PAGE_COUNT,
        searchQuery: undefined,
        totalPagesCount: undefined
    };

    async function getPagesTotalCount() {
        const productsCount = await productsService
            .getProductsCount(state.productsApprovalStatus, state.searchQuery)

        const totalPagesCount 
            = Math.max(1, Math.ceil(productsCount / state.productsPerPageCount));

        state.totalPagesCount = totalPagesCount;
        return totalPagesCount;
    }

    async function getCurrPageProducts() {
        return await productsService.getCurrPageProducts(
            state.currPageNumber,
            state.productsPerPageCount,
            state.productsApprovalStatus,
            state.searchQuery
        );
    }
    
    function setSearchQuery(searchQuery) {
        if(searchQuery.trim() === "" && state.searchQuery === "") {
            return;
        }
        
        state.searchQuery = searchQuery;
        state.currPageNumber = 1;
    }

    function setPageNumber(pageNumber) {
        if(pageNumber < 1 ||
            state.totalPagesCount !== undefined && pageNumber > state.totalPagesCount
        ) {
            return;
        }
        
        state.currPageNumber = pageNumber;
    }

    return {
        getCurrSearchQuery: () => state.searchQuery,
        setSearchQuery,
        getCurrPageNumber: () => state.currPageNumber,
        getPagesTotalCount,
        getCurrPageProducts,
        setPageNumber,
    }; 
}