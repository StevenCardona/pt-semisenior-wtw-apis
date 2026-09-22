using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Repository.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddTaskAdditionalInfo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AdditionalInfo",
                table: "Tasks",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddCheckConstraint(
                name: "CK_Tasks_AdditionalInfo_IsJson",
                table: "Tasks",
                sql: "[AdditionalInfo] IS NULL OR ISJSON([AdditionalInfo]) = 1");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_Tasks_AdditionalInfo_IsJson",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "AdditionalInfo",
                table: "Tasks");
        }
    }
}
