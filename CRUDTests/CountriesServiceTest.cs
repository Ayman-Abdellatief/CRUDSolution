using Entities;
using Microsoft.EntityFrameworkCore;
using ServiceContracts;
using ServiceContracts.DTO;
using Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace CRUDTests
{
    public class CountriesServiceTest
    {
        private readonly ICountriesService _countriesService;

        public CountriesServiceTest()
        {
                _countriesService = new CountriesService(new PersonsDbContext(new DbContextOptionsBuilder<PersonsDbContext>().Options));
           
        }

        [Fact]
        public async Task AddCountry_NullCountry()
        {
            //Arrange
            CountryAddRequest? request = null;

            //Assert
         await   Assert.ThrowsAsync<ArgumentNullException>(async() =>
            {
                //Act
             await   _countriesService.AddCountry(request);
            });
        }

        //When the CountryName is null, it should throw ArgumentException
        [Fact]
        public async Task AddCountry_CountryNameIsNull()
        {
            //Arrange
            CountryAddRequest? request = new CountryAddRequest() { CountryName = null };

            //Assert
          await  Assert.ThrowsAsync<ArgumentException>(async() =>
            {
                //Act
              await  _countriesService.AddCountry(request);
            });
        }


        //When the CountryName is duplicate, it should throw ArgumentException
        [Fact]
        public async Task AddCountry_DuplicateCountryName()
        {
            //Arrange
            CountryAddRequest? request1 = new CountryAddRequest() { CountryName = "USA" };
            CountryAddRequest? request2 = new CountryAddRequest() { CountryName = "USA" };

            //Assert
          await  Assert.ThrowsAsync<ArgumentException>(async() =>
            {
                //Act
               await  _countriesService.AddCountry(request1);
               await _countriesService.AddCountry(request2);
            });
        }


        //When you supply proper country name, it should insert (add) the country to the existing list of countries
        [Fact]
        public async Task AddCountry_ProperCountryDetails()
        {
            //Arrange
            CountryAddRequest? request = new CountryAddRequest() { CountryName = "Japan" };

            //Act
            CountryResponse response = await _countriesService.AddCountry(request);
            List<CountryResponse> countris_from_GetAllcountries =await _countriesService.GetAllCountries();

            //Assert
            Assert.True(response.CountryID != Guid.Empty);
            Assert.Contains(response, countris_from_GetAllcountries);
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
            Assert.Null(response);

        }

        //if you supply  countryID, it should return the corresponding country object
        [Fact]
        public async Task GetCountryByCountryID_ValidCountryID()
        {
            //Arrange
            CountryAddRequest? request = new CountryAddRequest() { CountryName = "India" };
            CountryResponse addedCountry =await _countriesService.AddCountry(request);
            Guid countryID = addedCountry.CountryID;
            //Act
            CountryResponse? response =await _countriesService.GetCountryByCountryID(countryID);
            //Assert
            Assert.NotNull(response);
            Assert.Equal(addedCountry, response);
        }

        [Fact]
        public async Task GetAllCountries_EmptyList()
        {
            //Ac
            List<CountryResponse> response =await _countriesService.GetAllCountries();
            //Assert
            Assert.Empty(response);
        }
        

        #endregion
    }
}
