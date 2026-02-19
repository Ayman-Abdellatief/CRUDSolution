using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using ServiceContracts.Enums;

namespace ServiceContracts.DTO
{

    /// <summary>
    /// Represents the response containing information about a person.
    /// </summary>
    public class PersonResponse
    {
      
        public Guid PersonID { get; set; }

   
        public string? PersonName { get; set; }
        public string? Email { get; set; }
        public DateTime? DateOfBirth { get; set; }

        public string? GenderID { get; set; }
        public string? Gender { get; set; }

        public Guid? CountryID { get; set; }

        public string? Country { get; set; }

        public string? Address { get; set; }

        public bool ReceiveNewsLetters { get; set; }

        public double? Age { get; set; }

        public override bool Equals(object? obj)
        {
          if(obj == null)
                return false;
            if(obj.GetType() != typeof(PersonResponse))
                return false;
            PersonResponse person = (PersonResponse)obj;
            return (PersonID == person.PersonID) &&
                   (PersonName == person.PersonName) &&
                   (Email == person.Email) &&
                   (DateOfBirth == person.DateOfBirth) &&
                   (GenderID == person.GenderID) &&
                   (CountryID == person.CountryID) &&
                   (Country == person.Country) &&
                   (Address == person.Address) &&
                   (ReceiveNewsLetters == person.ReceiveNewsLetters) &&
                   (Age == person.Age);


        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override  string ToString()
        {
            return $"PersonID: {PersonID}, PersonName: {PersonName}, Email: {Email}, DateOfBirth: {DateOfBirth}, GenderID: {GenderID}, CountryID: {CountryID}, Country: {Country}, Address: {Address}, ReceiveNewsLetters: {ReceiveNewsLetters}, Age: {Age}";
        }

        public PersonUpdateRequest ToPersonUpdteRequest()
        {
            GenderOptions genderEnum = default;

            if (!string.IsNullOrEmpty(Gender) &&
                Enum.TryParse(Gender, true, out GenderOptions parsedGender))
            {
                genderEnum = parsedGender;
            }

            return new PersonUpdateRequest()
            {
                PersonID = this.PersonID,
                PersonName = this.PersonName,
                Email = this.Email,
                DateOfBirth = this.DateOfBirth,
                Address = this.Address,
                Gender = genderEnum,
                CountryID = this.CountryID,
                ReceiveNewsLetters = this.ReceiveNewsLetters
            };
        }
    }

    public static class PersonExtensions
    {

        /// <summary>
        /// An extension method to convert Person entity to PersonResponse DTO
        /// </summary>
        /// <param name="person"></param>
        /// <returns></returns>
        public static PersonResponse ToPersonResponse(this Person person)
        {
            return new PersonResponse()
            {
                PersonID = person.PersonID,
                PersonName = person.PersonName,
                Email = person.Email,
                DateOfBirth = person.DateOfBirth,
                GenderID = person.GenderID,
                Gender = person.Gender,
                CountryID = person.CountryID,
                Country =person.Country,
                Address = person.Address,
                ReceiveNewsLetters = person.ReceiveNewsLetters,
                Age = person.DateOfBirth.HasValue ? Math.Round((DateTime.Now - person.DateOfBirth.Value).TotalDays / 365.25, 1) : null
            };
        }
    }
}
