// Repositories/ICustomerRepository.cs
using CarRentalApp.Models;

namespace CarRentalApp.Repositories
{
    public interface ICustomer
    {
        Task<IEnumerable<Customer>> GetAllAsync();
        Task<Customer?> GetByIdAsync(int id);
        Task AddAsync(Customer customer);
        Task UpdateAsync(Customer customer);
        Task DeleteAsync(int id);
    }
}
