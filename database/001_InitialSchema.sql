/* اسکیمای اولیه SQL Server سامانه پایش گوسفندان */
CREATE TABLE Sheep (
    Id INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Sheep PRIMARY KEY,
    Number NVARCHAR(50) NOT NULL,
    ImagePath NVARCHAR(500) NULL,
    Gender NVARCHAR(20) NOT NULL,
    BirthDate DATE NULL,
    InitialWeighingDate DATE NOT NULL,
    InitialWeightKg DECIMAL(8,2) NOT NULL,
    IsSick BIT NOT NULL CONSTRAINT DF_Sheep_IsSick DEFAULT 0,
    HealthStatus NVARCHAR(50) NOT NULL CONSTRAINT DF_Sheep_HealthStatus DEFAULT N'سالم',
    Notes NVARCHAR(2000) NULL,
    CONSTRAINT UQ_Sheep_Number UNIQUE (Number),
    CONSTRAINT CK_Sheep_InitialWeight CHECK (InitialWeightKg >= 0)
);

CREATE TABLE WeightSessions (
    Id INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_WeightSessions PRIMARY KEY,
    WeighingDate DATE NOT NULL,
    Notes NVARCHAR(1000) NULL
);

CREATE TABLE WeightRecords (
    Id INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_WeightRecords PRIMARY KEY,
    WeightSessionId INT NOT NULL,
    SheepId INT NOT NULL,
    WeightKg DECIMAL(8,2) NOT NULL,
    CONSTRAINT FK_WeightRecords_Session FOREIGN KEY (WeightSessionId) REFERENCES WeightSessions(Id),
    CONSTRAINT FK_WeightRecords_Sheep FOREIGN KEY (SheepId) REFERENCES Sheep(Id),
    CONSTRAINT UQ_WeightRecords_SessionSheep UNIQUE (WeightSessionId, SheepId),
    CONSTRAINT CK_WeightRecords_Weight CHECK (WeightKg >= 0)
);

CREATE TABLE HealthRecords (
    Id INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_HealthRecords PRIMARY KEY,
    SheepId INT NOT NULL,
    RecordDate DATE NOT NULL,
    Status NVARCHAR(50) NOT NULL,
    DiseaseName NVARCHAR(200) NULL,
    Severity NVARCHAR(30) NULL,
    Diagnosis NVARCHAR(1000) NULL,
    TreatmentNotes NVARCHAR(2000) NULL,
    CONSTRAINT FK_HealthRecords_Sheep FOREIGN KEY (SheepId) REFERENCES Sheep(Id)
);

CREATE TABLE Symptoms (
    Id INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Symptoms PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    CONSTRAINT UQ_Symptoms_Name UNIQUE (Name)
);

CREATE TABLE HealthRecordSymptoms (
    HealthRecordId INT NOT NULL,
    SymptomId INT NOT NULL,
    CONSTRAINT PK_HealthRecordSymptoms PRIMARY KEY (HealthRecordId, SymptomId),
    CONSTRAINT FK_HRS_HealthRecord FOREIGN KEY (HealthRecordId) REFERENCES HealthRecords(Id),
    CONSTRAINT FK_HRS_Symptom FOREIGN KEY (SymptomId) REFERENCES Symptoms(Id)
);

CREATE TABLE FeedTypes (
    Id INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_FeedTypes PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    Unit NVARCHAR(20) NOT NULL CONSTRAINT DF_FeedTypes_Unit DEFAULT N'کیلوگرم',
    CurrentPricePerKg DECIMAL(12,2) NULL,
    CONSTRAINT UQ_FeedTypes_Name UNIQUE (Name)
);

INSERT INTO FeedTypes (Name) VALUES
(N'کاه'), (N'یونجه'), (N'کنسانتره'), (N'جو'), (N'ذرت'), (N'سبوس'), (N'مکمل معدنی'), (N'نمک');
