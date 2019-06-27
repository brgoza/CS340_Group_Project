using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CS340_Group_Project.Models;
using CS340_Group_Project.Models.ViewModels;
using MySql.Data;
using System.Text;

namespace CS340_Group_Project.Controllers
{
    public class HomeController : Controller
    {
        public string ConnectionString =
            "Server=cs340maria-db.mariadb.database.azure.com; Port=3306; Database=cs340_401_groupproject; Uid=gozab@cs340maria-db; Pwd=Pa$$word; SslMode=Preferred;";

        public PeopleIndexViewModel GetPeopleIndexViewModel()
        {
            PeopleIndexViewModel vm = new PeopleIndexViewModel();
            MySql.Data.MySqlClient.MySqlConnection connection = new MySql.Data.MySqlClient.MySqlConnection(ConnectionString);
            connection.Open();
            MySql.Data.MySqlClient.MySqlCommand cmd = connection.CreateCommand();
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("SELECT * FROM bsg_people\n");
            sb.AppendFormat("LEFT JOIN bsg_planets ON bsg_people.homeworld = bsg_planets.id");
            cmd.CommandText = sb.ToString();
            vm.People = new List<Person>();
            var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                Person person = new Person
                {
                    Id = reader.GetInt32("id"),
                    FirstName = reader.GetString("fname")
                };
                if (!reader.IsDBNull(reader.GetOrdinal("lname")))
                {
                    person.LastName = reader.GetString("lname");
                }
                person.Homeworld = new Planet();
                if (!reader.IsDBNull(reader.GetOrdinal("homeworld")))
                {
                    person.Homeworld.Name = reader.GetString("name");
                }
                if (!reader.IsDBNull(reader.GetOrdinal("age")))
                {
                    person.Age = (int)reader.GetValue(reader.GetOrdinal("age"));
                }
                vm.People.Add(person);
            }
            return vm;

        }

        public IActionResult Index()
        {
            PeopleIndexViewModel vm = GetPeopleIndexViewModel();
            return View(vm);
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
