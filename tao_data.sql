USE GreenMart;
GO

-- ==========================================
-- 1. THÊM DỮ LIỆU PHÂN QUYỀN (ROLES)
-- ==========================================
INSERT INTO Roles (RoleName) VALUES 
('Admin'), 
('Manager'), 
('Staff'), 
('Customer');
GO

-- ==========================================
-- 2. THÊM DỮ LIỆU TÀI KHOẢN (USERS) (10 Tài khoản)
-- ID sinh ra tự động từ 1 đến 10
-- ==========================================
INSERT INTO Users (RoleID, FullName, Email, PhoneNumber, PasswordHash, IsActive) VALUES 
(1, N'Nguyễn Quản Trị', 'admin@greenmart.vn', '0901000001', 'hash_admin', 1),     -- ID: 1
(2, N'Trần Quản Lý', 'manager@greenmart.vn', '0902000002', 'hash_manager', 1),    -- ID: 2
(3, N'Lê Nhân Viên', 'staff@greenmart.vn', '0903000003', 'hash_staff', 1),        -- ID: 3
(4, N'Khách Hàng Một', 'khachhang1@gmail.com', '0988111222', 'hash_kh1', 1),      -- ID: 4
(4, N'Khách Hàng Hai', 'khachhang2@gmail.com', '0977333444', 'hash_kh2', 1),      -- ID: 5
(4, N'Phạm Văn C', 'phamvanc@gmail.com', '0911111111', 'hash_kh3', 1),            -- ID: 6
(4, N'Trần Thị D', 'tranthid@gmail.com', '0922222222', 'hash_kh4', 1),            -- ID: 7
(4, N'Lê Minh E', 'leminhe@gmail.com', '0933333333', 'hash_kh5', 1),              -- ID: 8
(4, N'Hoàng Thị F', 'hoangthif@gmail.com', '0944444444', 'hash_kh6', 0),          -- ID: 9 (Bị khóa)
(3, N'Nhân Viên Giao Hàng', 'shipper@greenmart.vn', '0999888777', 'hash_ship', 1); -- ID: 10
GO

-- ==========================================
-- 3. THÊM DỮ LIỆU SỔ ĐỊA CHỈ (ADDRESSES)
-- ==========================================
INSERT INTO Addresses (UserID, AddressType, FullAddress, IsDefault) VALUES 
(4, N'Nhà riêng', N'Số 123 Đường A, Quận 1, TP. HCM', 1),          -- ID: 1
(4, N'Công ty', N'Tòa nhà B, Số 456 Đường C, Quận 3, TP. HCM', 0), -- ID: 2
(5, N'Nhà riêng', N'Số 789 Đường X, Quận Cầu Giấy, Hà Nội', 1),    -- ID: 3
(6, N'Nhà riêng', N'12 Lê Lợi, Quận 1, TP.HCM', 1),                -- ID: 4
(7, N'Cơ quan', N'Tòa nhà X, Cầu Giấy, Hà Nội', 1),                -- ID: 5
(8, N'Nhà riêng', N'KĐT Mới, Thủ Đức, TP.HCM', 1);                 -- ID: 6
GO

-- ==========================================
-- 4. THÊM DỮ LIỆU DANH MỤC (CATEGORIES)
-- ==========================================
INSERT INTO Categories (CategoryName, Description, IsActive) VALUES 
(N'Đồ uống', N'Các loại nước giải khát, trà, cà phê', 1),          -- ID: 1
(N'Đồ ăn vặt', N'Bánh kẹo, hạt sấy khô, snack', 1),                -- ID: 2
(N'Thực phẩm tươi sống', N'Thịt, cá, rau củ quả tươi', 1),         -- ID: 3
(N'Gia vị', N'Mắm, muối, mì chính, dầu ăn', 1),                    -- ID: 4
(N'Sữa & Sản phẩm từ sữa', N'Sữa tươi, sữa chua, phô mai', 1),     -- ID: 5
(N'Trái cây tươi', N'Trái cây nội địa và nhập khẩu', 1),           -- ID: 6
(N'Hóa mỹ phẩm', N'Dầu gội, sữa tắm, nước giặt', 1);               -- ID: 7
GO

