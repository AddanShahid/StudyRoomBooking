using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudyRoomBooking.Data;
using StudyRoomBooking.Models;
using StudyRoomBooking.ViewModels;

namespace StudyRoomBooking.Controllers
{
    // Only admins can create, edit and delete equipment. Everyone can see the list.
    [Authorize(Roles = "Admin")]
    public class EquipmentController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<EquipmentController> _logger;

        public EquipmentController(ApplicationDbContext context, ILogger<EquipmentController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // Viser alt utstyr
        [AllowAnonymous]
        public IActionResult Index()
        {
            var equipment = _context.Equipments
                .Select(e => new EquipmentViewModel
                {
                    Id = e.Id,
                    Name = e.Name,
                    Description = e.Description,
                })
                .ToList();

            _logger.LogInformation("[EquipmentController] Henter alt utstyr");
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
            {
                _logger.LogWarning("[EquipmentController] Ugyldige data ved oppretting av utstyr");
                return View(equipment);
            }

            _context.Equipments.Add(equipment);
            _context.SaveChanges();

            _logger.LogInformation("[EquipmentController] Utstyr {Name} opprettet", equipment.Name);
            return RedirectToAction(nameof(Index));
        }

        // Redigere utstyr
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var equipment = _context.Equipments.Find(id);

            if (equipment == null)
            {
                _logger.LogWarning("[EquipmentController] Fant ikke utstyr {Id}", id);
                return NotFound();
            }

            return View(equipment);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Equipment equipment)
        {
            if (id != equipment.Id)
                return NotFound();

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("[EquipmentController] Ugyldige data ved redigering av utstyr {Id}", id);
                return View(equipment);
            }

            _context.Equipments.Update(equipment);
            _context.SaveChanges();

            _logger.LogInformation("[EquipmentController] Utstyr {Id} oppdatert", id);
            return RedirectToAction(nameof(Index));
        }

        // Slette utstyr
        [HttpGet]
        public IActionResult Delete(int id)
        {
            var equipment = _context.Equipments.Find(id);

            if (equipment == null)
            {
                _logger.LogWarning("[EquipmentController] Fant ikke utstyr {Id}", id);
                return NotFound();
            }
            return View(equipment);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var equipment = _context.Equipments.Find(id);

            if (equipment == null)
            {
                _logger.LogWarning("[EquipmentController] Fant ikke utstyr {Id} for sletting", id);
                return NotFound();
            }

            _context.Equipments.Remove(equipment);
            _context.SaveChanges();
            _logger.LogInformation("[EquipmentController] Utstyr {Id} slettet", id);
            return RedirectToAction(nameof(Index));
        }
    }
}