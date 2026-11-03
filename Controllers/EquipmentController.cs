using Microsoft.AspNetCore.Mvc;
using StudyRoomBooking.Data;
using StudyRoomBooking.Models;
using StudyRoomBooking.ViewModels;

namespace StudyRoomBooking.Controllers
{
    public class EquipmentController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EquipmentController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Viser alt utstyr
        public IActionResult Index()
        {
            var equipment = _context.Equipments
                .Select(e => new EquipmentViewModel
                {
                    Id = e.Id,
                    Name = e.Name,
                    Description = e.Description,
                    Quantity = e.Quantity
                })
                .ToList();

            return View(equipment);
        }

        // Opprette utstyr
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Equipment equipment)
        {
            if (!ModelState.IsValid)
                return View(equipment);

            _context.Equipments.Add(equipment);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        // Redigere utstyr
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var equipment = _context.Equipments.Find(id);

            if (equipment == null)
                return NotFound();

            return View(equipment);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Equipment equipment)
        {
            if (id != equipment.Id)
                return NotFound();

            if (!ModelState.IsValid)
                return View(equipment);

            _context.Equipments.Update(equipment);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        // Slette utstyr
        [HttpGet]
        public IActionResult Delete(int id)
        {
            var equipment = _context.Equipments.Find(id);

            if (equipment == null)
                return NotFound();

            return View(equipment);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var equipment = _context.Equipments.Find(id);

            if (equipment == null)
                return NotFound();

            _context.Equipments.Remove(equipment);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }
    }
}