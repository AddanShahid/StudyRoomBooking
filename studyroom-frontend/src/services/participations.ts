import { apiRequest } from './api'
import type { Participation } from './types'

export async function getParticipations(
  bookingId: number
): Promise<Participation[]> {
  const data = await apiRequest<Participation[]>(
    `/api/participations/booking/${bookingId}`
  )

  if (!data) throw new Error('Could not load participants.')
  return data
}

export async function joinBooking(
  bookingId: number,
  studentName: string,
  message: string,
  token: string
) {
  return apiRequest<Participation>('/api/participations', {
    method: 'POST',
    body: JSON.stringify({
      bookingId,
      studentName,
      message: message.trim() || null
    })
  }, token)
}

export async function updateParticipation(
  id: number,
  message: string,
  token: string
) {
  return apiRequest<void>(`/api/participations/${id}`, {
    method: 'PUT',
    body: JSON.stringify({ message: message.trim() || null })
  }, token)
}

export async function leaveBooking(
  participationId: number,
  token: string
) {
  return apiRequest<void>(
    `/api/participations/${participationId}`,
    { method: 'DELETE' },
    token
  )
}
