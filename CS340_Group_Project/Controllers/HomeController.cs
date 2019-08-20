using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CS340_Group_Project.Models;
using MySql.Data;
using System.Text;
using Dapper;
using System.Data;
using CS340_Group_Project.Models;

namespace CS340_Group_Project.Controllers
{
    public class HomeController : Controller
    {
        public string ConnectionString =
            "Server=cs340maria-db.mariadb.database.azure.com; Port=3306; Database=recipe_schema; Uid=gozab@cs340maria-db; Pwd=Pa$$word; SslMode=Preferred;";

        #region SQLInteractions
        public IngredientIndexViewModel GetIngredientIndexViewModel()
        {
            IngredientIndexViewModel vm = new IngredientIndexViewModel();
            var conn = new MySql.Data.MySqlClient.MySqlConnection(ConnectionString);
            vm.Ingredients = conn.Query<Ingredient>("SELECT * FROM ingredient").ToList();
            return vm;
        }

        public RecipeCollectionIndexViewModel GetRecipeCollectionIndexViewModel()
        {
            RecipeCollectionIndexViewModel vm = new RecipeCollectionIndexViewModel();
            var conn = new MySql.Data.MySqlClient.MySqlConnection(ConnectionString);
            vm.RecipeCollections = conn.Query<RecipeCollection>("SELECT * FROM recipe_collection").ToList();
            return vm;
        }

        public void AddIngredientToDatabase(Ingredient ingredient)
        {
            var conn = new MySql.Data.MySqlClient.MySqlConnection(ConnectionString);
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("INSERT INTO recipe_schema.ingredient" + "({0},{1})  VALUES ('{2}', '{3}')", "Name", "Description", ingredient.Name, ingredient.Description);
            conn.Query(sb.ToString());
        }

        public RecipeIndexViewModel GetRecipeIndexViewModel()
        {
            RecipeIndexViewModel vm = new RecipeIndexViewModel();
            IDbConnection conn;
            conn = new MySql.Data.MySqlClient.MySqlConnection(ConnectionString);
            vm.Recipes = conn.Query<Recipe>("SELECT * FROM Recipe").ToList();
            return vm;
        }

        public Recipe GetRecipeDetails(int recipeId)
        {
            Recipe r = new Recipe();
            IDbConnection conn;
            conn = new MySql.Data.MySqlClient.MySqlConnection(ConnectionString);
            r = conn.Query<Recipe>("SELECT * FROM Recipe WHERE Id = " + recipeId).FirstOrDefault();
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("SELECT * FROM recipe_ingredient AS A LEFT JOIN ingredient AS B ON A.IngredientId = B.Id " +
                 "LEFT JOIN quantity_unit AS C ON A.QuantityUnitId = C.Id WHERE RecipeId = {0}", recipeId);
            r.RecipeIngredients = conn.Query<RecipeIngredient, Ingredient, QuantityUnit, RecipeIngredient>(sb.ToString(),
                (recipeIngredient, ingredient, quantityUnit) =>
                {
                    recipeIngredient.Ingredient = ingredient;
                    recipeIngredient.QuantityUnit = quantityUnit;
                    return recipeIngredient;
                }, splitOn: "Id").ToList();
            return r;

        }

        public RecipeAddViewModel GetRecipeAddViewModel()
        {
            RecipeAddViewModel vm = new RecipeAddViewModel();
            vm.Recipe = new Recipe();
            return vm;
        }

        public RecipeCollection GetRecipeCollectionDetails(int rcId)
        {
            RecipeCollection rc = new RecipeCollection();
            var conn = new MySql.Data.MySqlClient.MySqlConnection(ConnectionString);
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("SELECT * FROM recipe_collection WHERE Id = {0}", rcId);
            rc = conn.Query<RecipeCollection>(sb.ToString()).FirstOrDefault();
            sb.Clear();
            rc.Recipes = new List<Recipe>();
            sb.AppendFormat("SELECT * FROM recipe_collection_recipe INNER JOIN recipe ON RecipeId = Id WHERE RecipeCollectionId = {0}", rcId);
            rc.Recipes = conn.Query<Recipe>(sb.ToString()).ToList();
            conn.Close();
            return rc;
        }


        public RecipeCollectionEditViewModel GetRecipeCollectionEditViewModel(int rcId, string search)
        {
            RecipeCollectionEditViewModel vm = new RecipeCollectionEditViewModel();

            var conn = new MySql.Data.MySqlClient.MySqlConnection(ConnectionString);
            StringBuilder sb = new StringBuilder();

            sb.AppendFormat("SELECT * FROM recipe_collection WHERE Id = {0}", rcId);
            vm.RecipeCollection = conn.Query<RecipeCollection>(sb.ToString()).FirstOrDefault();
            sb.Clear();

            sb.AppendFormat("SELECT * FROM recipe_collection_recipe INNER JOIN recipe ON RecipeId = Id WHERE RecipeCollectionId = {0}", rcId);
            vm.RecipeCollection.Recipes = conn.Query<Recipe>(sb.ToString()).ToList();
            sb.Clear();

            sb.AppendFormat("SELECT * FROM recipe WHERE name LIKE '%" + search + "%'");
            vm.RecipesList = conn.Query<Recipe>(sb.ToString()).ToList();

            conn.Close();
            return vm;
        }

