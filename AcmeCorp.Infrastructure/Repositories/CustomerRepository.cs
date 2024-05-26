using AcmeCorp.Domain.Entities;
using AcmeCorp.Infrastructure.Context;
using AcmeCorp.Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AcmeCorp.Infrastructure.Repositories
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly DatabaseContext _context;

        public CustomerRepository(DatabaseContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Customer>> GetAllAsync()
        {
            return await _context.Customers.Include(c => c.ContactInfos).Include(c => c.Orders).ToListAsync();
        }

        public async Task<Customer> GetByIdAsync(int id)
        {
            return await _context.Customers.Include(c => c.ContactInfos).Include(c => c.Orders).FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task AddAsync(Customer customer)
        {
            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Customer customer)
        {
            _context.Entry(customer).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var customer = await _context.Customers.FindAsync(id);
            _context.Customers.Remove(customer);
            await _context.SaveChangesAsync();
        }
    }
}
