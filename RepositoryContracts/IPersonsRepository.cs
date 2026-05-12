using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryContracts
{

    /// <summary>
    /// Repository contract for managing persons. This interface defines the methods for performing CRUD operations on person data.
    /// </summary>
    public interface IPersonsRepository
    {

        /// <summary>
        /// Add a new person to the repository. This method takes a Person object as input and returns the added Person object, which may include additional information such as a generated ID.
        /// </summary>
        /// <param name="person"></param>
        /// <returns></returns>
        Task<Person> AddPerson(Person person);

        /// <summary>
        /// retrieve a person by their unique identifier. This method takes a Guid representing the person's ID and returns the corresponding Person object if found, or null if no person with the given ID exists.
        /// </summary>
        /// <returns></returns>
        Task<List<Person>> GetAllPersons();


        /// <summary>
        /// Retrieves a list of persons based on their associated country ID. This method takes a Guid representing the country ID and returns a list of Person objects who belong to that country.
        /// </summary>
        /// <param name="PersonID"></param>
        /// <returns></returns>
        Task <Person?> GetPersonsByPersonID(Guid PersonID);


        /// <summary>
        /// rturns all persons that satisfy a specified condition. The condition is defined by a predicate function that takes a Person object as input and returns a boolean value indicating whether the person meets the criteria. This method allows for flexible querying of the person data based on various attributes or conditions.
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        Task<List<Person>> GetFilteredPersons(Expression<Func<Person, bool>> predicate);

        /// <summary>
        /// Updates an existing person's information in the repository. This method takes a Person object containing the updated information and returns the updated Person object after the changes have been saved to the data store. The method typically identifies the person to be updated based on a unique identifier (such as PersonID) included in the provided Person object.
        /// </summary>
        /// <param name="person"></param>
        /// <returns>Returns the updated Person object after the changes have been saved to the data store.</returns>
        Task<Person> UpdatePerson(Person person);

        /// <summary>
        /// deletes a person from the repository based on their unique identifier. This method takes a Guid representing the person's ID and removes the corresponding Person object from the data store. It returns a boolean value indicating whether the deletion was successful (true if the person was found and deleted, false otherwise).
        /// </summary>
        /// <param name="personID"></param>
        /// <returns></returns>
        Task<bool> DeletePersonByPersonID(Guid personID);



    }
}
