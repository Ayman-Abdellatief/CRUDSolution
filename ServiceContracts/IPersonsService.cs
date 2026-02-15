using System;
using System.Collections.Generic;
using ServiceContracts.DTO;
using ServiceContracts.Enums;

namespace ServiceContracts
{
    public interface IPersonsService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="personAddRequest"></param>
        /// <returns></returns>
        PersonResponse AddPerson(PersonAddRequest? personAddRequest);

        /// <summary>
        /// Retrieves a list of all persons available in the system.
        /// </summary>
        /// <returns>A list of <see cref="PersonResponse"/> objects representing all persons. The list will be empty if no
        /// persons are found.</returns>
        List<PersonResponse> GetAllPersons();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="personID"></param>
        /// <returns></returns>
        PersonResponse? GetPersonByPersonID(Guid personID);

        /// <summary>
        ///   Retrieves a list of persons based on the specified search criteria. The search can be performed by either "PersonName" or "Email".
        /// </summary>
        /// <param name="searchBy"></param>
        /// <param name="searchString"></param>
        /// <returns></returns>
        List<PersonResponse>   GetFilteredPersons(string? searchBy, string? searchString);


        /// <summary>
        /// Retrieves a list of persons sorted based on the specified sorting criteria. The sorting can be performed by either "PersonName" or "Email", and the sort order can be ascending (ASC) or descending (DESC).
        /// </summary>
        /// <param name="allPersons"></param>
        /// <param name="sortBy"></param>
        /// <param name="sortOrder"></param>
        /// <returns></returns>
        List<PersonResponse> GetSortedPersons(List<PersonResponse> allPersons, string? sortBy,SortOrderOptions sortOrder);

        /// <summary>
        /// Uodate the  spcified Person details based on the provided information in the PersonUpdateRequest object. The update operation will modify the existing person's details with the new values provided in the request. If the person with the specified ID is found, their details will be updated accordingly, and a PersonResponse object containing the updated information will be returned. If the person is not found, an appropriate response indicating that the update was unsuccessful may be returned (e.g., null or an error message).
        /// </summary>
        /// <param name="personUpdateRequest"></param>
        /// <returns></returns>
        PersonResponse UpdatePerson(PersonUpdateRequest? personUpdateRequest);

        /// <summary>
        ///  
        /// </summary>
        /// <param name="personID"></param>
        /// <returns></returns>
       bool DeletePerson(Guid? personID);
    }
}
