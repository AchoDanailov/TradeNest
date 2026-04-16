const productsService = {
    getCurrPageProducts,
    getCurrProdDetails,
    getProductsCount,
    removeProduct,
}
export default productsService;

const BASE = "/api/v1/products";
const endpoints = {
    byId: (productId) => `${BASE}/${productId}`,
    getCount: buildGetCountPath,
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
            return [];
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

export async function getProductsCount(productsApprovalStatus, searchQuery) {
    const approved = productsApprovalStatus === "Approved";
    
    try {
        const res = await fetch(endpoints.getCount(approved, searchQuery));
        if(!res.ok) 
            throw new Error(await res.json());
        
        return await res.json();
    } catch(err) {
        if (err instanceof Error) {
            console.error(`Error fetching products count.`, err.status);
            return 0;
        }
    }
}

async function removeProduct(productId) {
    try {
        const servRes = await fetch(endpoints.byId(productId), { method: "DELETE" });
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

function buildGetCountPath(approved, searchQuery) {
    let endpoint = `${BASE}/count`;
    
     if(approved !== undefined)
        endpoint = endpoint.concat(`?approved=${approved}`);

    if(searchQuery) {
        if(approved !== undefined)
            endpoint = endpoint.concat(`&search=${searchQuery}`);
        else
            endpoint = endpoint.concat(`?search=${searchQuery}`);
    }

    return endpoint;
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