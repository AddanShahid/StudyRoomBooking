export type Room = {
  id: number
  name: string
  location: string
  capacity: number
  description?: string | null
  hasWhiteboard: boolean
  hasScreen: boolean
  openingTime: string
  closingTime: string
}

export type Booking = {
  id: number
  studentName: string
  roomId: number
  subject: string
  topic: string | null
  date: string
  startTime: string
  endTime: string
}

export type Participation = {
  id: number
  studentName: string
  bookingId: number
  message: string | null
}
