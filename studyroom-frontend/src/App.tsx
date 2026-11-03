
import { useState } from 'react'
import './App.css'

const rooms = [
  { id: 1, name: 'Study Room A101', capacity: 4, location: 'Building A' },
  { id: 2, name: 'Study Room B205', capacity: 6, location: 'Building B' },
  { id: 3, name: 'Study Room C302', capacity: 8, location: 'Building C' },
]

function App() {
  const [search, setSearch] = useState('')
  const [bookedRooms, setBookedRooms] = useState<number[]>([])
  const [selectedRoom, setSelectedRoom] = useState<number | null>(null)

  const filteredRooms = rooms.filter(room =>
    room.name.toLowerCase().includes(search.toLowerCase())
  )

  function toggleBooking(id: number) {
    setBookedRooms(previous =>
      previous.includes(id)
        ? previous.filter(roomId => roomId !== id)
        : [...previous, id]
    )
  }

  return (
    <main>
      <h1>StudyRoomBooking</h1>
      <p>Find and book a study room on campus.</p>

      <input
        type="text"
        placeholder="Search for a study room..."
        value={search}
        onChange={event => setSearch(event.target.value)}
      />

      <h2>Available study rooms</h2>

      {filteredRooms.length === 0 && <p>No rooms found.</p>}

      {filteredRooms.map(room => (
        <div className="card" key={room.id}>
          <h3>{room.name}</h3>
          <p>Location: {room.location}</p>
          <p>Capacity: {room.capacity} students</p>
          {bookedRooms.includes(room.id) && (
  <p>✅ This room is booked!</p>
)}
          <button onClick={() => {
  if (bookedRooms.includes(room.id)) {
    toggleBooking(room.id)
  } else {
    setSelectedRoom(room.id)
  }
}}>
            {bookedRooms.includes(room.id) ? 'Cancel booking' : 'Book room'}
          </button>
        </div>
      ))}
   {selectedRoom !== null && (
  <div className="card">
    <h2>Create Booking</h2>

    <p>
      Room: {rooms.find(room => room.id === selectedRoom)?.name}
    </p>

    <form onSubmit={(event) => {
      event.preventDefault()
      toggleBooking(selectedRoom)
      setSelectedRoom(null)
    }}>
      <input
        type="text"
        placeholder="Student name"
        required
      />

      <input
        type="text"
        placeholder="Subject"
        required
      />

      <input
        type="date"
        required
      />

      <input
        type="time"
        required
      />

      <button type="submit">
        Confirm Booking
      </button>

      <button
        type="button"
        onClick={() => setSelectedRoom(null)}
      >
        Cancel
      </button>
    </form>
  </div>
)}
    </main>
  )
}

export default App
  