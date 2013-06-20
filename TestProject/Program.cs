using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestProject
{
    class Program
    {
        class Person
        {
            public string Name { get; set; }
            public int ID { get; set; }
        }

        class Pet
        {
            public string Name { get; set; }
            public int OwnerID { get; set; }
        }

        public static void Main()
        {
            Person magnus = new Person { Name = "Hedlund, Magnus", ID = 1 };
            Person terry = new Person { Name = "Adams, Terry", ID = 2 };
            Person charlotte = new Person { Name = "Weiss, Charlotte", ID = 3 };

            Pet barley = new Pet { Name = "Barley", OwnerID = 1 };
            Pet boots = new Pet { Name = "Boots", OwnerID = 2 };
            Pet whiskers = new Pet { Name = "Whiskers", OwnerID = 2 };
            Pet daisy = new Pet { Name = "Daisy", OwnerID = 3 };

            List<Person> people = new List<Person> { magnus, terry, charlotte };
            List<Pet> pets = new List<Pet> { barley, boots, whiskers, daisy };

            // Join the list of Person objects and the list of Pet objects  
            // to create a list of person-pet pairs where each element is  
            // an anonymous type that contains the name of pet and the name 
            // of the person that owns the pet. 
            var query = people.AsQueryable().Join(pets,
                            person => person.ID,
                            pet => pet.OwnerID,
                            (person, pet) =>
                               new { Name = person.Name, Pet = pet.Name });

            foreach (var obj in query)
            {
                Console.WriteLine(
                    "{0}",
                    obj.Name);
            }

            Console.ReadLine();
        }


    }
}
