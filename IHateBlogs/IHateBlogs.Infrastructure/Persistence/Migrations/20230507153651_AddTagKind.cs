using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IHateBlogs.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddTagKind : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "kind",
                table: "tags",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "kind",
                table: "tags");
        }
    }
}
