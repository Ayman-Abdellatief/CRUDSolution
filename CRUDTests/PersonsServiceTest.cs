using Entities;
using EntityFrameworkCoreMock;
using Microsoft.EntityFrameworkCore;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;
using Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using AutoFixture;
using FluentAssertions;
using RepositoryContracts;
using Moq;
using System.Linq.Expressions;



namespace CRUDTests
{
    public class PersonsServiceTest
    {
        private readonly IPersonsService _personsService;

        private  readonly Mock<IPersonsRepository> _personsRepositoryMock;
        private readonly IPersonsRepository _personsRepository;
        private readonly ITestOutputHelper _testOutput;
        private readonly IFixture _fixture;
        //test commit to check the git integration with github
        public PersonsServiceTest(ITestOutputHelper testOutput)
        {
            _fixture = new Fixture();
            _personsRepositoryMock = new Mock<IPersonsRepository>();
            _personsRepository = _personsRepositoryMock.Object;

            _personsService = new PersonsService(_personsRepository);
       
            _testOutput = testOutput;
        }
        [Fact]
        //when we supply null or empty values for mandatory fields, then the service should throw an exception
        public async Task AddPerson_NullPerson_ToBeArgumentNullException()
        {
            // Arrange
            PersonAddRequest? personAddRequest = null;
            // Act & Assert

            Func<Task> act = async () => await _personsService.AddPerson(personAddRequest);

        await   act.Should().ThrowAsync<ArgumentNullException>();
            //await    Assert.ThrowsAsync<ArgumentNullException>(async () =>await _personsService.AddPerson(personAddRequest!));
        }

        //When we supply valid values for all mandatory fields, then the service should add the person and return the added person details
        [Fact]
        public async Task AddPerson_PersonNameISNull_ToBeArgumentException()
        {
            // Arrange
            PersonAddRequest personAddRequest = _fixture.Build<PersonAddRequest>()
                .With(temp => temp.PersonName,  null as string)
                .Create();

            Person person = personAddRequest.ToPerson();

            //when PersonRepository.AddPerson is called, it should return the same person object
            _personsRepositoryMock
                .Setup(temp => temp.AddPerson(It.IsAny<Person>()))
                .ReturnsAsync(person);

            // Act & Assert
            Func<Task> act = async () => await _personsService.AddPerson(personAddRequest);


             await   act.Should().ThrowAsync<ArgumentException>();
          

            await Assert.ThrowsAsync<ArgumentException>(async() => await _personsService.AddPerson(personAddRequest));
        }

        //when we supply persondetails, it should insert the persob into persons list;and should return an object of personResponse , which includes with the newly generated person id
        [Fact]
        public async Task AddPerson_FullPersonDetails_ToBeSuccessful()
        {
            // Arrange
            PersonAddRequest personAddRequest = _fixture.Build<PersonAddRequest>()
             .With(temp => temp.Email, "example@example.com")
             .Create();

            Person person = personAddRequest.ToPerson();
            PersonResponse Person_response_expexted = person.ToPersonResponse();

            _personsRepositoryMock.Setup(temp => temp.AddPerson(It.IsAny<Person>()))
                .ReturnsAsync(person);
            // Act
            PersonResponse personResponse_from_add =await _personsService.AddPerson(personAddRequest);
            Person_response_expexted.PersonID = personResponse_from_add.PersonID;


            // Assert
            // Assert.True(personResponse_from_add.PersonID != Guid.Empty);
            personResponse_from_add.PersonID.Should().NotBe(Guid.Empty);

            personResponse_from_add.Should().Be(Person_response_expexted);

        }


        #region GetPersonByPersonID
        //If we supply null as personID, it should return null as PersonReponse

        [Fact]
        public async Task GetPersonByPersonID_NullPersonID_ToBeNull()
        {                      // Arrange
            Guid personID = Guid.Empty;

            // Act
            PersonResponse? personResponse =await _personsService.GetPersonByPersonID(personID);

            // Assert
            //Assert.Null(personResponse);
            personResponse.Should().BeNull();
        }