-- ==========================================
-- 5. THÊM DỮ LIỆU SẢN PHẨM (PRODUCTS) - Mặc định Tồn kho = 0
-- ==========================================
INSERT INTO Products (CategoryID, ProductName, Description, Price, ImageURL, StockQuantity, IsActive) VALUES 
(1, N'Nước ngọt Coca Cola 320ml', N'Nước giải khát có gas', 10000, 'coca.jpg', 0, 1),        -- 1
(1, N'Nước suối Aquafina 500ml', N'Nước tinh khiết', 5000, 'aquafina.jpg', 0, 1),            -- 2
(2, N'Bánh quy Cosy', N'Bánh quy bơ hộp 400g', 45000, 'cosy.jpg', 0, 1),                     -- 3
(2, N'Hạt điều rang muối 200g', N'Hạt điều Bình Phước', 80000, 'hatdieu.jpg', 0, 1),         -- 4
(3, N'Thịt ba chỉ heo', N'Thịt heo sạch 500g', 75000, 'thitbachi.jpg', 0, 1),                -- 5
(3, N'Rau muống VietGAP', N'Rau bó 500g', 15000, 'raumuong.jpg', 0, 1),                      -- 6
(4, N'Nước mắm Nam Ngư', N'Chai 750ml', 35000, 'namngu.jpg', 0, 1),                          -- 7
(4, N'Dầu ăn Tường An', N'Chai 1 Lít', 42000, 'dauan.jpg', 0, 1),                            -- 8
(5, N'Sữa tươi tiệt trùng Vinamilk 1L', N'Hộp giấy 1 Lít', 35000, 'vinamilk1l.jpg', 0, 1),   -- 9
(5, N'Sữa chua TH True Milk (Lốc 4)', N'Nha đam', 28000, 'thtruemilk.jpg', 0, 1),            -- 10
(5, N'Phô mai Con Bò Cười', N'Hộp 8 miếng', 32000, 'phomai.jpg', 0, 1),                      -- 11
(6, N'Táo Envy Mỹ (1kg)', N'Táo nhập khẩu', 180000, 'taoenvy.jpg', 0, 1),                    -- 12
(6, N'Nho mẫu đơn Hàn Quốc (500g)', N'Nho xanh không hạt', 350000, 'nhomaudon.jpg', 0, 1),   -- 13
(6, N'Chuối Dole Caven', N'Nải 1kg', 45000, 'chuoi.jpg', 0, 1),                              -- 14
(7, N'Dầu gội Clear Men 630g', N'Bạc hà', 155000, 'clearmen.jpg', 0, 1),                     -- 15
(7, N'Nước giặt OMO Matic 3.6kg', N'Cửa trước', 210000, 'omo.jpg', 0, 1),                    -- 16
(7, N'Kem đánh răng P/S 230g', N'Bảo vệ 12h', 38000, 'ps.jpg', 0, 1),                        -- 17
(2, N'Khoai tây chiên Oishi', N'Gói 90g vị tảo biển', 12000, 'oishi.jpg', 0, 1);             -- 18
GO

-- ==========================================
-- 6. THÊM DỮ LIỆU NHÀ CUNG CẤP (SUPPLIERS)
-- ==========================================
INSERT INTO Suppliers (SupplierName, ContactPerson, PhoneNumber, Email, Address, IsActive) VALUES 
(N'Công ty TNHH Nước Giải Khát', N'Anh Hoàng', '0911222333', 'hoang.ngk@test.com', N'Bình Dương', 1),    -- 1
(N'HTX Nông Sản Sạch', N'Chị Lan', '0922333444', 'nongsanlan@test.com', N'Củ Chi, TP. HCM', 1),          -- 2
(N'Tạp Hóa Tổng Hợp', N'Bác Tuấn', '0933444555', 'taphoatuan@test.com', N'Quận 5, TP. HCM', 1),          -- 3
(N'Công ty CP Sữa Vinamilk', N'Nguyễn Văn Sữa', '0909000111', 'npp@vinamilk.vn', N'Quận 7, TP.HCM', 1),  -- 4
(N'CTY XNK Trái Cây VN', N'Lê Nho', '0909000222', 'fruit@vietnam.vn', N'Chợ Thủ Đức', 1),                -- 5
(N'Tập đoàn Unilever', N'Trần Bột Giặt', '0909000333', 'contact@unilever.com', N'Củ Chi', 1);            -- 6
GO

