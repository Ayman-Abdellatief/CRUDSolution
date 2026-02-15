using System;
using System.ComponentModel.DataAnnotations;
using Entities;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;
using Services.Helper;
namespace Services
{
    public class PersonsService : IPersonsService
    {
        //private readonly List<Person> _persons = new List<Person>();
        private readonly List<Person> _persons;
        private  readonly ICountriesService _countryService;

        public PersonsService()
        {
            _persons = new List<Person>();
            _countryService = new CountriesService();
        }
        
        private PersonResponse ConvertPersonToPersonResponse(Person person)
        {

            PersonResponse personResponse = person.ToPersonResponse();
            personResponse.Country = _countryService.GetCountryByCountryID(person.CountryID)?.CountryName;
            return personResponse;
        }
        public PersonResponse AddPerson(PersonAddRequest? personAddRequest)
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
            _persons.Add(person);

            //Create a PersonResponse object to return
         return   ConvertPersonToPersonResponse(person);


        }
        public List<PersonResponse> GetAllPersons()
        {
         return  _persons.Select(temp => temp.ToPersonResponse()).ToList();
        }

        public PersonResponse? GetPersonByPersonID(Guid personID)
        {
            //Check whether personID is empty
          if(personID == null)
                return null;
 
          Person? person =   _persons.FirstOrDefault(temp => temp.PersonID == personID);
         if (person == null)
                return null;
            return ConvertPersonToPersonResponse(person);

        }

        public List<PersonResponse> GetFilteredPersons(string? searchBy, string? searchString)
        {
            //Check if"Search By" is not Null

            List<PersonResponse> allPersons = GetAllPersons();

            List<PersonResponse> matchingPersons = allPersons;

            if (string.IsNullOrEmpty(searchBy)&& string.IsNullOrEmpty(searchString))
                return matchingPersons;

            switch (searchBy)
            {
                case nameof(Person.PersonName):
                    matchingPersons = allPersons.Where(x => (string.IsNullOrEmpty(x.PersonName) ? x.PersonName.Contains(searchString, StringComparison.OrdinalIgnoreCase) : true)).ToList();
                    break;


                case nameof(Person.Email):
                    matchingPersons = allPersons.Where(x => (string.IsNullOrEmpty(x.Email) ? x.Email.Contains(searchString, StringComparison.OrdinalIgnoreCase) : true)).ToList();
                    break;

                case nameof(Person.DateOfBirth):
                    matchingPersons = allPersons.Where(x => (x.DateOfBirth != null) ? x.DateOfBirth.Value.ToString("dd MMMM yyyy").Contains(searchString, StringComparison.OrdinalIgnoreCase) : true).ToList();
                    break;


                case nameof(Person.Gender):
                    matchingPersons = allPersons.Where(x => (!string.IsNullOrEmpty(x.Gender) ? x.Gender.Contains(searchString, StringComparison.OrdinalIgnoreCase) : true)).ToList();
                    break;



                case nameof(Person.Country):
                    matchingPersons = allPersons.Where(x => (!string.IsNullOrEmpty(x.Country) ? x.Country.Contains(searchString, StringComparison.OrdinalIgnoreCase) : true)).ToList();
                    break;




                case nameof(Person.Address):
                    matchingPersons  = allPersons.Where(x => (!string.IsNullOrEmpty(x.Address) ? x.Address.Contains(searchString, StringComparison.OrdinalIgnoreCase) : true)).ToList();
                    break;


                default: matchingPersons = allPersons;
                    break;
            }
                    return matchingPersons;

            }

        public List<PersonResponse> GetSortedPersons(List<PersonResponse> allPersons, string? sortBy, SortOrderOptions sortOrder)
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

            public PersonResponse UpdatePerson(PersonUpdateRequest? personUpdateRequest)
            {
                if (personUpdateRequest == null)
                    throw new ArgumentNullException(nameof(personUpdateRequest), "Person details cannot be null.");

                //Model validation for PersonUpdateRequest
                ValidationHelper.ModelValidation(personUpdateRequest);

                //Find the person to be updated
                Person? person = _persons.FirstOrDefault(temp => temp.PersonID == personUpdateRequest.PersonID);
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

                return  person.ToPersonResponse();
            }

        public bool DeletePerson(Guid? personID)
        {
            if(personID == null)
                throw new ArgumentNullException(nameof(personID), "Person ID cannot be null.");

          Person? person =  _persons.FirstOrDefault(temp => temp.PersonID == personID);
            if (person == null)
                    return false;
    
                 _persons.RemoveAll (x =>x.PersonID == personID);
            return true;
        }
    }
}
