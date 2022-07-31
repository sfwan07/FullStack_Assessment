using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Staff;
using Staff.Controller;
using Staff.Core;
using Staff.ViewModel;

namespace UnitTest
{
    public class StaffIntegrationTest
    {
        private readonly List<Employee> _employees = Enumerable.Range(1, 15).Select(s => new Employee
        {
            FirstName = $"first ${s}",
            MiddleName = $"middle ${s}",
            LastName = $"last ${s}",
            Age = s * 3
        }).ToList();

        private readonly List<EmployeeViewModel> _employeeViews;

        private readonly IMapper _mapper;

        public StaffIntegrationTest()
        {
            _employeeViews = _employees.Select(s => new EmployeeViewModel
            {
                Id = s.Id,
                FullName = s.FullName(),
                Age = s.Age
            }).ToList();
            if (_mapper == null)
            {
                _mapper = new MapperConfiguration(mc =>
                {
                    mc.AddProfile(new StaffProfile());
                }).CreateMapper();
            }
        }

        [Fact]
        public async Task When_Get_ALL_For_Employee_Excepted_NotHasData()
        {
            Mock<IRepository<Employee, Guid>> repositoryMock = new Mock<IRepository<Employee, Guid>>();

            EmployeeController controller = new EmployeeController(
                repositoryMock.Object,
                _mapper
            );

            List<EmployeeViewModel> actual = await controller.Get();

            Assert.Equal(0, actual.Count);
        }

        [Fact]
        public async Task When_Get_ALL_For_Employee_Excepted_HasData()
        {
            Mock<IRepository<Employee, Guid>> repositoryMock = new Mock<IRepository<Employee, Guid>>();

            repositoryMock.Setup(s => s.All(default)).Returns(Task.FromResult(_employees));
            EmployeeController controller = new EmployeeController(
                repositoryMock.Object,
                _mapper
            );

            List<EmployeeViewModel> actual = await controller.Get();

            Assert.NotEqual(0, actual.Count);
        }

        [Theory]
        [InlineData("first", "middle", "last", 50)]
        public async Task GiveData_When_Add_For_Employee_Excepted_201(
                string firstName,
                string middelName,
                string lastName,
                int age
            )
        {
            Mock<IRepository<Employee, Guid>> repositoryMock = new Mock<IRepository<Employee, Guid>>();
            EmployeeCreateModel model = new EmployeeCreateModel
            {
                FirstName = firstName,
                MiddleName = middelName,
                LastName = lastName,
                Age = age
            };

            repositoryMock.Setup(s => s.Add(_mapper.Map<Employee>(model), default))
                .Returns(Task.CompletedTask);
            EmployeeController controller = new EmployeeController(
                repositoryMock.Object,
                _mapper
            );

            IActionResult result = await controller.Post(model);
            dynamic actual = result ;

            //Assert.NotNull(actual);
            Assert.Equal(201, actual.StatusCode);
        }

        [Theory]
        [InlineData("first", "middle", "last", 50)]
        public async Task GiveData_When_Update_For_Employee_Excepted_200(
                string firstName,
                string middelName,
                string lastName,
                int age
            )
        {
            Mock<IRepository<Employee, Guid>> repositoryMock = new Mock<IRepository<Employee, Guid>>();
            EmployeeUpdateModel model = new EmployeeUpdateModel
            {
                FirstName = firstName,
                MiddleName = middelName,
                LastName = lastName,
                Age = age
            };

            repositoryMock.Setup(s => s.Update(_mapper.Map<Employee>(model), default))
                .Returns(Task.CompletedTask);
            EmployeeController controller = new EmployeeController(
                repositoryMock.Object,
                _mapper
            );

            IActionResult result = await controller.Put(model);
            dynamic actual = result;

            //Assert.NotNull(actual);
            Assert.Equal(200, actual.StatusCode);
        }

        [Fact]
        public async Task GiveRowndomId_When_Delete_For_Employee_Excepted_200()
        {
            Mock<IRepository<Employee, Guid>> repositoryMock = new Mock<IRepository<Employee, Guid>>();
            EmployeeDeleteModel model = new EmployeeDeleteModel
            {
                Id = Guid.NewGuid(),
            };

            repositoryMock.Setup(s => s.Delete(model.Id, default))
                .Returns(Task.CompletedTask);
            EmployeeController controller = new EmployeeController(
                repositoryMock.Object,
                _mapper
            );

            IActionResult result = await controller.Delete(model.Id);
            dynamic actual = result;

            //Assert.NotNull(actual);
            Assert.Equal(200, actual.StatusCode);
        }
    }
}