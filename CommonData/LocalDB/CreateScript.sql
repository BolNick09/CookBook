--�������� ��


--CREATE DATABASE CookBookDB

--Use CookBookDB
--GO

-- ������� "��������"
CREATE TABLE [Products] 
(
    [Id] INT PRIMARY KEY IDENTITY(1,1),
    [Name] NVARCHAR(100) NOT NULL,
    [Proteins] DECIMAL(10, 2) NOT NULL,  -- ����� �� 100�
    [Fats] DECIMAL(10, 2) NOT NULL,      -- ���� �� 100�
    [Carbohydrates] DECIMAL(10, 2) NOT NULL, -- �������� �� 100�
    [Calories] DECIMAL(10, 2) NOT NULL,  -- ������� �� 100�
    [Image] VARBINARY(MAX) NULL          -- �������� (�������������)
);


-- ������� "����"
CREATE TABLE [Tags] 
(
    [Id] INT PRIMARY KEY IDENTITY(1,1),
    [Name] NVARCHAR(50) NOT NULL
);


-- ������� "�����"
CREATE TABLE [Dishes] 
(
    [Id] INT PRIMARY KEY IDENTITY(1,1),
    [Name] NVARCHAR(100) NOT NULL,
    [Comment] NVARCHAR(MAX) NULL,        -- ������� �����������
    [IsFavorite] BIT DEFAULT 0,         -- ��������� (��/���)
    [Recipe] NVARCHAR(MAX) NOT NULL,      -- ������ (������� �����)
	[Image] VARBINARY(MAX) NULL			-- �������� (�������������)
);

--ALTER TABLE [Dishes] 
--ADD [Image] VARBINARY(MAX) NULL; 


-- ������������� ������� "�������� � ������" (������ �� ������)
CREATE TABLE [DishProducts] 
(
    [DishId] INT NOT NULL,
    [ProductId] INT NOT NULL,
    [Weight] DECIMAL(10, 2) NOT NULL,   -- ����� �������� � �������
    PRIMARY KEY ([DishId], [ProductId]),
    FOREIGN KEY ([DishId]) REFERENCES [Dishes]([Id]) ON DELETE CASCADE,
    FOREIGN KEY ([ProductId]) REFERENCES [Products]([Id])
);


-- ������������� ������� "���� ����" (������ �� ������)
CREATE TABLE [DishTags] 
(
    [DishId] INT NOT NULL,
    [TagId] INT NOT NULL,
    PRIMARY KEY ([DishId], [TagId]),
    FOREIGN KEY ([DishId]) REFERENCES [Dishes]([Id]) ON DELETE CASCADE,
    FOREIGN KEY ([TagId]) REFERENCES [Tags]([Id])
);

--������� ������ � ����� � ��������

USE master;  -- ������������� �� ��������� ��
GO

-- ��������� ��� ���������� � �� � ��������� � � ����� OFFLINE
ALTER DATABASE CookBookDB SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
GO

-- ����������� �� (����� ��������� �� �����, �� SQL Server �� ������ �� ����� �����������)
EXEC sp_detach_db 'CookBookDB', 'true';
GO

--���� CREATE DATABASE �� �����������, ���������� �� ������� � ����� C:\Program Files\Microsoft SQL Server\MSSQL16.MSSQLSERVER\MSSQL\DATA, ��� ��� � ��� ���������� ���� ������� ��
--� ������� �� � ������� �������� ������ EFC

CREATE DATABASE [CookBookDB] ON 
(
    FILENAME = 'C:\Users\������\source\repos\CookBook\CommonData\LocalDB\CookBookDB.mdf',
    FILENAME = 'C:\Users\������\source\repos\CookBook\CommonData\LocalDB\CookBookDB_log.ldf'
) FOR ATTACH;
GO

--����������� �� �������, ���� �� ��������, ������ ������������ ��

 CREATE DATABASE [CookBookDB] ON 
(
    FILENAME = 'C:\Program Files\Microsoft SQL Server\MSSQL16.MSSQLSERVER\MSSQL\DATA\CookBookDB.mdf',
    FILENAME = 'C:\Program Files\Microsoft SQL Server\MSSQL16.MSSQLSERVER\MSSQL\DATA\CookBookDB_log.ldf'
) FOR ATTACH;
GO

