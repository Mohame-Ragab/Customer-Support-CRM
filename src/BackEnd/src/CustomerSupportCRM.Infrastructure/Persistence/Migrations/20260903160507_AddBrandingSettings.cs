using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CustomerSupportCRM.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddBrandingSettings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BrandingSettings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PrimaryColor = table.Column<string>(type: "nvarchar(9)", maxLength: 9, nullable: false),
                    SecondaryColor = table.Column<string>(type: "nvarchar(9)", maxLength: 9, nullable: false),
                    LogoContentType = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    LogoBytes = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BrandingSettings", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "BrandingSettings",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "IsDeleted", "LogoBytes", "LogoContentType", "PrimaryColor", "SecondaryColor", "UpdatedAt", "UpdatedBy" },
                values: new object[] { new Guid("22222222-2222-2222-2222-222222222201"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, false, null, null, "#1565c0", "#546e7a", null, null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BrandingSettings");
        }
    }
}