-- ==========================================
-- 7. THỰC HIỆN NHẬP KHO (TỰ ĐỘNG CỘNG TỒN KHO)
-- ==========================================
DECLARE @ReceiptID INT;

-- Đợt 1: Nhập đồ uống
INSERT INTO PurchaseReceipts (SupplierID, CreatedBy, ReceiptDate, TotalAmount, Status, Notes) 
VALUES (1, 2, GETDATE() - 15, 1000000, N'Hoàn thành', N'Nhập nước giải khát');
SET @ReceiptID = SCOPE_IDENTITY();
INSERT INTO PurchaseReceiptDetails (ReceiptID, ProductID, Quantity, CostPrice) VALUES 
(@ReceiptID, 1, 100, 8000), (@ReceiptID, 2, 100, 3000);
UPDATE Products SET StockQuantity = StockQuantity + 100 WHERE ProductID IN (1, 2);

-- Đợt 2: Nhập Sữa và Hóa mỹ phẩm
INSERT INTO PurchaseReceipts (SupplierID, CreatedBy, ReceiptDate, TotalAmount, Status, Notes) 
VALUES (4, 2, GETDATE() - 10, 5000000, N'Hoàn thành', N'Nhập hàng đầu tháng');
SET @ReceiptID = SCOPE_IDENTITY();
INSERT INTO PurchaseReceiptDetails (ReceiptID, ProductID, Quantity, CostPrice) VALUES 
(@ReceiptID, 9, 200, 25000), (@ReceiptID, 10, 150, 20000), 
(@ReceiptID, 15, 50, 110000), (@ReceiptID, 16, 50, 160000);
UPDATE Products SET StockQuantity = StockQuantity + 200 WHERE ProductID = 9;
UPDATE Products SET StockQuantity = StockQuantity + 150 WHERE ProductID = 10;
UPDATE Products SET StockQuantity = StockQuantity + 50 WHERE ProductID IN (15, 16);

-- Đợt 3: Nhập Trái cây tươi
INSERT INTO PurchaseReceipts (SupplierID, CreatedBy, ReceiptDate, TotalAmount, Status, Notes) 
VALUES (5, 2, GETDATE() - 1, 3000000, N'Hoàn thành', N'Nhập trái cây tươi');
SET @ReceiptID = SCOPE_IDENTITY();
INSERT INTO PurchaseReceiptDetails (ReceiptID, ProductID, Quantity, CostPrice) VALUES 
(@ReceiptID, 12, 30, 130000), (@ReceiptID, 13, 20, 280000), (@ReceiptID, 14, 50, 30000);
UPDATE Products SET StockQuantity = StockQuantity + 30 WHERE ProductID = 12;
UPDATE Products SET StockQuantity = StockQuantity + 20 WHERE ProductID = 13;
UPDATE Products SET StockQuantity = StockQuantity + 50 WHERE ProductID = 14;
GO

-- ==========================================
-- 8. THÊM DỮ LIỆU GIỎ HÀNG
-- ==========================================
INSERT INTO CartItems (UserID, ProductID, Quantity) VALUES 
(4, 3, 2), -- User 4 đang để 2 bánh Cosy trong giỏ
(4, 7, 1);
GO

-- ==========================================
-- 9. TẠO CÁC ĐƠN HÀNG MẪU (VỚI ĐỦ TRẠNG THÁI)
-- ==========================================
DECLARE @OrderID INT;

-- Đơn hàng 1: Chờ duyệt
INSERT INTO Orders (UserID, OrderDate, TotalAmount, OrderStatus, PaymentStatus, ShippingAddressID, Notes) 
VALUES (5, GETDATE() - 2, 250000, N'Chờ duyệt', N'Chưa thanh toán', 3, N'Giao ngoài giờ hành chính');
SET @OrderID = SCOPE_IDENTITY();
INSERT INTO OrderDetails (OrderID, ProductID, Quantity, UnitPrice) VALUES 
(@OrderID, 1, 5, 10000), (@OrderID, 4, 2, 80000), (@OrderID, 8, 1, 40000);

