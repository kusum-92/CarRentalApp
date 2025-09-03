// Repositories/CustomerRepository.cs
using CarRentalApp.Data;
using CarRentalApp.Models;
using Microsoft.EntityFrameworkCore;

namespace CarRentalApp.Repositories
{
    public class CustomerRepository : ICustomer
    {
        private readonly CarRentalContext _ctx;
        public CustomerRepository(CarRentalContext ctx) => _ctx = ctx;

        public async Task AddAsync(Customer customer)
        {
            await _ctx.Customers.AddAsync(customer);
            await _ctx.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _ctx.Customers.FindAsync(id);
            if (entity != null)
            {
                _ctx.Customers.Remove(entity);
                await _ctx.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Customer>> GetAllAsync() =>
            await _ctx.Customers.ToListAsync();

        public async Task<Customer?> GetByIdAsync(int id) =>
            await _ctx.Customers.FirstOrDefaultAsync(x => x.Id == id);

        public async Task UpdateAsync(Customer customer)
        {
            _ctx.Customers.Update(customer);
            await _ctx.SaveChangesAsync();
        }
        public async Task<Customer?> GetByUserIdAsync(string userId)
        {
            return await _ctx.Customers.FirstOrDefaultAsync(c => c.UserId == userId);
        }
    }
}
