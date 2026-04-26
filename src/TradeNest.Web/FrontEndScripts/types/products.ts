export type ProductsApprovalStatus = "Approved" | "Disapproved" | "WaitingApproval" | string;

type ApprovalDecision = {
    approvalDecisionMakerUsername?: string,
    approvalStatus: ProductsApprovalStatus,
    decisionJustification?: string,
    timeOfDecision?: string
}

export type Product = {
    id: string;
    name: string;
    ownerName: string;
    categoryName: string;
    approvalStatus: ProductsApprovalStatus
}

export type ProductDetails = Omit<Product, "approvalStatus"> & {
    quantityInStock: number,
    sellingPrice: number,
    isEnabled: boolean,
    description?: string,
    approvalDecision: ApprovalDecision,
    imagesUrls: string[],
}

export type EditProductApprovalStatus = Pick<Product, "approvalStatus"> & {
    productId: string
    decisionJustification?: string;
}

export type CartProduct = {
    name: string;
    quantityAdded: number;
    unitPrice: number;
    totalPrice: number;
    addedOn: Date
}

export type RequestCartProductQty = {
    productId: string;
    quantity: number;
}

export type UpdateCartProductQty = RequestCartProductQty & {
    cartId: string;
}

export type MetaData = {
    totalSpecifiedProductsCount: number;
    xsrfToken: string;
}