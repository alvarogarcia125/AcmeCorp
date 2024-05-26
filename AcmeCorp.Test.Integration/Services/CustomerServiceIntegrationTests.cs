using AcmeCorp.Application.Interfaces;
using AcmeCorp.Application.Services;
using AcmeCorp.Domain.Entities;
using AcmeCorp.Infrastructure.Context;
using AcmeCorp.Infrastructure.Interfaces;
using AcmeCorp.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace AcmeCorp.Test.Integration.Services
{
    public class CustomerServiceIntegrationTests
    {
        private readonly ICustomerService _customerService;
        private readonly DatabaseContext _dbContext;

        public CustomerServiceIntegrationTests()
        {
            var serviceProvider = new ServiceCollection()
                .AddEntityFrameworkInMemoryDatabase()
                .AddDbContext<DatabaseContext>(options =>
                {
                    options.UseInMemoryDatabase("TestDatabase");
                })
                .AddScoped<ICustomerRepository, CustomerRepository>()
                .AddScoped<ICustomerService, CustomerService>()
                .BuildServiceProvider();

            _dbContext = serviceProvider.GetService<DatabaseContext>();
            _customerService = serviceProvider.GetService<ICustomerService>();

            ClearDatabase();
            SeedDatabase();
        }

        private void ClearDatabase()
        {
            _dbContext.Customers.RemoveRange(_dbContext.Customers);
            _dbContext.ContactInfos.RemoveRange(_dbContext.ContactInfos);
            _dbContext.Orders.RemoveRange(_dbContext.Orders);
            _dbContext.SaveChanges();
        }

        private void SeedDatabase()
        {
            var customers = new List<Customer>
            {
                new Customer { Id = 1, Name = "John Doe", Email = "jhon.doe@example.com" },
                new Customer { Id = 2, Name = "Jane Doe", Email = "jane.doe@example.com" }
            };
            _dbContext.Customers.AddRange(customers);
            _dbContext.SaveChanges();
        }

        [Fact]
        public async Task GetAllCustomersAsync_ShouldReturnAllCustomers()
        {
            // Act
            var result = await _customerService.GetAllCustomersAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, ((List<Customer>)result).Count);
        }

        [Fact]
        public async Task GetCustomerByIdAsync_ShouldReturnCustomer_WhenCustomerExists()
        {
            // Act
            var result = await _customerService.GetCustomerByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("John Doe", result.Name);
        }

        [Fact]
        public async Task GetCustomerByIdAsync_ShouldReturnNull_WhenCustomerDoesNotExist()
        {
            // Act
            var result = await _customerService.GetCustomerByIdAsync(999);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task AddCustomerAsync_ShouldAddCustomer()
        {
            // Arrange
            var newCustomer = new Customer { Id = 3, Name = "New Customer", Email = "new@example.com" };

            // Act
            await _customerService.AddCustomerAsync(newCustomer);
            var result = await _customerService.GetCustomerByIdAsync(3);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("New Customer", result.Name);
        }

        [Fact]
        public async Task UpdateCustomerAsync_ShouldUpdateCustomer()
        {
            // Arrange
            var customerToUpdate = await _customerService.GetCustomerByIdAsync(1);
            customerToUpdate.Name = "Updated Name";

            // Act
            await _customerService.UpdateCustomerAsync(customerToUpdate);
            var result = await _customerService.GetCustomerByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Updated Name", result.Name);
        }

        [Fact]
        public async Task DeleteCustomerAsync_ShouldDeleteCustomer()
        {
            // Act
            await _customerService.DeleteCustomerAsync(1);
            var result = await _customerService.GetCustomerByIdAsync(1);

            // Assert
            Assert.Null(result);
        }
    }
}
