using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using System.Text.Json.Serialization;

namespace Models
{
    public class RecipeTag
    {
        [JsonPropertyName("Id")]
        public virtual long? Id { get; set; }
        [JsonPropertyName("RecipeId")]
        public virtual long? RecipeId { get; set; }
        [JsonPropertyName("TagId")]
        public virtual long? TagId { get; set; }
        [JsonPropertyName("Recipe")]
        public virtual Recipe Recipe { get; set; }
        [JsonPropertyName("Tag")]
        public virtual Tag Tag { get; set; }
        public static bool SameModelIdentification(RecipeTag i, RecipeTag mightBeADuplicate)
        { //Same measurement, same preparation, and same Ingredient
            if (i == null || mightBeADuplicate == null) return false;
            return (i.TagId != null && mightBeADuplicate.TagId != null && i.TagId == mightBeADuplicate.TagId) || (i.Tag != null && mightBeADuplicate.Tag != null && Tag.SameModelIdentification(i.Tag, mightBeADuplicate.Tag));
        }
    }
}
