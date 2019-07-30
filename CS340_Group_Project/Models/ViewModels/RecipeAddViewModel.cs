using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CS340_Group_Project.Models
{
    public class RecipeAddViewModel
    {
        public Recipe Recipe { get; set; }
        public List<QuantityUnit> QuantityUnits { get; set; }
        public List<Ingredient> Ingredients { get; set; }
        public List<RecipeIngredient> Components { get; set; }
        public List<RecipeCollection> RecipeCollections { get; set; }
    }
}