-- Đơn hàng 2: Hoàn thành (Cần trừ tồn kho)
INSERT INTO Orders (UserID, OrderDate, TotalAmount, OrderStatus, PaymentStatus, ShippingAddressID, Notes) 
VALUES (4, GETDATE() - 5, 420000, N'Hoàn thành', N'Đã thanh toán', 1, N'Giao giờ hành chính');
SET @OrderID = SCOPE_IDENTITY();
INSERT INTO OrderDetails (OrderID, ProductID, Quantity, UnitPrice) VALUES 
(@OrderID, 16, 2, 210000); 
UPDATE Products SET StockQuantity = StockQuantity - 2 WHERE ProductID = 16; 

-- Đơn hàng 3: Đang giao hàng (Cần trừ tồn kho)
INSERT INTO Orders (UserID, OrderDate, TotalAmount, OrderStatus, PaymentStatus, ShippingAddressID, Notes) 
VALUES (6, GETDATE() - 1, 350000, N'Đang giao', N'Chưa thanh toán', 4, N'Gọi trước khi giao');
SET @OrderID = SCOPE_IDENTITY();
INSERT INTO OrderDetails (OrderID, ProductID, Quantity, UnitPrice) VALUES 
(@OrderID, 13, 1, 350000); 
UPDATE Products SET StockQuantity = StockQuantity - 1 WHERE ProductID = 13;

-- Đơn hàng 4: Đã hủy (Không trừ tồn kho)
INSERT INTO Orders (UserID, OrderDate, TotalAmount, OrderStatus, PaymentStatus, ShippingAddressID, Notes) 
VALUES (7, GETDATE() - 2, 85000, N'Đã hủy', N'Chưa thanh toán', 5, N'Hết hàng táo Envy nên khách hủy đơn');
SET @OrderID = SCOPE_IDENTITY();
INSERT INTO OrderDetails (OrderID, ProductID, Quantity, UnitPrice) VALUES 
(@OrderID, 9, 1, 35000), (@OrderID, 14, 1, 50000);

-- Đơn hàng 5: Chờ duyệt (Khách mới đặt hôm nay)
INSERT INTO Orders (UserID, OrderDate, TotalAmount, OrderStatus, PaymentStatus, ShippingAddressID, Notes) 
VALUES (8, GETDATE(), 535000, N'Chờ duyệt', N'Đã thanh toán', 6, N'Khách đã thanh toán qua VNPay');
SET @OrderID = SCOPE_IDENTITY();
INSERT INTO OrderDetails (OrderID, ProductID, Quantity, UnitPrice) VALUES 
(@OrderID, 12, 1, 180000), (@OrderID, 15, 1, 155000), (@OrderID, 16, 1, 200000);
GO


USE GreenMart;
GO

-- Cập nhật mật khẩu cho Admin (Email: admin@greenmart.vn) thành 'admin123' (đã băm SHA-256)
UPDATE Users 
SET PasswordHash = '240be518fabd2724ddb6f04eeb1da5967448d7e831c08c8fa822809f74c720a9' 
-- pass: admin123
--ham hash là sha-256
WHERE Email = 'admin@greenmart.vn';
GO


USE GreenMart;
GO

-- ==========================================
-- 1. XEM DỮ LIỆU TÀI KHOẢN & PHÂN QUYỀN
-- ==========================================
SELECT * FROM Roles;
SELECT * FROM Users;
SELECT * FROM Addresses;

-- ==========================================
-- 2. XEM DỮ LIỆU SẢN PHẨM & DANH MỤC
-- ==========================================
SELECT * FROM Categories;
SELECT * FROM Products;

-- ==========================================
-- 3. XEM DỮ LIỆU NHÀ CUNG CẤP & NHẬP KHO
-- ==========================================
SELECT * FROM Suppliers;
SELECT * FROM PurchaseReceipts;
SELECT * FROM PurchaseReceiptDetails;

-- ==========================================
-- 4. XEM DỮ LIỆU GIỎ HÀNG & ĐƠN HÀNG
-- ==========================================
SELECT * FROM CartItems;
SELECT * FROM Orders;
SELECT * FROM OrderDetails;