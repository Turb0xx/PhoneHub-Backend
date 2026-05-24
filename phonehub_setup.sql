-- ============================================================
-- PhoneHub - Script completo de base de datos (limpio)
-- Base de datos: DbPhoneHub
-- Motor: MySQL 8.x
-- ============================================================

DROP DATABASE IF EXISTS DbPhoneHub;

CREATE DATABASE DbPhoneHub
    CHARACTER SET utf8mb4
    COLLATE utf8mb4_unicode_ci;

USE DbPhoneHub;

-- ============================================================
-- TABLA: users
-- ============================================================
CREATE TABLE users (
    Id          INT             NOT NULL AUTO_INCREMENT,
    FirstName   VARCHAR(50)     NOT NULL,
    LastName    VARCHAR(50)     NOT NULL,
    Email       VARCHAR(100)    NOT NULL,
    Password    VARCHAR(200)    NOT NULL,
    Role        VARCHAR(20)     NOT NULL,
    Telephone   VARCHAR(15)     NULL,
    IsActive    BIT             NOT NULL DEFAULT 1,
    PRIMARY KEY (Id),
    CONSTRAINT UQ_User_Email UNIQUE (Email)
);

-- ============================================================
-- TABLA: products
-- ============================================================
CREATE TABLE products (
    Id          INT             NOT NULL AUTO_INCREMENT,
    Brand       VARCHAR(50)     NOT NULL,
    Model       VARCHAR(100)    NOT NULL,
    Description VARCHAR(500)    NULL,
    Price       DECIMAL(18,2)   NOT NULL,
    Stock       INT             NOT NULL DEFAULT 0,
    CreatedAt   DATETIME        NOT NULL DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (Id)
);

-- ============================================================
-- TABLA: sales
-- ============================================================
CREATE TABLE sales (
    Id          INT             NOT NULL AUTO_INCREMENT,
    ProductId   INT             NOT NULL,
    UserId      INT             NOT NULL,
    Quantity    INT             NOT NULL DEFAULT 1,
    TotalAmount DECIMAL(18,2)   NOT NULL,
    Date        DATETIME        NOT NULL,
    IsActive    BIT             NOT NULL,
    PRIMARY KEY (Id),
    CONSTRAINT FK_Sale_Product FOREIGN KEY (ProductId) REFERENCES products(Id),
    CONSTRAINT FK_Sale_User    FOREIGN KEY (UserId)    REFERENCES users(Id)
);

-- ============================================================
-- TABLA: invoices  (RN-12, RN-13)
-- ============================================================
CREATE TABLE invoices (
    Id              INT             NOT NULL AUTO_INCREMENT,
    SaleId          INT             NOT NULL,
    InvoiceNumber   VARCHAR(20)     NOT NULL,
    IssuedAt        DATETIME        NOT NULL,
    PRIMARY KEY (Id),
    CONSTRAINT UQ_Invoice_SaleId  UNIQUE (SaleId),
    CONSTRAINT UQ_Invoice_Number  UNIQUE (InvoiceNumber),
    CONSTRAINT FK_Invoice_Sale    FOREIGN KEY (SaleId) REFERENCES sales(Id)
);

-- ============================================================
-- SEED: users
-- Hash = SHA-256(SaltKey + password) en Base64
-- SaltKey = "PhoneHub$Salt2025!"
--
--   admin@phonehub.com  → password: admin123
--   carlos@phonehub.com → password: seller123
--   maria@phonehub.com  → password: seller456
--   jorge@phonehub.com  → password: seller123  (IsActive=0 para probar RN-07)
-- ============================================================
INSERT INTO users (FirstName, LastName, Email, Password, Role, Telephone, IsActive) VALUES
('Admin',   'PhoneHub',  'admin@phonehub.com',   'W/B7SCBcL4UvS9v4kEaIGJdQoMyYbGyO9+QRyYA4pJg=', 'Admin',  '70000001', 1),
('Carlos',  'Mendoza',   'carlos@phonehub.com',  'ws/oCNBq4GlC7KxkjdRcOhtYwCPapYqdpBvCu5vNBOE=', 'Seller', '70000002', 1),
('Maria',   'Gutierrez', 'maria@phonehub.com',   'OblhECfyMrTiyM7kY4EdZVPhhTSjcU7kDxLAWcs0sw4=', 'Seller', '70000003', 1),
('Jorge',   'Quispe',    'jorge@phonehub.com',   'ws/oCNBq4GlC7KxkjdRcOhtYwCPapYqdpBvCu5vNBOE=', 'Seller', '70000004', 0);

