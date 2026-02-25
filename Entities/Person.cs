using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    /// <summary>
    /// Person modeal model class
    /// </summary>
    public class Person
    {
        [Key]
        public Guid PersonID { get; set; }

        [StringLength(100)] //nvarchar(100)
        public string? PersonName { get; set; }
        [StringLength(50)]
        public string? Email { get; set; }
        public DateTime? DateOfBirth { get; set; }
        [StringLength(10)]
        public string? GenderID { get; set; }


        public Guid? CountryID { get; set; }

 
        public string? Gender { get; set; }
        [StringLength(300)]
        public string? Address { get; set; }

        public bool ReceiveNewsLetters { get; set; }

    }
}
