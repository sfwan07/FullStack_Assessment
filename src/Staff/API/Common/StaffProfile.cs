using AutoMapper;
using Staff.Core;
using Staff.ViewModel;

namespace Staff
{
    public class StaffProfile : Profile
    {
        public StaffProfile()
        {
            CreateMap<Employee, EmployeeViewModel>()
                .ForMember(d => d.FullName, opt => opt.MapFrom(s => s.FullName()));

            CreateMap<EmployeeCreateModel, Employee>();

            CreateMap<EmployeeUpdateModel, Employee>();
        }

    }
}
