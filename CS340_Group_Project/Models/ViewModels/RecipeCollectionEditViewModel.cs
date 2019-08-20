using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CS340_Group_Project.Models
{
    public class RecipeCollectionEditViewModel
    {
        public RecipeCollection RecipeCollection { get; set; }
        public Recipe RecipeToAdd { get; set; }
        public string Search { get; set; }
        public List<Recipe> RecipesList { get; set; }
    }
}
