import type {
    EditProductApprovalStatus,
    MetaData,
    Product,
    ProductDetails,
    ProductsApprovalStatus,
} from "../types/products.ts";
import type { ProductsService } from "../types/services.ts";

const productsService: ProductsService = {
    getCurrPageProducts,
    getCurrProdDetails,
    changeProductApprovalStatus,
    removeProduct
} 

export default productsService;

const BASE = "/api/v1/products";
const endpoints = {
    byId: (productId: string) => `${BASE}/${productId}`,
    getProdsWithPagination: buildPathWithPagination,
    approvalByProdId: (productId: string) => `${BASE}/approval/${productId}`,
};

export async function getCurrPageProducts(
    page: number,
    productsPerPageCount: number,
    productsApprovalStatus: ProductsApprovalStatus,
    searchQuery: string | undefined
): Promise<{ products: Product[], metaData: MetaData }> {
    try {
        const res = await fetch(endpoints.getProdsWithPagination(
            page,
            productsPerPageCount,
            productsApprovalStatus,
            searchQuery
        ));
        if(!res.ok)
            throw new Error(await res.json());

        return await res.json();
    } catch (err) {
        if(err instanceof Error){
            console.error("Error fetching current page products", err.status);
        }

        return {
            products: [] as Product[],
            metaData: {
                totalSpecifiedProductsCount: 0,
                xsrfToken: ""
            } as MetaData
        };
    }
}

export async function getCurrProdDetails(productId: string): Promise<ProductDetails | null> {
    try {
        const res = await fetch(endpoints.byId(productId));
        if(!res.ok)
            throw new Error(await res.json());

        return await res.json();
    } catch (err) {
        if(err instanceof Error) {
            console.error(`Error fetching product details.`, err.status);
        }

        return null;
    }
}

export async function changeProductApprovalStatus(
    productApprovalData: EditProductApprovalStatus,
    xsrfToken: string
): Promise<boolean> {
    try {
        const servRes = await fetch(endpoints.approvalByProdId(productApprovalData.productId), {
            method: "PUT",
            headers: {
                "Content-Type": "application/json",
                "X-XSRF-TOKEN": xsrfToken,
            },
            body: JSON.stringify(productApprovalData)
        });
        if(!servRes.ok)
            throw new Error(await servRes.json());

        return await servRes.json();
    } catch (err) {
        if(err instanceof Error) {
            console.error(`Error modifying product's approval status.`, err.status);
        }

        return false;
    }
}

export async function removeProduct(productId: string, xsrfToken: string): Promise<boolean> {
    try {
        const servRes = await fetch(endpoints.byId(productId), {
            method: "DELETE",
            headers: { "X-XSRF-TOKEN": xsrfToken }
        });
        if(!servRes.ok)
            throw new Error(await servRes.json());

        return await servRes.json();
    } catch(err) {
        if (err instanceof Error) {
            console.error(`Error while removing product. productId: ${productId}`, err.status);
        }

        return false;
    }
}

function buildPathWithPagination(
    page: number,
    limit: number,
    productsApprovalStatus: ProductsApprovalStatus,
    searchQuery: string | undefined
): string {
    let endpoint = `${BASE}?page=${page}&limit=${limit}`;

    if(productsApprovalStatus) {
        const approved = productsApprovalStatus === "Approved";
        endpoint = endpoint.concat(`&approved=${approved}`);
    }

    if(searchQuery)
        endpoint = endpoint.concat(`&search=${searchQuery}`);

    return endpoint;
}