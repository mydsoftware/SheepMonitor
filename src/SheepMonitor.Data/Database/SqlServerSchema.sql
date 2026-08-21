-- طرح اولیه SQL Server برای پایش گوسفندان
-- تمام داده‌های قابل تنظیم جیره از همین جداول خوانده می‌شوند.

IF OBJECT_ID(N'dbo.RationCalculationRules', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.RationCalculationRules
    (
        Id INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_RationCalculationRules PRIMARY KEY,
        Name NVARCHAR(150) NOT NULL,
        Code NVARCHAR(100) NOT NULL,
        FeedCode NVARCHAR(100) NOT NULL,
        TargetGroupCode NVARCHAR(100) NULL,
        BasePercent DECIMAL(8,3) NOT NULL DEFAULT 0,
        WeightCoefficient DECIMAL(10,5) NOT NULL DEFAULT 0,
        MinimumKg DECIMAL(8,3) NOT NULL DEFAULT 0,
        MaximumKg DECIMAL(8,3) NOT NULL DEFAULT 1000,
        ProteinPercent DECIMAL(8,3) NOT NULL DEFAULT 0,
        EnergyPerKg DECIMAL(10,3) NOT NULL DEFAULT 0,
        DryMatterPercent DECIMAL(8,3) NOT NULL DEFAULT 100,
        Formula NVARCHAR(1000) NULL,
        IsActive BIT NOT NULL DEFAULT 1,
        Notes NVARCHAR(2000) NULL
    );
    CREATE INDEX IX_RationCalculationRules_Code_IsActive ON dbo.RationCalculationRules(Code, IsActive);
END;

IF OBJECT_ID(N'dbo.RationMealRules', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.RationMealRules
    (
        Id INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_RationMealRules PRIMARY KEY,
        RationCalculationRuleId INT NOT NULL,
        MealCode NVARCHAR(100) NOT NULL,
        PercentOfDailyAmount DECIMAL(8,3) NOT NULL DEFAULT 0,
        Notes NVARCHAR(2000) NULL,
        CONSTRAINT FK_RationMealRules_RationCalculationRules FOREIGN KEY (RationCalculationRuleId)
            REFERENCES dbo.RationCalculationRules(Id) ON DELETE CASCADE
    );
END;

IF OBJECT_ID(N'dbo.RationPeriods', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.RationPeriods
    (
        Id INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_RationPeriods PRIMARY KEY,
        Name NVARCHAR(150) NOT NULL,
        StartDate DATETIME2 NOT NULL,
        DurationDays INT NOT NULL,
        IsActive BIT NOT NULL DEFAULT 1,
        Notes NVARCHAR(2000) NULL
    );
END;

-- داده‌های پایه را عمداً داخل کد C# قرار نمی‌دهیم؛ این بخش باید توسط کاربر یا اسکریپت مدیریت داده تکمیل شود.
