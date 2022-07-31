using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Staff.Core
{
    public class Employee : BaseEntity<Guid>
    {
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string MiddleName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public int Age { get; set; }

        public string FullName()
        {
            return @$"{FirstName} {MiddleName} {LastName}";
        }
    }
}
