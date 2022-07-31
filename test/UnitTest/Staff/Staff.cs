using AutoMapper;
using Moq;
using Staff.Controller;
using Staff.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTest.Staff
{
    public class Staff
    {
            private readonly Mock<IRepository<Employee, Guid>> repositoryMock;
            private readonly Mock<IMapper> mapperMock;
        private readonly EmployeeController controller;

        public Staff()
        {
            repositoryMock = new Mock<IRepository<Employee, Guid>>();
            mapperMock = new Mock<IMapper>();
            controller = new EmployeeController(repositoryMock.Object, mapperMock.Object);
        }

        [Fact]
        public void ShouldBe_GetAllStaff()
        {

        }
    }
}
