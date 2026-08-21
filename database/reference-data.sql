-- فقط ساختار داده‌های پایه؛ مقادیر توسط مدیر سیستم در SQL Server یا رابط مدیریت اطلاعات پایه وارد می‌شوند.
-- این فایل عمداً INSERT ندارد تا داده قابل تغییر داخل کد یا Seed هاردکد نشود.

CREATE TABLE ReferenceData (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Category NVARCHAR(100) NOT NULL,
    Code NVARCHAR(100) NOT NULL,
    Title NVARCHAR(200) NOT NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    SortOrder INT NOT NULL DEFAULT 0,
    Notes NVARCHAR(2000) NULL,
    CONSTRAINT UQ_ReferenceData_Category_Code UNIQUE (Category, Code)
);

CREATE INDEX IX_ReferenceData_Category_Active
ON ReferenceData(Category, IsActive, SortOrder);
