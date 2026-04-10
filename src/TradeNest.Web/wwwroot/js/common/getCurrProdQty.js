export default async function getCurrProdQty(productId) {
    try {
        const res = await fetch(`/api/v1/products/${productId}`);
        if(!res.ok) 
            throw new Error(await res.json());

        const productData = await res.json();
        return productData.quantityInStock;
    } catch (err) {
        console.error(`Error fetching product quantity.`, err.status);
        return -1;
    }
}

