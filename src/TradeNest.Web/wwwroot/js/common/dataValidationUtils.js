export function isValidQty(quantity, maxQtyAllowedToAdd) {
    const correctTypes = !isNaN(quantity) && !isNaN(maxQtyAllowedToAdd);
    const validNumber = quantity > 0 && quantity <= maxQtyAllowedToAdd;
    
    return correctTypes && validNumber;
}
