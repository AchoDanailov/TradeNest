# Catalog and Product Management Requirements

This module details the requirements for browsing the product catalog and the lifecycle of product listings from creation to public visibility.

## 1. Product Catalog (Public/User View)
*   **F.R. 1.1**: The system shall display a public catalog of all **approved** and **enabled** products.
*   **F.R. 1.2**: Users shall be able to filter the catalog by product category.
*   **F.R. 1.3**: Users shall be able to search for products using a text-based search that matches product names or category names.
*   **F.R. 1.4**: The system shall allow users to view detailed information for any product, including its name, description, price, category, images, and the seller's information.
*   **F.R. 1.5**: The system shall highlight "Best Seller" products based on the number of successful orders.

## 2. Selling Products (User/Seller View)
*   **F.R. 2.1**: Users shall be able to create new product listings by providing a name, description, category, price (cost and selling), and at least one image.
*   **F.R. 2.2**: Users shall be able to edit their own product listings. Any significant modification (e.g., price change, image update, listing title) shall trigger a re-approval process.
*   **F.R. 2.3**: Users shall be able to delete their own product listings. Deletion is typically "soft" to preserve historical order data.
*   **F.R. 2.4**: Users shall have access to a "MyProducts" dashboard to manage their listings and view their current approval status (Pending, Approved, Rejected).

## 3. Product Approval Workflow
*   **F.R. 3.1**: Newly created or modified product listings shall be placed in a "Pending" state and shall NOT be visible in the public catalog.
*   **F.R. 3.2**: Administrators shall be able to review pending products and either **Approve** or **Reject** them.
*   **F.R. 3.3**: Administrators shall provide an optional reason/comment when rejecting a product to guide the seller on necessary changes.
*   **F.R. 3.4**: Once approved, the product shall become "Enabled" and visible in the public catalog, provided it is in stock.

## 4. Product Statistics (Seller)
*   **F.R. 4.1**: The system shall calculate and display total sales and total surplus (selling price - cost price) for each of the seller's products.
*   **F.R. 4.2**: Sellers shall be able to see how many times each of their products has been ordered.
