using Entities;
using Microsoft.EntityFrameworkCore;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;
using Services.Helper;
using System;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Reflection;
namespace Services
{
    public class PersonsService : IPersonsService
    {
        //private readonly List<Person> _persons = new List<Person>();
        private readonly PersonsDbContext _db;
        private  readonly ICountriesService _countryService;

        public PersonsService(PersonsDbContext personsDbContext,ICountriesService countriesService)
        {
            _db = personsDbContext;
            _countryService = countriesService;
        

        }
  
        public async Task<PersonResponse> AddPerson(PersonAddRequest? personAddRequest)
        {
            if (personAddRequest == null)
                throw new ArgumentNullException(nameof(personAddRequest), "Person details cannot be null.");

            //Model validation for PersonAddRequest

            ValidationHelper.ModelValidation(personAddRequest);


            //convert PersonAddRequest into Person type
            Person person = personAddRequest.ToPerson();
            //generta personID
            person.PersonID = Guid.NewGuid();
            //Add person to the list
            _db.Persons.Add(person);
           await _db.SaveChangesAsync();

            //_db.sp_InsertPerson(person);
            //Create a PersonResponse object to return
            return person.ToPersonResponse();


        }
        public async Task<List<PersonResponse>> GetAllPersons()
        {

            var persons = await _db.Persons.Include("Country").ToListAsync();
            return persons.Select(temp => temp.ToPersonResponse()).ToList();
            //return _db.sp_GetAllPersons().Select(temp => temp.ToPersonResponse()).ToList(); 
        }

        public async Task<PersonResponse?> GetPersonByPersonID(Guid personID)
        {
            //Check whether personID is empty

            if (personID == null)
                return null;
 
          Person? person =await  _db.Persons.Include("Country").FirstOrDefaultAsync(temp => temp.PersonID == personID);
           
         if (person == null)
                return null;
            return person.ToPersonResponse();

        }

        public async Task<List<PersonResponse>> GetFilteredPersons(string? searchBy, string? searchString)
        {
            //Check if"Search By" is not Null

            List<PersonResponse> allPersons =await GetAllPersons();

            List<PersonResponse> matchingPersons = allPersons;

            if (string.IsNullOrEmpty(searchBy)&& string.IsNullOrEmpty(searchString))
                return matchingPersons;

            switch (searchBy)
            {
                case nameof(PersonResponse.PersonName):
                    matchingPersons = allPersons.Where(x => !string.IsNullOrEmpty(x.PersonName) && x.PersonName.Contains(searchString, StringComparison.OrdinalIgnoreCase)).ToList();
                    break;


                case nameof(PersonResponse.Email):
                    matchingPersons = allPersons.Where(x => !string.IsNullOrEmpty(x.Email) && x.Email.Contains(searchString, StringComparison.OrdinalIgnoreCase)).ToList();
                    break;

                case nameof(PersonResponse.DateOfBirth):
                    matchingPersons = allPersons.Where(x => (x.DateOfBirth != null) ? x.DateOfBirth.Value.ToString("dd MMMM yyyy").Contains(searchString, StringComparison.OrdinalIgnoreCase) : true).ToList();
                    break;


                case nameof(PersonResponse.Gender):
                    matchingPersons = allPersons.Where(x => (!string.IsNullOrEmpty(x.Gender) ? x.Gender.Contains(searchString, StringComparison.OrdinalIgnoreCase) : true)).ToList();
                    break;



                case nameof(PersonResponse.Country):
                    matchingPersons = allPersons.Where(x => (!string.IsNullOrEmpty(x.Country) ? x.Country.Contains(searchString, StringComparison.OrdinalIgnoreCase) : true)).ToList();
                    break;




                case nameof(PersonResponse.Address):
                    matchingPersons  = allPersons.Where(x => (!string.IsNullOrEmpty(x.Address) ? x.Address.Contains(searchString, StringComparison.OrdinalIgnoreCase) : true)).ToList();
                    break;


                default: matchingPersons = allPersons;
                    break;
            }
                    return matchingPersons;

            }

