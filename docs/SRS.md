# TradeNest - Software Requirements Specification 

---

## Navigation
- [User Account](#user-account-requirements)
    - [Registration](#1-registration)
    - [Authentication](#2-authentication)
    - [Account Management](#3-account-management)
    - [Role-Based Access Control (RBAC)](#4-role-based-access-control-rbac)
    - [User roles](#user-roles)
- [Administration](#administration-requirements)
  - [Category Management](#1-category-management)
  - [Global Product Oversight](#2-global-product-oversight)
  - [User Management](#3-user-management)
  - [Platform Analytics (Admin View)](#4-platform-analytics-admin-view)
- [Catalog and Product Management](#catalog-and-product-management-requirements)
  - [Product Catalog](#1-product-catalog-publicuser-view)
  - [Selling Products](#2-selling-products-userseller-view)
  - [Product Approval Workflow](#3-product-approval-workflow)
- [Shopping Experience Requirements](#shopping-experience-requirements)
  - [Shopping Cart Management](#1-shopping-cart-management)
  - [Product Availability & Notifications](#2-product-availability--notifications)
  - [Order Submission (Checkout)](#3-order-submission-checkout)
  - [Order History](#4-order-history)

---

## User Account Requirements
This module defines the objectives and functional requirements related to identity, authentication, and user profile management within the TradeNest platform.
It also defines the different roles and their capabilities and meaning.

### 1. Registration
*   **F.R. 1.1**: The system shall allow new users to register an account by providing a unique username, a unique email address, and a secure password.
*   **F.R. 1.2**: The system shall validate that the email address is in a valid format and is not already associated with an existing account.
*   **F.R. 1.3**: The system shall enforce password complexity requirements (e.g., minimum length, numeric and alphanumeric characters) as configured in the system settings.

### 2. Authentication
*   **F.R. 2.1**: Users shall be able to log in securely using either their username or email address along with their password.
*   **F.R. 2.2**: The system shall provide a secure logout mechanism that terminates the user session and clears authentication cookies.
*   **F.R. 2.3**: The system shall implement a lockout policy after a configurable number of failed login attempts to prevent brute-force attacks.

### 3. Account Management
*   **F.R. 3.1**: Users shall be able to view their account details and basic profile information.
*   **F.R. 3.2**: Users shall have the right to delete their account and request erasure of personal data, in compliance with GDPR Article 17 ("Right to Erasure"). Data required for the continued functioning of other core system features shall be retained.

### 4. Role-Based Access Control (RBAC)
*   **F.R. 4.1**: The system shall restrict access to administrative functions (e.g., category management, global product approval) to users with the 'Admin' role.
*   **F.R. 4.2**: The system shall ensure that users can only modify or delete product listings that they personally own.

### User Roles
TradeNest simplifies the user experience by defining two primary roles:

1.  **User**: A standard registered member of the platform. A User has the dual capability to:
    *   **Buy**: Browse the catalog, manage a shopping cart, and place orders.
    *   **Sell**: Create product listings, manage their inventory, and track sales performance.
2.  **Administrator**: A specialized role responsible for platform oversight, including:
    *   Managing product categories.
    *   Reviewing and approving/rejecting product listings.
    *   Overseeing user accounts and platform integrity.

---

## Administration Requirements
This module defines the functional requirements for administrative oversight.

### 1. Category Management
*   **F.R. 1.1**: Administrators shall be able to create new product categories to organize the marketplace.
*   **F.R. 1.2**: Administrators shall be able to edit existing category names and descriptions.
*   **F.R. 1.3**: Administrators shall be able to delete categories. If a category is associated with product listings, those product listings shall be moved to a default category called "Other".

### 2. Global Product Oversight
*   **F.R. 2.1**: Administrators shall have access to a global dashboard showing all products across the platform, regardless of seller.
*   **F.R. 2.2**: Administrators shall be able to search and filter products by status (Pending, Approved, Rejected, Disabled).
*   **F.R. 2.3**: Administrators shall have the authority to "Disapprove" any product listing if it violates platform policies, even if it was previously approved.
*   **F.R. 2.4**: Administrators shall be able to perform the final review and approval/rejection of all product listings.

### 3. User Management
*   **F.R. 3.1**: Administrators shall be able to view a comprehensive list of all registered users.
*   **F.R. 3.2**: Administrators shall be able to view and modify user roles (e.g., promoting a User to an Admin).
*   **F.R. 3.3**: Administrators shall be able to suspend user accounts for disciplinary reasons.

### 4. Platform Analytics (Admin View)
*   **F.R. 4.1**: Administrators shall have a high-level view of platform activity, including total users, total products, and total categories.

---

## Catalog and Product Management Requirements
This module details the requirements for browsing the product catalog and the lifecycle of product listings from creation to public visibility.

### 1. Product Catalog (Public View)
*   **F.R. 1.1**: The system shall display a public catalog of all **approved** products. If a product is not approved (either **pending** or **disapproved**) it shall not be visible on the public catalog.
*   **F.R. 1.2**: Users shall be able to filter the catalog by product category.
*   **F.R. 1.3**: Users shall be able to search for products using a text-based search that matches product names or category names.
*   **F.R. 1.4**: The system shall allow regular users to view detailed information for any approved product, including its name, description, price, category, images, and the seller's information.
*   **F.R. 1.5**: The system shall highlight "Best Seller" products based on the number of successful orders.

### 2. Selling Products (User/Seller View)
*   **F.R. 2.1**: Users shall be able to create new product listings by providing a name, description, category, price (cost and selling), and images.
*   **F.R. 2.2**: Users shall be able to edit their own product listings. Any significant modification (e.g., price change, image update, listing title) shall trigger a re-approval process.
*   **F.R. 2.3**: Users shall be able to delete their own product listings. Deletion is typically "soft" to preserve historical order data.
*   **F.R. 2.4**: Users shall have access to a "My Nest" dashboard to manage their listings and view their current approval status (Pending, Approved, Disapproved).
*   **F.R. 2.5**: Users shall be able to display total sales and total surplus (selling price - cost price) for each of their products, given they have provided product **cost price** either in the product listing creation process or by later editing the listing.
*   **F.R. 2.6**: Sellers shall be able to see how many times each of their products has been ordered.
*   **F.R. 2.7**: Sellers shall be able to trigger availability of their products, by changing a product's "IsEnabled" status. A product that is **approved** but **disabled** shall be visible on the public catalog, but it shall not be available for sale.

### 3. Product Approval Workflow
*   **F.R. 3.1**: Newly created or modified product listings shall be placed in a "Pending" state and shall NOT be visible in the public catalog.
*   **F.R. 3.2**: Administrators shall be able to review pending products and either **Approve** or **Disapprove** them.
*   **F.R. 3.3**: Administrators shall provide an optional reason/comment when disapproving a product to guide the seller on necessary changes.
*   **F.R. 3.4**: Once approved, the product shall become visible in the public catalog. If a product is with status **Pending** or **Disapproved** it is not allowed to be vissible on the public catalog.

---

## Shopping Experience Requirements
This module covers the functional requirements for the shopping cart, stock validation, and the order submission process.

### 1. Shopping Cart Management
*   **F.R. 1.1**: The system shall provide a persistent shopping cart for every registered user.
*   **F.R. 1.2**: Users shall be able to add approved products to their cart from the catalog or product details page.
*   **F.R. 1.3**: Users shall NOT be able to add their own products to their shopping cart.
*   **F.R. 1.4**: Users shall be able to update the quantity of items in their cart or remove items entirely, this shall be available both from the product details page and the user cart page.
*   **F.R. 1.5**: The system shall validate that the requested quantity does not exceed the available stock. 

### 2. Product Availability & Notifications
*   **F.R. 2.1**: The system shall notify the user if a product in their cart becomes unavailable (e.g., sold out, disabled by seller).
*   **F.R. 2.2**: If a product in the cart is no longer available, the system shall prevent the user from proceeding to checkout until the issue is resolved (e.g., item removed or quantity adjusted).
*   **F.R. 2.3**: The cart shall display clear error messages if a product's status changes while it is in the cart.
*   **F.R. 2.4**: If a product in the cart has lost its approval it shall be removed from the user's cart automatically.

### 3. Order Submission (Checkout)
*   **F.R. 3.1**: Users shall be able to submit their cart to create an Order.
*   **F.R. 3.2**: Upon order submission, the system shall atomically:
    *   Verify final stock availability for all items.
    *   Deduct the ordered quantities from the product stock.
    *   Clear the user's shopping cart.
    *   Record the sale for the respective sellers.
*   **F.R. 3.3**: The system shall generate a unique Order ID and record the date of purchase.

### 4. Order History
*   **F.R. 4.1**: Users shall be able to view a history of all their past orders.
*   **F.R. 4.2**: Order details shall include the list of products purchased, quantities, prices at the time of purchase, and the total order value.