        public void UpdateCollectionDescription(RecipeCollectionEditViewModel obj)
        {
            var conn = new MySql.Data.MySqlClient.MySqlConnection(ConnectionString);
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("UPDATE recipe_collection\n");
            sb.AppendFormat("SET description = '" + obj.RecipeCollection.Description + "'\n");
            sb.AppendFormat("WHERE id = " + obj.RecipeCollection.Id);
            conn.Execute(sb.ToString());
        }

        public void AddRecipeToCollectionInDatabase(int recipeId, int rcId)
        {
            var conn = new MySql.Data.MySqlClient.MySqlConnection(ConnectionString);
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("INSERT INTO recipe_collection_recipe (RecipeId, RecipeCollectionId) \n");
            sb.AppendFormat("VALUES ({0}, {1})", recipeId, rcId);
            conn.Execute(sb.ToString());
        }

        public int AddRecipeCollectionToDatabase(RecipeCollection rc)
        {
            var conn = new MySql.Data.MySqlClient.MySqlConnection(ConnectionString);
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("INSERT INTO recipe_schema.recipe_collection" + "({0},{1})  VALUES ('{2}', '{3}')", "Name", "Description", rc.Name, rc.Description);
            conn.Query(sb.ToString());
            return (int)(UInt64)conn.ExecuteScalar("SELECT LAST_INSERT_ID()");
        }

        public int AddRecipeToDatabase(Recipe recipe)
        {
            var conn = new MySql.Data.MySqlClient.MySqlConnection(ConnectionString);
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("INSERT INTO recipe_schema.recipe" + "({0},{1},{2})  VALUES ('{3}', '{4}', '{5}')", "Name", "Contributor", "Description", recipe.Name, recipe.Contributor, recipe.Description);
            conn.Query(sb.ToString());
            return (int)(UInt64)conn.ExecuteScalar("SELECT LAST_INSERT_ID()");
        }

        public void UpdateInstructions(Recipe recipe)
        {
            var conn = new MySql.Data.MySqlClient.MySqlConnection(ConnectionString);
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("UPDATE recipe_schema.recipe\n");
            sb.AppendFormat("SET instructions = '" + recipe.Instructions + "'\n");
            sb.AppendFormat("WHERE id = " + recipe.Id);
            conn.Query(sb.ToString());
        }

        public RecipeIngredientsEditViewModel GetRecipeIngredientsEditViewModel(int recipeId)
        {
            RecipeIngredientsEditViewModel vm = new RecipeIngredientsEditViewModel();

            IDbConnection conn;
            conn = new MySql.Data.MySqlClient.MySqlConnection(ConnectionString);
            vm.Component = new RecipeIngredient();
            vm.Component.RecipeId = recipeId;
            vm.Component.Recipe = conn.Query<Recipe>("SELECT * FROM recipe WHERE Id = " + recipeId).FirstOrDefault();
            vm.Component.Recipe.RecipeIngredients =
                conn.Query<RecipeIngredient>("SELECT * FROM recipe_component WHERE RecipeId = " + recipeId).ToList();
            vm.QuantityUnits = conn.Query<QuantityUnit>("SELECT * FROM quantity_unit").ToList();
            vm.Ingredients = conn.Query<Ingredient>("SELECT * FROM ingredient").ToList();
            conn.Close();
            return vm;
        }
        public void AddComponentToDatabase(RecipeIngredient component)
        {
            var conn = new MySql.Data.MySqlClient.MySqlConnection(ConnectionString);
            StringBuilder sb = new StringBuilder();
            string qu = string.Empty;
            qu = component.QuantityUnitId.ToString();
            if (component.QuantityUnitId == 0) qu = "NULL";
            sb.AppendFormat("INSERT INTO recipe_schema.recipe_ingredient" + "({0},{1},{2},{3})  VALUES ({4}, {5}, {6}, {7})", "RecipeId", "IngredientId", "Quantity", "QuantityUnitId", component.RecipeId, component.IngredientId, component.Quantity, qu);

            conn.Query(sb.ToString());
            conn.Close();
        }
        public RecipeEditViewModel GetRecipeEditViewModel(int recipeId)
        {
            RecipeEditViewModel vm = new RecipeEditViewModel();

            IDbConnection conn;
            conn = new MySql.Data.MySqlClient.MySqlConnection(ConnectionString);
            vm.Recipe = conn.Query<Recipe>("SELECT * FROM recipe WHERE Id = " + recipeId).ToList().FirstOrDefault();
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("SELECT * FROM recipe_ingredient AS A LEFT JOIN ingredient AS B ON A.IngredientId = B.Id " +
                "LEFT JOIN quantity_unit AS C ON A.QuantityUnitId = C.Id WHERE RecipeId = {0}", recipeId);
            vm.Recipe.RecipeIngredients = conn.Query<RecipeIngredient, Ingredient, QuantityUnit, RecipeIngredient>(sb.ToString(),
                (component, ingredient, quantityUnit) =>
                {
                    component.Ingredient = ingredient;
                    component.QuantityUnit = quantityUnit;
                    return component;
                }, splitOn: "Id").ToList();
            vm.IngredientsList = conn.Query<Ingredient>("SELECT * FROM ingredient").ToList();
            vm.QuantityUnitsList = conn.Query<QuantityUnit>("SELECT * FROM quantity_unit").ToList();
            return vm;
        }

