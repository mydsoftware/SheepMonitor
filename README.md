# سامانه پایش گوسفندان (SheepMonitor)

نرم‌افزار دسکتاپ مدیریت و پایش دامداری گوسفند — وزن‌گیری، سلامت، درمان، جیره و مصرف خوراک.

## معماری

| لایه | پروژه | نقش |
|------|--------|------|
| UI | `SheepMonitor.App` | WPF، فارسی، RTL، تقویم شمسی |
| Core | `SheepMonitor.Core` | مدل‌ها، قرارداد سرویس‌ها، محاسبات |
| Data | `SheepMonitor.Data` | EF Core + SQL Server |
| API | `SheepMonitor.Api` | endpointهای گزارش مصرف |
| Tests | `SheepMonitor.Tests` | Unit / Integration |

- Dependency Injection
- لایه‌ای (بدون Hard-code تنظیمات مدیریتی)
- تمام داده‌های قابل تنظیم از SQL Server / ReferenceData
- GitHub Actions CI

## قابلیت‌های فعلی

1. مدیریت گوسفندان (ثبت / ویرایش / حذف، شماره یکتا، جنسیت، تصویر، تاریخ تولد)
2. وزن‌گیری تکی و گروهی + میانگین وزن
3. **گزارش رشد**: وزن اولیه، آخرین وزن، حداقل/حداکثر، افزایش کل، میانگین روزانه، وضعیت رشد، روند وزن‌گیری
4. سلامت و بیماری: ثبت سابقه با کد بیماری/علائم/شدت از ReferenceData، ثبت بهبودی، به‌روزرسانی خودکار وضعیت دام
5. درمان: دارو، دوز، واحد، دفعات، نتیجه — وابسته به سابقه بیماری
6. برنامه غذایی، تخصیص جیره، موتور جیره پویا و قوانین جیره از دیتابیس
7. گزارش مصرف خوراک، ضایعات، انحراف و آستانه‌های قابل تنظیم
8. داشبورد مدیریتی

## پیش‌نیاز

- .NET 10 SDK
- SQL Server (LocalDB / Express / کامل)
- Windows (WPF)

## راه‌اندازی

1. Connection string در `src/SheepMonitor.App/appsettings.json`
2. اسکریپت‌های `database/` و Migrationهای `SheepMonitor.Data` را روی SQL Server اجرا کنید
3. اطلاعات پایه (بیماری، علائم، دارو، جیره و …) را از صفحه «اطلاعات پایه» یا مستقیم در جدول `ReferenceData` وارد کنید
4. اجرا:

```bash
dotnet restore SheepMonitor.sln
dotnet build SheepMonitor.sln -c Release
dotnet run --project src/SheepMonitor.App
```

## تست

```bash
dotnet test tests/SheepMonitor.Tests/SheepMonitor.Tests.csproj -c Release
```

## CI

سه workflow روی شاخه `agent/**` و PR به `main`:

- `ci.yml` — build + test
- `dotnet-tests.yml` — تست خودکار
- `agent-ci.yml` — validate برای شاخه‌های agent

## قوانین توسعه

- کامنت‌ها، Commit Message، README و UI به **فارسی**
- هیچ درصد، آستانه، مقدار جیره یا تنظیم مدیریتی Hard-code نشود
- معماری موجود ادامه یابد؛ لایه موازی نسازید

## شاخه فعال

`agent/database-model` — PR #1 برای ادغام در `main`
