using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Json.Serialization;

namespace Models
{
    public class RecipeIngredient
    { //The four parts to this class' keys are RecipeId, IngredientId, Measurement, and Preparation
        [Required, JsonPropertyName("RecipeId")]
        public long? RecipeId { get; set; }
        [JsonPropertyName("Recipe")]
        public Recipe Recipe { get; set; }
        [Required, JsonPropertyName("IngredientId")]
        public long? IngredientId { get; set; }
        [JsonPropertyName("Ingredient")]
        public Ingredient Ingredient { get; set; }
        [JsonPropertyName("Quantity")]
        public virtual Fractionable Quantity { get; set; }
        [JsonPropertyName("Measurement")]
        public virtual string Measurement { get; set; } //cup, liter, pound...
        [JsonPropertyName("Preparation")]
        public virtual string Preparation { get; set; } //chopped, cubed, blended, grated...        

        public static bool SameModelIdentification(RecipeIngredient i, RecipeIngredient mightBeADuplicate)
        { //Same measurement, same preparation, and same Ingredient
            return i.Measurement == mightBeADuplicate.Measurement &&
                i.Preparation == mightBeADuplicate.Preparation &&
                (
                    (i.Ingredient != null && mightBeADuplicate.Ingredient != null && Ingredient.SameModelIdentification(i.Ingredient, mightBeADuplicate.Ingredient))
                    ||
                    (i.IngredientId.HasValue && mightBeADuplicate.IngredientId.HasValue && i.IngredientId == mightBeADuplicate.IngredientId)
                );
        }
    }
}
