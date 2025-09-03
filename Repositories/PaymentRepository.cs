using CarRentalApp.Data;
using CarRentalApp.Models;
using Microsoft.EntityFrameworkCore;

namespace CarRentalApp.Repositories
{
    public class PaymentRepository : IPayment
    {
        private readonly CarRentalContext _context;

        public PaymentRepository(CarRentalContext context)
        {
            _context = context;
        }

        // Process and save new payment
        public async Task<Payment> ProcessPaymentAsync(int rentalId, decimal amount, string method)
        {
            var rental = await _context.Rentals.FindAsync(rentalId);
            if (rental == null)
                throw new Exception("Rental not found for payment.");

            var payment = new Payment
            {
                RentalId = rentalId,
                Amount = (decimal)amount,
                PaymentDate = DateTime.Now,
                Method = method
            };

            await _context.Payments.AddAsync(payment);
            await _context.SaveChangesAsync();

            return payment;
        }

        // Get all payments
        public async Task<IEnumerable<Payment>> GetAllPaymentsAsync()
        {
            return await _context.Payments
                .Include(p => p.Rental)
                .ToListAsync();
        }

        // Get single payment by Id
        public async Task<Payment?> GetPaymentByIdAsync(int id)
        {
            return await _context.Payments
                .Include(p => p.Rental)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        // Get all payments for a rental
        public async Task<IEnumerable<Payment>> GetPaymentsByRentalIdAsync(int rentalId)
        {
            return await _context.Payments
                .Where(p => p.RentalId == rentalId)
                .ToListAsync();
        }
        public async Task<IEnumerable<Payment>> GetPaymentsByCustomerIdAsync(int customerId)
        {
            return await _context.Payments
                .Include(p => p.Rental)
                .ThenInclude(r => r.Car)
                .Where(p => p.Rental.CustomerId == customerId)
                .ToListAsync();
        }
    }
}
