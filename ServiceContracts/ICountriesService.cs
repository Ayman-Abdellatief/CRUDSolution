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
        CountryResponse AddCountry(CountryAddRequest? countryAddRequest);

        List<CountryResponse> GetAllCountries();

        ///get country by country ID
        
        CountryResponse? GetCountryByCountryID(Guid? countryID);
    }
}
