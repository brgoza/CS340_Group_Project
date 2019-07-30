using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CS340_Group_Project.Models
{
    public class RecipeIngredientsEditViewModel

    {
        public List<RecipeIngredient> Components { get; set; }
        public RecipeIngredient Component { get; set; }
        public List<Ingredient> Ingredients { get; set; }
        public List<QuantityUnit> QuantityUnits { get; set; }
    }
}
