
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudyRoomBooking.DAL;
using StudyRoomBooking.Models;

namespace StudyRoomBooking.Controllers
{
    public class RoomController : Controller
    {
        private readonly IRoomRepository _roomRepository;
        private readonly IBookingRepository _bookingRepository;
        private readonly ILogger<RoomController> _logger;

        public RoomController(
            IRoomRepository roomRepository,
            IBookingRepository bookingRepository,
            ILogger<RoomController> logger)
        {
            _roomRepository = roomRepository;
            _bookingRepository = bookingRepository;
            _logger = logger;
        }

        // Show rooms and apply filters
        public async Task<IActionResult> Index(
            DateTime? date,
            TimeOnly? startTime,
            TimeOnly? endTime,
            int? equipmentId,
            int? minCapacity)
        {
            _logger.LogInformation("Henter alle rom.");

            var rooms = await _roomRepository.GetAll();

            if (rooms == null)
            {
                return Problem("Det oppstod en feil ved henting av rommene.");
            }

            var roomList = rooms.ToList();

            ViewBag.Equipments = roomList
                .SelectMany(r => r.Equipment)
                .GroupBy(e => e.Id)
                .Select(g => g.First())
                .OrderBy(e => e.Name)
                .ToList();

            ViewBag.SelectedDate = date;
            ViewBag.SelectedStartTime = startTime;
            ViewBag.SelectedEndTime = endTime;
            ViewBag.SelectedEquipmentId = equipmentId;
            ViewBag.SelectedMinCapacity = minCapacity;

            // Filter by minimum capacity
            if (minCapacity.HasValue)
            {
                roomList = roomList
                    .Where(r => r.Capacity >= minCapacity.Value)
                    .ToList();
            }

            // Filter by equipment
            if (equipmentId.HasValue)
            {
                roomList = roomList
                    .Where(r => r.Equipment.Any(e =>
                        e.Id == equipmentId.Value))
                    .ToList();
            }

            // Filter by date and time availability
            if (date.HasValue &&
                startTime.HasValue &&
                endTime.HasValue)
            {
                if (startTime.Value >= endTime.Value)
                {
                    ModelState.AddModelError(
                        "",
                        "Sluttid må være etter starttid.");
                }
                else
                {
                    var bookings = await _bookingRepository.GetAll();

                    if (bookings == null)
                    {
                        return Problem(
                            "Det oppstod en feil ved henting av bookinger.");
                    }

                    var bookedRoomIds = bookings
                        .Where(b =>
                            b.Date.Date == date.Value.Date &&
                            b.StartTime < endTime.Value &&
                            startTime.Value < b.EndTime)
                        .Select(b => b.RoomId)
                        .ToHashSet();

                    roomList = roomList
                        .Where(r =>
                            !bookedRoomIds.Contains(r.Id) &&
                            r.OpeningTime <= startTime.Value &&
                            r.ClosingTime >= endTime.Value)
                        .ToList();
                }
            }

            return View(roomList.Select(r =>
                new StudyRoomBooking.ViewModels.RoomViewModel
                {
                    Id = r.Id,
                    Name = r.Name,
                    Location = r.Location,
                    Capacity = r.Capacity,
                    Description = r.Description,
                    OpeningTime = r.OpeningTime,
                    ClosingTime = r.ClosingTime,
                    Equipments = r.Equipment.ToList()
                }).ToList());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                _logger.LogWarning("Details ble kalt uten rom-ID.");
                return NotFound();
            }

            var room = await _roomRepository.GetById(id.Value);

            if (room == null)
            {
                _logger.LogWarning("Fant ikke rom med ID {RoomId}.", id);
                return NotFound();
            }

            return View(room);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View(new Room());
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("Name,Location,Capacity,HasWhiteboard,HasScreen,Description,OpeningTime,ClosingTime")]
            Room room)
        {
            ValidateOpeningHours(room);

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Forsøk på å opprette rom med ugyldige data.");
                return View(room);
            }

            if (!await _roomRepository.Create(room))
            {
                ModelState.AddModelError(
                    "",
                    "Det oppstod en feil ved lagring av rommet.");

                return View(room);
            }

            _logger.LogInformation(
                "Rom opprettet med ID {RoomId} ({Name}).",
                room.Id,
                room.Name);

            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                _logger.LogWarning("Edit ble kalt uten rom-ID.");
                return NotFound();
            }

            var room = await _roomRepository.GetById(id.Value);

            if (room == null)
            {
                _logger.LogWarning(
                    "Fant ikke rom {RoomId} for redigering.", id);

                return NotFound();
            }

            return View(room);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            [Bind("Id,Name,Location,Capacity,HasWhiteboard,HasScreen,Description,OpeningTime,ClosingTime")]
            Room room)
        {
            if (id != room.Id)
            {
                _logger.LogWarning("Rom-ID stemmer ikke.");
                return NotFound();
            }

            ValidateOpeningHours(room);

            if (!ModelState.IsValid)
            {
                _logger.LogWarning(
                    "Ugyldige data ved redigering av rom {RoomId}.", id);

                return View(room);
            }

            if (!await _roomRepository.Update(room))
            {
                ModelState.AddModelError(
                    "",
                    "Det oppstod en feil ved oppdatering av rommet.");

                return View(room);
            }

            _logger.LogInformation(
                "Rom {RoomId} ble oppdatert.", room.Id);

            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                _logger.LogWarning("Delete ble kalt uten rom-ID.");
                return NotFound();
            }

            var room = await _roomRepository.GetById(id.Value);

            if (room == null)
            {
                _logger.LogWarning(
                    "Fant ikke rom {RoomId} for sletting.", id);

                return NotFound();
            }

            ViewBag.HasBookings = await HasBookings(room.Id);

            return View(room);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            _logger.LogInformation(
                "Forsøker å slette rom {RoomId}.", id);

            if (await HasBookings(id))
            {
                _logger.LogWarning(
                    "Rom {RoomId} har bookinger og kan ikke slettes.", id);

                return RedirectToAction(nameof(Delete), new { id });
            }

            if (!await _roomRepository.Delete(id))
            {
                return Problem(
                    "Det oppstod en feil ved sletting av rommet.");
            }

            _logger.LogInformation(
                "Rom {RoomId} ble slettet.", id);

            return RedirectToAction(nameof(Index));
        }

        private void ValidateOpeningHours(Room room)
        {
            if (room.OpeningTime >= room.ClosingTime)
            {
                ModelState.AddModelError(
                    nameof(Room.ClosingTime),
                    "Closing time must be after opening time.");
            }
        }

        private async Task<bool> HasBookings(int roomId)
        {
            var bookings = await _bookingRepository.GetAll();

            return bookings == null ||
                   bookings.Any(b => b.RoomId == roomId);
        }
    }
}
