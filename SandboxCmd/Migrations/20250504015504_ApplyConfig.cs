using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SandboxCmd.Migrations
{
    /// <inheritdoc />
    public partial class ApplyConfig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "TableName",
                table: "SerilogTableMarkers",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "SchemaName",
                table: "SerilogTableMarkers",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_SerilogTableMarkers_SchemaName_TableName",
                table: "SerilogTableMarkers",
                columns: new[] { "SchemaName", "TableName" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_SerilogTableMarkers_SchemaName_TableName",
                table: "SerilogTableMarkers");

            migrationBuilder.AlterColumn<string>(
                name: "TableName",
                table: "SerilogTableMarkers",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255);

            migrationBuilder.AlterColumn<string>(
                name: "SchemaName",
                table: "SerilogTableMarkers",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128);
        }
    }
}
