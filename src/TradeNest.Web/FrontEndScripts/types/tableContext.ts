import type {
    EditProductApprovalStatus,
    Product,
    ProductDetails,
    ProductsApprovalStatus
} from "./products.ts";

export type StateConfig = {
    productsApprovalStatus: ProductsApprovalStatus,
    startPageNumber: number,
    productsPerPageCount: number,
}

export type TableContext = {
    getProductsCount: () => number,
    getCurrItemsOnPageCount: () => number,
    getCurrPageNumber: () => number,
    getCurrSearchQuery: () => string,
    getPagesTotalCount: () => number,
    setPageNumber: (pageNumber: number) => void,
    setSearchQuery: (searchQuery: string) => void,
    getCurrPageProducts: () => Promise<Product[]>,
    getProductDetails: (productId: string) => Promise<ProductDetails | null>,
    modifyProductApproval: (productApprovalData: EditProductApprovalStatus)
        => Promise<boolean>,
    removeProduct: (productId: string) => Promise<boolean>,
}