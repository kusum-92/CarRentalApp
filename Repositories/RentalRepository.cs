// Repositories/RentalRepository.cs
using CarRentalApp.Data;
using CarRentalApp.Models;
using Microsoft.EntityFrameworkCore;

namespace CarRentalApp.Repositories
{
    public class RentalRepository : IRental
    {
        private readonly CarRentalContext _ctx;
        public RentalRepository(CarRentalContext ctx) => _ctx = ctx;

        // CRUD
        public async Task<IEnumerable<Rental>> GetAllAsync() =>
            await _ctx.Rentals
                .Include(r => r.Car)
                .Include(r => r.Customer)
                .ToListAsync();

        public async Task<Rental?> GetByIdAsync(int id) =>
            await _ctx.Rentals
                .Include(r => r.Car)
                .Include(r => r.Customer)
                .FirstOrDefaultAsync(r => r.Id == id);

        public async Task AddAsync(Rental rental)
        {
            await _ctx.Rentals.AddAsync(rental);
            await _ctx.SaveChangesAsync();
        }

        public async Task UpdateAsync(Rental rental)
        {
            _ctx.Rentals.Update(rental);
            await _ctx.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _ctx.Rentals.FindAsync(id);
            if (entity != null)
            {
                _ctx.Rentals.Remove(entity);
                await _ctx.SaveChangesAsync();
            }
        }

        // Queries
        public async Task<IEnumerable<Rental>> GetByCustomerIdAsync(int customerId) =>
            await _ctx.Rentals
                .Include(r => r.Car)
                .Where(r => r.CustomerId == customerId)
                .ToListAsync();

        public async Task<IEnumerable<Rental>> GetActiveAsync() =>
            await _ctx.Rentals
                .Include(r => r.Car)
                .Include(r => r.Customer)
                .Where(r => r.Status)
                .ToListAsync();

        // Business
        public async Task<Rental> BookAsync(int carId, int customerId, DateTime start, DateTime end)
        {
            var car = await _ctx.Cars.FindAsync(carId);
            if (car is null || !car.IsAvailable)
                throw new InvalidOperationException("Car not available.");

            var days = Math.Max(1, (end.Date - start.Date).Days);
            var total = days * car.DailyRate;

            var rental = new Rental
            {
                CarId = carId,
                CustomerId = customerId,
                StartDate = start,
                EndDate = end,
                TotalCost = total,
                Status = true
            };

            car.IsAvailable = false;

            await _ctx.Rentals.AddAsync(rental);
            await _ctx.SaveChangesAsync();
            return rental;
        }

        public async Task<Rental> CheckInAsync(int rentalId)
        {
            var rental = await _ctx.Rentals.FindAsync(rentalId)
                         ?? throw new KeyNotFoundException("Rental not found.");
            rental.Status = true; // active
            await _ctx.SaveChangesAsync();
            return rental;
        }

        public async Task<Rental> CheckOutAsync(int rentalId)
        {
            var rental = await _ctx.Rentals
                .Include(r => r.Car)
                .FirstOrDefaultAsync(r => r.Id == rentalId)
                ?? throw new KeyNotFoundException("Rental not found.");

            rental.Status = false;           // closed
            if (rental.Car != null) rental.Car.IsAvailable = true;

            await _ctx.SaveChangesAsync();
            return rental;
           
        }

        public async Task<Rental> CancelAsync(int rentalId)
        {
            var rental = await _ctx.Rentals
                .Include(r => r.Car)
                .FirstOrDefaultAsync(r => r.Id == rentalId)
                ?? throw new KeyNotFoundException("Rental not found.");

            rental.Status = false;
            if (rental.Car != null) rental.Car.IsAvailable = true;

            await _ctx.SaveChangesAsync();
            return rental;
        }
    }
}
