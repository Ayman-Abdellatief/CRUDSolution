using Entities;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;
using Services;
using System;
using System.Collections.Generic;
using Xunit;
using Xunit.Abstractions;


namespace CRUDTests
{
    public class PersonsServiceTest
    {
        private readonly IPersonsService _personsService;
        private readonly ICountriesService _countriesService;
        private readonly ITestOutputHelper _testOutput;
        public PersonsServiceTest(ITestOutputHelper testOutput)
        {
            _personsService = new PersonsService();
            _countriesService = new CountriesService();
            _testOutput = testOutput;
        }
        [Fact]
        //when we supply null or empty values for mandatory fields, then the service should throw an exception
        public void AddPerson_NullPerson()
        {
            // Arrange
            PersonAddRequest? personAddRequest = null;
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => _personsService.AddPerson(personAddRequest!));
        }

        //When we supply valid values for all mandatory fields, then the service should add the person and return the added person details
        [Fact]
        public void AddPerson_PersonNameISNull()
        {
            // Arrange
            PersonAddRequest personAddRequest = new PersonAddRequest
            {
                PersonName = null
            };

            // Act & Assert
            Assert.Throws<ArgumentException>(() => _personsService.AddPerson(personAddRequest));
        }

        //when we supply persondetails, it should insert the persob into persons list;and should return an object of personResponse , which includes with the newly generated person id
        [Fact]
        public void AddPerson_ProperPersonDetails()
        {
            // Arrange
            PersonAddRequest personAddRequest = new PersonAddRequest
            {
                PersonName = "John Doe",
                Email = "PErson@example.com",
                Address = "123 Main St",
                CountryID = Guid.NewGuid(),
                DateOfBirth = new DateTime(1990, 1, 1),
                Gender = GenderOptions.Male,
                ReceiveNewsLetters = true
            };

            // Act
            PersonResponse personResponse_from_add = _personsService.AddPerson(personAddRequest);

            List<PersonResponse> personResponses_list = _personsService.GetAllPersons();

            // Assert
            Assert.True(personResponse_from_add.PersonID != Guid.Empty);

            Assert.Contains(personResponse_from_add, personResponses_list);

        }


        #region GetPersonByPersonID
        //If we supply null as personID, it should return null as PersonReponse

        [Fact]
        public void GetPersonByPersonID_NullPersonID()
        {                      // Arrange
            Guid personID = Guid.Empty;

            // Act
            PersonResponse? personResponse = _personsService.GetPersonByPersonID(personID);

            // Assert
            Assert.Null(personResponse);
        }

        //IF we supply a valid person id ,it should return the valid person details as person response object
        [Fact]
        public void GetPersonByPersonID_withPersonID()
        {

            // Arrange
            CountryAddRequest countryAddRequest = new CountryAddRequest
            {
                CountryName = "India"
            };
            CountryResponse countryResponse = _countriesService.AddCountry(countryAddRequest);


            // Act
            PersonAddRequest personAddRequest = new PersonAddRequest
            {
                PersonName = "John Doe",
                Email = "exampile@sample.com",
                Address = "123 Main",
                CountryID = countryResponse.CountryID,
                DateOfBirth = new DateTime(1990, 1, 1),
                Gender = GenderOptions.Male,
                ReceiveNewsLetters = true
            };

            PersonResponse personResponse_from_add = _personsService.AddPerson(personAddRequest);



            PersonResponse? personResponse_from_get = _personsService.GetPersonByPersonID(personResponse_from_add.PersonID);

            // Assert
            Assert.Equal(personResponse_from_add, personResponse_from_get);
        }


        #endregion

        #region GetAllPersons

        //The GetAllPersons method should return a list of all persons added to the service

