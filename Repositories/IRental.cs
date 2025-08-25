// Repositories/IRentalRepository.cs
using CarRentalApp.Models;

namespace CarRentalApp.Repositories
{
    public interface IRental
    {
        // CRUD
        Task<IEnumerable<Rental>> GetAllAsync();
        Task<Rental?> GetByIdAsync(int id);
        Task AddAsync(Rental rental);
        Task UpdateAsync(Rental rental);
        Task DeleteAsync(int id);

        // Queries
        Task<IEnumerable<Rental>> GetByCustomerIdAsync(int customerId);
        Task<IEnumerable<Rental>> GetActiveAsync();

        // Business
        Task<Rental> BookAsync(int carId, int customerId, DateTime start, DateTime end);
        Task<Rental> CheckInAsync(int rentalId);
        Task<Rental> CheckOutAsync(int rentalId);
        Task<Rental> CancelAsync(int rentalId);
    }
}
