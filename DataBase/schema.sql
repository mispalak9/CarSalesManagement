USE CarSalesManagementDB;
GO

SET NOCOUNT ON;
GO

/* =========================================================
   Database Schema: Car Sales Management System
   ========================================================= */

/* -----------------------------
   Master Tables
   ----------------------------- */

-- Brands master
CREATE TABLE dbo.Brands (
    BrandID INT PRIMARY KEY IDENTITY(1,1),
    BrandName NVARCHAR(50) NOT NULL UNIQUE,
    BrandCode NVARCHAR(20) NOT NULL UNIQUE,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedBy INT NULL,
    CreatedOn DATETIME NOT NULL DEFAULT GETDATE(),
    LastUpdatedBy INT NULL,
    LastUpdatedOn DATETIME NULL
);
GO

-- Car classification master
CREATE TABLE dbo.CarClasses (
    ClassID INT PRIMARY KEY IDENTITY(1,1),
    ClassName NVARCHAR(20) NOT NULL UNIQUE,
    ClassCode NVARCHAR(10) NOT NULL UNIQUE,
    DisplayOrder INT NOT NULL DEFAULT 0,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedBy INT NULL,
    CreatedOn DATETIME NOT NULL DEFAULT GETDATE(),
    LastUpdatedBy INT NULL,
    LastUpdatedOn DATETIME NULL
);
GO

-- Sales team master
CREATE TABLE dbo.Salesmen (
    SalesmanID INT PRIMARY KEY IDENTITY(1,1),
    SalesmanCode NVARCHAR(20) NOT NULL UNIQUE,
    SalesmanName NVARCHAR(100) NOT NULL,
    Email NVARCHAR(100) NULL UNIQUE,
    Phone NVARCHAR(20) NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedBy INT NULL,
    CreatedOn DATETIME NOT NULL DEFAULT GETDATE(),
    LastUpdatedBy INT NULL,
    LastUpdatedOn DATETIME NULL
);
GO

/* -----------------------------
   Inventory
   ----------------------------- */

-- Car models
CREATE TABLE dbo.CarModels (
    ModelID INT PRIMARY KEY IDENTITY(1,1),
    BrandID INT NOT NULL,
    ClassID INT NOT NULL,
    ModelName NVARCHAR(100) NOT NULL,
    ModelCode NVARCHAR(10) NOT NULL UNIQUE,
    Description NVARCHAR(MAX) NOT NULL,
    Features NVARCHAR(MAX) NOT NULL,
    Price DECIMAL(18,2) NOT NULL,
    DateOfManufacturing DATETIME NOT NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    SortOrder INT NOT NULL DEFAULT 0,
    CreatedBy INT NULL,
    CreatedOn DATETIME NOT NULL DEFAULT GETDATE(),
    LastUpdatedBy INT NULL,
    LastUpdatedOn DATETIME NULL,

    CONSTRAINT FK_CarModels_Brands FOREIGN KEY (BrandID)
        REFERENCES dbo.Brands(BrandID),
    CONSTRAINT FK_CarModels_CarClasses FOREIGN KEY (ClassID)
        REFERENCES dbo.CarClasses(ClassID),
    CONSTRAINT UQ_CarModels UNIQUE (BrandID, ClassID, ModelName),
    CONSTRAINT CHK_CarModels_Price CHECK (Price > 0)
);
GO

-- Car model images
CREATE TABLE dbo.CarModelImages (
    ImageID INT PRIMARY KEY IDENTITY(1,1),
    ModelID INT NOT NULL,
    ImagePath NVARCHAR(500) NOT NULL,
    ImageName NVARCHAR(255) NOT NULL,
    ImageSize BIGINT NOT NULL,
    IsDefault BIT NOT NULL DEFAULT 0,
    SortOrder INT NOT NULL DEFAULT 0,
    CreatedBy INT NULL,
    CreatedOn DATETIME NOT NULL DEFAULT GETDATE(),

    CONSTRAINT FK_CarModelImages_CarModels FOREIGN KEY (ModelID)
        REFERENCES dbo.CarModels(ModelID) ON DELETE CASCADE
);
GO

/* -----------------------------
   Sales & Commission
   ----------------------------- */

-- Commission rules by brand and class
CREATE TABLE dbo.CommissionRules (
    RuleID INT PRIMARY KEY IDENTITY(1,1),
    BrandID INT NOT NULL,
    ClassID INT NOT NULL,
    FixedCommission DECIMAL(18,2) NOT NULL,
    MinPriceForFixedCommission DECIMAL(18,2) NOT NULL,
    PercentageCommission DECIMAL(5,2) NOT NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedOn DATETIME NOT NULL DEFAULT GETDATE(),

    CONSTRAINT FK_CommissionRules_Brands FOREIGN KEY (BrandID)
        REFERENCES dbo.Brands(BrandID),
    CONSTRAINT FK_CommissionRules_Classes FOREIGN KEY (ClassID)
        REFERENCES dbo.CarClasses(ClassID),
    CONSTRAINT UQ_CommissionRules UNIQUE (BrandID, ClassID)
);
GO

-- Sales transactions
CREATE TABLE dbo.Sales (
    SaleID INT PRIMARY KEY IDENTITY(1,1),
    SaleNumber NVARCHAR(50) NOT NULL UNIQUE,
    SalesmanID INT NOT NULL,
    ModelID INT NOT NULL,
    SaleDate DATETIME NOT NULL DEFAULT GETDATE(),
    Quantity INT NOT NULL,
    UnitPrice DECIMAL(18,2) NOT NULL,
    TotalAmount DECIMAL(18,2) NOT NULL,
    SaleMonth INT NOT NULL,
    SaleYear INT NOT NULL,
    Status NVARCHAR(20) NOT NULL DEFAULT 'Completed',

    CONSTRAINT FK_Sales_Salesmen FOREIGN KEY (SalesmanID)
        REFERENCES dbo.Salesmen(SalesmanID),
    CONSTRAINT FK_Sales_Models FOREIGN KEY (ModelID)
        REFERENCES dbo.CarModels(ModelID)
);
GO

/* -----------------------------
   Security & Navigation
   ----------------------------- */

-- Roles
CREATE TABLE dbo.Roles (
    RoleID INT PRIMARY KEY IDENTITY(1,1),
    RoleName NVARCHAR(50) NOT NULL UNIQUE,
    RoleCode NVARCHAR(20) NOT NULL UNIQUE,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedOn DATETIME NOT NULL DEFAULT GETDATE()
);
GO

-- Users
CREATE TABLE dbo.Users (
    UserID INT PRIMARY KEY IDENTITY(1,1),
    Username NVARCHAR(50) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(255) NOT NULL,
    Email NVARCHAR(100) NOT NULL UNIQUE,
    FullName NVARCHAR(100) NOT NULL,
    SalesmanID INT NULL,
    IsActive BIT NOT NULL DEFAULT 1,

    CONSTRAINT FK_Users_Salesmen FOREIGN KEY (SalesmanID)
        REFERENCES dbo.Salesmen(SalesmanID)
);
GO
