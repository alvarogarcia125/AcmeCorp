using AcmeCorp.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AcmeCorp.Infrastructure.Context
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options) { }

        public DbSet<Customer> Customers { get; set; }
        public DbSet<ContactInfo> ContactInfos { get; set; }
        public DbSet<Order> Orders { get; set; }
    }
}
