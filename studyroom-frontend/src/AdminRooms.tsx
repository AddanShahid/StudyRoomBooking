import { useState } from 'react'
import type { FormEvent } from 'react'
import type { Room } from './services/types'
import { isAdminToken } from './services/roles'
import { createRoom, deleteRoom, updateRoom } from './services/rooms'
import type { RoomInput } from './services/rooms'

type AdminRoomsProps = {
  rooms: Room[]
  token: string
  onRoomsChanged: () => Promise<void>
}

type RoomForm = {
  name: string
  location: string
  capacity: string
  description: string
  hasWhiteboard: boolean
  hasScreen: boolean
  openingTime: string
  closingTime: string
}

const emptyRoomForm: RoomForm = {
  name: '',
  location: '',
  capacity: '1',
  description: '',
  hasWhiteboard: false,
  hasScreen: false,
  openingTime: '08:00',
  closingTime: '16:00'
}

function formFromRoom(room: Room): RoomForm {
  return {
    name: room.name,
    location: room.location,
    capacity: String(room.capacity),
    description: room.description ?? '',
    hasWhiteboard: room.hasWhiteboard,
    hasScreen: room.hasScreen,
    openingTime: room.openingTime.slice(0, 5),
    closingTime: room.closingTime.slice(0, 5)
  }
}

