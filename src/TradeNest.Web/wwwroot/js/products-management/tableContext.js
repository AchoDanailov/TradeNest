import productsService from "../api/productsService.js";

const DEFAULT_PRODUCTS_APPROVAL_STATUS = "Approved";
const DEFAULT_START_PAGE_NUMBER = 1;
const DEFAULT_PRODUCTS_PER_PAGE_COUNT = 5;

export default function getNewContextInstance(stateConfig) {
    const state = {
        productsApprovalStatus: stateConfig?.productsApprovalStatus ?? DEFAULT_PRODUCTS_APPROVAL_STATUS,
        currPageNumber: stateConfig?.startPageNumber ?? DEFAULT_START_PAGE_NUMBER,
        productsPerPageCount: stateConfig?.productsPerPageCount ?? DEFAULT_PRODUCTS_PER_PAGE_COUNT,
        xsrfToken: "",
        searchQuery: "",
        productsCount: undefined,
        totalPagesCount: undefined,
    };
    
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

    async function getCurrPageProducts() {
        const { products, metaData } = await productsService.getCurrPageProducts(
            state.currPageNumber,
            state.productsPerPageCount,
            state.productsApprovalStatus,
            state.searchQuery
        );
        
        const totalPagesCount 
            = Math.max(1, Math.ceil(metaData.totalSpecifiedProductsCount / state.productsPerPageCount));

        state.productsCount = metaData.totalSpecifiedProductsCount;
        state.totalPagesCount = totalPagesCount;
        state.xsrfToken = metaData.xsrfToken;
        
        return products;
    }
    
    async function getProductDetails(productId) {
        return await productsService
            .getCurrProdDetails(productId);
    }
    
    async function modifyProductApproval(productApprovalData) {
        return await productsService
            .changeProductApprovalStatus(productApprovalData, state.xsrfToken);
    }
    
    async function removeProduct(productId) {
        return await productsService
            .removeProduct(productId, state.xsrfToken);
    }

    return {
        getProductsCount: () => state.productsCount,
        getCurrItemsOnPageCount: () => state.productsPerPageCount,
        getCurrPageNumber: () => state.currPageNumber,
        getCurrSearchQuery: () => state.searchQuery,
        getPagesTotalCount: () => state.totalPagesCount,
        setPageNumber,
        setSearchQuery,
        getCurrPageProducts,
        getProductDetails,
        modifyProductApproval,
        removeProduct,
    } 
}