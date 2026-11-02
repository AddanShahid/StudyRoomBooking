using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudyRoomBooking.Data;
using StudyRoomBooking.Models;

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
            var equipment = _context.Equipments.ToList();
            return View(equipment);
        }

        // Åpner skjemaet for å legge til utstyr
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // Lagrer nytt utstyr
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Equipment equipment)
        {
            if (!ModelState.IsValid)
            {
                return View(equipment);
            }

            _context.Equipments.Add(equipment);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }
    }
}