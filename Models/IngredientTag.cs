using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using System.Text.Json.Serialization;

namespace Models
{
    public class IngredientTag
    {
        [JsonPropertyName("Id")]
        public virtual long? Id { get; set; }
        [JsonPropertyName("IngredientId")]
        public virtual long? IngredientId { get; set; }
        [JsonPropertyName("TagId")]
        public virtual long? TagId { get; set; }
        [JsonPropertyName("CookId")]
        public virtual long? CookId { get; set; }
        [JsonPropertyName("Ingredient")]
        public virtual Ingredient Ingredient { get; set; }
        [JsonPropertyName("Tag")]
        public virtual Tag Tag { get; set; }
        [JsonPropertyName("Cook")]
        public virtual Cook Cook { get; set; }        
    }
}
