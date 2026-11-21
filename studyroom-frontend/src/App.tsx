
import { useEffect, useState } from 'react'
import './App.css'
import { loginUser } from './services/auth'
import {
  getAllParticipations,
  joinBooking as joinParticipation,
  leaveBooking as leaveParticipation,
  updateParticipation as updateParticipationMessage
} from './services/participations'
import type { Participation } from './services/types'
import { getRooms } from './services/rooms'
import {
  getBookings,
  createBooking,
  updateBooking,
  deleteBooking as deleteBookingRequest
} from './services/bookings'

type Room = {
  id: number
  name: string
  location: string
  capacity: number
  description?: string | null
  hasWhiteboard: boolean
  hasScreen: boolean
}

type Booking = {
  id: number
  studentName: string
  roomId: number
  subject: string
  topic: string | null
  date: string
  startTime: string
  endTime: string
}

type BookingForm = {
  studentName: string
  roomId: number
  subject: string
  topic: string
  date: string
  startTime: string
  endTime: string
}

const emptyForm: BookingForm = {
  studentName: '',
  roomId: 0,
  subject: '',
  topic: '',
  date: '',
  startTime: '',
  endTime: ''
}

function App() {
  const [rooms, setRooms] = useState<Room[]>([])
  const [bookings, setBookings] = useState<Booking[]>([])
  const [participations, setParticipations] = useState<Participation[]>([])
  const [editingParticipationId, setEditingParticipationId] = useState<number | null>(null)
  const [participationDraft, setParticipationDraft] = useState('')
  const [search, setSearch] = useState('')
  const [token, setToken] = useState('')
  const [email, setEmail] = useState('')
  const [password, setPassword] = useState('')
  const [form, setForm] = useState<BookingForm>(emptyForm)
  const [editingId, setEditingId] = useState<number | null>(null)
  const [showForm, setShowForm] = useState(false)
  const [formScrollRequest, setFormScrollRequest] = useState(0)
  const [message, setMessage] = useState('')
  const [error, setError] = useState('')
  const [loading, setLoading] = useState(false)

  async function loadData() {
    try {
      setError('')
      const [roomData, bookingData, participationData] = await Promise.all([
        getRooms(),
        getBookings(),
        getAllParticipations()
      ])
      setRooms(roomData)
      setBookings(bookingData)
      setParticipations(participationData)
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Could not load data.')
    }
  }

  useEffect(() => {
    loadData()
  }, [])

  useEffect(() => {
    if (showForm && formScrollRequest > 0) {
      document.getElementById('booking-form')?.scrollIntoView({
        behavior: 'smooth',
        block: 'start'
      })
    }
  }, [showForm, formScrollRequest])

  async function login(event: React.FormEvent<HTMLFormElement>) {
    event.preventDefault()
    setError('')
    setMessage('')
    setLoading(true)

    try {
      const userToken = await loginUser(email, password)
      setToken(userToken)
      setPassword('')
      setMessage('Successfully logged in!')
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Login failed.')
    } finally {
      setLoading(false)
    }
  }

  function startBooking(roomId: number) {
    setForm({ ...emptyForm, roomId })
    setEditingId(null)
    setShowForm(true)
    setFormScrollRequest(n => n + 1)
    setError('')
    setMessage('')
  }

  function startEditing(booking: Booking) {
    setForm({
      studentName: booking.studentName,
      roomId: booking.roomId,
      subject: booking.subject,
      topic: booking.topic || '',
      date: booking.date.slice(0, 10),
      startTime: booking.startTime.slice(0, 5),
      endTime: booking.endTime.slice(0, 5)
    })
    setEditingId(booking.id)
    setShowForm(true)
    setFormScrollRequest(n => n + 1)
    setError('')
    setMessage('')
  }

  async function saveBooking(event: React.FormEvent<HTMLFormElement>) {
    event.preventDefault()
    setError('')
    setMessage('')

    if (!token) {
      setError('Please log in before booking.')
      return
    }

    if (form.endTime <= form.startTime) {
      setError('End time must be after start time.')
      return
    }

    const payload = {
      studentName: form.studentName.trim(),
      roomId: form.roomId,
      subject: form.subject.trim(),
      topic: form.topic.trim() || null,
      date: `${form.date}T00:00:00`,
      startTime: `${form.startTime}:00`,
      endTime: `${form.endTime}:00`
    }

    setLoading(true)

    try {
      const editing = editingId !== null

      if (editing && editingId !== null) {
        await updateBooking(editingId, payload, token)
      } else {
        await createBooking(payload, token)
      }

      setShowForm(false)
      setEditingId(null)
      setForm(emptyForm)
      await loadData()
      setMessage(editing ? 'Booking updated!' : 'Booking created!')
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Could not save booking.')
    } finally {
      setLoading(false)
    }
  }

  async function joinExistingBooking(bookingId: number) {
    if (!token || !email.trim()) {
      setError('Please log in first.')
      return
    }

    setLoading(true)
    setError('')
    setMessage('')

    try {
      await joinParticipation(bookingId, email.trim(), '', token)
      await loadData()
      setMessage('Successfully joined the booking!')
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Could not join booking.')
    } finally {
      setLoading(false)
    }
  }

  async function leaveExistingBooking(participationId: number) {
    if (!token) {
      setError('Please log in first.')
      return
    }

    if (!window.confirm('Are you sure you want to leave this booking?')) {
      return
    }

    setLoading(true)
    setError('')
    setMessage('')

    try {
      await leaveParticipation(participationId, token)
      await loadData()
      setMessage('You have left the booking.')
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Could not leave booking.')
    } finally {
      setLoading(false)
    }
  }

  async function editParticipationMessage(participation: Participation) {
    if (!token) {
      setError('Please log in first.')
      return
    }

    if (participationDraft.length > 500) {
      setError('Message cannot exceed 500 characters.')
      return
    }

    setLoading(true)
    setError('')
    setMessage('')

    try {
      await updateParticipationMessage(participation.id, participationDraft, token)
      await loadData()
      setEditingParticipationId(null)
      setParticipationDraft('')
      setMessage('Participation message updated!')
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Could not update message.')
    } finally {
      setLoading(false)
    }
  }

  async function deleteBooking(id: number) {
    if (!token) {
      setError('Please log in first.')
      return
    }

    if (!window.confirm('Are you sure you want to cancel this booking?')) {
      return
    }

    setLoading(true)
    setError('')
    setMessage('')

    try {
      await deleteBookingRequest(id, token)
      await loadData()
      setMessage('Booking cancelled!')
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Could not cancel booking.')
    } finally {
      setLoading(false)
    }
  }

  const filteredRooms = rooms.filter(room =>
    `${room.name} ${room.location}`
      .toLowerCase()
      .includes(search.toLowerCase())
  )

  return (
    <main>
      <h1>StudyRoomBooking</h1>
      <p>Find and book a study room on campus.</p>

      {error && <p role="alert" style={{ color: 'crimson' }}>{error}</p>}
      {message && <p role="status">{message}</p>}

      <section className="card">
        {token ? (
          <>
            <p>Logged in as {email}</p>
            <button onClick={() => {
              setToken('')
              setShowForm(false)
              setMessage('Logged out.')
            }}>
              Log out
            </button>
          </>
        ) : (
          <>
            <h2>Student login</h2>
            <form onSubmit={login}>
              <input
                type="email"
                placeholder="Email"
                value={email}
                onChange={e => setEmail(e.target.value)}
                required
              />
              <input
                type="password"
                placeholder="Password"
                value={password}
                onChange={e => setPassword(e.target.value)}
                required
              />
              <button disabled={loading} type="submit">Log in</button>
            </form>
          </>
        )}
      </section>

      <h2>Available study rooms</h2>

      <input
        type="search"
        placeholder="Search for a study room..."
        value={search}
        onChange={e => setSearch(e.target.value)}
      />

      {filteredRooms.length === 0 && <p>No rooms found.</p>}

      {filteredRooms.map(room => (
        <div className="card" key={room.id}>
          <h3>{room.name}</h3>
          <p>Location: {room.location}</p>
          <p>Capacity: {room.capacity} students</p>
          {room.description && <p>{room.description}</p>}
          <p>Whiteboard: {room.hasWhiteboard ? 'Yes' : 'No'}</p>
          <p>Screen: {room.hasScreen ? 'Yes' : 'No'}</p>
          <button
            disabled={!token}
            onClick={() => startBooking(room.id)}
          >
            Book room
          </button>
        </div>
      ))}

      {showForm && (
        <section className="card" id="booking-form">
          <h2>{editingId !== null ? 'Edit Booking' : 'Create Booking'}</h2>

          <form onSubmit={saveBooking}>
            <label>
              Room
              <select
                value={form.roomId}
                onChange={e => setForm({
                  ...form, roomId: Number(e.target.value)
                })}
                required
              >
                {rooms.map(room => (
                  <option key={room.id} value={room.id}>
                    {room.name}
                  </option>
                ))}
              </select>
            </label>

            <input
              placeholder="Student name"
              value={form.studentName}
              minLength={2}
              maxLength={60}
              onChange={e => setForm({
                ...form, studentName: e.target.value
              })}
              required
            />

            <input
              placeholder="Subject"
              value={form.subject}
              maxLength={100}
              onChange={e => setForm({
                ...form, subject: e.target.value
              })}
              required
            />

            <input
              placeholder="Topic (optional)"
              value={form.topic}
              maxLength={200}
              onChange={e => setForm({
                ...form, topic: e.target.value
              })}
            />

            <label>
              Date
              <input
                type="date"
                value={form.date}
                onChange={e => setForm({
                  ...form, date: e.target.value
                })}
                required
              />
            </label>

            <label>
              Start time
              <input
                type="time"
                value={form.startTime}
                onChange={e => setForm({
                  ...form, startTime: e.target.value
                })}
                required
              />
            </label>

            <label>
              End time
              <input
                type="time"
                value={form.endTime}
                onChange={e => setForm({
                  ...form, endTime: e.target.value
                })}
                required
              />
            </label>

            <button type="submit" disabled={loading}>
              {editingId !== null ? 'Save changes' : 'Confirm Booking'}
            </button>

            <button type="button" onClick={() => setShowForm(false)}>
              Cancel
            </button>
          </form>
        </section>
      )}

      <h2>Existing Bookings</h2>

      {bookings.length === 0 && <p>No bookings found.</p>}

      {bookings.map(booking => (
        <div className="card booking-card" key={booking.id}>
          <h3>{booking.subject}</h3>
          <p>
            Participants (including organizer): {1 + participations.filter(
              p => p.bookingId === booking.id
            ).length}
          </p>
          <p>Student: {booking.studentName}</p>
          <p>Room: {rooms.find(r => r.id === booking.roomId)?.name || booking.roomId}</p>
          <p>Date: {booking.date.slice(0, 10)}</p>
          <p>Time: {booking.startTime} – {booking.endTime}</p>
          {booking.topic && <p>Topic: {booking.topic}</p>}

          {token &&
            booking.studentName.toLowerCase() !== email.trim().toLowerCase() &&
            !participations.some(p =>
              p.bookingId === booking.id &&
              p.studentName.toLowerCase() === email.trim().toLowerCase()
            ) && (
              <button
                disabled={loading}
                onClick={() => joinExistingBooking(booking.id)}
              >
                Join booking
              </button>
            )}

          {token && participations
            .filter(p =>
              p.bookingId === booking.id &&
              p.studentName.toLowerCase() === email.trim().toLowerCase()
            )
            .map(p => (
              <div key={p.id}>
                <p>Your message: {p.message || 'No message'}</p>
                {editingParticipationId === p.id ? (
                  <div>
                    <textarea
                      aria-label="Participation message"
                      placeholder="Write your message..."
                      maxLength={500}
                      value={participationDraft}
                      onChange={e => setParticipationDraft(e.target.value)}
                    />
                    <button
                      disabled={loading}
                      onClick={() => editParticipationMessage(p)}
                    >
                      Save message
                    </button>
                    <button
                      disabled={loading}
                      onClick={() => {
                        setEditingParticipationId(null)
                        setParticipationDraft('')
                      }}
                    >
                      Cancel edit
                    </button>
                  </div>
                ) : (
                  <button
                    disabled={loading}
                    onClick={() => {
                      setEditingParticipationId(p.id)
                      setParticipationDraft(p.message ?? '')
                    }}
                  >
                    Edit message
                  </button>
                )}
                <button
                  disabled={loading}
                  onClick={() => leaveExistingBooking(p.id)}
                >
                  Leave booking
                </button>
              </div>
            ))}

          {token &&
            booking.studentName.trim().toLowerCase() === email.trim().toLowerCase() && (
            <>
              <button onClick={() => startEditing(booking)}>
                Edit
              </button>
              <button
                disabled={loading}
                onClick={() => deleteBooking(booking.id)}
              >
                Cancel booking
              </button>
            </>
          )}
        </div>
      ))}
    </main>
  )
}

export default App
