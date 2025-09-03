using CarRentalApp.Data;
using CarRentalApp.Models;
using CarRentalApp.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;

namespace CarRentalApp.Controllers
{
    public class RentalController : Controller
    {
        private readonly IRental _rentalRepo;
        private readonly CarRentalContext _context;
        private readonly UserManager<IdentityUser> _user;

        public RentalController(IRental rentalRepo, CarRentalContext context,UserManager<IdentityUser> user)
        {
            _rentalRepo = rentalRepo;
            _context = context;
            _user = user;
        }
        public async Task<IActionResult> Details(int id)
        {
            var rental = await _rentalRepo.GetByIdAsync(id);
            if (rental == null) return NotFound();
            return View(rental);
        }
        // Show all rentals
        [Authorize]
        public async Task<IActionResult> Index()
        {
            if (User.IsInRole("Admin"))
            {
                var rentals = await _rentalRepo.GetAllAsync();
                return View(rentals);
            }
            else
            {
                // Normal user sees only their rentals
                var userId = _user.GetUserId(User);
                var customer = _context.Customers.FirstOrDefault(c => c.UserId == userId);

                if (customer == null)
                {
                    // user has no profile yet → redirect to customer details form
                    return RedirectToAction("Create", "Customer");
                }

                var rentals = await _rentalRepo.GetByCustomerIdAsync(customer.Id);

                return View(rentals);
            }
                
        }

        // Book rental page for a specific car
        public IActionResult Create(int carId)
        {
            var car = _context.Cars.FirstOrDefault(c => c.Id == carId && c.IsAvailable);
            if (car == null)
            {
                return NotFound("Car not available for booking.");
            }

            var userId = _user.GetUserId(User);
            var customer = _context.Customers.FirstOrDefault(c => c.UserId == userId);

            if (customer == null)
            {
                return RedirectToAction("Create", "Customer");
            }

            ViewBag.Car = car; // ✅ Pass car details to view
            ViewBag.CustomerId = customer.Id;

            return View(new Rental { CarId = car.Id });
        }

        //[HttpPost]
        //public async Task<IActionResult> Create(Rental rental)
        //{
        //    var userId = _user.GetUserId(User);
        //    var customer = _context.Customers.FirstOrDefault(c => c.UserId == userId);

        //    if (customer == null)
        //    {
        //        return RedirectToAction("Create", "Customer");
        //    }

        //    rental.CustomerId = customer.Id;

        //    if (ModelState.IsValid)
        //    {
        //        await _rentalRepo.BookAsync(rental.CarId, rental.CustomerId, rental.StartDate, rental.EndDate);
        //        return RedirectToAction(nameof(Index));
        //    }

        //    var car = _context.Cars.FirstOrDefault(c => c.Id == rental.CarId);
        //    ViewBag.Car = car; // rebind in case of error
        //    return View(rental);
        //}



        // Book rental page
 

        [HttpPost]
        public async Task<IActionResult> Create(Rental rental)
        {
            var userId = _user.GetUserId(User);
            var customer = _context.Customers.FirstOrDefault(c => c.UserId == userId);

            if (customer == null)
            {
                return RedirectToAction("Create", "Customer");
            }

            rental.CustomerId = customer.Id;
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
            return RedirectToAction("Details","Rental", new { id = id });
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

    //    public async Task<IActionResult> Edit(int id)
    //    {
    //        var rental=_rentalRepo.GetByIdAsync(id);
    //        if (rental == null) return NotFound();  return View(rental);

    //    }
    //    [HttpPost]
    //    [ValidateAntiForgeryToken]
    //    public async Task<IActionResult> Edit(int id, Rental rental])
    //    {
    //     if (id != Rental.Id) return BadRequest();
    //        if (!ModelState.IsValid) 
    //        {return View(rental);}
    //        await _rentalRepo.UpdateAsync(rental);
    //        return RedirectToAction(nameof(Index));
    //}
    public async Task<IActionResult> Edit(int id)
{
    var rental = await _rentalRepo.GetByIdAsync(id);
    return rental == null ? NotFound() : View(rental);
}

[HttpPost]
public async Task<IActionResult> Edit(int id, Rental rental)
{
    if (id != rental.Id) return BadRequest();
    if (!ModelState.IsValid) return View(rental);
    await _rentalRepo.UpdateAsync(rental);
    return RedirectToAction(nameof(Index));
}
    }
}
