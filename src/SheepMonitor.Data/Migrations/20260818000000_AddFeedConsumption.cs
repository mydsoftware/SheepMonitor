using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SheepMonitor.Data.Migrations;

public partial class AddFeedConsumption : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "FeedConsumptionRecords",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                SheepId = table.Column<int>(type: "int", nullable: false),
                Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                Notes = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_FeedConsumptionRecords", x => x.Id);
                table.ForeignKey("FK_FeedConsumptionRecords_Sheep_SheepId", x => x.SheepId, "Sheep", "Id", onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "FeedConsumptionItems",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                FeedConsumptionRecordId = table.Column<int>(type: "int", nullable: false),
                FeedCode = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                PlannedKg = table.Column<decimal>(type: "decimal(10,3)", precision: 10, scale: 3, nullable: false),
                ActualKg = table.Column<decimal>(type: "decimal(10,3)", precision: 10, scale: 3, nullable: false),
                WasteKg = table.Column<decimal>(type: "decimal(10,3)", precision: 10, scale: 3, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_FeedConsumptionItems", x => x.Id);
                table.ForeignKey("FK_FeedConsumptionItems_FeedConsumptionRecords_FeedConsumptionRecordId", x => x.FeedConsumptionRecordId, "FeedConsumptionRecords", "Id", onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex("IX_FeedConsumptionRecords_Date_SheepId", "FeedConsumptionRecords", new[] { "Date", "SheepId" });
        migrationBuilder.CreateIndex("IX_FeedConsumptionItems_FeedConsumptionRecordId_FeedCode", "FeedConsumptionItems", new[] { "FeedConsumptionRecordId", "FeedCode" });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable("FeedConsumptionItems");
        migrationBuilder.DropTable("FeedConsumptionRecords");
    }
}