        [Fact]
        public void GetAllPersons()
        {
            // Act

            List<PersonResponse> personResponses_from_get = _personsService.GetAllPersons();


            // Assert
            Assert.Empty(personResponses_from_get);
        }
        //first ,we will add few persons;and then when we call get AllPersons() , it should return a list of all added persons
        [Fact]
        public void GetAllPersons_AddFewPersons() {
            // Arrange
            CountryAddRequest countryAddRequest1 = new CountryAddRequest
            {
                CountryName = "India"
            };

            CountryAddRequest countryAddRequest2 = new CountryAddRequest
            {
                CountryName = "USA"
            };

         CountryResponse countryResponse1 =   _countriesService.AddCountry(countryAddRequest1);
           CountryResponse countryResponse2 = _countriesService.AddCountry(countryAddRequest2);
            
            PersonAddRequest personAddRequest1 = new PersonAddRequest
            {
                PersonName = "John Doe",
                Email = "John@example.com",
                Gender = GenderOptions.Male,
                Address = "8 Main St",
                CountryID = countryResponse1.CountryID,
                DateOfBirth = new DateTime(1990, 1, 1),
                ReceiveNewsLetters = true
            };

            PersonAddRequest personAddRequest2 = new PersonAddRequest
            {
                PersonName = "Smith",
                Email = "Smith@example.com",
                Gender = GenderOptions.Male,
                Address = "123 Main St",
                CountryID = countryResponse2.CountryID,
                DateOfBirth = new DateTime(1980, 1, 1),
                ReceiveNewsLetters = false
            };

            PersonAddRequest personAddRequest3 = new PersonAddRequest
            {
                PersonName = "Ali",
                Email = "Ali@example.com",
                Gender = GenderOptions.Male,
                Address = "Ali Main St",
                CountryID = countryResponse2.CountryID,
                DateOfBirth = new DateTime(1980, 1, 1),
                ReceiveNewsLetters = false
            };

            List<PersonAddRequest> personAddRequests_list = new List<PersonAddRequest> { personAddRequest1, personAddRequest2, personAddRequest3 };

            List<PersonResponse> personResponses_from_get_before_add = _personsService.GetAllPersons();

            foreach (PersonAddRequest personAddRequest in personAddRequests_list)
            {
              PersonResponse personResponse =  _personsService.AddPerson(personAddRequest);
                personResponses_from_get_before_add.Add(personResponse);
            }
            //printing the person responses from get before add
            _testOutput.WriteLine("Expected:");
            foreach (PersonResponse personResponse in personResponses_from_get_before_add)
            {
                _testOutput.WriteLine(personResponse.ToString());
            }

            // Act
            List<PersonResponse> personResponses_from_get_after_add = _personsService.GetAllPersons();
            //printing the person responses from get after add
            _testOutput.WriteLine("Actual:");
            foreach (PersonResponse personResponse in personResponses_from_get_after_add)
            {
                _testOutput.WriteLine(personResponse.ToString());   
            }
            // Assert
            foreach (PersonResponse personResponse in personResponses_from_get_before_add)
            {
                Assert.Contains(personResponse, personResponses_from_get_after_add);
            }
        }
        #endregion

        #region GetFilteredPersons

