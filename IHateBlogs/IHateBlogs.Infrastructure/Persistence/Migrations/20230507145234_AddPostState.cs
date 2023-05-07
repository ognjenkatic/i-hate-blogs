using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IHateBlogs.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddPostState : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "state",
                table: "posts",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "state",
                table: "posts");
        }
    }
}
