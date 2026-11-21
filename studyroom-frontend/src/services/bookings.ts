import { apiRequest } from './api'
import type { Booking } from './types'

export type BookingPayload = Omit<Booking, 'id'>

export async function getBookings(): Promise<Booking[]> {
  const bookings = await apiRequest<Booking[]>('/api/bookings')
  if (!bookings) throw new Error('Could not load bookings.')
  return bookings
}

export async function createBooking(
  booking: BookingPayload,
  token: string
) {
  return apiRequest<Booking>('/api/bookings', {
    method: 'POST',
    body: JSON.stringify(booking)
  }, token)
}

export async function updateBooking(
  id: number,
  booking: BookingPayload,
  token: string
) {
  return apiRequest<void>(`/api/bookings/${id}`, {
    method: 'PUT',
    body: JSON.stringify(booking)
  }, token)
}

export async function deleteBooking(id: number, token: string) {
  return apiRequest<void>(`/api/bookings/${id}`, {
    method: 'DELETE'
  }, token)
}