        //If the search tesxt is empty and search by is PersonName, then it should return all the persons as per GetAllPersons method
        [Fact]
        public void GetFilteredPersons_EmptySearchText()
        {
            // Arrange
            CountryAddRequest countryAddRequest1 = new CountryAddRequest
            {
                CountryName = "India"
            };

            CountryAddRequest countryAddRequest2 = new CountryAddRequest
            {
                CountryName = "USA"
            };

            CountryResponse countryResponse1 = _countriesService.AddCountry(countryAddRequest1);
            CountryResponse countryResponse2 = _countriesService.AddCountry(countryAddRequest2);

            PersonAddRequest personAddRequest1 = new PersonAddRequest
            {
                PersonName = "John Doe",
                Email = "John@example.com",
                Gender = GenderOptions.Male,
                Address = "8 Main St",
                CountryID = countryResponse1.CountryID,
                DateOfBirth = new DateTime(1990, 1, 1),
                ReceiveNewsLetters = true
            };

            PersonAddRequest personAddRequest2 = new PersonAddRequest
            {
                PersonName = "Smith",
                Email = "Smith@example.com",
                Gender = GenderOptions.Male,
                Address = "123 Main St",
                CountryID = countryResponse2.CountryID,
                DateOfBirth = new DateTime(1980, 1, 1),
                ReceiveNewsLetters = false
            };

            PersonAddRequest personAddRequest3 = new PersonAddRequest
            {
                PersonName = "Ali",
                Email = "Ali@example.com",
                Gender = GenderOptions.Male,
                Address = "Ali Main St",
                CountryID = countryResponse2.CountryID,
                DateOfBirth = new DateTime(1980, 1, 1),
                ReceiveNewsLetters = false
            };

            List<PersonAddRequest> personAddRequests_list = new List<PersonAddRequest> { personAddRequest1, personAddRequest2, personAddRequest3 };

            List<PersonResponse> personResponses_from_get_before_add = _personsService.GetAllPersons();

            foreach (PersonAddRequest personAddRequest in personAddRequests_list)
            {
                PersonResponse personResponse = _personsService.AddPerson(personAddRequest);
                personResponses_from_get_before_add.Add(personResponse);
            }
            //printing the person responses from get before add
            _testOutput.WriteLine("Expected:");
            foreach (PersonResponse personResponse in personResponses_from_get_before_add)
            {
                _testOutput.WriteLine(personResponse.ToString());
            }

            // Act
            List<PersonResponse> personResponses_from_search = _personsService.GetFilteredPersons(nameof(Person.PersonName),"");
            //printing the person responses from get after add
            _testOutput.WriteLine("Actual:");
            foreach (PersonResponse personResponse in personResponses_from_search)
            {
                _testOutput.WriteLine(personResponse.ToString());
            }
            // Assert
            foreach (PersonResponse personResponse in personResponses_from_get_before_add)
            {
                Assert.Contains(personResponse, personResponses_from_search);
            }
        }


        //Add few Persons and then we will search based on Person Name

        [Fact]
        public void GetFilteredPersons_SearchByPersonName()
        {
            // Arrange
            CountryAddRequest countryAddRequest1 = new CountryAddRequest
            {
                CountryName = "India"
            };

            CountryAddRequest countryAddRequest2 = new CountryAddRequest
            {
                CountryName = "USA"
            };

            CountryResponse countryResponse1 = _countriesService.AddCountry(countryAddRequest1);
            CountryResponse countryResponse2 = _countriesService.AddCountry(countryAddRequest2);

            PersonAddRequest personAddRequest1 = new PersonAddRequest
            {
                PersonName = "Smith",
                Email = "John@example.com",
                Gender = GenderOptions.Male,
                Address = "8 Main St",
                CountryID = countryResponse1.CountryID,
                DateOfBirth = new DateTime(1990, 1, 1),
                ReceiveNewsLetters = true
            };

            PersonAddRequest personAddRequest2 = new PersonAddRequest
            {
                PersonName = "Mary",
                Email = "Smith@example.com",
                Gender = GenderOptions.Male,
                Address = "123 Main St",
                CountryID = countryResponse2.CountryID,
                DateOfBirth = new DateTime(1980, 1, 1),
                ReceiveNewsLetters = false
            };

            PersonAddRequest personAddRequest3 = new PersonAddRequest
            {
                PersonName = "Rahman",
                Email = "Mai@example.com",
                Gender = GenderOptions.Male,
                Address = "Ali Main St",
                CountryID = countryResponse2.CountryID,
                DateOfBirth = new DateTime(1980, 1, 1),
                ReceiveNewsLetters = false
            };

            List<PersonAddRequest> personAddRequests_list = new List<PersonAddRequest> { personAddRequest1, personAddRequest2, personAddRequest3 };

            List<PersonResponse> personResponses_from_get_before_add = _personsService.GetAllPersons();

            foreach (PersonAddRequest personAddRequest in personAddRequests_list)
            {
                PersonResponse personResponse = _personsService.AddPerson(personAddRequest);
                personResponses_from_get_before_add.Add(personResponse);
            }
            //printing the person responses from get before add
            _testOutput.WriteLine("Expected:");
            foreach (PersonResponse personResponse in personResponses_from_get_before_add)
            {
                _testOutput.WriteLine(personResponse.ToString());
            }

            // Act
            List<PersonResponse> personResponses_from_search = _personsService.GetFilteredPersons(nameof(Person.PersonName), "ma");
            //printing the person responses from get after add
            _testOutput.WriteLine("Actual:");
            foreach (PersonResponse personResponse in personResponses_from_search)
            {
                _testOutput.WriteLine(personResponse.ToString());
            }
            // Assert
            foreach (PersonResponse personResponse in personResponses_from_get_before_add)
            {
                if(personResponse.PersonName != null && personResponse.PersonName.Contains("ma", StringComparison.OrdinalIgnoreCase))
                    Assert.Contains(personResponse, personResponses_from_search);
            }
        }
        #endregion

