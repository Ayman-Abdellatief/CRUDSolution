using Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;

using RepositoryContracts;
using ServiceContracts;
using ServiceContracts.DTO;

namespace Services
{
    public class CountriesService : ICountriesService
    {
        //privite filed  
        private readonly ICountriesRepository _CountriesRepository;


        //constructor 
        public CountriesService(ICountriesRepository countriesRepository )
        {
            _CountriesRepository = countriesRepository;
            

        }
        public async Task<CountryResponse> AddCountry(CountryAddRequest? countryAddRequest)
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
            if (await _CountriesRepository.GetCountryByCountryName(countryAddRequest.CountryName)!=null)
            {
                throw new ArgumentException($"Country with name '{countryAddRequest.CountryName}' already exists", nameof(countryAddRequest.CountryName));
            }
            //convert CountryAddRequest to Country entity
            Country country =  countryAddRequest.ToCountry();

            //generate new country ID
            country.CountryID = Guid.NewGuid();

            //add country to the list
            await  _CountriesRepository.AddCountry(country);
      

            //return country response
            return country.ToCountryResponse();
        }

        public async  Task<List<CountryResponse>> GetAllCountries()
        {

            List<Country> countries = await _CountriesRepository.GetAllCountries();
            //convert list of Country entities to list of CountryResponse DTOs


            return countries.Select(country => country.ToCountryResponse()).ToList();
        }

     

        public async Task<CountryResponse?> GetCountryByCountryID(Guid? countryID)
        {
            if (countryID == null)
                return null;
           
            //find country by countryID
            Country? country = await _CountriesRepository.GetCountryById(countryID.Value);
            //if country is found, convert to CountryResponse DTO and return
            return country?.ToCountryResponse();


        }

        public async Task<int> UploadCountriesFromExcelFile(IFormFile formfile)
        {
            MemoryStream memorystream = new MemoryStream();

            await formfile.CopyToAsync(memorystream);
            int countriesInserted = 0;
            using (ExcelPackage package = new ExcelPackage(memorystream))
            {
              ExcelWorksheet excelWorksheet =   package.Workbook.Worksheets["Countries"];

                int rowCount = excelWorksheet.Dimension.Rows;
            

                for (int row = 2; row <= rowCount; row++)
                {
                    string? countryName = excelWorksheet.Cells[row, 1].Value?.ToString();
                    if(!string.IsNullOrEmpty(countryName))
                    {
                     if(await _CountriesRepository.GetCountryByCountryName(countryName) == null)
                        {
                            Country country = new Country()
                            {
                                CountryID = Guid.NewGuid(),
                                CountryName = countryName
                            };
                            await _CountriesRepository.AddCountry(country);
                             
                            countriesInserted++;
                        }
                    }
                   
                }
            
            }

            return countriesInserted;
        }
    }
}
