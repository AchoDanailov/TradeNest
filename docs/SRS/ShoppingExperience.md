# Shopping Experience Requirements

This module covers the functional requirements for the shopping cart, stock validation, and the order submission process.

## 1. Shopping Cart Management
*   **F.R. 1.1**: The system shall provide a persistent shopping cart for every registered user.
*   **F.R. 1.2**: Users shall be able to add approved products to their cart from the catalog or product details page.
*   **F.R. 1.3**: Users shall NOT be able to add their own products to their shopping cart.
*   **F.R. 1.4**: Users shall be able to update the quantity of items in their cart or remove items entirely.
*   **F.R. 1.5**: The system shall validate that the requested quantity does not exceed the available stock in real-time.

## 2. Product Availability & Notifications
*   **F.R. 2.1**: The system shall notify the user if a product in their cart becomes unavailable (e.g., sold out, disabled by seller, or rejected by admin).
*   **F.R. 2.2**: If a product in the cart is no longer available, the system shall prevent the user from proceeding to checkout until the issue is resolved (e.g., item removed or quantity adjusted).
*   **F.R. 2.3**: The cart shall display clear error messages if a product's status changes while it is in the cart.

## 3. Order Submission (Checkout)
*   **F.R. 3.1**: Users shall be able to submit their cart to create a formal Order.
*   **F.R. 3.2**: Upon order submission, the system shall atomically:
    *   Verify final stock availability for all items.
    *   Deduct the ordered quantities from the product stock.
    *   Clear the user's shopping cart.
    *   Record the sale for the respective sellers.
*   **F.R. 3.3**: The system shall generate a unique Order ID and record the date of purchase.

## 4. Order History
*   **F.R. 4.1**: Users shall be able to view a history of all their past orders.
*   **F.R. 4.2**: Order details shall include the list of products purchased, quantities, prices at the time of purchase, and the total order value.