        //IF we supply a valid person id ,it should return the valid person details as person response object
        [Fact]
        public async Task GetPersonByPersonID_withPersonID_ToBeSucessful()
        {

            // Arrange
        


            // Act
            Person person = _fixture.Build<Person>()
                .With(temp => temp.Email, "example@example.com")
                .With(temp => temp.Country , null as Country).Create();

            PersonResponse person_respones_expected = person.ToPersonResponse();

         _personsRepositoryMock.Setup(temp => temp.GetPersonsByPersonID(It.IsAny<Guid>()))
                .ReturnsAsync(person);



            PersonResponse? personResponse_from_get =await _personsService.GetPersonByPersonID(person.PersonID);

            // Assert
    

            personResponse_from_get.Should().Be(person_respones_expected);
        }


        #endregion

        #region GetAllPersons

        //The GetAllPersons method should return a list of all persons added to the service

        [Fact]
        public async Task GetAllPersons_ToBeEmptyList()
        {

            //Arange

            var personsList = new List<Person>();

            _personsRepositoryMock.Setup(temp => temp.GetAllPersons())
                .ReturnsAsync(personsList);
            // Act

            List<PersonResponse> personResponses_from_get =await _personsService.GetAllPersons();


            // Assert
            //Assert.Empty(personResponses_from_get);
            personResponses_from_get.Should().BeEmpty();
        }
        //first ,we will add few persons;and then when we call get AllPersons() , it should return a list of all added persons
        [Fact]
        public async Task GetAllPersons_WithFewPersons_ToBeSuccessful() {
            // Arrange
          
            List<Person> personsList = new List<Person>()
            {
                _fixture.Build<Person>()
                 .With(temp => temp.Email, "example_1@example.com")
                 .With(temp => temp.Country, null as Country)
                 .Create(),

                  _fixture.Build<Person>()
                 .With(temp => temp.Email, "example_2@example.com")
                 .With(temp => temp.Country, null as Country)
                 .Create(),

                  _fixture.Build<Person>()
                 .With(temp => temp.Email, "example_3@example.com")
                 .With(temp => temp.Country, null as Country)
                 .Create()

            };

 

            List<PersonResponse> personResponses_list_expected = personsList.Select(temp => temp.ToPersonResponse()).ToList();

          
            //printing the person responses from get before add
            _testOutput.WriteLine("Expected:");
            foreach (PersonResponse personResponse in personResponses_list_expected)
            {
                _testOutput.WriteLine(personResponse.ToString());
            }
            _personsRepositoryMock.Setup(temp => temp.GetAllPersons())
                .ReturnsAsync(personsList);
            // Act
            List<PersonResponse> personResponses_from_get = await _personsService.GetAllPersons();
            //printing the person responses from get after add
            _testOutput.WriteLine("Actual:");
            foreach (PersonResponse personResponse in personResponses_from_get)
            {
                _testOutput.WriteLine(personResponse.ToString());   
            }

            // Assert


            personResponses_from_get.Should().BeEquivalentTo(personResponses_list_expected);
        }
        #endregion

        #region GetFilteredPersons

        //If the search tesxt is empty and search by is PersonName, then it should return all the persons as per GetAllPersons method
        [Fact]
        public async Task GetFilteredPersons_EmptySearchText_ToBeSuccessful()
        {
            // Arrange
            List<Person> personsList = new List<Person>()
            {
                _fixture.Build<Person>()
                 .With(temp => temp.Email, "example_1@example.com")
                 .With(temp => temp.Country, null as Country)
                 .Create(),

                  _fixture.Build<Person>()
                 .With(temp => temp.Email, "example_2@example.com")
                 .With(temp => temp.Country, null as Country)
                 .Create(),

                  _fixture.Build<Person>()
                 .With(temp => temp.Email, "example_3@example.com")
                 .With(temp => temp.Country, null as Country)
                 .Create()

            };

            List<PersonResponse> person_response_list_expected = personsList.Select(temp => temp.ToPersonResponse()).ToList();

            //printing the person responses from get before add
            _testOutput.WriteLine("Expected:");
            foreach (PersonResponse personResponse in person_response_list_expected)
            {
                _testOutput.WriteLine(personResponse.ToString());
            }
            _personsRepositoryMock.Setup(temp => temp.GetFilteredPersons(It.IsAny<Expression<Func<Person,bool>>>())).ReturnsAsync(personsList);
            // Act
            List<PersonResponse> personResponses_from_search =await _personsService.GetFilteredPersons(nameof(Person.PersonName),"");
            //printing the person responses from get after add
            _testOutput.WriteLine("Actual:");
            foreach (PersonResponse personResponse in personResponses_from_search)
            {
                _testOutput.WriteLine(personResponse.ToString());
            }
            // Assert
        
            personResponses_from_search.Should().BeEquivalentTo(person_response_list_expected);
        }


