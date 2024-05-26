using AcmeCorp.Application.DTOs;
using AcmeCorp.Application.Interfaces;
using AcmeCorp.Domain.Entities;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AcmeCorp.API.Controllers
{
    [Authorize(AuthenticationSchemes = "ApiKey")]
    [Route("api/customer")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService _customerService;
        private readonly IMapper _mapper;
        public CustomerController(ICustomerService customerService, IMapper mapper)
        {
            _customerService = customerService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CustomerDTO>>> GetCustomers()
        {
            var customers = await _customerService.GetAllCustomersAsync();
            var customerDTOs = _mapper.Map<List<CustomerDTO>>(customers);
            return Ok(customerDTOs);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CustomerDTO>> GetCustomer(int id)
        {
            var customer = await _customerService.GetCustomerByIdAsync(id);
            if (customer == null)
            {
                return NotFound();
            }
            var customerDTO = _mapper.Map<CustomerDTO>(customer);
            return Ok(customerDTO);
        }

        [HttpPost]
        public async Task<ActionResult<CustomerDTO>> PostCustomer(CustomerDTO customerDTO)
        {
            var customer = _mapper.Map<Customer>(customerDTO);
            await _customerService.AddCustomerAsync(customer);
            customerDTO.Id = customer.Id;
            return CreatedAtAction(nameof(GetCustomer), new { id = customerDTO.Id }, customerDTO);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutCustomer(int id, CustomerDTO customerDTO)
        {
            if (id != customerDTO.Id)
            {
                return BadRequest();
            }
            var customer = _mapper.Map<Customer>(customerDTO);
            await _customerService.UpdateCustomerAsync(customer);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCustomer(int id)
        {
            await _customerService.DeleteCustomerAsync(id);
            return NoContent();
        }
    }
}