        public void DeleteRecipeIngredientFromDatabase(int recipeId, int ingredientId)
        {
            IDbConnection conn;
            conn = new MySql.Data.MySqlClient.MySqlConnection(ConnectionString);
            conn.Execute("DELETE FROM recipe_ingredient WHERE recipeId = " + recipeId.ToString() + " AND ingredientId = " + ingredientId.ToString());
            conn.Close();
        }

        public void DeleteRecipeFromDatabase(int recipeId)
        {
            IDbConnection conn;
            conn = new MySql.Data.MySqlClient.MySqlConnection(ConnectionString);
            conn.Execute("DELETE FROM recipe WHERE id = " + recipeId.ToString());
            conn.Close();
        }
        #endregion


        public IActionResult RecipeCollectionIndex()
        {
            RecipeCollectionIndexViewModel vm = GetRecipeCollectionIndexViewModel();
            return View(vm);
        }

        public IActionResult RecipeCollectionDetails(int rcId)
        {
            RecipeCollection rc = GetRecipeCollectionDetails(rcId);
            return View(rc);
        }

        public IActionResult AddRecipeCollection()
        {
            return View(new RecipeCollection());
        }

        [HttpPost]
        public IActionResult AddRecipeCollection(RecipeCollection rc)
        {
            int rcId = AddRecipeCollectionToDatabase(rc);
            return RedirectToAction("EditRecipeCollection", new { recipeCollectionId = rcId });
        }

        public IActionResult EditRecipeCollection(int rcId, string search)
        {
            RecipeCollectionEditViewModel vm = GetRecipeCollectionEditViewModel(rcId, search);
            return View(vm);
        }
        public IActionResult AddRecipeToCollection(int recipeId, int rcId)
        {
            AddRecipeToCollectionInDatabase(recipeId, rcId);
            return RedirectToAction("EditRecipeCollection", new { rcId, string.Empty });
        }

        [HttpPost]
        public IActionResult ModifyCollectionDescription(RecipeCollectionEditViewModel obj)
        {
            UpdateCollectionDescription(obj);
            return RedirectToAction("EditRecipeCollection", new { recipeCollectionId = obj.RecipeCollection.Id });
        }


        public IActionResult Index()
        {
            RecipeIndexViewModel vm = GetRecipeIndexViewModel();
            return View(vm);
        }

        public IActionResult RecipeDetails(int recipeId)
        {
            Recipe r = GetRecipeDetails(recipeId);
            return View(r);
        }

        public IActionResult AddRecipe()
        {
            RecipeAddViewModel vm = GetRecipeAddViewModel();
            return View(vm);
        }

        [HttpPost]
        public IActionResult AddRecipe(RecipeAddViewModel obj)
        {
            int recipeId = AddRecipeToDatabase(obj.Recipe);
            return RedirectToAction("EditRecipe", new { recipeId = recipeId });
        }

        public IActionResult EditRecipe(int recipeId)
        {
            RecipeEditViewModel vm = GetRecipeEditViewModel(recipeId);
            return View(vm);
        }

        public IActionResult DeleteRecipe(int recipeId)
        {
            DeleteRecipeFromDatabase(recipeId);
            return RedirectToAction("Index");
        }

        public IActionResult DeleteRecipeIngredient(int recipeId, int ingredientId)
        {
            DeleteRecipeIngredientFromDatabase(recipeId, ingredientId);
            return RedirectToAction("EditRecipe", new { recipeId = recipeId });
        }

        [HttpPost]
        public IActionResult ModifyInstructions(RecipeEditViewModel obj)
        {
            obj.Recipe.Id = obj.RecipeId;
            UpdateInstructions(obj.Recipe);
            return RedirectToAction("EditRecipe", new { recipeId = obj.RecipeId });
        }

        [HttpPost]
        public IActionResult AddComponent(RecipeEditViewModel obj)
        {
            obj.NewRecipeIngredient.RecipeId = obj.RecipeId;
            AddComponentToDatabase(obj.NewRecipeIngredient);
            return RedirectToAction("EditRecipe", new { recipeId = obj.RecipeId });
        }


        public IActionResult IngredientIndex()
        {
            var vm = GetIngredientIndexViewModel();
            return View(vm);
        }

        public IActionResult AddIngredient()
        {
            var vm = new Ingredient();
            return View(vm);
        }

        [HttpPost]
        public IActionResult AddIngredient(Ingredient ingredient)
        {
            AddIngredientToDatabase(ingredient);
            return RedirectToAction("IngredientIndex");
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }


        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
