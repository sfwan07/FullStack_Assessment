using Staff.Core;

namespace UnitTest
{
    public class StaffTest
    {
        [Theory]
        [InlineData("First", "Middle", "Last")]
        public void Given_FirstName_And_MiddleName_And_LastName_When_Get_Full_Name(
            string firstName,
            string middleName,
            string lastName
            )
        {
            Employee employee = new Employee
            {
                FirstName = firstName,
                MiddleName = middleName,
                LastName = lastName
            };

            string actual = employee.FullName();
            string expected = $@"{firstName} {middleName} {lastName}";

            Assert.Equal(expected, actual);
        }
    }
}