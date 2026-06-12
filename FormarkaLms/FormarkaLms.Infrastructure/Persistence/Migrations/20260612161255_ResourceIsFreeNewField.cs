using System;
using System.Collections.Generic;
using FormarkaLms.Domain.Entities;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace FormarkaLms.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ResourceIsFreeNewField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Features",
                table: "Courses");

            migrationBuilder.DropColumn(
                name: "LearningObjectives",
                table: "Courses");

            migrationBuilder.AddColumn<bool>(
                name: "IsFree",
                table: "Resources",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "CourseFeatures",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CourseId = table.Column<int>(type: "integer", nullable: false),
                    Icon = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Text = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourseFeatures", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CourseFeatures_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CourseObjectives",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CourseId = table.Column<int>(type: "integer", nullable: false),
                    Text = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourseObjectives", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CourseObjectives_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CourseFeatures_CourseId",
                table: "CourseFeatures",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_CourseObjectives_CourseId",
                table: "CourseObjectives",
                column: "CourseId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CourseFeatures");

            migrationBuilder.DropTable(
                name: "CourseObjectives");

            migrationBuilder.DropColumn(
                name: "IsFree",
                table: "Resources");

            migrationBuilder.AddColumn<List<CourseFeature>>(
                name: "Features",
                table: "Courses",
                type: "jsonb",
                nullable: false);

            migrationBuilder.AddColumn<List<string>>(
                name: "LearningObjectives",
                table: "Courses",
                type: "jsonb",
                nullable: false);
        }
    }
}
