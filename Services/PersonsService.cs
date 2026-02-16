using Entities;
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
        private readonly List<Person> _persons;
        private  readonly ICountriesService _countryService;

        public PersonsService(bool Initialize = true)
        {
            _persons = new List<Person>();
            _countryService = new CountriesService();
            if (Initialize)
            {
              
                _persons.AddRange(new List<Person>() { new Person() {PersonID =Guid.Parse("5767C78B-C6A7-4F3D-A455-ABA4EBCF4710"),PersonName="Lewiss"
                    ,Email="ltucknutt0@myspace.com",DateOfBirth=DateTime.Parse("1996-03-15"),Address="Genderqueer,83 Summerview Place",Gender="Male",ReceiveNewsLetters= true
                ,CountryID = Guid.Parse("FA22B4FC-A720-490B-939D-B72A7EAD8BE1")},
                new Person() {PersonID =Guid.Parse("F2AF91D7-AF3C-4A05-8A9B-81CFD1808259"),PersonName="Gibbie"
                    ,Email="gstidworthy1@adobe.com",DateOfBirth=DateTime.Parse("1998-10-15"),Address=",Female,1655 Eggendart Road",Gender="Male",ReceiveNewsLetters= false
                ,CountryID = Guid.Parse("D5409333-E453-47C9-A2B8-6D5C0439FE8D")},
                    new Person() {PersonID =Guid.Parse("E1CF11AB-EE32-4D7C-B515-52729A762543"),PersonName="Clark"
                    ,Email="cerni2@prweb.com",DateOfBirth=DateTime.Parse("1996-12-30"),Address="80362 Knutson Junction",Gender="Male",ReceiveNewsLetters= false
                ,CountryID = Guid.Parse("95776507-9DA9-44F5-B106-F7ACE2E474FCD")},
                        new Person() {PersonID =Guid.Parse("2A2E6E57-8EDD-49BD-8E64-0B34CFBF515E"),PersonName="Madelena"
                    ,Email="Andria,aduchesne4@prweb.com",DateOfBirth=DateTime.Parse("1997-06-10"),Address="7318 Johnson Plaza",Gender="Female",ReceiveNewsLetters= true
                ,CountryID = Guid.Parse("D5409333-E453-47C9-A2B8-6D5C0439FE8D")},
                            new Person() {PersonID =Guid.Parse("F1DA467E-E7B2-42E2-92EF-353A3BAC5CAB"),PersonName="Cathie"
                    ,Email="nchurches7@histats.com",DateOfBirth=DateTime.Parse("1999-07-16"),Address="Female,1655 Eggendart Road",Gender="Male",ReceiveNewsLetters= false
                ,CountryID = Guid.Parse("95776507-9DA9-44F5-B106-F7ACE2E474FC")},
                                new Person() {PersonID =Guid.Parse("6DE79119-CF1A-4FA1-8CEE-AB6B46808A31"),PersonName="Nicol"
                    ,Email="jclemerson9@ovh.net",DateOfBirth=DateTime.Parse("1992-11-29"),Address="83 Summerview Place",Gender="Female",ReceiveNewsLetters= true
                ,CountryID = Guid.Parse("FA22B4FC-A720-490B-939D-B72A7EAD8BE1") }
                      });
            }

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
