using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Models
{
    public class Meal
    {
        [JsonPropertyName("Id")]
        public long? Id { get; set; }
        [JsonPropertyName("Recipes")]
        public virtual ICollection<Recipe> Recipes { get; set; }
    }
}
