using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CS340_Group_Project.Models
{
    public class Person
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public Planet Homeworld { get; set; }
        public List<Certification> Certifications { get; set; }
        public int? Age { get; set; }
    }
}
