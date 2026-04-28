-- ============================================================
-- PhoneHub Database Script
-- Motor: MySQL 8.0
-- ============================================================

DROP DATABASE IF EXISTS DbPhoneHub;
CREATE DATABASE DbPhoneHub
    CHARACTER SET utf8mb4
    COLLATE utf8mb4_0900_ai_ci;

USE DbPhoneHub;

-- ============================================================
-- TABLA: users
-- ============================================================
CREATE TABLE `users` (
    `Id`        INT             NOT NULL AUTO_INCREMENT,
    `FirstName` VARCHAR(50)     NOT NULL,
    `LastName`  VARCHAR(50)     NOT NULL,
    `Email`     VARCHAR(100)    NOT NULL,
    `Password`  VARCHAR(200)    NOT NULL,
    `Role`      VARCHAR(20)     NOT NULL,
    `Telephone` VARCHAR(15)     NULL,
    `IsActive`  BIT(1)          NOT NULL,
    PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- ============================================================
-- TABLA: products
-- ============================================================
CREATE TABLE `products` (
    `Id`          INT             NOT NULL AUTO_INCREMENT,
    `Brand`       VARCHAR(50)     NOT NULL,
    `Model`       VARCHAR(100)    NOT NULL,
    `Description` VARCHAR(500)    NULL,
    `Price`       DECIMAL(18,2)   NOT NULL,
    `Stock`       INT             NOT NULL DEFAULT 0,
    `CreatedAt`   DATETIME        NOT NULL DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- ============================================================
-- TABLA: sales
-- ============================================================
CREATE TABLE `sales` (
    `Id`          INT             NOT NULL AUTO_INCREMENT,
    `ProductId`   INT             NOT NULL,
    `UserId`      INT             NOT NULL,
    `Quantity`    INT             NOT NULL DEFAULT 1,
    `TotalAmount` DECIMAL(18,2)   NOT NULL,
    `Date`        DATETIME        NOT NULL,
    `IsActive`    BIT(1)          NOT NULL,
    PRIMARY KEY (`Id`),
    INDEX `FK_Sale_Product` (`ProductId`),
    INDEX `FK_Sale_User`    (`UserId`),
    CONSTRAINT `FK_Sale_Product` FOREIGN KEY (`ProductId`) REFERENCES `products` (`Id`),
    CONSTRAINT `FK_Sale_User`    FOREIGN KEY (`UserId`)    REFERENCES `users`    (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- ============================================================
-- DATOS: users
-- ============================================================
INSERT INTO `users` (`FirstName`, `LastName`, `Email`, `Password`, `Role`, `Telephone`, `IsActive`) VALUES
('Carlos', 'Mamani',  'carlos@phonehub.com', 'password123', 'Admin',   '77712345', 1),
('Ana',    'Flores',  'ana@phonehub.com',    'password123', 'Cliente', '76698765', 1),
('Luis',   'Quispe',  'luis@phonehub.com',   'password123', 'Cliente', '71134567', 1),
('María',  'Torrez',  'maria@phonehub.com',  'password123', 'Cliente', '79923456', 1);

-- ============================================================
-- DATOS: products
-- ============================================================
INSERT INTO `products` (`Brand`, `Model`, `Description`, `Price`, `Stock`, `CreatedAt`) VALUES
('Samsung',  'Galaxy A55',    '128GB AMOLED 6.6" cámara 50MP',          1850.00, 15, NOW()),
('Samsung',  'Galaxy S24',    '256GB IA integrada flagship',             4200.00,  8, NOW()),
('Xiaomi',   'Redmi Note 13', '128GB batería 5000mAh cámara 108MP',     1200.00, 20, NOW()),
('Xiaomi',   'POCO X6',       '256GB Snapdragon 7s Gen 2 carga 67W',    1650.00, 12, NOW()),
('Apple',    'iPhone 15',     '128GB chip A16 Dynamic Island',           5800.00,  5, NOW()),
('Apple',    'iPhone 14',     '128GB chip A15 MagSafe',                  4500.00,  3, NOW()),
('Motorola', 'Moto G84',      '256GB pOLED 5G',                          1400.00, 18, NOW()),
('Motorola', 'Edge 40',       '256GB pantalla curva 144Hz',              2100.00,  7, NOW()),
('Huawei',   'Nova 11',       '256GB cámara frontal 60MP',               1750.00,  0, NOW()),
('Samsung',  'Galaxy A14',    '64GB económico entrada de gama',           850.00,  0, NOW());

-- ============================================================
-- DATOS: sales
-- ============================================================
INSERT INTO `sales` (`ProductId`, `UserId`, `Quantity`, `TotalAmount`, `Date`, `IsActive`) VALUES
(1, 2, 1, 1850.00, DATE_SUB(NOW(), INTERVAL 30 DAY), 1),
(3, 3, 2, 2400.00, DATE_SUB(NOW(), INTERVAL 22 DAY), 1),
(5, 4, 1, 5800.00, DATE_SUB(NOW(), INTERVAL 15 DAY), 1),
(7, 2, 1, 1400.00, DATE_SUB(NOW(), INTERVAL  8 DAY), 1),
(2, 3, 1, 4200.00, DATE_SUB(NOW(), INTERVAL  3 DAY), 1);