        public async Task<List<PersonResponse>> GetSortedPersons(List<PersonResponse> allPersons, string? sortBy, SortOrderOptions sortOrder)
        {
            if (string.IsNullOrEmpty(sortBy))
                return allPersons;
            List<PersonResponse> sortedPersons = (sortBy, sortOrder)
          switch
            {
                (nameof(PersonResponse.PersonName), SortOrderOptions.ASC) =>
                  allPersons.OrderBy(x => x.PersonName, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.PersonName), SortOrderOptions.DESC) =>
               allPersons.OrderByDescending(x => x.PersonName, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.Email), SortOrderOptions.ASC) =>
                allPersons.OrderBy(x => x.Email, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.Email), SortOrderOptions.DESC) =>
                allPersons.OrderByDescending(x => x.Email, StringComparer.OrdinalIgnoreCase).ToList(),


                (nameof(PersonResponse.DateOfBirth), SortOrderOptions.ASC) =>
                allPersons.OrderBy(x => x.DateOfBirth).ToList(),

                (nameof(PersonResponse.DateOfBirth), SortOrderOptions.DESC) =>
                   allPersons.OrderByDescending(x => x.DateOfBirth).ToList(),


                (nameof(PersonResponse.Age), SortOrderOptions.ASC) =>
              allPersons.OrderBy(x => x.Age).ToList(),

                (nameof(PersonResponse.Age), SortOrderOptions.DESC) =>
                      allPersons.OrderByDescending(x => x.Age).ToList(),


                (nameof(PersonResponse.Gender), SortOrderOptions.ASC) =>
                       allPersons.OrderBy(x => x.Gender, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.Gender), SortOrderOptions.DESC) =>
                allPersons.OrderByDescending(x => x.Gender, StringComparer.OrdinalIgnoreCase).ToList(),


                (nameof(PersonResponse.Country), SortOrderOptions.ASC) =>
                  allPersons.OrderBy(x => x.Country, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.Country), SortOrderOptions.DESC) =>
                  allPersons.OrderByDescending(x => x.Country, StringComparer.OrdinalIgnoreCase).ToList(),


                (nameof(PersonResponse.Address), SortOrderOptions.ASC) =>
                 allPersons.OrderBy(x => x.Address, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.Address), SortOrderOptions.DESC) =>
                   allPersons.OrderByDescending(x => x.Address, StringComparer.OrdinalIgnoreCase).ToList(),


                (nameof(PersonResponse.ReceiveNewsLetters), SortOrderOptions.ASC) =>
                  allPersons.OrderBy(x => x.ReceiveNewsLetters ).ToList(),

                (nameof(PersonResponse.ReceiveNewsLetters), SortOrderOptions.DESC) =>
                   allPersons.OrderByDescending(x => x.ReceiveNewsLetters ).ToList(),

                 _  => allPersons


            };

            return sortedPersons;
        }

            public async Task<PersonResponse> UpdatePerson(PersonUpdateRequest? personUpdateRequest)
            {
                if (personUpdateRequest == null)
                    throw new ArgumentNullException(nameof(personUpdateRequest), "Person details cannot be null.");

                //Model validation for PersonUpdateRequest
                ValidationHelper.ModelValidation(personUpdateRequest);

                //Find the person to be updated
                Person? person = await _db.Persons.FirstOrDefaultAsync(temp => temp.PersonID == personUpdateRequest.PersonID);
                if (person == null)
                    throw new KeyNotFoundException($"Person with ID {personUpdateRequest.PersonID} not found.");

                //Update the person details
                person.PersonName = personUpdateRequest.PersonName;
                person.Email = personUpdateRequest.Email;
                person.DateOfBirth = personUpdateRequest.DateOfBirth;
                person.Gender = personUpdateRequest.Gender.ToString();
                person.CountryID = personUpdateRequest.CountryID;
                person.Address = personUpdateRequest.Address;
                person.ReceiveNewsLetters = personUpdateRequest.ReceiveNewsLetters;
            await _db.SaveChangesAsync();  //Update the person details in the list

            return person.ToPersonResponse() ;
            }

        public async Task<bool> DeletePerson(Guid? personID)
        {
            if(personID == null)
                throw new ArgumentNullException(nameof(personID), "Person ID cannot be null.");

          Person? person = await _db.Persons.FirstOrDefaultAsync(temp => temp.PersonID == personID);
            if (person == null)
                    return false;
    
                 _db.Persons.Remove(person);
          await  _db.SaveChangesAsync();
            return true;
        }
    }
}
