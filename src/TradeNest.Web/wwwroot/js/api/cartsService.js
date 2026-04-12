const cartsService = {
    getProdInCart,
    addToCart,
    saveChanges: updateCartProductQty,
}
export default cartsService;

const BASE = "/api/v1/cart";
const endpoints = {
    getProdInCartByProductId: (productId) => `${BASE}/cartProducts/${productId}`,
    addProductToCart: `${BASE}/addProduct`,
    modifyCartProduct: (cartId, productId) => `${BASE}/${cartId}?productId=${productId}`,
}

export async function getProdInCart(productId) {
    try {
        const res = await fetch(endpoints.getProdInCartByProductId(productId));
        if(!res.ok)
            throw new Error(await res.json());
        
        return await res.json();
    } catch (err) {
        if (err instanceof Error) {
            console.error("Error fetching product qty from user's cart.", err.status);
            return null;
        }
    }
}

export async function addToCart(requestedProductQtyPayload, requestVerificationToken) {
    try {
        const res = await fetch(endpoints.addProductToCart, {
            method: "POST",
            headers: { 
                "Content-Type": "application/json",
                "RequestVerificationToken": requestVerificationToken,
            },
            body: JSON.stringify(requestedProductQtyPayload)
        });
        if(!res.ok) 
            throw new Error(await res.json());
            
        return await res.json();
    } catch (err) {
        if (err instanceof Error) {
            console.error("Error adding product qty to user's cart.", err.status);
            return false;
        }
    }
}

export async function updateCartProductQty(updatedCartProdPayload, requestVerificationToken) {
    try {
        const res = await fetch(
            endpoints.modifyCartProduct(updatedCartProdPayload.cartId, updatedCartProdPayload.productId), 
            {
                method: "PUT",
                headers: {
                    "Content-Type": "application/json",
                    "RequestVerificationToken": requestVerificationToken,
                },
                body: JSON.stringify(updatedCartProdPayload),
            }
        );
        if(!res.ok) 
            throw new Error(await res.json());
        
        return await res.json();
    } catch (err) {
        if(err instanceof Error) {
            console.error("Error saving new cart product qty.", err.status);
            return false;
        }
    }
}