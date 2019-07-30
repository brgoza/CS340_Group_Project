using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CS340_Group_Project.Models
{
    public class RecipeIngredient
    {
        public int RecipeId { get; set; }
        public Recipe Recipe { get; set; }
        public int IngredientId { get; set; }
        public Ingredient Ingredient { get; set; }
        public string Quantity { get; set; }
        public int QuantityUnitId { get; set; }
        public QuantityUnit QuantityUnit { get; set; }
        
    }
}
