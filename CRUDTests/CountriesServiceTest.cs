using AutoFixture;
using Entities;
using EntityFrameworkCoreMock;
using Microsoft.EntityFrameworkCore;
using Moq;
using ServiceContracts;
using ServiceContracts.DTO;
using Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;


namespace CRUDTests
{
    public class CountriesServiceTest
    {
        private readonly ICountriesService _countriesService;
        private readonly IFixture _fixture;
        public CountriesServiceTest()
        {
            _fixture = new Fixture();
            var CountriesInitialData = new List<Country>() {};
            DbContextMock<ApplicationDbContext> dbContextMock = new DbContextMock<ApplicationDbContext>(new DbContextOptionsBuilder<ApplicationDbContext>().Options);

            ApplicationDbContext dbContext = dbContextMock.Object;
            dbContextMock.CreateDbSetMock(x => x.Countries, CountriesInitialData);

            _countriesService = new CountriesService(null);

        }

        [Fact]
        public async Task AddCountry_NullCountry()
        {
            //Arrange
            CountryAddRequest? request = null;

            //Assert
            Func<Task> act = async () => await _countriesService.AddCountry(request);
            await act.Should().ThrowAsync<ArgumentNullException>();

            //await   Assert.ThrowsAsync<ArgumentNullException>(async() =>
            //{
            //    //Act
            // await   _countriesService.AddCountry(request);
            //});
        }

        //When the CountryName is null, it should throw ArgumentException
        [Fact]
        public async Task AddCountry_CountryNameIsNull()
        {
            //Arrange
            CountryAddRequest? request = _fixture.Build<CountryAddRequest>()
                .With(x => x.CountryName, null as string)
                .Create();

            //Assert
            Func<Task> act = async () => await _countriesService.AddCountry(request);
            await act.Should().ThrowAsync<ArgumentException>();
            //await  Assert.ThrowsAsync<ArgumentException>(async() =>
            //{
            //    //Act
            //  await  _countriesService.AddCountry(request);
            //});
        }


        //When the CountryName is duplicate, it should throw ArgumentException
        [Fact]
        public async Task AddCountry_DuplicateCountryName()
        {
            //Arrange
            CountryAddRequest? request1 = _fixture.Build<CountryAddRequest>().With(temp => temp.CountryName ,"India").Create();
            CountryAddRequest? request2 = _fixture.Build<CountryAddRequest>().With(temp => temp.CountryName,"India").Create();

            //Assert
            Func<Task> act = async () =>
            {
                //Act
                await _countriesService.AddCountry(request1);
                await _countriesService.AddCountry(request2);
            };
            await act.Should().ThrowAsync<ArgumentException>();
            //await  Assert.ThrowsAsync<ArgumentException>(async() =>
            //{
            //    //Act
            //   await  _countriesService.AddCountry(request1);
            //   await _countriesService.AddCountry(request2);
            //});
        }


        //When you supply proper country name, it should insert (add) the country to the existing list of countries
        [Fact]
        public async Task AddCountry_ProperCountryDetails()
        {
            //Arrange
            CountryAddRequest? request = _fixture.Build<CountryAddRequest>().Create();

            //Act
            CountryResponse response = await _countriesService.AddCountry(request);
            List<CountryResponse> countris_from_GetAllcountries =await _countriesService.GetAllCountries();

            //Assert

            //Assert.True(response.CountryID != Guid.Empty);
            response.CountryID.Should().NotBe(Guid.Empty);
            //Assert.Contains(response, countris_from_GetAllcountries);
            countris_from_GetAllcountries.Should().Contain(response);
        }
        #region GetCountry by CountryID tests
        //When you supply valid countryID, it should return the corresponding country object
        [Fact]
        public async Task GetCountryByCountryID_NullCountryID()
        {
            //Arrange
            Guid? countryID = null;
            //Act
            CountryResponse? response =await _countriesService.GetCountryByCountryID(countryID);
            //Assert
          //  Assert.Null(response);
            response.Should().BeNull();

        }

        //if you supply  countryID, it should return the corresponding country object
        [Fact]
        public async Task GetCountryByCountryID_ValidCountryID()
        {
            //Arrange
            CountryAddRequest? request = _fixture.Build<CountryAddRequest>().Create();
            CountryResponse addedCountry =await _countriesService.AddCountry(request);
            Guid countryID = addedCountry.CountryID;
            //Act
            CountryResponse? response =await _countriesService.GetCountryByCountryID(countryID);
            //Assert
            //Assert.NotNull(response);
            response.Should().NotBeNull();
            //Assert.Equal(addedCountry, response);
            response.Should().BeEquivalentTo(addedCountry);
        }

        [Fact]
        public async Task GetAllCountries_EmptyList()
        {
            //Ac
            List<CountryResponse> response =await _countriesService.GetAllCountries();
            //Assert
            //Assert.Empty(response);
            response.Should().BeEmpty();
        }
        

        #endregion
    }
}
