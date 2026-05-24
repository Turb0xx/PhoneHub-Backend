# PhoneHub API — Documentación Técnica Completa

Sistema de gestión y venta de celulares. Backend REST API desarrollado en **C# .NET 9** con arquitectura limpia en 4 capas.

---

## Índice

1. [Arquitectura del proyecto](#1-arquitectura-del-proyecto)
2. [Tecnologías y paquetes](#2-tecnologías-y-paquetes)
3. [Base de datos — Tablas y tipos de datos](#3-base-de-datos--tablas-y-tipos-de-datos)
4. [Entidades del dominio](#4-entidades-del-dominio)
5. [DTOs (Data Transfer Objects)](#5-dtos-data-transfer-objects)
6. [Endpoints de la API](#6-endpoints-de-la-api)
7. [Reglas de negocio implementadas](#7-reglas-de-negocio-implementadas)
8. [Casos de uso implementados](#8-casos-de-uso-implementados)
9. [Autenticación JWT](#9-autenticación-jwt)
10. [Validaciones (FluentValidation)](#10-validaciones-fluentvalidation)
11. [Paginación y filtros](#11-paginación-y-filtros)
12. [Dapper vs Entity Framework](#12-dapper-vs-entity-framework)
13. [Manejo de errores](#13-manejo-de-errores)
14. [Datos de prueba (Seed)](#14-datos-de-prueba-seed)
15. [Archivos del proyecto](#15-archivos-del-proyecto)

---

## 1. Arquitectura del proyecto

```
PhoneHub/
├── PhoneHub.Core/           — Entidades, interfaces, DTOs, enums, excepciones
├── PhoneHub.Infrastructure/ — Repositorios, EF DbContext, Dapper, queries SQL, mappings
├── PhoneHub.Services/       — Lógica de negocio, validadores FluentValidation
└── PhoneHub.Api/            — Controllers, middleware, Program.cs, Swagger
```

**Patrón Repository + Unit of Work**: todas las operaciones de escritura pasan por `IUnitOfWork`. Los GETs usan Dapper directamente.

---

## 2. Tecnologías y paquetes

| Paquete | Versión | Uso |
|---|---|---|
| .NET 9 | 9.0 | Framework base |
| Entity Framework Core | 9.0.13 | Operaciones de escritura (INSERT, UPDATE, DELETE) |
| Pomelo.EntityFrameworkCore.MySql | — | Proveedor MySQL para EF |
| Dapper | — | Todas las consultas SELECT (GETs) |
| MySqlConnector | — | Conexión Dapper a MySQL |
| Microsoft.AspNetCore.Authentication.JwtBearer | 9.0.15 | Autenticación JWT |
| AutoMapper | 13.0.1 | Mapeo entidades → DTOs (Product) |
| FluentValidation | — | Validación de DTOs de entrada |
| Swashbuckle.AspNetCore | 9.0.6 | Documentación Swagger/OpenAPI |
| Newtonsoft.Json | — | Serialización JSON con manejo de referencias circulares |

**Base de datos**: MySQL 8.x
**Puerto API**: `http://localhost:5065`
**Swagger UI**: `http://localhost:5065` (raíz)

---

## 3. Base de datos — Tablas y tipos de datos

### Base de datos: `DbPhoneHub`

---

### Tabla `users`

| Columna | Tipo MySQL | Restricciones | Descripción |
|---|---|---|---|
| `Id` | `INT` | PK, AUTO_INCREMENT | Identificador único |
| `FirstName` | `VARCHAR(50)` | NOT NULL | Nombre del usuario |
| `LastName` | `VARCHAR(50)` | NOT NULL | Apellido del usuario |
| `Email` | `VARCHAR(100)` | NOT NULL, UNIQUE | Email — debe ser único (RN-04) |
| `Password` | `VARCHAR(200)` | NOT NULL | Contraseña hasheada SHA-256 + Salt en Base64 |
| `Role` | `VARCHAR(20)` | NOT NULL | Rol: `Admin` o `Seller` |
| `Telephone` | `VARCHAR(15)` | NULL | Teléfono opcional |
| `IsActive` | `BIT` | NOT NULL, DEFAULT 1 | Estado activo/inactivo (baja lógica RN-06) |

**Índices**: `UQ_User_Email` (único sobre Email)

---

### Tabla `products`

| Columna | Tipo MySQL | Restricciones | Descripción |
|---|---|---|---|
| `Id` | `INT` | PK, AUTO_INCREMENT | Identificador único |
| `Brand` | `VARCHAR(50)` | NOT NULL | Marca del teléfono |
| `Model` | `VARCHAR(100)` | NOT NULL | Modelo del teléfono |
| `Description` | `VARCHAR(500)` | NULL | Descripción opcional |
| `Price` | `DECIMAL(18,2)` | NOT NULL | Precio unitario — debe ser > 0 (RN-08) |
| `Stock` | `INT` | NOT NULL, DEFAULT 0 | Unidades disponibles — nunca negativo (RN-09) |
| `CreatedAt` | `DATETIME` | NOT NULL, DEFAULT CURRENT_TIMESTAMP | Fecha de creación |

---

### Tabla `sales`

| Columna | Tipo MySQL | Restricciones | Descripción |
|---|---|---|---|
| `Id` | `INT` | PK, AUTO_INCREMENT | Identificador único |
| `ProductId` | `INT` | NOT NULL, FK → products(Id) | Producto vendido |
| `UserId` | `INT` | NOT NULL, FK → users(Id) | Vendedor que procesó la venta |
| `Quantity` | `INT` | NOT NULL, DEFAULT 1 | Cantidad de unidades vendidas |
| `TotalAmount` | `DECIMAL(18,2)` | NOT NULL | Total = Price × Quantity, calculado automáticamente (RN-02) |
| `Date` | `DATETIME` | NOT NULL | Fecha y hora de la venta |
| `IsActive` | `BIT` | NOT NULL | Estado: `1` activa, `0` anulada (baja lógica RN-05) |

**FK**: `FK_Sale_Product` → `products(Id)`, `FK_Sale_User` → `users(Id)`

---

### Tabla `invoices`

| Columna | Tipo MySQL | Restricciones | Descripción |
|---|---|---|---|
| `Id` | `INT` | PK, AUTO_INCREMENT | Identificador único |
| `SaleId` | `INT` | NOT NULL, FK → sales(Id), UNIQUE | Una venta = una sola factura (RN-12) |
| `InvoiceNumber` | `VARCHAR(20)` | NOT NULL, UNIQUE | Número único auto-generado ej: `PH-2026-000001` (RN-13) |
| `IssuedAt` | `DATETIME` | NOT NULL | Fecha y hora de emisión de la factura |

**Índices únicos**: `UQ_Invoice_SaleId`, `UQ_Invoice_Number`
**FK**: `FK_Invoice_Sale` → `sales(Id)`

---

## 4. Entidades del dominio

### `BaseEntity`
```
Id: int  (clave primaria compartida por todas las entidades)
```

### `User : BaseEntity`
```
FirstName: string
LastName:  string
Email:     string
Password:  string  (SHA-256 hasheada)
Role:      string  (Admin | Seller)
Telephone: string?
IsActive:  bool
Sales:     ICollection<Sale>
```

### `Product : BaseEntity`
```
Brand:       string
Model:       string
Description: string?
Price:       decimal
Stock:       int
CreatedAt:   DateTime
Sales:       ICollection<Sale>
```

### `Sale : BaseEntity`
```
ProductId:   int
UserId:      int
Quantity:    int
TotalAmount: decimal
Date:        DateTime
IsActive:    bool
Product:     Product  (navegación)
User:        User     (navegación)
```

### `Invoice : BaseEntity`
```
SaleId:        int
InvoiceNumber: string
IssuedAt:      DateTime
Sale:          Sale  (navegación)
```

### `UserLogin` (no es entidad BD)
```
Email:    string
Password: string
```

---

## 5. DTOs (Data Transfer Objects)

### `ProductDto` — respuesta de producto
```
Id:               int
Brand:            string
Model:            string
Description:      string?
Price:            decimal
Stock:            int
RegistrationDate: string?
```

### `UserDto` — respuesta de usuario (sin contraseña)
```
Id:        int
FirstName: string
LastName:  string
Email:     string
Telephone: string?
Role:      string
IsActive:  bool
```

### `CreateUserDto` — solicitud crear/actualizar usuario
```
FirstName: string   (obligatorio, max 50)
LastName:  string   (obligatorio, max 50)
Email:     string   (obligatorio, formato email, max 100)
Password:  string   (obligatorio, min 6 caracteres)
Role:      string   (Admin | Seller, default: Seller)
Telephone: string?  (opcional, max 15)
```

### `SaleRequestDto` — solicitud de venta
```
ProductId: int
UserId:    int
Quantity:  int
```

### `SaleResponseDto` — respuesta de venta (JOIN con Dapper)
```
SaleId:        int
Date:          string
ProductId:     int
Brand:         string
Model:         string
UnitPrice:     decimal
Quantity:      int
TotalAmount:   decimal
UserId:        int
CustomerName:  string
CustomerEmail: string
```

### `InvoiceDto` — respuesta de factura (JOIN completo con Dapper)
```
InvoiceId:     int
InvoiceNumber: string
IssuedAt:      string
SaleId:        int
SaleDate:      string
ProductId:     int
Brand:         string
Model:         string
UnitPrice:     decimal
Quantity:      int
TotalAmount:   decimal
UserId:        int
SellerName:    string
SellerEmail:   string
```

### `InventoryIngressDto` — solicitud ingreso de stock
```
ProductId: int
Quantity:  int  (debe ser > 0, RN-03)
```

### `SellerSummaryDto` — resumen por vendedor en reporte
```
UserId:      int
SellerName:  string
SellerEmail: string
SalesCount:  int
SubTotal:    decimal
```

### `CashCloseReportDto` — reporte cierre de caja
```
Date:        string
TotalSales:  int
TotalAmount: decimal
Sellers:     IEnumerable<SellerSummaryDto>
```

---

## 6. Endpoints de la API

Todos los endpoints requieren `Authorization: Bearer {token}` excepto `POST /api/Token`.

### TokenController — Autenticación

| Método | Ruta | Auth | Descripción |
|---|---|---|---|
| POST | `/api/Token` | No | Login — retorna JWT |

**Body POST /api/Token:**
```json
{ "email": "admin@phonehub.com", "password": "admin123" }
```
**Respuesta:**
```json
{ "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..." }
```

---

### ProductController — Gestión de productos

| Método | Ruta | Roles | Descripción |
|---|---|---|---|
| GET | `/api/Product` | Admin, Seller | Lista paginada con filtros (CU-04) |
| GET | `/api/Product/{id}` | Admin, Seller | Detalle de un producto (CU-04) |
| POST | `/api/Product` | Admin, Seller | Crear producto |
| PUT | `/api/Product/{id}` | Admin, Seller | Actualizar producto |
| DELETE | `/api/Product/{id}` | Admin, Seller | Eliminar producto (falla si tiene ventas) |
| POST | `/api/Product/ingreso-inventario` | Admin, Seller | Agregar stock (CU-01) |
| GET | `/api/Product/dapper` | Admin, Seller | Lista con límite (endpoint legacy) |

**Query filters GET /api/Product:**
```
Brand=Samsung          → filtra por marca (contiene)
Model=Galaxy           → filtra por modelo (contiene)
MaxPrice=500           → precio máximo
OnlyAvailable=true     → solo con stock > 0
PageNumber=1           → página (default: 1)
PageSize=10            → registros por página (default: 10)
```

---

### SaleController — Ventas

| Método | Ruta | Roles | Descripción |
|---|---|---|---|
| GET | `/api/Sale` | Admin, Seller | Lista paginada de ventas |
| GET | `/api/Sale/{id}` | Admin, Seller | Detalle de una venta (CU-05) |
| POST | `/api/Sale/procesar` | Admin, Seller | Procesar nueva venta (CU-03) |
| DELETE | `/api/Sale/{id}` | **Admin** | Anular venta — baja lógica (RN-05) |
| GET | `/api/Sale/reporte/cierre-diario` | **Admin** | Reporte de cierre de caja (CU-07) |
| GET | `/api/Sale/dapper` | Admin, Seller | Lista con límite (endpoint legacy) |

**Body POST /api/Sale/procesar:**
```json
{ "productId": 3, "userId": 2, "quantity": 2 }
```

**Query filter GET /api/Sale:**
```
UserId=2      → solo ventas de ese vendedor
ProductId=1   → solo ventas de ese producto
PageNumber=1
PageSize=10
```

---

### InvoiceController — Facturas

| Método | Ruta | Roles | Descripción |
|---|---|---|---|
| POST | `/api/Invoice/generate/{saleId}` | Admin, Seller | Generar factura para una venta (CU-05) |
| GET | `/api/Invoice/{id}` | Admin, Seller | Obtener factura por ID |
| GET | `/api/Invoice/sale/{saleId}` | Admin, Seller | Obtener factura por ID de venta |

---

### UserController — Gestión de usuarios

| Método | Ruta | Roles | Descripción |
|---|---|---|---|
| GET | `/api/User` | **Admin** | Lista todos los usuarios (CU-06) |
| GET | `/api/User/{id}` | **Admin** | Detalle de un usuario (CU-06) |
| POST | `/api/User` | **Admin** | Crear usuario (CU-02) |
| PUT | `/api/User/{id}` | **Admin** | Actualizar usuario (CU-02) |
| PATCH | `/api/User/{id}/activar` | **Admin** | Reactivar usuario desactivado |
| DELETE | `/api/User/{id}` | **Admin** | Desactivar usuario — baja lógica (RN-06) |

---

## 7. Reglas de negocio implementadas

| ID | Descripción | Dónde se aplica |
|---|---|---|
| RN-01 | No se puede vender si el stock es menor a la cantidad solicitada | `SaleService.ProcessSaleAsync` |
| RN-02 | TotalAmount = Price × Quantity, calculado automáticamente | `SaleService.ProcessSaleAsync` |
| RN-03 | La cantidad en ingreso de inventario debe ser > 0 | `InventoryIngressDtoValidator` |
| RN-04 | El email de cada usuario debe ser único | `UserService.CreateUserAsync / UpdateUserAsync` |
| RN-05 | Una venta no se elimina físicamente: `IsActive = false` | `SaleService.AnnulSaleAsync` |
| RN-06 | Solo Admin puede gestionar usuarios | `[Authorize(Roles = "Admin")]` en `UserController` |
| RN-07 | Un usuario con `IsActive = false` no puede iniciar sesión | `UserService.GetByCredentialsAsync` → 403 |
| RN-08 | El precio del producto debe ser > 0 | `CrearProductoDtoValidator` |
| RN-09 | El stock no puede ser negativo | `SaleService` (verifica antes de descontar) |
| RN-10 | El reporte de cierre solo considera ventas con `IsActive = true` | `SaleQueries.GetDailyReportMySql` (WHERE IsActive = 1) |
| RN-11 | Una venta debe tener al menos un producto | `SaleRequestDtoValidator` |
| RN-12 | Una venta solo puede tener una factura | `InvoiceService` (verifica `ExistsBySaleIdAsync`) → 409 Conflict |
| RN-13 | El número de factura es único y auto-generado | `InvoiceRepository.GenerateInvoiceNumberAsync` → formato `PH-{AÑO}-{000001}` |

---

## 8. Casos de uso implementados

| ID | Nombre | Endpoints |
|---|---|---|
| CU-01 | Nuevo ingreso de inventario | `POST /api/Product/ingreso-inventario` |
| CU-02 | Gestión de usuarios (Admin) | `POST/PUT /api/User`, `PATCH /api/User/{id}/activar` |
| CU-03 | Procesar una venta | `POST /api/Sale/procesar` |
| CU-04 | Consultar disponibilidad y detalles de stock | `GET /api/Product` (con filtros) |
| CU-05 | Generar factura / comprobante de venta | `POST /api/Invoice/generate/{saleId}`, `GET /api/Invoice/sale/{saleId}` |
| CU-06 | Autenticar usuario (Login) | `POST /api/Token` |
| CU-07 | Reporte de cierre de caja diario | `GET /api/Sale/reporte/cierre-diario` |

---

## 9. Autenticación JWT

**Algoritmo**: HS256 (HMAC SHA-256)
**Expiración**: 8 horas
**Claims en el token**:

| Claim | Valor |
|---|---|
| `Id` | ID del usuario |
| `Name` | Nombre completo |
| `Email` | Email del usuario |
| `ClaimTypes.Role` | `Admin` o `Seller` |
| `nbf` | Not before (emisión) |
| `exp` | Expiración (8h) |
| `iss` | `https://api.phonehub.local` |
| `aud` | `https://frontend.phonehub.local` |

**Configuración en appsettings.json:**
```json
"Authentication": {
  "SecretKey": "PhoneHub$SuperSecretKey2025!XyZ#987654321",
  "Issuer":    "https://api.phonehub.local",
  "Audience":  "https://frontend.phonehub.local"
}
```

**Uso en Swagger**: botón **Authorize** (candado) — pegar solo el token sin la palabra "Bearer".

---

## 10. Validaciones (FluentValidation)

### `CrearProductoDtoValidator`
- Brand: obligatorio, max 50
- Model: obligatorio, max 100
- Price: obligatorio, > 0 (RN-08)
- Stock: >= 0 (RN-09)

### `ActualizarProductoDtoValidator`
- Mismas reglas + Id obligatorio > 0

### `InventoryIngressDtoValidator`
- ProductId: obligatorio, > 0
- Quantity: obligatorio, > 0 (RN-03)

### `SaleRequestDtoValidator`
- ProductId: obligatorio, > 0
- UserId: obligatorio, > 0
- Quantity: obligatorio, >= 1 (RN-11)

### `CreateUserDtoValidator`
- FirstName: obligatorio, max 50
- LastName: obligatorio, max 50
- Email: obligatorio, formato válido, max 100
- Password: obligatorio, min 6 caracteres
- Role: obligatorio, solo `Admin` o `Seller`
- Telephone: opcional, max 15

---

## 11. Paginación y filtros

### `PaginationQueryFilter` (base)
```
PageNumber: int  (default: 1)
PageSize:   int  (default: 10)
```

### `ProductQueryFilter : PaginationQueryFilter`
```
Brand:         string?   — filtra por marca (contiene, case-insensitive)
Model:         string?   — filtra por modelo (contiene, case-insensitive)
MaxPrice:      decimal?  — precio máximo
OnlyAvailable: bool?     — solo productos con Stock > 0
```

### `SaleQueryFilter : PaginationQueryFilter`
```
UserId:    int?  — filtra por vendedor
ProductId: int?  — filtra por producto
```

### Respuesta paginada `ApiResponse<T>`
```json
{
  "data": [...],
  "pagination": {
    "totalCount": 8,
    "pageSize": 10,
    "currentPage": 1,
    "totalPages": 1,
    "hasNextPage": false,
    "hasPreviousPage": false
  },
  "messages": [
    { "type": "success", "description": "Productos recuperados correctamente." }
  ]
}
```

**Tipos de mensaje**: `success`, `warning`, `error`

---

## 12. Dapper vs Entity Framework

### GETs — Dapper (SELECT)

| Operación | Clase Query | Descripción |
|---|---|---|
| GET todos los productos | `ProductQueries.GetAllMySql` | SELECT con ORDER BY Brand |
| GET producto por ID | `ProductQueries.GetByIdMySql` | SELECT WHERE Id = @Id |
| GET todos los usuarios | `UserQueries.GetAllMySql` | SELECT sin password |
| GET usuario por ID | `UserQueries.GetByIdMySql` | SELECT WHERE Id = @Id |
| GET todas las ventas | `SaleQueries.GetAllWithDetailsMySql` | JOIN sales + products + users |
| GET venta por ID | `SaleQueries.GetByIdWithDetailsMySql` | JOIN con WHERE Id = @Id |
| GET reporte diario | `SaleQueries.GetDailyReportMySql` | GROUP BY vendedor, WHERE IsActive=1 y fecha |
| GET factura por ID | `InvoiceQueries.GetByIdMySql` | JOIN invoices + sales + products + users |
| GET factura por SaleId | `InvoiceQueries.GetBySaleIdMySql` | JOIN con WHERE SaleId = @SaleId |

### Escrituras — Entity Framework (INSERT / UPDATE / DELETE lógico)

| Operación | Método EF |
|---|---|
| Crear producto | `ProductRepository.Add` |
| Actualizar producto | `ProductRepository.Update` |
| Eliminar producto | `ProductRepository.Delete` |
| Agregar stock | `ProductRepository.Update` (modifica Stock) |
| Crear venta | `SaleRepository.Add` |
| Anular venta (RN-05) | `SaleRepository.Update` (IsActive = false) |
| Crear usuario | `UserRepository.Add` |
| Actualizar usuario | `UserRepository.Update` |
| Desactivar usuario | `UserRepository.Update` (IsActive = false) |
| Reactivar usuario | `UserRepository.Update` (IsActive = true) |
| Generar factura | `InvoiceRepository.Add` |
| Verificar email único | `UserRepository.GetByEmailAsync` (EF — validación interna) |
| Verificar credenciales | `UserRepository.GetByCredentialsAsync` (EF — autenticación) |
| Verificar factura existe | `InvoiceRepository.ExistsBySaleIdAsync` (EF — validación RN-12) |
| Generar número factura | `InvoiceRepository.GenerateInvoiceNumberAsync` (EF Count — RN-13) |

---

## 13. Manejo de errores

El middleware `ExceptionHandlingMiddleware` captura todas las excepciones y retorna:

```json
{
  "status": 400,
  "title": "BadRequest",
  "message": "Errores de validación en los datos enviados.",
  "errors": [
    { "field": "Quantity", "error": "La cantidad debe ser mayor a cero." }
  ],
  "traceId": "0HN8..."
}
```

### Tipos de excepción

| Excepción | HTTP Code | Cuándo |
|---|---|---|
| `NotFoundException` | 404 | Recurso no encontrado |
| `BusinessException` | Configurable | Violación de regla de negocio |
| `ValidationException` | 400 | Datos de entrada inválidos (FluentValidation) |
| Sin capturar | 500 | Error interno no controlado |

---

## 14. Datos de prueba (Seed)

### Usuarios

| ID | Email | Password | Rol | Activo |
|---|---|---|---|---|
| 1 | `admin@phonehub.com` | `admin123` | Admin | Sí |
| 2 | `carlos@phonehub.com` | `seller123` | Seller | Sí |
| 3 | `maria@phonehub.com` | `seller456` | Seller | Sí |
| 4 | `jorge@phonehub.com` | `seller123` | Seller | **No** (prueba RN-07) |

> Las contraseñas se almacenan como `SHA-256("PhoneHub$Salt2025!" + password)` en Base64.

### Productos (8)

| ID | Marca | Modelo | Precio | Stock |
|---|---|---|---|---|
| 1 | Samsung | Galaxy S24 | $899.99 | 13 |
| 2 | Apple | iPhone 15 | $1,099.99 | 9 |
| 3 | Xiaomi | Redmi Note 13 | $249.99 | 27 |
| 4 | Motorola | Edge 40 | $449.99 | 20 |
| 5 | Huawei | P60 Pro | $749.99 | 8 |
| 6 | OnePlus | 12 | $649.99 | 11 |
| 7 | Samsung | Galaxy A55 | $399.99 | 23 |
| 8 | Apple | iPhone 14 | $849.99 | 5 |

### Ventas (6)

| ID | Producto | Vendedor | Qty | Total | IsActive |
|---|---|---|---|---|---|
| 1 | Galaxy S24 | Carlos | 2 | $1,799.98 | Activa |
| 2 | iPhone 15 | Maria | 1 | $1,099.99 | Activa |
| 3 | Redmi Note 13 | Carlos | 3 | $749.97 | Activa |
| 4 | Motorola Edge 40 | Maria | 1 | $449.99 | **Anulada** (RN-05) |
| 5 | Galaxy A55 | Carlos | 2 | $799.98 | Activa (hoy) |
| 6 | OnePlus 12 | Maria | 1 | $649.99 | Activa (hoy) |

### Facturas (2)

| ID | SaleId | Número | Estado |
|---|---|---|---|
| 1 | 1 | `PH-2026-000001` | Emitida |
| 2 | 2 | `PH-2026-000002` | Emitida |

> Ventas 3, 5 y 6 no tienen factura — disponibles para probar `POST /api/Invoice/generate/{saleId}`.

---

## 15. Archivos del proyecto

### PhoneHub.Core
```
Entities/
  BaseEntity.cs          — Id compartido
  User.cs                — Entidad usuario
  Product.cs             — Entidad producto
  Sale.cs                — Entidad venta
  Invoice.cs             — Entidad factura
  UserLogin.cs           — Credenciales de login (no es tabla)

DTOs/
  UserDto.cs             — Respuesta usuario (sin password)
  CreateUserDto.cs       — Request crear/actualizar usuario
  ProductDto.cs          — Respuesta producto
  SaleRequestDto.cs      — Request procesar venta
  SaleResponseDto.cs     — Respuesta venta (JOIN Dapper)
  InvoiceDto.cs          — Respuesta factura (JOIN Dapper)
  InventoryIngressDto.cs — Request ingreso de stock
  SellerSummaryDto.cs    — Resumen vendedor en reporte
  CashCloseReportDto.cs  — Reporte cierre de caja

Interfaces/
  IBaseRepository.cs     — CRUD genérico
  IProductRepository.cs  — Dapper GETs de producto
  ISaleRepository.cs     — Dapper GETs de venta + reporte
  IUserRepository.cs     — Dapper GETs + validaciones EF
  IInvoiceRepository.cs  — Dapper GETs + lógica EF
  IUnitOfWork.cs         — Coordinador de repositorios
  IDapperContext.cs      — Abstracción Dapper
  IDbConnectionFactory.cs

CustomEntities/
  PagedList.cs           — Lista paginada genérica
  Pagination.cs          — Metadatos de paginación
  ResponseData.cs        — Wrapper paginado para controllers
  Message.cs             — Mensaje con tipo y descripción
  PasswordOptions.cs     — Configuración salt/iteraciones

QueryFilters/
  PaginationQueryFilter.cs  — PageNumber, PageSize
  ProductQueryFilter.cs     — + Brand, Model, MaxPrice, OnlyAvailable
  SaleQueryFilter.cs        — + UserId, ProductId

Enum/
  RoleType.cs            — Admin, Seller
  TypeMessage.cs         — success, warning, error
  DataBaseProvider.cs    — MySql, SqlServer

Exceptions/
  BusinessException.cs   — Excepción con HttpStatusCode configurable
  NotFoundException.cs   — Excepción 404
```

### PhoneHub.Infrastructure
```
Data/
  PhoneHubContext.cs     — DbContext con DbSets
  DapperContext.cs       — Implementación Dapper
  Configurations/
    ProductConfiguration.cs  — Mapeo tabla products
    SaleConfiguration.cs     — Mapeo tabla sales + FKs
    UserConfiguration.cs     — Mapeo tabla users
    InvoiceConfiguration.cs  — Mapeo tabla invoices + UNIQUE constraints

Repositories/
  BaseRepository.cs      — CRUD genérico EF
  ProductRepository.cs   — Dapper GETs + EF writes
  SaleRepository.cs      — Dapper GETs + reporte diario + EF
  UserRepository.cs      — Dapper GETs + EF para auth/validación
  InvoiceRepository.cs   — Dapper GETs + EF para lógica RN-12/13
  UnitOfWork.cs          — Coordina todos los repositorios
  DbConnectionFactory.cs — Crea conexiones según provider

Queries/
  ProductQueries.cs      — SQL SELECT para productos (MySQL + SqlServer)
  SaleQueries.cs         — SQL SELECT para ventas + reporte (MySQL + SqlServer)
  InvoiceQueries.cs      — SQL SELECT para facturas (MySQL + SqlServer)
  UserQueries.cs         — SQL SELECT para usuarios (MySQL + SqlServer)

Mappings/
  ProductProfile.cs      — AutoMapper Product ↔ ProductDto
  UserProfile.cs         — AutoMapper User ↔ UserDto
```

### PhoneHub.Services
```
Interfaces/
  IProductService.cs     — Contrato servicio productos
  ISaleService.cs        — Contrato servicio ventas
  IUserService.cs        — Contrato servicio usuarios
  IPasswordService.cs    — Contrato hashing contraseñas
  IInvoiceService.cs     — Contrato servicio facturas

Services/
  ProductService.cs      — Lógica productos + filtros + paginación
  SaleService.cs         — Lógica ventas: procesar, anular, reporte
  UserService.cs         — Lógica usuarios + RN-04 + RN-07
  PasswordService.cs     — SHA-256 + SaltKey hashing
  InvoiceService.cs      — RN-12 + RN-13 + generación factura

Validators/
  CrearProductoDtoValidator.cs      — Validación crear producto
  ActualizarProductoDtoValidator.cs — Validación actualizar producto
  InventoryIngressDtoValidator.cs   — Validación ingreso stock (RN-03)
  SaleRequestDtoValidator.cs        — Validación venta (RN-11)
  CreateUserDtoValidator.cs         — Validación crear/actualizar usuario
```

### PhoneHub.Api
```
Controllers/
  TokenController.cs   — POST /api/Token (login JWT)
  ProductController.cs — CRUD productos + ingreso inventario
  SaleController.cs    — Ventas + anulación + reporte cierre
  UserController.cs    — CRUD usuarios + activar/desactivar
  InvoiceController.cs — Generar y consultar facturas

Filters/
  ExceptionHandlingMiddleware.cs — Captura global de excepciones

Responses/
  ApiResponse.cs       — Wrapper genérico con Data, Pagination, Messages

Program.cs             — Registro de servicios, JWT, Swagger, middleware
appsettings.json       — ConnectionString, Authentication, PasswordOptions
phonehub_setup.sql     — Script completo BD: tablas + seed de datos
```

---

*Documentación generada el 24/05/2026 — PhoneHub v1.0*
