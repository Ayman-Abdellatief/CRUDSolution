using Entities;
using ServiceContracts;
using ServiceContracts.DTO;

namespace Services
{
    public class CountriesService : ICountriesService
    {
        //privite filed field to store list of countries
        private readonly List<Country> _countries;

        public CountriesService(bool initialize =true)
        {
            _countries = new List<Country>();
            if (initialize)
            {

                //add some initial countries to the list
                _countries.AddRange(new List<Country>() { new Country { CountryID = Guid.Parse("85289649-599B-4387-B32A-79C501387302"), CountryName = "India" },
                new Country { CountryID = Guid.Parse("D5409333-E453-47C9-A2B8-6D5C0439FE8D"), CountryName = "USA" },
                new Country { CountryID = Guid.Parse("FA22B4FC-A720-490B-939D-B72A7EAD8BE1"), CountryName = "UK" },
                new Country { CountryID = Guid.Parse("95776507-9DA9-44F5-B106-F7ACE2E474FC"), CountryName = "Australia" },
                new Country { CountryID = Guid.Parse("852788A1-66D7-4ADF-9001-32FD9E6E2944"), CountryName = "Germany" }
                });
            }

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
