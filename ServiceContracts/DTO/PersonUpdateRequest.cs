using Entities;
using ServiceContracts.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceContracts.DTO
{
    public class PersonUpdateRequest
    {
        [Required(ErrorMessage ="Person ID can't be blank")]
        public Guid PersonID { get; set; }
        [Required(ErrorMessage = "Person Name Can't be blank")]
        public string? PersonName { get; set; }
        [Required(ErrorMessage = "Person Email Can't be blank")]
        [EmailAddress(ErrorMessage = "Email value should be valid")]
        public string? Email { get; set; }
        public DateTime? DateOfBirth { get; set; }

        public GenderOptions? Gender { get; set; }

        public Guid? CountryID { get; set; }

        public string? Address { get; set; }

        public bool ReceiveNewsLetters { get; set; }


        /// <summary>
        /// Converts the current object to a new instance of the Person class, copying relevant property values.
        /// </summary>
        /// <returns>A new Person object populated with the corresponding values from the current instance.</returns>
        public Person ToPerson()
        {
            return new Person()
            {
                PersonID = this.PersonID,
                PersonName = this.PersonName,
                Email = this.Email,
                DateOfBirth = this.DateOfBirth,
                Gender = this.Gender?.ToString(),
                Address = this.Address,
                CountryID = this.CountryID,
                ReceiveNewsLetters = this.ReceiveNewsLetters
            };
        }
    }
}
