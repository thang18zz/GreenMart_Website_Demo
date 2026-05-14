-- 1. TẠO CƠ SỞ DỮ LIỆU
CREATE DATABASE GreenMart;
GO
USE GreenMart;
GO

-- ==========================================
-- PHẦN 1: QUẢN LÝ TÀI KHOẢN & NGƯỜI DÙNG (Epic: Quản lý tài khoản)
-- ==========================================

-- Bảng Phân quyền (Role)
CREATE TABLE Roles (
    RoleID INT IDENTITY(1,1) PRIMARY KEY,
    RoleName NVARCHAR(50) NOT NULL -- Ví dụ: Customer, Admin, Manager, Staff
);

-- Bảng Người dùng (Dùng chung cho cả Khách hàng và Nhân viên - US-01, US-16, US-20)
CREATE TABLE Users (
    UserID INT IDENTITY(1,1) PRIMARY KEY,
    RoleID INT NOT NULL,
    FullName NVARCHAR(100) NOT NULL,
    Email VARCHAR(100) UNIQUE NOT NULL,
    PhoneNumber VARCHAR(20) UNIQUE NOT NULL,
    PasswordHash VARCHAR(255) NOT NULL,
    IsActive BIT DEFAULT 1, -- Trạng thái khóa/mở tài khoản (US-23)
    CreatedAt DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (RoleID) REFERENCES Roles(RoleID)
);

-- Bảng Sổ địa chỉ (US-05)
CREATE TABLE Addresses (
    AddressID INT IDENTITY(1,1) PRIMARY KEY,
    UserID INT NOT NULL,
    AddressType NVARCHAR(50), -- Ví dụ: Nhà riêng, Công ty
    FullAddress NVARCHAR(255) NOT NULL,
    IsDefault BIT DEFAULT 0,
    FOREIGN KEY (UserID) REFERENCES Users(UserID)
);

-- ==========================================
-- PHẦN 2: QUẢN LÝ SẢN PHẨM & KHO HÀNG (Epic: Quản lý Danh mục, Sản phẩm)
-- ==========================================

-- Bảng Danh mục sản phẩm (US-25 -> US-29)
CREATE TABLE Categories (
    CategoryID INT IDENTITY(1,1) PRIMARY KEY,
    CategoryName NVARCHAR(100) NOT NULL,
    Description NVARCHAR(500),
    IsActive BIT DEFAULT 1
);

-- Bảng Sản phẩm (US-30 -> US-34)
CREATE TABLE Products (
    ProductID INT IDENTITY(1,1) PRIMARY KEY,
    CategoryID INT NOT NULL,
    ProductName NVARCHAR(200) NOT NULL,
    Description NVARCHAR(MAX),
    Price DECIMAL(18, 2) NOT NULL,
    ImageURL VARCHAR(255),
    StockQuantity INT DEFAULT 0, -- Quản lý tồn kho (US-35)
    IsActive BIT DEFAULT 1,
    CreatedAt DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (CategoryID) REFERENCES Categories(CategoryID)
);

-- ==========================================
-- PHẦN 3: MUA SẮM, GIỎ HÀNG & ĐƠN HÀNG (Epic: Giỏ hàng & Thanh toán)
-- ==========================================

-- Bảng Giỏ hàng (US-11, US-12)
CREATE TABLE CartItems (
    CartItemID INT IDENTITY(1,1) PRIMARY KEY,
    UserID INT NOT NULL,
    ProductID INT NOT NULL,
    Quantity INT NOT NULL CHECK (Quantity > 0),
    AddedAt DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (UserID) REFERENCES Users(UserID),
    FOREIGN KEY (ProductID) REFERENCES Products(ProductID)
);

-- Bảng Đơn đặt hàng (US-13, US-14, US-48, US-49)
CREATE TABLE Orders (
    OrderID INT IDENTITY(1,1) PRIMARY KEY,
    UserID INT NOT NULL,
    OrderDate DATETIME DEFAULT GETDATE(),
    TotalAmount DECIMAL(18, 2) NOT NULL,
    OrderStatus NVARCHAR(50) DEFAULT N'Chờ duyệt', -- Chờ duyệt, Đang giao, Hoàn thành, Đã hủy
    PaymentStatus NVARCHAR(50) DEFAULT N'Chưa thanh toán', -- Chưa thanh toán, Đã thanh toán
    ShippingAddressID INT NOT NULL,
    Notes NVARCHAR(500),
    FOREIGN KEY (UserID) REFERENCES Users(UserID),
    FOREIGN KEY (ShippingAddressID) REFERENCES Addresses(AddressID)
);

-- Bảng Chi tiết đơn hàng (US-15)
CREATE TABLE OrderDetails (
    OrderDetailID INT IDENTITY(1,1) PRIMARY KEY,
    OrderID INT NOT NULL,
    ProductID INT NOT NULL,
    Quantity INT NOT NULL CHECK (Quantity > 0),
    UnitPrice DECIMAL(18, 2) NOT NULL, -- Lưu lại giá tại thời điểm mua
    FOREIGN KEY (OrderID) REFERENCES Orders(OrderID),
    FOREIGN KEY (ProductID) REFERENCES Products(ProductID)
);

-- ==========================================
-- PHẦN 4: NHÀ CUNG CẤP & NHẬP KHO (Epic: Quản lý Nhà cung cấp, Nhập hàng)
-- ==========================================

-- Bảng Nhà cung cấp (US-39 -> US-43)
CREATE TABLE Suppliers (
    SupplierID INT IDENTITY(1,1) PRIMARY KEY,
    SupplierName NVARCHAR(150) NOT NULL,
    ContactPerson NVARCHAR(100),
    PhoneNumber VARCHAR(20),
    Email VARCHAR(100),
    Address NVARCHAR(255),
    IsActive BIT DEFAULT 1
);

-- Bảng Phiếu nhập hàng (US-44 -> US-47)
CREATE TABLE PurchaseReceipts (
    ReceiptID INT IDENTITY(1,1) PRIMARY KEY,
    SupplierID INT NOT NULL,
    CreatedBy INT NOT NULL, -- Nhân viên/Quản lý tạo phiếu (Link tới Users)
    ReceiptDate DATETIME DEFAULT GETDATE(),
    TotalAmount DECIMAL(18, 2) NOT NULL,
    Status NVARCHAR(50) DEFAULT N'Hoàn thành',
    Notes NVARCHAR(500),
    FOREIGN KEY (SupplierID) REFERENCES Suppliers(SupplierID),
    FOREIGN KEY (CreatedBy) REFERENCES Users(UserID)
);

-- Bảng Chi tiết phiếu nhập
CREATE TABLE PurchaseReceiptDetails (
    ReceiptDetailID INT IDENTITY(1,1) PRIMARY KEY,
    ReceiptID INT NOT NULL,
    ProductID INT NOT NULL,
    Quantity INT NOT NULL CHECK (Quantity > 0),
    CostPrice DECIMAL(18, 2) NOT NULL, -- Giá nhập vào
    FOREIGN KEY (ReceiptID) REFERENCES PurchaseReceipts(ReceiptID),
    FOREIGN KEY (ProductID) REFERENCES Products(ProductID)
);
GO