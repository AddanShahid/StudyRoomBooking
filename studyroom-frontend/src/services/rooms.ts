import { apiRequest } from './api'
import type { Room } from './types'

export async function getRooms(): Promise<Room[]> {
  const rooms = await apiRequest<Room[]>('/api/rooms')
  if (!rooms) throw new Error('Could not load rooms.')
  return rooms
}

export async function getRoom(id: number): Promise<Room | null> {
  return apiRequest<Room>(`/api/rooms/${id}`)
}

export type RoomInput = Omit<Room, 'id'>

export async function createRoom(
  room: RoomInput,
  token: string
): Promise<Room> {
  const created = await apiRequest<Room>(
    '/api/rooms',
    {
      method: 'POST',
      body: JSON.stringify(room)
    },
    token
  )

  if (!created) {
    throw new Error('Could not create room.')
  }

  return created
}

export async function updateRoom(
  id: number,
  room: RoomInput,
  token: string
): Promise<void> {
  await apiRequest<void>(
    `/api/rooms/${id}`,
    {
      method: 'PUT',
      body: JSON.stringify(room)
    },
    token
  )
}

export async function deleteRoom(
  id: number,
  token: string
): Promise<void> {
  await apiRequest<void>(
    `/api/rooms/${id}`,
    {
      method: 'DELETE'
    },
    token
  )
}
