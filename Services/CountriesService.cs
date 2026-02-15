using Entities;
using ServiceContracts;
using ServiceContracts.DTO;

namespace Services
{
    public class CountriesService : ICountriesService
    {
        //privite filed field to store list of countries
        private readonly List<Country> _countries;

        public CountriesService()
        {
            _countries = new List<Country>();
        }
        public CountryResponse AddCountry(CountryAddRequest? countryAddRequest)
        {
            //validation: countryAddRequest should not be null
            if (countryAddRequest == null)
            {
                throw new ArgumentNullException(nameof(countryAddRequest), "Country add request cannot be null");
            }

            //validation: CountryName should not be null or empty
            if (string.IsNullOrEmpty(countryAddRequest.CountryName))
            {
                throw new ArgumentException("Country name cannot be null or empty", nameof(countryAddRequest.CountryName));
            }

            //country name should be unique
            if (_countries.Any(c => c.CountryName?.Equals(countryAddRequest.CountryName, StringComparison.OrdinalIgnoreCase) == true))
            {
                throw new ArgumentException($"Country with name '{countryAddRequest.CountryName}' already exists", nameof(countryAddRequest.CountryName));
            }
            //convert CountryAddRequest to Country entity
            Country country =  countryAddRequest.ToCountry();

            //generate new country ID
            country.CountryID = Guid.NewGuid();

            //add country to the list
            _countries.Add(country);

            //return country response
            return country.ToCountryResponse();
        }

        public List<CountryResponse> GetAllCountries()
        {
            //convert list of Country entities to list of CountryResponse DTOs
            return _countries.Select(c => c.ToCountryResponse()).ToList();
        }

     

        public CountryResponse? GetCountryByCountryID(Guid? countryID)
        {
            if (countryID == null)
            {
                return null;
            }
            //find country by countryID
            Country? country = _countries.FirstOrDefault(c => c.CountryID == countryID);
            //if country is found, convert to CountryResponse DTO and return
            return country?.ToCountryResponse();


        }
    }
}
