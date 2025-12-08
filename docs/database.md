# Database Schema

This document describes the PostgreSQL database schema for the CSE325 E-Commerce application.

## Overview

The database uses PostgreSQL with Entity Framework Core for ORM. The schema includes tables for users, products, orders, carts, and recommendations.

## Main Tables

### Clients

Stores customer information.

```sql
CREATE TABLE clients (
    user_id BIGINT PRIMARY KEY,
    name VARCHAR(255) NOT NULL,
    email VARCHAR(100) NOT NULL UNIQUE,
    password_hash VARCHAR(500) NOT NULL,
    phone_number VARCHAR(50),
    address1 VARCHAR(255) NOT NULL,
    address2 VARCHAR(255),
    city VARCHAR(50) NOT NULL,
    state VARCHAR(50) NOT NULL,
    country VARCHAR(50) NOT NULL,
    zip_code VARCHAR(20)
);
```

**Relationships:**
- One-to-many with `cart` (a client can have multiple carts)

### Sellers

Stores seller/vendor information.

```sql
CREATE TABLE sellers (
    seller_id BIGINT PRIMARY KEY,
    business_name VARCHAR(255) NOT NULL,
    email VARCHAR(100) NOT NULL UNIQUE,
    password_hash VARCHAR(500) NOT NULL,
    phone VARCHAR(50),
    address1 VARCHAR(255) NOT NULL,
    address2 VARCHAR(255),
    city VARCHAR(50) NOT NULL,
    state VARCHAR(50) NOT NULL,
    country VARCHAR(50) NOT NULL,
    zip_code VARCHAR(20)
);
```

**Relationships:**
- One-to-many with `products` (a seller can have multiple products)

### Products

Stores product information.

```sql
CREATE TABLE products (
    product_id BIGINT PRIMARY KEY,
    name VARCHAR(255) NOT NULL,
    description TEXT NOT NULL,
    price INTEGER NOT NULL, -- Stored in cents
    inventory DECIMAL NOT NULL,
    category_id BIGINT NOT NULL,
    seller_id BIGINT NOT NULL,
    FOREIGN KEY (category_id) REFERENCES category(category_id),
    FOREIGN KEY (seller_id) REFERENCES sellers(seller_id)
);
```

**Relationships:**
- Many-to-one with `sellers`
- Many-to-one with `category`
- One-to-many with `cart_items`
- One-to-many with `orders_products`

### Categories

Product categorization.

```sql
CREATE TABLE category (
    category_id BIGINT PRIMARY KEY,
    name VARCHAR(100) NOT NULL,
    description VARCHAR(500)
);
```

**Relationships:**
- One-to-many with `products`

### Cart

Shopping carts for users.

```sql
CREATE TABLE cart (
    cart_id BIGINT PRIMARY KEY,
    user_id BIGINT,
    created_date TIMESTAMP,
    updated_date TIMESTAMP,
    FOREIGN KEY (user_id) REFERENCES clients(user_id) ON DELETE SET NULL
);
```

**Relationships:**
- Many-to-one with `clients` (optional for anonymous carts)
- One-to-many with `cart_items`

### Cart Items

Items in shopping carts.

```sql
CREATE TABLE cart_items (
    cart_item_id BIGINT PRIMARY KEY GENERATED ALWAYS AS IDENTITY,
    cart_id BIGINT NOT NULL,
    product_id BIGINT NOT NULL,
    quantity INTEGER NOT NULL,
    FOREIGN KEY (cart_id) REFERENCES cart(cart_id) ON DELETE CASCADE,
    FOREIGN KEY (product_id) REFERENCES products(product_id) ON DELETE CASCADE,
    UNIQUE(cart_id, product_id)
);
```

**Relationships:**
- Many-to-one with `cart`
- Many-to-one with `products`

### Orders

Customer orders.

```sql
CREATE TYPE order_status AS ENUM (
    'Pending Payment', 'Payment Confirmed', 'Payment Failed', 'Cancelled',
    'Processing', 'Shipped', 'Out for Delivery', 'Delivered', 'Completed', 'Returned'
);

CREATE TABLE orders (
    order_id BIGINT PRIMARY KEY,
    user_id BIGINT NOT NULL,
    order_date TIMESTAMP NOT NULL,
    total_amount DECIMAL NOT NULL,
    status order_status NOT NULL,
    shipping_address1 VARCHAR(255) NOT NULL,
    shipping_address2 VARCHAR(255),
    shipping_city VARCHAR(50) NOT NULL,
    shipping_state VARCHAR(50) NOT NULL,
    shipping_country VARCHAR(50) NOT NULL,
    shipping_zip_code VARCHAR(20),
    FOREIGN KEY (user_id) REFERENCES clients(user_id)
);
```

**Relationships:**
- Many-to-one with `clients`
- One-to-many with `orders_products`
- One-to-one with `payments`

