using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Models
{
    public class Recipe
    { //Collections: N:N -- Ingredient (via RecipeIngredient), Tag, Recipe (via Variation)

        [JsonPropertyName("Id")]
        public long? Id { get; set; }
        [JsonPropertyName("CookId")]
        public long CookId { get; set; }
        [JsonPropertyName("Name")]
        public string Name { get; set; }
        [JsonPropertyName("Instructions")]
        public string Instructions { get; set; }
        [JsonPropertyName("MainGraphicLink")]
        public string MainGraphicLink { get; set; }
        [JsonPropertyName("Cook")]
        public virtual Cook Cook { get; set; }
        [JsonPropertyName("Ingredients")]
        public virtual ICollection<RecipeIngredient> RecipeIngredients { get; set; }
        [JsonPropertyName("Tags")]
        public virtual ICollection<RecipeTag> RecipeTags { get; set; }
    }
}