        #region GetSortedPersons
        //when sort based on PersonName in descending order, then it should return the list of persons sorted based on person name in descending order
        [Fact]

        public void GetSortedPersons()
        {
            // Arrange
            CountryAddRequest countryAddRequest1 = new CountryAddRequest
            {
                CountryName = "India"
            };

            CountryAddRequest countryAddRequest2 = new CountryAddRequest
            {
                CountryName = "USA"
            };

            CountryResponse countryResponse1 = _countriesService.AddCountry(countryAddRequest1);
            CountryResponse countryResponse2 = _countriesService.AddCountry(countryAddRequest2);

            PersonAddRequest personAddRequest1 = new PersonAddRequest
            {
                PersonName = "Smith",
                Email = "John@example.com",
                Gender = GenderOptions.Male,
                Address = "8 Main St",
                CountryID = countryResponse1.CountryID,
                DateOfBirth = new DateTime(1990, 1, 1),
                ReceiveNewsLetters = true
            };

            PersonAddRequest personAddRequest2 = new PersonAddRequest
            {
                PersonName = "Mary",
                Email = "Smith@example.com",
                Gender = GenderOptions.Male,
                Address = "123 Main St",
                CountryID = countryResponse2.CountryID,
                DateOfBirth = new DateTime(1980, 1, 1),
                ReceiveNewsLetters = false
            };

            PersonAddRequest personAddRequest3 = new PersonAddRequest
            {
                PersonName = "Rahman",
                Email = "Mai@example.com",
                Gender = GenderOptions.Male,
                Address = "Ali Main St",
                CountryID = countryResponse2.CountryID,
                DateOfBirth = new DateTime(1980, 1, 1),
                ReceiveNewsLetters = false
            };

            List<PersonAddRequest> personAddRequests_list = new List<PersonAddRequest> { personAddRequest1, personAddRequest2, personAddRequest3 };

            List<PersonResponse> personResponses_from_get_from_add = _personsService.GetAllPersons();

            foreach (PersonAddRequest personAddRequest in personAddRequests_list)
            {
                PersonResponse personResponse = _personsService.AddPerson(personAddRequest);
                personResponses_from_get_from_add.Add(personResponse);
            }
            //printing the person responses from get before add
            _testOutput.WriteLine("Expected:");
            foreach (PersonResponse personResponse in personResponses_from_get_from_add)
            {
                _testOutput.WriteLine(personResponse.ToString());
            }
            List<PersonResponse> allPersons = _personsService.GetAllPersons();
            // Act
            List<PersonResponse> personResponses_from_Sort = _personsService.GetSortedPersons(allPersons, nameof(Person.PersonName), SortOrderOptions.DESC);
            //printing the person responses from get after add
            _testOutput.WriteLine("Actual:");
            foreach (PersonResponse personResponse in personResponses_from_Sort)
            {
                _testOutput.WriteLine(personResponse.ToString());
            }
            personResponses_from_get_from_add = personResponses_from_get_from_add.OrderByDescending(temp => temp.PersonName).ToList();

            // Assert
            for (int i = 0; i < personResponses_from_get_from_add.Count; i++)
            {
                Assert.Equal(personResponses_from_get_from_add[i], personResponses_from_Sort[i]);
            }

        }

        #endregion

        #region UpdatePerson

        //When we supply null as PersonUpdateRequest, it should throwgh ArgumentNullException

