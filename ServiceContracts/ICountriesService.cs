using Microsoft.AspNetCore.Http;
using ServiceContracts.DTO;

namespace ServiceContracts
{

    /// <summary>
    /// Represent business logic for manipulating Country entity
    /// </summary>
    public interface ICountriesService
    {

        /// <summary>
        /// Add a country object to the list of countries
        /// </summary>
        /// <param name="countryAddRequest"></param>
        /// <returns></returns>
       Task<CountryResponse> AddCountry(CountryAddRequest? countryAddRequest);

       Task<List<CountryResponse>> GetAllCountries();

        ///get country by country ID
        
       Task <CountryResponse?> GetCountryByCountryID(Guid? countryID);

        /// <summary>
        ///  Uploafd  a 
        /// </summary>
        /// <param name="formfile"></param>
        /// <returns></returns>
        Task<int> UploadCountriesFromExcelFile(IFormFile formfile);
    }
}
