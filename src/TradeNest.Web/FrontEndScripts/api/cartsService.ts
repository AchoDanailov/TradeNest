import type {
    CartProduct,
    RequestCartProductQty,
    UpdateCartProductQty,
} from "../types/products.ts";
import type { CartsService } from "../types/services.ts";

const cartsService: CartsService = {
    getProdInCart,
    addToCart,
    updateCartProductQty
}

export default cartsService;

const BASE = "/api/v1/cart";
const endpoints = {
    getProdInCartByProductId: (productId: string) =>
         `${BASE}/cartProducts/${productId}`,

    addProductToCart: `${BASE}/addProduct`,

    modifyCartProduct: (cartId: string, productId: string) =>
         `${BASE}/${cartId}?productId=${productId}`,
}

export async function getProdInCart(productId: string): Promise<CartProduct | null> {
    try {
        const res = await fetch(endpoints.getProdInCartByProductId(productId));
        if(!res.ok)
            throw new Error(await res.json());

        return await res.json();
    } catch (err) {
        if (err instanceof Error) {
            console.error("Error fetching product qty from user's cart.", err.status);
        }

        return null;
    }
}

export async function addToCart(
    requestedProductQtyPayload: RequestCartProductQty,
    requestVerificationToken: string
): Promise<boolean> {
    try {
        const res = await fetch(endpoints.addProductToCart, {
            method: "POST",
            headers: {
                "Content-Type": "application/json",
                "X-XSRF-TOKEN": requestVerificationToken,
            },
            body: JSON.stringify(requestedProductQtyPayload)
        });
        if(!res.ok)
            throw new Error(await res.json());

        return await res.json();
    } catch (err) {
        if (err instanceof Error) {
            console.error("Error adding product qty to user's cart.", err.status);
        }

        return false;
    }
}

export async function updateCartProductQty(
    updatedCartProdPayload: UpdateCartProductQty,
    requestVerificationToken: string
): Promise<boolean> {
    try {
        const res = await fetch(
            endpoints.modifyCartProduct(updatedCartProdPayload.cartId, updatedCartProdPayload.productId),
            {
                method: "PUT",
                headers: {
                    "Content-Type": "application/json",
                    "X-XSRF-TOKEN": requestVerificationToken,
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
        }

        return false;
    }
}