        //Add few Persons and then we will search based on Person Name

        [Fact]
        public async Task GetFilteredPersons_SearchByPersonName_ToBeSuccessful()
        {
            // Arrange
            List<Person> personsList = new List<Person>()
            {
                _fixture.Build<Person>()
                 .With(temp => temp.Email, "example_1@example.com")
                 .With(temp => temp.Country, null as Country)
                 .Create(),

                  _fixture.Build<Person>()
                 .With(temp => temp.Email, "example_2@example.com")
                 .With(temp => temp.Country, null as Country)
                 .Create(),

                  _fixture.Build<Person>()
                 .With(temp => temp.Email, "example_3@example.com")
                 .With(temp => temp.Country, null as Country)
                 .Create()

            };

            List<PersonResponse> person_response_list_expected = personsList.Select(temp => temp.ToPersonResponse()).ToList();

            //printing the person responses from get before add
            _testOutput.WriteLine("Expected:");
            foreach (PersonResponse personResponse in person_response_list_expected)
            {
                _testOutput.WriteLine(personResponse.ToString());
            }
            _personsRepositoryMock.Setup(temp => temp.GetFilteredPersons(It.IsAny<Expression<Func<Person, bool>>>())).ReturnsAsync(personsList);
            // Act
            List<PersonResponse> personResponses_from_search = await _personsService.GetFilteredPersons(nameof(Person.PersonName), "Sa");
            //printing the person responses from get after add
            _testOutput.WriteLine("Actual:");
            foreach (PersonResponse personResponse in personResponses_from_search)
            {
                _testOutput.WriteLine(personResponse.ToString());
            }
            // Assert

            personResponses_from_search.Should().BeEquivalentTo(person_response_list_expected);

        }
        #endregion

        #region GetSortedPersons
        //when sort based on PersonName in descending order, then it should return the list of persons sorted based on person name in descending order
        [Fact]

        public async Task GetSortedPersons_ToBeSuccessful()
        {
            // Arrange
            List<Person> personsList = new List<Person>()
            {
                _fixture.Build<Person>()
                 .With(temp => temp.Email, "example_1@example.com")
                 .With(temp => temp.Country, null as Country)
                 .Create(),

                  _fixture.Build<Person>()
                 .With(temp => temp.Email, "example_2@example.com")
                 .With(temp => temp.Country, null as Country)
                 .Create(),

                  _fixture.Build<Person>()
                 .With(temp => temp.Email, "example_3@example.com")
                 .With(temp => temp.Country, null as Country)
                 .Create()

            };

            List<PersonResponse> person_response_list_expected = personsList.Select(temp => temp.ToPersonResponse()).ToList();

            _personsRepositoryMock.Setup(temp => temp.GetAllPersons()).ReturnsAsync(personsList);


           
            //printing the person responses from get before add
            _testOutput.WriteLine("Expected:");
            foreach (PersonResponse personResponse in person_response_list_expected)
            {
                _testOutput.WriteLine(personResponse.ToString());
            }
            List<PersonResponse> allPersons =await _personsService.GetAllPersons();
            // Act
            List<PersonResponse> personResponses_from_Sort =await _personsService.GetSortedPersons(allPersons, nameof(Person.PersonName), SortOrderOptions.DESC);
            //printing the person responses from get after add
            _testOutput.WriteLine("Actual:");
            foreach (PersonResponse personResponse in personResponses_from_Sort)
            {
                _testOutput.WriteLine(personResponse.ToString());
            }
            //personResponses_from_get_from_add = personResponses_from_get_from_add.OrderByDescending(temp => temp.PersonName).ToList();
            personResponses_from_Sort.Should().BeInDescendingOrder(temp => temp.PersonName);
            // Assert
            //for (int i = 0; i < personResponses_from_get_from_add.Count; i++)
            //{
            //    Assert.Equal(personResponses_from_get_from_add[i], personResponses_from_Sort[i]);
            //}

            personResponses_from_Sort.Should().BeInDescendingOrder(temp => temp.PersonName);

        }

        #endregion

        #region UpdatePerson

        //When we supply null as PersonUpdateRequest, it should throwgh ArgumentNullException

