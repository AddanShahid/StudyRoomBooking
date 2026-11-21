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
