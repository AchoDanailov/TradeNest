import type {
    CartProduct,
    EditProductApprovalStatus,
    MetaData,
    Product,
    ProductDetails,
    ProductsApprovalStatus,
    RequestCartProductQty,
    UpdateCartProductQty
} from "./products.ts"

export type CartsService = {
    getProdInCart: (productId: string) => Promise<CartProduct | null>,
    addToCart: (
        requestedProductQtyPayload: RequestCartProductQty,
        requestVerificationToken: string
    ) => Promise<boolean>,
    updateCartProductQty: (
        updatedCartProdPayload: UpdateCartProductQty,
        requestVerificationToken: string
    ) => Promise<boolean>
}

export type ProductsService = {
    getCurrPageProducts: (
        page: number,
        productsPerPageCount: number,
        productsApprovalStatus: ProductsApprovalStatus,
        searchQuery?: string
    ) => Promise<{ products: Product[], metaData: MetaData }>,

    getCurrProdDetails: (productId: string) => Promise<ProductDetails | null>,

    changeProductApprovalStatus: (
        productApprovalData: EditProductApprovalStatus,
        xsrfToken: string
    ) => Promise<boolean>,

    removeProduct: (productId: string, xsrfToken: string) => Promise<boolean>
}