        [Fact]
        public async Task UpdatePerson_NullPerson_ToBeArgumentNullException()
        {
            // Arrange
            PersonUpdateRequest? personUpdateRequest = null;
            // Act & Assert
            Func<Task> act = async () => await _personsService.UpdatePerson(personUpdateRequest);
             await   act.Should().ThrowAsync<ArgumentNullException>();
            //await   Assert.ThrowsAsync<ArgumentNullException>(async () =>await _personsService.UpdatePerson(personUpdateRequest));
        }
        //invalid person id : when we supply invalid person id in the person update request, then it should throw ArgumentException

        [Fact]
        public async Task UpdatePerson_InvalidPersonID_ToBeArgumentException()
        {
            // Arrange
            PersonUpdateRequest? Person_update_request = _fixture.Build<PersonUpdateRequest>()
                .Create();
            // Act & Assert

            Func<Task> act = async () => await _personsService.UpdatePerson(Person_update_request);
             await   act.Should().ThrowAsync<ArgumentException>();
            //await   Assert.ThrowsAsync<ArgumentException>(async() => await _personsService.UpdatePerson(Person_update_request));
        }

        // When Personname is null , it should throw ArgumentException
        [Fact]
        public async Task UpdatePerson_PersonNameIsNull_ToBeArgumentException()
        {
            // Arrange
           
           

            Person person = _fixture.Build<Person>()
                .With(temp => temp.PersonName, null as string)
                .With(temp => temp.Country, null as Country)
                .With(temp => temp.Email, "Someone@example.com")
                 .With(temp => temp.Gender, "Male")
                 .Create();

         

            PersonResponse personResponse_from_add = person.ToPersonResponse();

            PersonUpdateRequest? Person_update_request = personResponse_from_add.ToPersonUpdteRequest();

         


            Func<Task> act = async () => await _personsService.UpdatePerson(Person_update_request);
            // Act & Assert
                await   act.Should().ThrowAsync<ArgumentException>();
            //await  Assert.ThrowsAsync<ArgumentException>(async () =>await _personsService.UpdatePerson(Person_update_request));
        }

        //First add new PErson and try Update Person Name and Email
        [Fact]
        public async Task UpdatePerson_PersonFullDetails_ToBeSuccessful()
        {
            // Arrange
       

          

            Person person = _fixture.Build<Person>()
                .With(temp => temp.PersonName, "Smith")
                .With(temp => temp.Country, null as Country)
                .With(temp => temp.Email, "Someone@example.com")
                .With(temp => temp.Gender, "Male")
                .Create();

            PersonResponse person_rsponse_expected = person.ToPersonResponse();




            PersonUpdateRequest? Person_update_request = person_rsponse_expected.ToPersonUpdteRequest();

            _personsRepositoryMock.Setup(temp => temp.UpdatePerson(It.IsAny<Person>()))
                .ReturnsAsync(person);
            _personsRepositoryMock.Setup(temp => temp.GetPersonsByPersonID(It.IsAny<Guid>())).ReturnsAsync(person);


            // Act 
          PersonResponse personResponse_from_Update=await _personsService.UpdatePerson(Person_update_request);

 

            //Assert
                        //Assert.Equal(personResponse_from_get, personResponse_from_Update);

            personResponse_from_Update.Should().Be(person_rsponse_expected);
        }


        #endregion


        #region DeletePerson
        //When we supply valid personID, it should return true
        [Fact]
        public async Task DeletePerson_validPersonID()
        {
            // Arrange
     


            Person person = _fixture.Build<Person>()
                .With(temp => temp.Country,  null as Country)
                .With(temp => temp.Email, "Someone@example.com")
                .With(temp => temp.Gender, "Female")
                .Create();

            _personsRepositoryMock.Setup(temp => temp.DeletePersonByPersonID(It.IsAny<Guid>()))
                .ReturnsAsync(true);

            _personsRepositoryMock.Setup(temp => temp.GetPersonsByPersonID(It.IsAny<Guid>()))
                .ReturnsAsync(person);

            bool isDeleted =await _personsService.DeletePerson(person.PersonID);
            // Assert
            //Assert.True(isDeleted);

            isDeleted.Should().BeTrue();
        }

        //When we supply invalid personID, it should return false

        [Fact]
        public async Task DeletePerson_InvalidPersonID()
        {
            //Arrange


            // Act
            bool isDeleted =await _personsService.DeletePerson(Guid.NewGuid());
            // Assert
            //Assert.False(isDeleted);
            isDeleted.Should().BeFalse();
        }
        #endregion
    }
}



