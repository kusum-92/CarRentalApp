// Repositories/ICarRepository.cs
using CarRentalApp.Models;

namespace CarRentalApp.Repositories
{
    public interface ICar
    {
        Task<IEnumerable<Car>> GetAllAsync();
        Task<Car?> GetByIdAsync(int id);
        Task AddAsync(Car car);
        Task UpdateAsync(Car car);
        Task DeleteAsync(int id);

        Task<IEnumerable<Car>> GetAvailableAsync();
    }
}
