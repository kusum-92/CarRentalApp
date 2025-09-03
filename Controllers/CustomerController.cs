using CarRentalApp.Models;
using CarRentalApp.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace CarRentalApp.Controllers
{
    [Authorize]
    public class CustomerController : Controller
    {
        private readonly ICustomer _customerRepo;
        private readonly UserManager<IdentityUser> _userManager;

        public CustomerController(ICustomer customerRepo,UserManager<IdentityUser> userManager)
        {
            _customerRepo = customerRepo;
            _userManager = userManager;
        }

        // GET: Customers
        [Authorize(Roles ="Admin")]
        public async Task<IActionResult> Index()
        {
            var customers = await _customerRepo.GetAllAsync();
            return View(customers);
        }

        // GET: Customer/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var customer = await _customerRepo.GetByIdAsync(id);
            if (customer == null) return NotFound();
            // Non-admins can only see their own profile
            if (!User.IsInRole("Admin") && customer.UserId != _userManager.GetUserId(User))
            {
                return RedirectToAction("Index", "Home");
            }

            return View(customer);
        }

        // GET: Customer/Create
        [Authorize]
        public async Task<IActionResult> Create()
        {
            var userId = _userManager.GetUserId(User);
            var existing = await _customerRepo.GetByUserIdAsync(userId);

            if (existing != null)
            {
                // redirect to Home if already added
                return RedirectToAction("Details", "Customer", new { id = existing.Id });
            }
            return View(new Customer());
        }

        // POST: Customer/Create
        [HttpPost]
       
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Customer customer)
        {
            var userId = _userManager.GetUserId(User);
            var existing = await _customerRepo.GetByUserIdAsync(userId);

            if (existing != null)
            {
                // Prevent duplicate profile
                return RedirectToAction("Details", "Customer", new { id = existing.Id });
            }
            if (ModelState.IsValid)
            {
                
                customer.UserId = userId;

                await _customerRepo.AddAsync(customer);

                //var savedCustomer = await _customerRepo.GetByUserIdAsync(customer.UserId);
                if (User.IsInRole("Admin"))
                {
                    return RedirectToAction("Index", "Customer");
                }
                else
                {
                    return RedirectToAction("Details", "Customer", new {id=customer.Id});
                }
            }
            
            return View(customer);
        }

        // GET: Customer/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var customer = await _customerRepo.GetByIdAsync(id);
            if (customer == null) return NotFound();
            // Non-admins can only edit their own profile
            if (!User.IsInRole("Admin") && customer.UserId != _userManager.GetUserId(User))
            {
                return RedirectToAction("Index", "Home");
            }
            return View(customer);
        }

        // POST: Customer/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Customer customer)
        {
            // Ensure the logged-in user owns this profile (if not Admin)
            //if (!User.IsInRole("Admin") && customer.UserId != _userManager.GetUserId(User))
            //{
            //    return RedirectToAction("Index", "Home");
            //}
            if (ModelState.IsValid)
            {
                await _customerRepo.UpdateAsync(customer);
                if (User.IsInRole("Admin"))
                    return RedirectToAction(nameof(Index));
                //else
                //{
                //    return RedirectToAction("Index", "Home");
                //}
               return RedirectToAction("Details", "Customer", new { id = customer.Id });
            }
            return View(customer);
        }

        // GET: Customer/Delete/5
        [Authorize(Roles="Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var customer = await _customerRepo.GetByIdAsync(id);
            if (customer == null) return NotFound();
            return View(customer);
        }

        // POST: Customer/DeleteConfirmed
        [HttpPost, ActionName("DeleteConfirmed")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles ="Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _customerRepo.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
