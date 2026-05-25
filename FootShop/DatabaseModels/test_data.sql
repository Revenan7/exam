-- ==================== Справочники ====================

INSERT INTO Roles (RoleName) VALUES 
('Admin'), ('Manager'), ('Client');

INSERT INTO Addresses (AddressName) VALUES 
('г. Москва, ул. Тверская, д. 1'),
('г. Санкт-Петербург, Невский пр., д. 25'),
('г. Казань, ул. Баумана, д. 10');

INSERT INTO Categories (CategoryName) VALUES 
('Кроссовки'), ('Ботинки'), ('Сандалии'), ('Сапоги'), ('Аксессуары');

INSERT INTO Manufacturers (ManufacturerName) VALUES 
('Nike'), ('Adidas'), ('Puma'), ('Reebok'), ('New Balance');

INSERT INTO Providers (ProviderName) VALUES 
('ООО "Спорт-Опт"'), ('ИП Иванов'), ('ООО "Обувь-Плюс"'), ('ЗАО "Фут-Трейд"');

INSERT INTO ProductNames (ProductName) VALUES 
('Air Max 90'), ('Ultraboost 22'), ('RS-X'), ('Classic Leather'), ('574 Core');

INSERT INTO OrderStatuses (StatusName) VALUES 
('Новый'), ('В обработке'), ('Доставляется'), ('Доставлен'), ('Отменён');

-- ==================== Пользователи ====================

INSERT INTO Users (FullName, Login, Password, RoleID) VALUES 
('Администратор Системы', 'admin', 'admin123', 1),
('Менеджер Анна', 'anna_manager', 'anna2024', 2),
('Клиент Иван', 'ivan_client', 'ivan_pass', 3),
('Клиент Мария', 'maria_client', 'maria_pass', 3);

-- ==================== Товары ====================

INSERT INTO Products (Article, ProductNameID, Unit, Price, ProviderID, ManufacturerID, CategoryID, Discount, QuantityInStock, Description, PhotoFileName) VALUES 
('NM-AM90-BLK-42', 1, 'пара', 12990.00, 1, 1, 1, 10, 25, 'Классические кроссовки Air Max 90, чёрные', 'airmax90_black.jpg'),
('AD-UB22-WHT-43', 2, 'пара', 15490.00, 1, 2, 1, 15, 18, 'Беговые кроссовки Ultraboost с амортизацией', 'ultraboost_white.jpg'),
('PM-RSX-RED-41', 3, 'пара', 8990.00, 2, 3, 1, 5, 30, 'Яркие кроссовки RS-X в ретро-стиле', 'rsx_red.jpg'),
('RB-CL-BRN-44', 4, 'пара', 7490.00, 3, 4, 2, 0, 12, 'Кожаные ботинки Classic Leather', 'classic_brown.jpg'),
('NB-574-GRY-42', 5, 'пара', 9990.00, 4, 5, 1, 20, 8, 'Легендарная модель 574, серый цвет', 'nb574_grey.jpg');

-- ==================== Заказы ====================

INSERT INTO Orders (OrderDate, DeliveryDate, AddressID, UserID, ReceiptCode, StatusID) VALUES 
('2026-04-10', '2026-04-15', 1, 3, 10001, 4),  -- Доставлен
('2026-04-12', '2026-04-18', 2, 4, 10002, 3),  -- Доставляется
('2026-04-15', '2026-04-20', 3, 3, NULL, 2);   -- В обработке

-- ==================== Позиции заказов ====================

INSERT INTO OrderItems (OrderID, ProductID, Quantity) VALUES 
(1, 1, 1),  -- Заказ 1: 1 пара Air Max 90
(1, 5, 2),  -- Заказ 1: 2 пары NB 574
(2, 2, 1),  -- Заказ 2: 1 пара Ultraboost
(3, 3, 1),  -- Заказ 3: 1 пара RS-X
(3, 4, 1);  -- Заказ 3: 1 пара Classic Leather

GO