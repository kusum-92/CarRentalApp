using CarRentalApp.Data;
using CarRentalApp.Models;
using CarRentalApp.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CarRentalApp.Controllers
{
    public class PaymentController : Controller
    {
        private readonly IPayment _paymentRepo;
        private readonly CarRentalContext _context;

        public PaymentController(IPayment paymentRepo, CarRentalContext context)
        {
            _paymentRepo = paymentRepo;
            _context = context;
        }

        // List all payments
        public async Task<IActionResult> Index()
        {
            var payments = await _paymentRepo.GetAllPaymentsAsync();
            return View(payments);
        }

        // Create Payment after Checkout
        public async Task<IActionResult> Create(int rentalId)
        {
            var rental = await _context.Rentals
                .Include(r => r.Car)
                .Include(r => r.Customer)
                .FirstOrDefaultAsync(r => r.Id == rentalId);

            if (rental == null) return NotFound();

            var payment = new Payment
            {
                RentalId = rental.Id,
                Amount = rental.TotalCost,
                PaymentDate = DateTime.Now
            };

            ViewBag.CarDetails = $"{rental.Car.Make} {rental.Car.Model} ({rental.Car.LicensePlate})";
            ViewBag.CustomerName = rental.Customer.Name;

            return View(payment);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Payment payment)
        {
            if (ModelState.IsValid)
            {
                await _paymentRepo.ProcessPaymentAsync(payment.RentalId, (decimal)payment.Amount, payment.Method);
                return RedirectToAction("Index");
            }
            return View(payment);
        }
    }
}
