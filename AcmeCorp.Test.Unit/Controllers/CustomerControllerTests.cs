using AcmeCorp.API.Controllers;
using AcmeCorp.Application.DTOs;
using AcmeCorp.Application.Interfaces;
using AcmeCorp.Domain.Entities;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace AcmeCorp.Test.Unit.Controllers
{
    public class CustomerControllerTests
    {
        private readonly Mock<ICustomerService> _mockCustomerService;
        private readonly Mock<IMapper> _mockMapper;
        private readonly CustomerController _customerController;

        public CustomerControllerTests()
        {
            _mockCustomerService = new Mock<ICustomerService>();
            _mockMapper = new Mock<IMapper>();
            _customerController = new CustomerController(_mockCustomerService.Object, _mockMapper.Object);
        }

        [Fact]
        public async Task GetCustomers_ShouldReturnAllCustomers()
        {
            // Arrange
            var customers = new List<Customer>
            {
                new Customer { Id = 1, Name = "John Doe", Email = "john@example.com" },
                new Customer { Id = 2, Name = "Jane Doe", Email = "jane@example.com" }
            };
            var customerDTOs = new List<CustomerDTO>
            {
                new CustomerDTO { Id = 1, Name = "John Doe", Email = "john@example.com" },
                new CustomerDTO { Id = 2, Name = "Jane Doe", Email = "jane@example.com" }
            };

            _mockCustomerService.Setup(service => service.GetAllCustomersAsync()).ReturnsAsync(customers);
            _mockMapper.Setup(mapper => mapper.Map<List<CustomerDTO>>(customers)).Returns(customerDTOs);

            // Act
            var result = await _customerController.GetCustomers();

            // Assert
            var actionResult = Assert.IsType<ActionResult<IEnumerable<CustomerDTO>>>(result);
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var returnValue = Assert.IsType<List<CustomerDTO>>(okResult.Value);
            Assert.Equal(2, returnValue.Count);
        }

        [Fact]
        public async Task GetCustomer_ShouldReturnCustomer_WhenCustomerExists()
        {
            // Arrange
            var customer = new Customer { Id = 1, Name = "John Doe", Email = "john@example.com" };
            var customerDTO = new CustomerDTO { Id = 1, Name = "John Doe", Email = "john@example.com" };

            _mockCustomerService.Setup(service => service.GetCustomerByIdAsync(1)).ReturnsAsync(customer);
            _mockMapper.Setup(mapper => mapper.Map<CustomerDTO>(customer)).Returns(customerDTO);

            // Act
            var result = await _customerController.GetCustomer(1);

            // Assert
            var actionResult = Assert.IsType<ActionResult<CustomerDTO>>(result);
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var returnValue = Assert.IsType<CustomerDTO>(okResult.Value);
            Assert.Equal(customerDTO, returnValue);
        }

        [Fact]
        public async Task GetCustomer_ShouldReturnNotFound_WhenCustomerDoesNotExist()
        {
            // Arrange
            _mockCustomerService.Setup(service => service.GetCustomerByIdAsync(1)).ReturnsAsync((Customer)null);

            // Act
            var result = await _customerController.GetCustomer(1);

            // Assert
            var actionResult = Assert.IsType<ActionResult<CustomerDTO>>(result);
            Assert.IsType<NotFoundResult>(actionResult.Result);
        }

        [Fact]
        public async Task PostCustomer_ShouldCreateCustomer()
        {
            // Arrange
            var customer = new Customer { Name = "John Doe", Email = "john@example.com" };
            var customerDTO = new CustomerDTO { Name = "John Doe", Email = "john@example.com" };

            _mockMapper.Setup(mapper => mapper.Map<Customer>(customerDTO)).Returns(customer);
            _mockCustomerService.Setup(service => service.AddCustomerAsync(customer)).Returns(Task.CompletedTask);
            _mockMapper.Setup(mapper => mapper.Map<CustomerDTO>(customer)).Returns(customerDTO);

            // Act
            var result = await _customerController.PostCustomer(customerDTO);

            // Assert
            var actionResult = Assert.IsType<ActionResult<CustomerDTO>>(result);
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(actionResult.Result);
            var returnValue = Assert.IsType<CustomerDTO>(createdAtActionResult.Value);
            Assert.Equal(customerDTO, returnValue);
        }

        [Fact]
        public async Task PutCustomer_ShouldUpdateCustomer()
        {
            // Arrange
            var customer = new Customer { Id = 1, Name = "John Doe", Email = "john@example.com" };
            var customerDTO = new CustomerDTO { Id = 1, Name = "John Doe", Email = "john@example.com" };

            _mockMapper.Setup(mapper => mapper.Map<Customer>(customerDTO)).Returns(customer);
            _mockCustomerService.Setup(service => service.UpdateCustomerAsync(customer)).Returns(Task.CompletedTask);

            // Act
            var result = await _customerController.PutCustomer(1, customerDTO);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task PutCustomer_ShouldReturnBadRequest_WhenIdsDoNotMatch()
        {
            // Arrange
            var customerDTO = new CustomerDTO { Id = 1, Name = "John Doe", Email = "john@example.com" };

            // Act
            var result = await _customerController.PutCustomer(2, customerDTO);

            // Assert
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task DeleteCustomer_ShouldDeleteCustomer()
        {
            // Arrange
            var customer = new Customer { Id = 1, Name = "John Doe", Email = "john@example.com" };

            _mockCustomerService.Setup(service => service.GetCustomerByIdAsync(1)).ReturnsAsync(customer);
            _mockCustomerService.Setup(service => service.DeleteCustomerAsync(1)).Returns(Task.CompletedTask);

            // Act
            var result = await _customerController.DeleteCustomer(1);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteCustomer_ShouldReturnNotFound_WhenCustomerDoesNotExist()
        {
            // Arrange
            _mockCustomerService.Setup(service => service.GetCustomerByIdAsync(1)).ReturnsAsync((Customer)null);

            // Act
            var result = await _customerController.DeleteCustomer(1);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }
    }
}