### Orders Products

Junction table for order items.

```sql
CREATE TABLE orders_products (
    order_id BIGINT NOT NULL,
    product_id BIGINT NOT NULL,
    quantity INTEGER NOT NULL,
    price DECIMAL NOT NULL,
    PRIMARY KEY (order_id, product_id),
    FOREIGN KEY (order_id) REFERENCES orders(order_id),
    FOREIGN KEY (product_id) REFERENCES products(product_id)
);
```

### Payments

Payment information for orders.

```sql
CREATE TYPE payment_status AS ENUM ('Pending', 'Approved', 'Failed', 'Refunded', 'Captured');

CREATE TABLE payments (
    payment_id BIGINT PRIMARY KEY,
    order_id BIGINT NOT NULL UNIQUE,
    payment_date TIMESTAMP NOT NULL,
    amount DECIMAL NOT NULL,
    payment_method VARCHAR(50) NOT NULL,
    status payment_status NOT NULL,
    transaction_id VARCHAR(255),
    FOREIGN KEY (order_id) REFERENCES orders(order_id)
);
```

### Product Images

Product image storage.

```sql
CREATE TABLE product_images (
    image_id BIGINT PRIMARY KEY,
    product_id BIGINT NOT NULL,
    image_url VARCHAR(500) NOT NULL,
    alt_text VARCHAR(255),
    is_primary BOOLEAN DEFAULT FALSE,
    FOREIGN KEY (product_id) REFERENCES products(product_id) ON DELETE CASCADE
);
```

### Recommendations

Product recommendation system.

```sql
CREATE TABLE recomendations (
    recomendation_id BIGINT PRIMARY KEY,
    name VARCHAR(255) NOT NULL,
    description TEXT
);

CREATE TABLE recomendations_clients (
    recomendation_id BIGINT NOT NULL,
    user_id BIGINT NOT NULL,
    PRIMARY KEY (recomendation_id, user_id),
    FOREIGN KEY (recomendation_id) REFERENCES recomendations(recomendation_id),
    FOREIGN KEY (user_id) REFERENCES clients(user_id)
);

CREATE TABLE recomendations_products (
    recomendation_id BIGINT NOT NULL,
    product_id BIGINT NOT NULL,
    PRIMARY KEY (recomendation_id, product_id),
    FOREIGN KEY (recomendation_id) REFERENCES recomendations(recomendation_id),
    FOREIGN KEY (product_id) REFERENCES products(product_id)
);
```

### Currency

Currency support (future use).

```sql
CREATE TABLE currencies (
    currency_id BIGINT PRIMARY KEY,
    code VARCHAR(3) NOT NULL UNIQUE,
    name VARCHAR(50) NOT NULL,
    symbol VARCHAR(5) NOT NULL
);
```

## Entity Relationships

```
Clients (1) ──── (M) Cart
    │
    └── (1) ──── (M) Orders ──── (1) Payments
                        │
                        └── (M) ──── (M) Products (via orders_products)

Sellers (1) ──── (M) Products ──── (M) Cart Items
    │                    │
    └──                  └── (M) Product Images

Categories (1) ──── (M) Products

Recomendations (M) ──── (M) Clients
                │
                └── (M) ──── (M) Products
```

## Indexes

Recommended indexes for performance:

```sql
-- Products
CREATE INDEX idx_products_category_id ON products(category_id);
CREATE INDEX idx_products_seller_id ON products(seller_id);
CREATE INDEX idx_products_name ON products(name);

-- Cart items
CREATE INDEX idx_cart_items_cart_id ON cart_items(cart_id);
CREATE INDEX idx_cart_items_product_id ON cart_items(product_id);

-- Orders
CREATE INDEX idx_orders_user_id ON orders(user_id);
CREATE INDEX idx_orders_status ON orders(status);
CREATE INDEX idx_orders_date ON orders(order_date);

-- Clients/Sellers
CREATE UNIQUE INDEX idx_clients_email ON clients(email);
CREATE UNIQUE INDEX idx_sellers_email ON sellers(email);
```

## Migrations

Database schema changes are managed through Entity Framework Core migrations. The current migration is `20251117112901_AddEcommerceSeedData.cs`.

## Seed Data

Initial data is populated through the `SeedData` class, including:
- Sample categories
- Sample sellers
- Sample products
- Sample clients

## Performance Considerations

- Use appropriate data types (e.g., INTEGER for price in cents)
- Implement proper indexing
- Consider partitioning for large tables (orders, products)
- Use connection pooling
- Implement caching for frequently accessed data

## Backup Strategy

- Regular automated backups
- Point-in-time recovery capability
- Test restore procedures
- Off-site backup storage

## Future Enhancements

- Add full-text search capabilities
- Implement audit logging
- Add soft deletes
- Support for product variants
- Multi-currency support
- Inventory management improvements