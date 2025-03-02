using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SupportTicketingSystem.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTeamFullName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "FullName",
                table: "AspNetUsers",
                type: "nvarchar(255)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Team",
                table: "AspNetUsers",
                type: "nvarchar(255)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);


            migrationBuilder.AddCheckConstraint(
                name: "CK_Team",
                table: "AspNetUsers",
                sql: "Team IN ('Development', 'Support', 'Sales')");


            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_Email",
                table: "AspNetUsers",
                column: "Email",
                unique: true,
                filter: "[Email] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.DropCheckConstraint(
                name: "CK_Team",
                table: "AspNetUsers");

            migrationBuilder.AlterColumn<string>(
                name: "FullName",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldNullable: false);

            migrationBuilder.AlterColumn<string>(
                name: "Team",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldNullable: false);

            migrationBuilder.AddCheckConstraint(
                name: "CK_Team",
                table: "AspNetUsers",
                sql: "Team IN ('Development', 'Support', 'Sales')");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_Email",
                table: "AspNetUsers");
        }
    }
}
