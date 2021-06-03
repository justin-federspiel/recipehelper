using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Models
{
    public class MealPlan
    {
        [JsonPropertyName("Id")]
        public long? Id { get; set; }
        [JsonPropertyName("Meals")]
        public virtual ICollection<Meal> Meals { get; set; }
    }
}
