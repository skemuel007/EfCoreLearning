-- ======================
-- CUSTOMERS
-- ======================
CREATE TABLE customers (
                           customerID        INT            NOT NULL AUTO_INCREMENT,
                           emailAddress      VARCHAR(255)   NOT NULL,
                           password          VARCHAR(60)    NOT NULL,
                           firstName         VARCHAR(60)    NOT NULL,
                           lastName          VARCHAR(60)    NOT NULL,
                           shipAddressID     INT            DEFAULT NULL,
                           billingAddressID  INT            DEFAULT NULL,
                           PRIMARY KEY (customerID),
                           UNIQUE INDEX emailAddress (emailAddress)
);

-- ======================
-- ADDRESSES
-- ======================
CREATE TABLE addresses (
                           addressID         INT            NOT NULL AUTO_INCREMENT,
                           customerID        INT            NOT NULL,
                           line1             VARCHAR(60)    NOT NULL,
                           line2             VARCHAR(60)    DEFAULT NULL,
                           city              VARCHAR(40)    NOT NULL,
                           state             VARCHAR(2)     NOT NULL,
                           zipCode           VARCHAR(10)    NOT NULL,
                           phone             VARCHAR(12)    NOT NULL,
                           disabled          TINYINT(1)     NOT NULL DEFAULT 0,
                           PRIMARY KEY (addressID),
                           INDEX customerID (customerID),
                           CONSTRAINT fk_addresses_customer
                               FOREIGN KEY (customerID)
                                   REFERENCES customers(customerID)
                                   ON DELETE CASCADE
);

-- ======================
-- CATEGORIES
-- ======================
CREATE TABLE categories (
                            categoryID        INT            NOT NULL AUTO_INCREMENT,
                            categoryName      VARCHAR(255)   NOT NULL,
                            PRIMARY KEY (categoryID)
);

-- ======================
-- PRODUCTS
-- ======================
CREATE TABLE products (
                          productID         INT            NOT NULL AUTO_INCREMENT,
                          categoryID        INT            NOT NULL,
                          productCode       VARCHAR(10)    NOT NULL,
                          productName       VARCHAR(255)   NOT NULL,
                          description       TEXT           NOT NULL,
                          listPrice         DECIMAL(10,2)  NOT NULL,
                          discountPercent   DECIMAL(10,2)  NOT NULL DEFAULT 0.00,
                          dateAdded         DATETIME       NOT NULL,
                          PRIMARY KEY (productID),
                          UNIQUE INDEX productCode (productCode),
                          INDEX categoryID (categoryID),
                          CONSTRAINT fk_products_category
                              FOREIGN KEY (categoryID)
                                  REFERENCES categories(categoryID)
);

-- ======================
-- ORDERS
-- ======================
CREATE TABLE orders (
                        orderID           INT            NOT NULL AUTO_INCREMENT,
                        customerID        INT            NOT NULL,
                        orderDate         DATETIME       NOT NULL,
                        shipAmount        DECIMAL(10,2)  NOT NULL,
                        taxAmount         DECIMAL(10,2)  NOT NULL,
                        shipDate          DATETIME       DEFAULT NULL,
                        shipAddressID     INT            NOT NULL,
                        billingAddressID  INT            NOT NULL,
                        cardType          CHAR(1)        NOT NULL,
                        cardNumber        CHAR(16)       NOT NULL,
                        cardExpires       CHAR(7)        NOT NULL,
                        PRIMARY KEY (orderID),
                        INDEX customerID (customerID),
                        CONSTRAINT fk_orders_customer
                            FOREIGN KEY (customerID)
                                REFERENCES customers(customerID),
                        CONSTRAINT fk_orders_ship_address
                            FOREIGN KEY (shipAddressID)
                                REFERENCES addresses(addressID),
                        CONSTRAINT fk_orders_billing_address
                            FOREIGN KEY (billingAddressID)
                                REFERENCES addresses(addressID)
);

-- ======================
-- ORDER ITEMS
-- ======================
CREATE TABLE orderItems (
                            itemID            INT            NOT NULL AUTO_INCREMENT,
                            orderID           INT            NOT NULL,
                            productID         INT            NOT NULL,
                            itemPrice         DECIMAL(10,2)  NOT NULL,
                            discountAmount    DECIMAL(10,2)  NOT NULL,
                            quantity          INT            NOT NULL,
                            PRIMARY KEY (itemID),
                            INDEX orderID (orderID),
                            INDEX productID (productID),
                            CONSTRAINT fk_orderitems_order
                                FOREIGN KEY (orderID)
                                    REFERENCES orders(orderID)
                                    ON DELETE CASCADE,
                            CONSTRAINT fk_orderitems_product
                                FOREIGN KEY (productID)
                                    REFERENCES products(productID)
);
