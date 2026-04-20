using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class PersonsDbContext : DbContext
    {
        public DbSet<Person> Persons { get; set; }
        public DbSet<Country> Countries { get; set; }


        public PersonsDbContext(DbContextOptions options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Country>().ToTable("Countries");

            modelBuilder.Entity<Person>().ToTable("Persons");

            //Seed to Countries
            string CountriesJson = System.IO.File.ReadAllText("countries.json");
            List<Country> countries = System.Text.Json.JsonSerializer.Deserialize<List<Country>>(CountriesJson);

            foreach (var country in countries)
            {
                modelBuilder.Entity<Country>().HasData(country);
            }


            //Seed to Persons
            string PersonsJson = System.IO.File.ReadAllText("Persons.json");
            List<Person> Persons = System.Text.Json.JsonSerializer.Deserialize<List<Person>>(PersonsJson);

            foreach (var person in Persons)
            {
                modelBuilder.Entity<Person>().HasData(person);
            }

        }

        public List<Person> sp_GetAllPersons()
        {
           return this.Persons.FromSqlRaw("EXECUTE dbo.GetAllPersons").ToList();
        }

        public int sp_InsertPerson(Person person)
        {
            SqlParameter[] parameters = new SqlParameter[] { new SqlParameter("@PersonID", person.PersonID),
                                                            new SqlParameter("@PersonName", person.PersonName),
                                                            new SqlParameter("@Email", person.Email),
                                                            new SqlParameter("@DateOfBirth", person.DateOfBirth),
                                                            new SqlParameter("@Gender",person.Gender),
                                                            new SqlParameter("@CountryID",person.CountryID),
                                                            new SqlParameter("@Address",person.Address),
                                                            new SqlParameter("@ReceiveNewsLetters",person.ReceiveNewsLetters)
                                                            };
          return  Database.ExecuteSqlRaw("EXECUTE dbo.InsertPerson @PersonID, @PersonName, @Email, @DateOfBirth, @Gender, @CountryID, @Address, @ReceiveNewsLetters", parameters);
            
        }
    }
}
