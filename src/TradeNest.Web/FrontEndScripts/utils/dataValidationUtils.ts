export function isValidQty(quantity: number, maxQtyAllowedToAdd: number): boolean {
    const correctTypes = !isNaN(quantity) && !isNaN(maxQtyAllowedToAdd);
    const validNumber = quantity > 0 && quantity <= maxQtyAllowedToAdd;

    return correctTypes && validNumber;
}