        [Fact]
        public void UpdatePerson_NullPerson()
        {
            // Arrange
            PersonUpdateRequest? personUpdateRequest = null;
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => _personsService.UpdatePerson(personUpdateRequest));
        }
        //invalid person id : when we supply invalid person id in the person update request, then it should throw ArgumentException

        [Fact]
        public void UpdatePerson_InvalidPersonID()
        {
            // Arrange
            PersonUpdateRequest? Person_update_request = new PersonUpdateRequest() {PersonID= Guid.NewGuid() };
            // Act & Assert
            Assert.Throws<ArgumentException>(() => _personsService.UpdatePerson(Person_update_request));
        }

        // When Personname is null , it should throw ArgumentException
        [Fact]
        public void UpdatePerson_PersonNameIsNull()
        {
            // Arrange
            CountryAddRequest countryAddRequest = new CountryAddRequest
            {
                CountryName = "India"
            };

            CountryResponse countryResponse =    _countriesService.AddCountry(countryAddRequest);
            PersonAddRequest personAddRequest = new PersonAddRequest
            {
                PersonName = "John Doe",
                Email = "emp@example.com",
                CountryID = countryResponse.CountryID,
                Gender = GenderOptions.Male
            };
            PersonResponse personResponse_from_add = _personsService.AddPerson(personAddRequest);




            PersonUpdateRequest? Person_update_request = personResponse_from_add.ToPersonUpdteRequest();

            Person_update_request.PersonName = null;



            // Act & Assert
            Assert.Throws<ArgumentException>(() => _personsService.UpdatePerson(Person_update_request));
        }

        //First add new PErson and try Update Person Name and Email
        [Fact]
        public void UpdatePerson_PersonFullDetailsUpdation()
        {
            // Arrange
            CountryAddRequest countryAddRequest = new CountryAddRequest
            {
                CountryName = "India"
            };

            CountryResponse countryResponse = _countriesService.AddCountry(countryAddRequest);
            PersonAddRequest personAddRequest = new PersonAddRequest
            {
                PersonName = "John Doe",
                Email = "emp@example.com",
                CountryID = countryResponse.CountryID,
                 Address = "123 Main St",
                    DateOfBirth = new DateTime(1990, 1, 1),
                    ReceiveNewsLetters = true
            };
            PersonResponse personResponse_from_add = _personsService.AddPerson(personAddRequest);




            PersonUpdateRequest? Person_update_request = personResponse_from_add.ToPersonUpdteRequest();

            Person_update_request.PersonName = "Ayman";
            Person_update_request.Email = "Willam@example.com";



            // Act 
          PersonResponse personResponse_from_Update= _personsService.UpdatePerson(Person_update_request);

        PersonResponse personResponse_from_get =    _personsService.GetPersonByPersonID(personResponse_from_Update.PersonID);

            //Assert
                        Assert.Equal(personResponse_from_get, personResponse_from_Update);
        }


        #endregion


        #region DeletePerson
        //When we supply valid personID, it should return true
        [Fact]
        public void DeletePerson_validPersonID()
        {
            // Arrange
            CountryAddRequest countryAddRequest = new CountryAddRequest
            {
                CountryName = "India"
            };
          CountryResponse countryResponse =  _countriesService.AddCountry(countryAddRequest);

            PersonAddRequest personAddRequest = new PersonAddRequest
            {
                PersonName = "John Doe",
                Email = "test@test.com",
                CountryID = countryResponse.CountryID,
                    Address = "123 Main ",
                    Gender = GenderOptions.Male
            };

            PersonResponse personResponse = _personsService.AddPerson(personAddRequest);
            // Act
            bool isDeleted = _personsService.DeletePerson(personResponse.PersonID);
            // Assert
            Assert.True(isDeleted);
        }

        //When we supply invalid personID, it should return false

        [Fact]
        public void DeletePerson_InvalidPersonID()
        {
            //Arrange


            // Act
            bool isDeleted = _personsService.DeletePerson(Guid.NewGuid());
            // Assert
            Assert.False(isDeleted);
        }
        #endregion
    }
}



