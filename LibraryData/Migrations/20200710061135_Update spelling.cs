using Microsoft.EntityFrameworkCore.Migrations;

namespace LibraryData.Migrations
{
    public partial class Updatespelling : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CheckoutHistories_LibraryCard_LibraryCardId",
                table: "CheckoutHistories");

            migrationBuilder.DropForeignKey(
                name: "FK_Checkouts_LibraryCard_LibraryCardId",
                table: "Checkouts");

            migrationBuilder.DropForeignKey(
                name: "FK_Holds_LibraryCard_LibraryCardId",
                table: "Holds");

            migrationBuilder.DropForeignKey(
                name: "FK_Patrons_LibraryCard_LibraryCardId",
                table: "Patrons");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LibraryCard",
                table: "LibraryCard");

            migrationBuilder.DropColumn(
                name: "Adress",
                table: "Patrons");

            migrationBuilder.RenameTable(
                name: "LibraryCard",
                newName: "LibraryCards");

            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "Patrons",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_LibraryCards",
                table: "LibraryCards",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CheckoutHistories_LibraryCards_LibraryCardId",
                table: "CheckoutHistories",
                column: "LibraryCardId",
                principalTable: "LibraryCards",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Checkouts_LibraryCards_LibraryCardId",
                table: "Checkouts",
                column: "LibraryCardId",
                principalTable: "LibraryCards",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Holds_LibraryCards_LibraryCardId",
                table: "Holds",
                column: "LibraryCardId",
                principalTable: "LibraryCards",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Patrons_LibraryCards_LibraryCardId",
                table: "Patrons",
                column: "LibraryCardId",
                principalTable: "LibraryCards",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CheckoutHistories_LibraryCards_LibraryCardId",
                table: "CheckoutHistories");

            migrationBuilder.DropForeignKey(
                name: "FK_Checkouts_LibraryCards_LibraryCardId",
                table: "Checkouts");

            migrationBuilder.DropForeignKey(
                name: "FK_Holds_LibraryCards_LibraryCardId",
                table: "Holds");

            migrationBuilder.DropForeignKey(
                name: "FK_Patrons_LibraryCards_LibraryCardId",
                table: "Patrons");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LibraryCards",
                table: "LibraryCards");

            migrationBuilder.DropColumn(
                name: "Address",
                table: "Patrons");

            migrationBuilder.RenameTable(
                name: "LibraryCards",
                newName: "LibraryCard");

            migrationBuilder.AddColumn<string>(
                name: "Adress",
                table: "Patrons",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_LibraryCard",
                table: "LibraryCard",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CheckoutHistories_LibraryCard_LibraryCardId",
                table: "CheckoutHistories",
                column: "LibraryCardId",
                principalTable: "LibraryCard",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Checkouts_LibraryCard_LibraryCardId",
                table: "Checkouts",
                column: "LibraryCardId",
                principalTable: "LibraryCard",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Holds_LibraryCard_LibraryCardId",
                table: "Holds",
                column: "LibraryCardId",
                principalTable: "LibraryCard",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Patrons_LibraryCard_LibraryCardId",
                table: "Patrons",
                column: "LibraryCardId",
                principalTable: "LibraryCard",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
