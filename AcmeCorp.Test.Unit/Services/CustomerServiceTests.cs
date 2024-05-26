using AcmeCorp.Application.Interfaces;
using AcmeCorp.Application.Services;
using AcmeCorp.Domain.Entities;
using AcmeCorp.Infrastructure.Interfaces;
using Castle.Core.Resource;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcmeCorp.Test.Unit.Services
{
    public class CustomerServiceTests
    {
        private readonly Mock<ICustomerRepository> _mockCustomerRepository;
        private readonly ICustomerService _customerService;

        public CustomerServiceTests()
        {
            _mockCustomerRepository = new Mock<ICustomerRepository>();
            _customerService = new CustomerService(_mockCustomerRepository.Object);
        }

        [Fact]
        public async Task GetAllCustomers_ShouldReturnAllCustomers()
        {
            // Arrange
            var customers = new List<Customer>
            {
                new Customer { Id = 1, Name = "John Doe" },
                new Customer { Id = 2, Name = "Jane Smith" }
            };
            _mockCustomerRepository.Setup(repo => repo.GetAllAsync()).ReturnsAsync(customers);

            // Act
            var result = await _customerService.GetAllCustomersAsync();

            // Assert
            Assert.Equal(2, result.Count());
            Assert.Equal("John Doe", result.First().Name);
            Assert.Equal("Jane Smith", result.Last().Name);
        }

        [Fact]
        public async Task GetCustomerByIdAsync_ShouldReturnCustomer_WhenCustomerExists()
        {
            // Arrange
            var customerId = 1;
            var customer = new Customer { Id = customerId, Name = "John Doe" };
            _mockCustomerRepository.Setup(repo => repo.GetByIdAsync(customerId)).ReturnsAsync(customer);

            // Act
            var result = await _customerService.GetCustomerByIdAsync(customerId);

            // Assert
            Assert.Equal(customer, result);
            _mockCustomerRepository.Verify(repo => repo.GetByIdAsync(customerId), Times.Once);
        }

        [Fact]
        public async Task GetCustomerByIdAsync_ShouldReturnNull_WhenCustomerDoesNotExist()
        {
            // Arrange
            var customerId = 1;
            _mockCustomerRepository.Setup(repo => repo.GetByIdAsync(customerId)).ReturnsAsync((Customer)null);

            // Act
            var result = await _customerService.GetCustomerByIdAsync(customerId);

            // Assert
            Assert.Null(result);
            _mockCustomerRepository.Verify(repo => repo.GetByIdAsync(customerId), Times.Once);
        }

        [Fact]
        public async Task AddCustomer_ShouldAddCustomer()
        {
            // Arrange
            var customer = new Customer { Id = 1, Name = "John Doe" };
            _mockCustomerRepository.Setup(repo => repo.AddAsync(It.IsAny<Customer>())).Returns(Task.CompletedTask);

            // Act
            await _customerService.AddCustomerAsync(customer);

            // Assert
            _mockCustomerRepository.Verify(repo => repo.AddAsync(customer), Times.Once);
        }

        [Fact]
        public async Task UpdateCustomerAsync_ShouldCallUpdateAsync()
        {
            // Arrange
            var customer = new Customer { Id = 1, Name = "John Doe" };

            // Act
            await _customerService.UpdateCustomerAsync(customer);

            // Assert
            _mockCustomerRepository.Verify(repo => repo.UpdateAsync(customer), Times.Once);
        }

        [Fact]
        public async Task DeleteCustomerAsync_ShouldCallDeleteAsync()
        {
            // Arrange
            var customer = new Customer { Id = 1, Name = "John Doe" };
            _mockCustomerRepository.Setup(repo => repo.DeleteAsync(customer.Id)).Returns(Task.CompletedTask);

            // Act
            await _customerService.DeleteCustomerAsync(customer.Id);

            // Assert
            _mockCustomerRepository.Verify(repo => repo.DeleteAsync(customer.Id), Times.Once);
        }
    }
}
