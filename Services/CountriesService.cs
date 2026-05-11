using Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using ServiceContracts;
using ServiceContracts.DTO;

namespace Services
{
    public class CountriesService : ICountriesService
    {
        //privite filed field to store list of countries
        private readonly ApplicationDbContext _db;


        //constructor to initialize  countries
        public CountriesService(ApplicationDbContext personsDbContext )
        {
            _db = personsDbContext;
            

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
            if (await _db.Countries.CountAsync(c => c.CountryName== countryAddRequest.CountryName)>0)
            {
                throw new ArgumentException($"Country with name '{countryAddRequest.CountryName}' already exists", nameof(countryAddRequest.CountryName));
            }
            //convert CountryAddRequest to Country entity
            Country country =  countryAddRequest.ToCountry();

            //generate new country ID
            country.CountryID = Guid.NewGuid();

            //add country to the list
              _db.Countries.Add(country);
           await _db.SaveChangesAsync();

            //return country response
            return country.ToCountryResponse();
        }

        public async  Task<List<CountryResponse>> GetAllCountries()
        {
            //convert list of Country entities to list of CountryResponse DTOs
            return await _db.Countries.Select(c => c.ToCountryResponse()).ToListAsync();
        }

     

        public async Task<CountryResponse?> GetCountryByCountryID(Guid? countryID)
        {
            if (countryID == null)
            {
                return null;
            }
            //find country by countryID
            Country? country = await _db.Countries.FirstOrDefaultAsync(c => c.CountryID == countryID);
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
                     if(_db.Countries.Where( temp => temp.CountryName == countryName).Count() == 0)
                        {
                            Country country = new Country()
                            {
                                CountryID = Guid.NewGuid(),
                                CountryName = countryName
                            };
                            _db.Countries.Add(country);
                            await _db.SaveChangesAsync();
                            countriesInserted++;
                        }
                    }
                   
                }
            
            }

            return countriesInserted;
        }
    }
}
