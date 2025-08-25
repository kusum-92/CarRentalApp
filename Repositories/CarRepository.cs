// Repositories/CarRepository.cs
using CarRentalApp.Data;
using CarRentalApp.Models;
using Microsoft.EntityFrameworkCore;

namespace CarRentalApp.Repositories
{
    public class CarRepository : ICar
    {
        private readonly CarRentalContext _ctx;
        public CarRepository(CarRentalContext ctx) => _ctx = ctx;

        public async Task AddAsync(Car car)
        {
            await _ctx.Cars.AddAsync(car);
            await _ctx.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var car = await _ctx.Cars.FindAsync(id);
            if (car != null)
            {
                _ctx.Cars.Remove(car);
                await _ctx.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Car>> GetAllAsync() =>
            await _ctx.Cars.ToListAsync();

        public async Task<Car?> GetByIdAsync(int id) =>
            await _ctx.Cars.FirstOrDefaultAsync(c => c.Id == id);

        public async Task<IEnumerable<Car>> GetAvailableAsync() =>
            await _ctx.Cars.Where(c => c.IsAvailable).ToListAsync();

        public async Task UpdateAsync(Car car)
        {
            _ctx.Cars.Update(car);
            await _ctx.SaveChangesAsync();
        }
    }
}
