
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using StudyRoomBooking.Data;
using StudyRoomBooking.DAL;
using StudyRoomBooking.Models;

namespace StudyRoomBooking.Tests
{
    public class ParticipationRepositoryTests
    {
        // Lager en separat testdatabase for hver test.
        private ApplicationDbContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new ApplicationDbContext(options);
        }

        // Lager en testdeltakelse.
        private Participation CreateParticipation(
            string studentName = "Test Student",
            int bookingId = 1)
        {
            return new Participation
            {
                StudentName = studentName,
                BookingId = bookingId,
                Message = "Test message"
            };
        }

        // Lager et rom og en booking som testene kan bruke.
        private async Task<Booking> CreateBooking(ApplicationDbContext context)
        {
            var room = new Room
            {
                Name = "Test Room",
                Location = "Oslo",
                Capacity = 4
            };

            context.Rooms.Add(room);
            await context.SaveChangesAsync();

            var booking = new Booking
            {
                StudentName = "Test Student",
                RoomId = room.Id,
                Subject = "Programming",
                Date = DateTime.Today.AddDays(1),
                StartTime = new TimeOnly(10, 0),
                EndTime = new TimeOnly(12, 0)
            };

            context.Bookings.Add(booking);
            await context.SaveChangesAsync();

            return booking;
        }

        private Mock<ILogger<ParticipationRepository>> CreateLogger()
        {
            return new Mock<ILogger<ParticipationRepository>>();
        }

        // TEST 1: GetAll skal hente alle deltakelser.
        [Fact]
        public async Task GetAll_ReturnsAllParticipations()
        {
            using var context = CreateContext();

            var booking = await CreateBooking(context);

            context.Participations.Add(CreateParticipation("Student 1", booking.Id));
            context.Participations.Add(CreateParticipation("Student 2", booking.Id));

            await context.SaveChangesAsync();

            var repository = new ParticipationRepository(
                context, CreateLogger().Object);

            var result = await repository.GetAll();

            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
        }

        // TEST 2: GetAll skal returnere en tom liste når databasen er tom.
        [Fact]
        public async Task GetAll_ReturnsEmptyListWhenNoParticipationsExist()
        {
            using var context = CreateContext();

            var repository = new ParticipationRepository(
                context, CreateLogger().Object);

            var result = await repository.GetAll();

            Assert.NotNull(result);
            Assert.Empty(result);
        }

        // TEST 3: GetById skal finne en deltakelse med riktig ID.
        [Fact]
        public async Task GetById_ReturnsParticipationWhenItExists()
        {
            using var context = CreateContext();

            var booking = await CreateBooking(context);

            var participation = CreateParticipation("Test Student", booking.Id);

            context.Participations.Add(participation);
            await context.SaveChangesAsync();

            var repository = new ParticipationRepository(
                context, CreateLogger().Object);

            var result = await repository.GetById(participation.Id);

            Assert.NotNull(result);
            Assert.Equal("Test Student", result.StudentName);
        }

        // TEST 4: GetById skal returnere null når ID ikke finnes.
        [Fact]
        public async Task GetById_ReturnsNullWhenParticipationDoesNotExist()
        {
            using var context = CreateContext();

            var repository = new ParticipationRepository(
                context, CreateLogger().Object);

            var result = await repository.GetById(999);

            Assert.Null(result);
        }

        // TEST 5: GetByBookingId skal bare hente deltakelser for riktig booking.
        [Fact]
        public async Task GetByBookingId_ReturnsOnlyMatchingParticipations()
        {
            using var context = CreateContext();

            context.Participations.Add(CreateParticipation("Student 1", 10));
            context.Participations.Add(CreateParticipation("Student 2", 10));
            context.Participations.Add(CreateParticipation("Student 3", 20));

            await context.SaveChangesAsync();

            var repository = new ParticipationRepository(
                context, CreateLogger().Object);

            var result = await repository.GetByBookingId(10);

            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.All(result, p => Assert.Equal(10, p.BookingId));
        }

        // TEST 6: GetByBookingId skal returnere en tom liste uten treff.
        [Fact]
        public async Task GetByBookingId_ReturnsEmptyListWhenNoMatchesExist()
        {
            using var context = CreateContext();

            var repository = new ParticipationRepository(
                context, CreateLogger().Object);

            var result = await repository.GetByBookingId(999);

            Assert.NotNull(result);
            Assert.Empty(result);
        }

        // TEST 7: Create skal opprette en ny deltakelse.
        [Fact]
        public async Task Create_ReturnsTrueWhenParticipationIsCreated()
        {
            using var context = CreateContext();

            var repository = new ParticipationRepository(
                context, CreateLogger().Object);

            var result = await repository.Create(CreateParticipation());

            Assert.True(result);
        }

        // TEST 8: Create skal lagre deltakelsen i databasen.
        [Fact]
        public async Task Create_SavesParticipationToDatabase()
        {
            using var context = CreateContext();

            var repository = new ParticipationRepository(
                context, CreateLogger().Object);

            await repository.Create(CreateParticipation("New Student"));

            var savedParticipation = await context.Participations
                .FirstOrDefaultAsync();

            Assert.NotNull(savedParticipation);
            Assert.Equal("New Student", savedParticipation.StudentName);
        }

        // TEST 9: Delete skal slette en eksisterende deltakelse.
        [Fact]
        public async Task Delete_ReturnsTrueWhenParticipationIsDeleted()
        {
            using var context = CreateContext();

            var participation = CreateParticipation();

            context.Participations.Add(participation);
            await context.SaveChangesAsync();

            var repository = new ParticipationRepository(
                context, CreateLogger().Object);

            var result = await repository.Delete(participation.Id);

            Assert.True(result);
        }

        // TEST 10: Delete skal fjerne deltakelsen fra databasen.
        [Fact]
        public async Task Delete_RemovesParticipationFromDatabase()
        {
            using var context = CreateContext();

            var participation = CreateParticipation();

            context.Participations.Add(participation);
            await context.SaveChangesAsync();

            var repository = new ParticipationRepository(
                context, CreateLogger().Object);

            await repository.Delete(participation.Id);

            var result = await context.Participations
                .FindAsync(participation.Id);

            Assert.Null(result);
        }

        // TEST 11: Delete skal returnere false når deltakelsen ikke finnes.
        [Fact]
        public async Task Delete_ReturnsFalseWhenParticipationDoesNotExist()
        {
            using var context = CreateContext();

            var repository = new ParticipationRepository(
                context, CreateLogger().Object);

            var result = await repository.Delete(999);

            Assert.False(result);
        }

        // TEST 12: Create skal returnere false dersom den får null.
        [Fact]
        public async Task Create_ReturnsFalseWhenParticipationIsNull()
        {
            using var context = CreateContext();

            var repository = new ParticipationRepository(
                context, CreateLogger().Object);

            var result = await repository.Create(null!);

            Assert.False(result);
        }

        // TEST 13: Update skal returnere false når deltakelsen ikke finnes.
[Fact]
public async Task Update_ReturnsFalseWhenParticipationDoesNotExist()
{
    using var context = CreateContext();

    var repository = new ParticipationRepository(
        context, CreateLogger().Object);

    var result = await repository.Update(999, "Updated message");

    Assert.False(result);
}
    }
}
