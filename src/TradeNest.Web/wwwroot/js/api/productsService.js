const productsService = {
    getCurrPageProducts,
    getCurrProdDetails,
    removeProduct,
}
export default productsService;

const BASE = "/api/v1/products";
const endpoints = {
    byId: (productId) => `${BASE}/${productId}`,
    getProdsWithPagination: buildPathWithPagination,
};

export async function getCurrPageProducts(
    page,
    productsPerPageCount,
    productsApprovalStatus,
    searchQuery
) {
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
            return { products: [], metaData: { totalSpecifiedProductsCount: 0, xsrfToken: "" } };
        }
    }
}

export async function getCurrProdDetails(productId) {
    try {
        const res = await fetch(endpoints.byId(productId));
        if(!res.ok) 
            throw new Error(await res.json());

        return await res.json();
    } catch (err) {
        if(err instanceof Error) {
            console.error(`Error fetching product quantity.`, err.status);
            return null;
        }
    }
}

export async function removeProduct(productId, xsrfToken) {
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
            return false;
        }
    }
}
    
function buildPathWithPagination(page, limit, productsApprovalStatus, searchQuery) {
    let endpoint = `${BASE}?page=${page}&limit=${limit}`;
    
    if(productsApprovalStatus) {
        const approved = productsApprovalStatus === "Approved";
        endpoint = endpoint.concat(`&approved=${approved}`);
    }
    
    if(searchQuery)
        endpoint = endpoint.concat(`&search=${searchQuery}`);

    return endpoint;
}