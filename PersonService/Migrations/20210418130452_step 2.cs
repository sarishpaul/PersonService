using Microsoft.EntityFrameworkCore.Migrations;

namespace PersonService.Migrations
{
    public partial class step2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PersonId",
                table: "Events",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Events_PersonId",
                table: "Events",
                column: "PersonId");

            migrationBuilder.AddForeignKey(
                name: "FK_Events_Persons_PersonId",
                table: "Events",
                column: "PersonId",
                principalTable: "Persons",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Events_Persons_PersonId",
                table: "Events");

            migrationBuilder.DropIndex(
                name: "IX_Events_PersonId",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "PersonId",
                table: "Events");
        }
    }
}
