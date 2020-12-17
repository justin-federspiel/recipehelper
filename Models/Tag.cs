using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Models
{
    public class Tag
    {
        [JsonPropertyName("Id")]
        public long? Id { get; set; }
        [JsonPropertyName("Text")]
        public virtual string Text { get; set; }
        [JsonPropertyName("RecipeTags")]
        public virtual ICollection<RecipeTag> RecipeTags { get; set; }
        [JsonPropertyName("IngredientTags")]
        public virtual ICollection<IngredientTag> IngredientTags { get; set; }

        public static bool SameModelIdentification(Tag tag1, Tag tag2)
        {
            return tag1.Text == tag2.Text;
        }
    }
}
