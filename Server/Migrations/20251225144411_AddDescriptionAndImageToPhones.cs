using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Server.Migrations
{
    /// <inheritdoc />
    public partial class AddDescriptionAndImageToPhones : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "phone",
                type: "text",
                nullable: false,
                defaultValue: "Описание отсутствует");

            migrationBuilder.AddColumn<string>(
                name: "Image",
                table: "phone",
                type: "text",
                nullable: false,
                defaultValue: "C:\\Users\\Denis\\Documents\\projects C#\\EntityHomework4567\\Server\\noimage.jpg");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "phone");

            migrationBuilder.DropColumn(
                name: "Image",
                table: "phone");
        }
    }
}
