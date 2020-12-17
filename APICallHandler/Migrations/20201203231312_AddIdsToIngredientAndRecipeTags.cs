using Microsoft.EntityFrameworkCore.Migrations;

namespace APICallHandler.Migrations
{
    public partial class AddIdsToIngredientAndRecipeTags : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_RecipeTags",
                table: "RecipeTags");

            migrationBuilder.DropPrimaryKey(
                name: "PK_IngredientTags",
                table: "IngredientTags");

            migrationBuilder.AddColumn<long>(
                name: "Id",
                table: "RecipeTags",
                type: "bigint",
                nullable: false,
                defaultValue: 0L)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<long>(
                name: "Id",
                table: "IngredientTags",
                type: "bigint",
                nullable: false,
                defaultValue: 0L)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RecipeTags",
                table: "RecipeTags",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_IngredientTags",
                table: "IngredientTags",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_RecipeTags_RecipeId",
                table: "RecipeTags",
                column: "RecipeId");

            migrationBuilder.CreateIndex(
                name: "IX_IngredientTags_IngredientId",
                table: "IngredientTags",
                column: "IngredientId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_RecipeTags",
                table: "RecipeTags");

            migrationBuilder.DropIndex(
                name: "IX_RecipeTags_RecipeId",
                table: "RecipeTags");

            migrationBuilder.DropPrimaryKey(
                name: "PK_IngredientTags",
                table: "IngredientTags");

            migrationBuilder.DropIndex(
                name: "IX_IngredientTags_IngredientId",
                table: "IngredientTags");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "RecipeTags");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "IngredientTags");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RecipeTags",
                table: "RecipeTags",
                columns: new[] { "RecipeId", "TagId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_IngredientTags",
                table: "IngredientTags",
                columns: new[] { "IngredientId", "TagId", "CookId" });
        }
    }
}
