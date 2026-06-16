using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Encyclopaedia.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddFeaturedImage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FeaturedImage",
                table: "Articles",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ArticleTranslations_LanguageId",
                table: "ArticleTranslations",
                column: "LanguageId");

            migrationBuilder.AddForeignKey(
                name: "FK_ArticleTranslations_Languages_LanguageId",
                table: "ArticleTranslations",
                column: "LanguageId",
                principalTable: "Languages",
                principalColumn: "LanguageId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ArticleTranslations_Languages_LanguageId",
                table: "ArticleTranslations");

            migrationBuilder.DropIndex(
                name: "IX_ArticleTranslations_LanguageId",
                table: "ArticleTranslations");

            migrationBuilder.DropColumn(
                name: "FeaturedImage",
                table: "Articles");
        }
    }
}