-- ============================================================
-- SEED: products (8 celulares de distintas marcas)
-- ============================================================
INSERT INTO products (Brand, Model, Description, Price, Stock, CreatedAt) VALUES
('Samsung',  'Galaxy S24',    'Smartphone 256GB, 8GB RAM, pantalla AMOLED 6.1", Snapdragon 8 Gen 3',   899.99, 15, NOW()),
('Apple',    'iPhone 15',     'Apple iPhone 15 128GB, chip A16 Bionic, cámara 48MP, USB-C',            1099.99, 10, NOW()),
('Xiaomi',   'Redmi Note 13', 'Xiaomi Redmi Note 13 256GB, 8GB RAM, batería 5000mAh, carga 33W',        249.99, 30, NOW()),
('Motorola', 'Edge 40',       'Motorola Edge 40 256GB, pantalla pOLED 144Hz, carga 68W, IP68',          449.99, 20, NOW()),
('Huawei',   'P60 Pro',       'Huawei P60 Pro 256GB, cámara Leica triple, Kirin 9000S',                 749.99,  8, NOW()),
('OnePlus',  '12',            'OnePlus 12 256GB, Snapdragon 8 Gen 3, carga SUPERVOOC 100W',             649.99, 12, NOW()),
('Samsung',  'Galaxy A55',    'Samsung Galaxy A55 128GB, 8GB RAM, resistencia IP67, pantalla 6.6"',     399.99, 25, NOW()),
('Apple',    'iPhone 14',     'Apple iPhone 14 128GB, chip A15 Bionic, cámara 12MP principal',          849.99,  5, NOW());

-- ============================================================
-- SEED: sales
-- UserId: 1=Admin, 2=Carlos(Seller), 3=Maria(Seller), 4=Jorge(inactivo)
-- ============================================================

-- Venta 1: Carlos vende 2x Galaxy S24 (activa, hace 2 días)
INSERT INTO sales (ProductId, UserId, Quantity, TotalAmount, Date, IsActive) VALUES
(1, 2, 2, 1799.98, NOW() - INTERVAL 2 DAY, 1);

-- Venta 2: Maria vende 1x iPhone 15 (activa, ayer)
INSERT INTO sales (ProductId, UserId, Quantity, TotalAmount, Date, IsActive) VALUES
(2, 3, 1, 1099.99, NOW() - INTERVAL 1 DAY, 1);

-- Venta 3: Carlos vende 3x Redmi Note 13 (activa, ayer)
INSERT INTO sales (ProductId, UserId, Quantity, TotalAmount, Date, IsActive) VALUES
(3, 2, 3, 749.97, NOW() - INTERVAL 1 DAY, 1);

-- Venta 4: Maria vende 1x Motorola Edge 40 (ANULADA IsActive=0 → prueba RN-05)
INSERT INTO sales (ProductId, UserId, Quantity, TotalAmount, Date, IsActive) VALUES
(4, 3, 1, 449.99, NOW() - INTERVAL 3 DAY, 0);

-- Venta 5: Carlos vende 2x Galaxy A55 (activa, hoy → aparece en reporte de cierre CU-07)
INSERT INTO sales (ProductId, UserId, Quantity, TotalAmount, Date, IsActive) VALUES
(7, 2, 2, 799.98, NOW(), 1);

-- Venta 6: Maria vende 1x OnePlus 12 (activa, hoy → aparece en reporte de cierre CU-07)
INSERT INTO sales (ProductId, UserId, Quantity, TotalAmount, Date, IsActive) VALUES
(6, 3, 1, 649.99, NOW(), 1);

-- Ajuste de stock según ventas activas
UPDATE products SET Stock = Stock - 2 WHERE Id = 1; -- Galaxy S24: 15-2=13
UPDATE products SET Stock = Stock - 1 WHERE Id = 2; -- iPhone 15: 10-1=9
UPDATE products SET Stock = Stock - 3 WHERE Id = 3; -- Redmi Note 13: 30-3=27
-- Venta 4 anulada: el stock no se descuenta (ya fue anulada)
UPDATE products SET Stock = Stock - 2 WHERE Id = 7; -- Galaxy A55: 25-2=23
UPDATE products SET Stock = Stock - 1 WHERE Id = 6; -- OnePlus 12: 12-1=11

-- ============================================================
-- SEED: invoices
-- Ventas 1 y 2 ya tienen factura
-- Ventas 3, 5 y 6 no tienen → para probar POST /api/invoice/generate/{saleId}
-- ============================================================
INSERT INTO invoices (SaleId, InvoiceNumber, IssuedAt) VALUES
(1, 'PH-2026-000001', NOW() - INTERVAL 2 DAY),
(2, 'PH-2026-000002', NOW() - INTERVAL 1 DAY);

-- ============================================================
-- VERIFICACIÓN FINAL
-- ============================================================
SELECT '== RESUMEN ==' AS info;

SELECT 'USUARIOS' AS tabla, COUNT(*) AS total FROM users
UNION ALL SELECT 'PRODUCTOS',  COUNT(*) FROM products
UNION ALL SELECT 'VENTAS',     COUNT(*) FROM sales
UNION ALL SELECT 'FACTURAS',   COUNT(*) FROM invoices;

SELECT '' AS '';
SELECT Id, FirstName, LastName, Email, Role, IsActive FROM users;
SELECT '' AS '';
SELECT Id, Brand, Model, Price, Stock FROM products;
SELECT '' AS '';
SELECT Id, ProductId, UserId, Quantity, TotalAmount, DATE(Date) AS Fecha, IsActive FROM sales;
SELECT '' AS '';
SELECT Id, SaleId, InvoiceNumber, DATE(IssuedAt) AS Fecha FROM invoices;
