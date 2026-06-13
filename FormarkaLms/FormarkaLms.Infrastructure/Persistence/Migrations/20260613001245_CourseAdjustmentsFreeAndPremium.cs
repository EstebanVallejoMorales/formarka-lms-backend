using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FormarkaLms.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class CourseAdjustmentsFreeAndPremium : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsFree",
                table: "Courses",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsFree",
                table: "Courses");
        }
    }
}
