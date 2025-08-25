using CarRentalApp.Data;
using CarRentalApp.Models;
using CarRentalApp.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CarRentalApp.Controllers
{
    public class RentalController : Controller
    {
        private readonly IRental _rentalRepo;
        private readonly CarRentalContext _context;

        public RentalController(IRental rentalRepo, CarRentalContext context)
        {
            _rentalRepo = rentalRepo;
            _context = context;
        }

        // Show all rentals
        public async Task<IActionResult> Index()
        {
            var rentals = await _rentalRepo.GetAllAsync();
            return View(rentals);
        }

        // Book rental page
        public IActionResult Create()
        {
            ViewBag.CarId = new SelectList(_context.Cars.Where(c => c.IsAvailable), "Id", "Make");
            ViewBag.CustomerId = new SelectList(_context.Customers, "Id", "Name");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Rental rental)
        {
            if (ModelState.IsValid)
            {
                await _rentalRepo.BookAsync(rental.CarId, rental.CustomerId, rental.StartDate, rental.EndDate);
                return RedirectToAction(nameof(Index));
            }
            return View(rental);
        }

        // CheckIn
        public async Task<IActionResult> CheckIn(int id)
        {
            await _rentalRepo.CheckInAsync(id);
            return RedirectToAction(nameof(Index));
        }

        // CheckOut
        public async Task<IActionResult> CheckOut(int id)
        {
            var rental = await _rentalRepo.CheckOutAsync(id);
            if (rental == null) return NotFound();

            // redirect to Payment Create with rentalId
            return RedirectToAction("Create", "Payment", new { rentalId = rental.Id });
        
        }

        // Cancel rental
        public async Task<IActionResult> Cancel(int id)
        {
            await _rentalRepo.CancelAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