export default function AdminRooms({ rooms, token, onRoomsChanged }: AdminRoomsProps) {
  const [editingRoomId, setEditingRoomId] = useState<number | null>(null)
  const [showForm, setShowForm] = useState(false)
  const [roomForm, setRoomForm] = useState<RoomForm>(emptyRoomForm)
  const [busy, setBusy] = useState(false)
  const [message, setMessage] = useState('')
  const [error, setError] = useState('')

  // UI visibility is not a security boundary: the API also enforces Admin authorization.
  if (!isAdminToken(token)) return null

  function startCreate() {
    setRoomForm({ ...emptyRoomForm })
    setEditingRoomId(null)
    setError('')
    setMessage('')
    setShowForm(true)
  }

  function startEdit(room: Room) {
    setRoomForm(formFromRoom(room))
    setEditingRoomId(room.id)
    setError('')
    setMessage('')
    setShowForm(true)
  }

  function cancelEditing() {
    setShowForm(false)
    setEditingRoomId(null)
    setError('')
  }

  async function handleSaveRoom(event: FormEvent<HTMLFormElement>) {
    event.preventDefault()
    setError('')
    setMessage('')

    const payload: RoomInput = {
      name: roomForm.name.trim(),
      location: roomForm.location.trim(),
      capacity: Number(roomForm.capacity),
      description: roomForm.description.trim() || null,
      hasWhiteboard: roomForm.hasWhiteboard,
      hasScreen: roomForm.hasScreen,
      openingTime: roomForm.openingTime,
      closingTime: roomForm.closingTime
    }

    if (!/^[\p{L}0-9 .-]+$/u.test(payload.name) || payload.name.length < 2 || payload.name.length > 50) {
      setError('Room name must contain 2–50 letters, numbers, spaces, periods or hyphens.')
      return
    }
    if (payload.location.length < 2 || payload.location.length > 100) {
      setError('Location must contain 2–100 characters.')
      return
    }
    if (!Number.isInteger(payload.capacity) || payload.capacity < 1 || payload.capacity > 50) {
      setError('Capacity must be a whole number between 1 and 50.')
      return
    }
    if (roomForm.description.length > 500) {
      setError('Description must be at most 500 characters.')
      return
    }
    if (!payload.openingTime || !payload.closingTime || payload.openingTime >= payload.closingTime) {
      setError('Opening time must be before closing time.')
      return
    }

    const isEditing = editingRoomId !== null
    setBusy(true)
    try {
      if (editingRoomId === null) {
        await createRoom(payload, token)
      } else {
        await updateRoom(editingRoomId, payload, token)
      }
      setShowForm(false)
      setEditingRoomId(null)
      setRoomForm({ ...emptyRoomForm })
      setMessage(isEditing ? 'Room updated successfully.' : 'Room created successfully.')
      try {
        await onRoomsChanged()
      } catch {
        setError('Room was saved, but the list could not refresh. Reload the page to see it.')
      }
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Could not save room.')
    } finally {
      setBusy(false)
    }
  }

  async function handleDeleteRoom(room: Room) {
    if (!window.confirm(`Delete room "${room.name}"? This cannot be undone.`)) return
    setError('')
    setMessage('')
    setBusy(true)
    try {
      await deleteRoom(room.id, token)
      setMessage(`Room "${room.name}" deleted.`)
      if (editingRoomId === room.id) cancelEditing()
      try {
        await onRoomsChanged()
      } catch {
        setError('Room was deleted, but the list could not refresh. Reload the page.')
      }
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Could not delete room.')
    } finally {
      setBusy(false)
    }
  }

  return (
    <section className="card admin-panel" aria-labelledby="manage-rooms-title">
      <h2 id="manage-rooms-title">Manage study rooms</h2>
      <p>Only administrators can create, edit or delete rooms.</p>
      {error && <p role="alert" className="form-error">{error}</p>}
      {message && <p role="status" className="form-success">{message}</p>}

      <button type="button" disabled={busy} onClick={showForm ? cancelEditing : startCreate}>
        {showForm ? 'Cancel' : 'Create room'}
      </button>

      {showForm && (
        <form onSubmit={handleSaveRoom} className="admin-form">
          <h3>{editingRoomId === null ? 'Create new room' : 'Edit room'}</h3>
          <label htmlFor="admin-room-name">Room name</label>
          <input id="admin-room-name" name="name" value={roomForm.name}
            onChange={e => setRoomForm({ ...roomForm, name: e.target.value })}
            minLength={2} maxLength={50} required />

          <label htmlFor="admin-room-location">Location</label>
          <input id="admin-room-location" name="location" value={roomForm.location}
            onChange={e => setRoomForm({ ...roomForm, location: e.target.value })}
            minLength={2} maxLength={100} required />

          <label htmlFor="admin-room-capacity">Capacity (1–50)</label>
          <input id="admin-room-capacity" name="capacity" type="number" value={roomForm.capacity}
            onChange={e => setRoomForm({ ...roomForm, capacity: e.target.value })}
            min={1} max={50} step={1} required />

          <label htmlFor="admin-room-description">Description (optional)</label>
          <textarea id="admin-room-description" name="description" rows={3} maxLength={500}
            value={roomForm.description}
            onChange={e => setRoomForm({ ...roomForm, description: e.target.value })} />

          <div className="room-time-fields">
            <label htmlFor="admin-opening-time">Opening time
              <input id="admin-opening-time" type="time" required value={roomForm.openingTime}
                onChange={e => setRoomForm({ ...roomForm, openingTime: e.target.value })} />
            </label>
            <label htmlFor="admin-closing-time">Closing time
              <input id="admin-closing-time" type="time" required value={roomForm.closingTime}
                onChange={e => setRoomForm({ ...roomForm, closingTime: e.target.value })} />
            </label>
          </div>

          <label className="check-label">
            <input type="checkbox" checked={roomForm.hasWhiteboard}
              onChange={e => setRoomForm({ ...roomForm, hasWhiteboard: e.target.checked })} />
            Has whiteboard
          </label>
          <label className="check-label">
            <input type="checkbox" checked={roomForm.hasScreen}
              onChange={e => setRoomForm({ ...roomForm, hasScreen: e.target.checked })} />
            Has screen
          </label>

          <button disabled={busy} type="submit">
            {busy ? 'Saving…' : editingRoomId === null ? 'Save new room' : 'Save changes'}
          </button>
        </form>
      )}

      <h3>Existing rooms ({rooms.length})</h3>
      {rooms.length === 0 && <p>No rooms found.</p>}
      <ul className="admin-room-list">
        {rooms.map(room => (
          <li key={room.id}>
            <span>{room.name} – {room.location} (capacity {room.capacity})</span>
            <div className="admin-actions">
              <button type="button" disabled={busy} onClick={() => startEdit(room)}>Edit</button>
              <button type="button" disabled={busy} onClick={() => handleDeleteRoom(room)}>Delete</button>
            </div>
          </li>
        ))}
      </ul>
    </section>
  )
}
