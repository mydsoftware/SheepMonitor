-- داده‌های نمونه اولیه برای تست محیط توسعه
INSERT INTO FeedPlans (Name, TargetGroup, Notes) VALUES
(N'جیره پایه رشد', N'بره‌های در حال رشد', N'جیره نمونه و قابل تنظیم.'),
(N'جیره نگهداری', N'دام بالغ', N'جیره نمونه و قابل تنظیم.'),
(N'جیره پرواری', N'دام پرواری', N'جیره نمونه و قابل تنظیم.');

INSERT INTO Sheep (Number, Gender, InitialWeighingDate, InitialWeightKg, IsSick, HealthStatus, Notes)
VALUES
(N'نمونه-001', N'نر', SYSDATETIME(), 35.00, 0, N'سالم', N'دام نمونه برای تست نرم‌افزار');
