using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Staff.Core;
using Staff.ViewModel;

namespace Staff.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly IRepository<Employee, Guid> _repository;
        private readonly IMapper _mapper;

        public EmployeeController(IRepository<Employee, Guid> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        // GET: api/<EmployeeController>
        [HttpGet]
        public IAsyncEnumerable<Employee> Get()
        {
            return _repository.All();
        }

        // GET api/<EmployeeController>/5
        [HttpGet("{id}")]
        public async Task<EmployeeViewModel> Get(Guid id)
        {
            return _mapper.Map<EmployeeViewModel>(await _repository.FindById(id));
        }

        // POST api/<EmployeeController>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] EmployeeCreateModel model)
        {
            try
            {
                await _repository.Add(_mapper.Map<Employee>(model));
                return StatusCode(201);
            }
            catch (Exception e)
            {
                return StatusCode(406);
            }
        }

        // PUT api/<EmployeeController>/5
        [HttpPut]
        public async Task<IActionResult> Put([FromBody] EmployeeUpdateModel model)
        {
            try
            {
                await _repository.Update(_mapper.Map<Employee>(model));
                return StatusCode(200);
            }
            catch (Exception)
            {
                return StatusCode(406);
            }
        }

        // DELETE api/<EmployeeController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                await _repository.Delete(id);
                return StatusCode(200);
            }
            catch (Exception)
            {
                return StatusCode(406);
            }
        }
    }
}
