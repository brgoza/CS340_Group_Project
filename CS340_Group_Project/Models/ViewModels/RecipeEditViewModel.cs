using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CS340_Group_Project.Models
{
    public class RecipeEditViewModel
    {
        public Recipe Recipe { get; set; }
        public int RecipeId { get; set; }
        public RecipeIngredient NewRecipeIngredient { get; set; }

        public List<Ingredient> IngredientsList { get; set; }
        public List<QuantityUnit> QuantityUnitsList { get; set; }
    }
}
