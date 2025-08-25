using CarRentalApp.Models;

namespace CarRentalApp.Repositories
{
    public interface IPayment
    {
        Task<Payment> ProcessPaymentAsync(int rentalId, decimal amount, string method);
        Task<IEnumerable<Payment>> GetAllPaymentsAsync();
        Task<Payment?> GetPaymentByIdAsync(int id);
        Task<IEnumerable<Payment>> GetPaymentsByRentalIdAsync(int rentalId);
    }
}
