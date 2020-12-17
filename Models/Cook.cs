using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Models
{
    public class Cook
    {
        [JsonPropertyName("Id")]
        public long? Id { get; set; }
        [JsonPropertyName("Name")]
        public string Name { get; set; }
        [JsonPropertyName("Recipes")]
        public virtual ICollection<Recipe> Recipes { get; set; }
        [JsonPropertyName("IngredientTags")]
        public virtual ICollection<IngredientTag> IngredientTags { get; set; }
    }
}
