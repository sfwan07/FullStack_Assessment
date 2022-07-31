using System.ComponentModel.DataAnnotations;

namespace Staff.ViewModel
{
    public class EmployeeViewModel
    {
        public Guid Id { get; set; }
        public string FullName { get; set; }
        public int Age { get; set; }
    }
    public class EmployeeCreateModel
    {
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string MiddleName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public int Age { get; set; }
    }
    public class EmployeeUpdateModel : EmployeeCreateModel
    {
        [Required]
        public Guid Id { get; set; }
    }
    public class EmployeeDeleteModel
    {
        [Required]
        public Guid Id { get; set; }
    }
}
