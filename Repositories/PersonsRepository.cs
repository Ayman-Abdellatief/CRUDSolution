using Entities;
using Microsoft.EntityFrameworkCore;
using RepositoryContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public class PersonsRepository : IPersonsRepository
    {

       private readonly Entities.ApplicationDbContext _db;

        public PersonsRepository(Entities.ApplicationDbContext db)
        {
            _db = db;
        }
        public async Task<Person> AddPerson(Person person)
        {
         _db.Persons.Add(person);
         await   _db.SaveChangesAsync();
         return person;

        }

        public async Task<bool> DeletePersonByPersonID(Guid personID)
        {
           _db.Persons.RemoveRange(_db.Persons.Where(temp => temp.PersonID == personID));
         int affectedRows =  await _db.SaveChangesAsync();
           return affectedRows>0;
        }

        public async Task<List<Person>> GetAllPersons()
        {
         return  await   _db.Persons.Include("Country").ToListAsync();
        }

        public async Task<List<Person>> GetFilteredPersons(Expression<Func<Person, bool>> predicate)
        {
            return await _db.Persons.Include("Country").Where(predicate).ToListAsync();
        }

        public async Task<Person?> GetPersonsByPersonID(Guid personID)
        {
            return await _db.Persons.Include("Country").FirstOrDefaultAsync(temp => temp.PersonID == personID);
        }

        public async Task<Person> UpdatePerson(Person person)
        {
       Person matchingperson = await _db.Persons.FirstOrDefaultAsync(temp => temp.PersonID == person.PersonID);
            if (matchingperson == null)
                return person;
            
            matchingperson.PersonName = person.PersonName;
            matchingperson.Email = person.Email;
            matchingperson.DateOfBirth = person.DateOfBirth;
            matchingperson.Gender = person.Gender;
            matchingperson.CountryID = person.CountryID;
            matchingperson.Address = person.Address;
            matchingperson.ReceiveNewsLetters = person.ReceiveNewsLetters;

         int countUpdated =    await _db.SaveChangesAsync();
           return matchingperson;
        }

      
    }
}
