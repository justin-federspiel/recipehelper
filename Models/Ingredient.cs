using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Models
{
    public class Ingredient
    {
        [JsonPropertyName("Id")]
        public long? Id { get; set; }
        [JsonPropertyName("Name")]
        public virtual string Name { get; set; } //celery, (white) flour, (olive) oil...
        [JsonPropertyName("RecipeIngredients")]
        public virtual ICollection<RecipeIngredient> RecipeIngredients { get; set; }
        [JsonPropertyName("Tags")]
        public virtual ICollection<IngredientTag> IngredientTags { get; set; }

        public static bool SameModelIdentification(Ingredient ingredient1, Ingredient ingredient2)
        {
            return ingredient1.Name == ingredient2.Name;
        }
    }
}
