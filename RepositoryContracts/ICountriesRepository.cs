using Entities;
using System.Diagnostics.Metrics;

namespace RepositoryContracts
{

    /// <summary>
    /// Repository contract for managing countries. This interface defines the methods for performing CRUD operations on country data.
    /// </summary>
    public interface ICountriesRepository
    {
        /// <summary>
        /// Adds a new country to the repository
        /// </summary>
        /// <param name="country"></param>
        /// <returns></returns>
        Task<Country> AddCountry(Country country);

        Task<List<Country>> GetAllCountries();

        /// <summary>
        ///  gets a country by its unique identifier. This method retrieves the country details based on the provided country ID.
        /// </summary>
        /// <param name="countryID"></param>
        /// <returns></returns>
        Task<Country?> GetCountryById(Guid countryID);

        /// <summary>
        /// retrieves a country by its name. This method allows you to search for a country using its name and returns the corresponding country details if found.
        /// </summary>
        /// <param name="countryName"></param>
        /// <returns></returns>
        Task<Country?> GetCountryByCountryName(string countryName);
        /// <summary>
        /// Updates the details of an existing country. This method allows you to modify the information of a country based on its unique identifier.
        /// </summary>
        /// <param name="country"></param>
        /// <returns></returns>
        Task<Country?> UpdateCountry(Country country);
        /// <summary>
        /// Deletes a country from the repository based on its unique identifier. This method removes the country from the data store.
        /// </summary>
        /// <param name="countryID"></param>
        /// <returns></returns>
        Task<bool> DeleteCountry(Guid countryID);

    }
}
