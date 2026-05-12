using Entities;
using Microsoft.EntityFrameworkCore;
using RepositoryContracts;

namespace Repositories
{
    public class CountriesRepository : RepositoryContracts.ICountriesRepository
    {

        private readonly Entities.ApplicationDbContext _db;

        public CountriesRepository(Entities.ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<Country> AddCountry(Country country)
        {
           _db.Countries.Add(country);
           await _db.SaveChangesAsync();
           return country;
        }


        public async Task<List<Country>> GetAllCountries()
        {
            return await _db.Countries.ToListAsync();
        }

        public async Task<Country?> GetCountryById(Guid countryID)
        {
          return await _db.Countries.FirstOrDefaultAsync(temp => temp.CountryID == countryID);
        }


        public async Task<Country?> GetCountryByCountryName(string countryName)
        {
           return await _db.Countries.FirstOrDefaultAsync(temp => temp.CountryName == countryName);
        }



        public  Task<bool> DeleteCountry(Guid countryID)
        {
            throw new NotImplementedException();
        }


     


        public Task<Country?> UpdateCountry(Country country)
        {
            throw new NotImplementedException();
        }
    }
}
