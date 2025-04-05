--Создание БД


--CREATE DATABASE CookBookDB

--Use CookBookDB
--GO

-- Таблица "Продукты"
CREATE TABLE [Products] 
(
    [Id] INT PRIMARY KEY IDENTITY(1,1),
    [Name] NVARCHAR(100) NOT NULL,
    [Proteins] DECIMAL(10, 2) NOT NULL,  -- белки на 100г
    [Fats] DECIMAL(10, 2) NOT NULL,      -- жиры на 100г
    [Carbohydrates] DECIMAL(10, 2) NOT NULL, -- углеводы на 100г
    [Calories] DECIMAL(10, 2) NOT NULL,  -- калории на 100г
    [Image] VARBINARY(MAX) NULL          -- картинка (необязательно)
);


-- Таблица "Теги"
CREATE TABLE [Tags] 
(
    [Id] INT PRIMARY KEY IDENTITY(1,1),
    [Name] NVARCHAR(50) NOT NULL
);


-- Таблица "Блюда"
CREATE TABLE [Dishes] 
(
    [Id] INT PRIMARY KEY IDENTITY(1,1),
    [Name] NVARCHAR(100) NOT NULL,
    [Comment] NVARCHAR(MAX) NULL,        -- большой комментарий
    [IsFavorite] BIT DEFAULT 0,         -- избранное (да/нет)
    [Recipe] NVARCHAR(MAX) NOT NULL,      -- рецепт (большой текст)
	[Image] VARBINARY(MAX) NULL			-- картинка (необязательно)
);

--ALTER TABLE [Dishes] 
--ADD [Image] VARBINARY(MAX) NULL; 


-- Ассоциативная таблица "Продукты в блюдах" (многие ко многим)
CREATE TABLE [DishProducts] 
(
    [DishId] INT NOT NULL,
    [ProductId] INT NOT NULL,
    [Weight] DECIMAL(10, 2) NOT NULL,   -- масса продукта в граммах
    PRIMARY KEY ([DishId], [ProductId]),
    FOREIGN KEY ([DishId]) REFERENCES [Dishes]([Id]) ON DELETE CASCADE,
    FOREIGN KEY ([ProductId]) REFERENCES [Products]([Id])
);


-- Ассоциативная таблица "Теги блюд" (многие ко многим)
CREATE TABLE [DishTags] 
(
    [DishId] INT NOT NULL,
    [TagId] INT NOT NULL,
    PRIMARY KEY ([DishId], [TagId]),
    FOREIGN KEY ([DishId]) REFERENCES [Dishes]([Id]) ON DELETE CASCADE,
    FOREIGN KEY ([TagId]) REFERENCES [Tags]([Id])
);

--Перенос файлов в папку с проектом

USE master;  -- Переключаемся на системную БД
GO

-- Отключаем все соединения с БД и переводим её в режим OFFLINE
ALTER DATABASE CookBookDB SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
GO

-- Отсоединяем БД (файлы останутся на диске, но SQL Server их больше не будет блокировать)
EXEC sp_detach_db 'CookBookDB', 'true';
GO

--Если CREATE DATABASE не выполняется, подключить БД обратно в папку C:\Program Files\Microsoft SQL Server\MSSQL16.MSSQLSERVER\MSSQL\DATA, или где у вас изначально была создана БД
--С файлами БД в проекте работать ечерез EFC

CREATE DATABASE [CookBookDB] ON 
(
    FILENAME = 'C:\Users\Никита\source\repos\CookBook\CommonData\LocalDB\CookBookDB.mdf',
    FILENAME = 'C:\Users\Никита\source\repos\CookBook\CommonData\LocalDB\CookBookDB_log.ldf'
) FOR ATTACH;
GO

--Подключение БД обратно, если не работает, просто пересоздайте БД

 CREATE DATABASE [CookBookDB] ON 
(
    FILENAME = 'C:\Program Files\Microsoft SQL Server\MSSQL16.MSSQLSERVER\MSSQL\DATA\CookBookDB.mdf',
    FILENAME = 'C:\Program Files\Microsoft SQL Server\MSSQL16.MSSQLSERVER\MSSQL\DATA\CookBookDB_log.ldf'
) FOR ATTACH;
GO

