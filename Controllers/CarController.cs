// Controllers/CarsController.cs
using CarRentalApp.Models;
using CarRentalApp.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace CarRentalApp.Controllers
{
    public class CarController : Controller
    {
        private readonly ICar _repo;
        public CarController(ICar repo) => _repo = repo;

        public async Task<IActionResult> Index() => View(await _repo.GetAllAsync());

        
        public IActionResult Create() => View(new Car());

        [HttpPost]
        public async Task<IActionResult> Create(Car car)
        {
            if (!ModelState.IsValid) return View(car);
            await _repo.AddAsync(car);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var car = await _repo.GetByIdAsync(id);
            return car == null ? NotFound() : View(car);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, Car car)
        {
            if (id != car.Id) return BadRequest();
            if (!ModelState.IsValid) return View(car);
            await _repo.UpdateAsync(car);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var car = await _repo.GetByIdAsync(id);
            return car == null ? NotFound() : View(car);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _repo.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Available() => View("Index", await _repo.GetAvailableAsync());
    }
}
