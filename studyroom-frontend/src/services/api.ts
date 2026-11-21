const API_URL = 'http://localhost:5095'

export async function apiRequest<T>(
  path: string,
  options: RequestInit = {},
  token?: string
): Promise<T | null> {
  const headers = new Headers(options.headers)

  if (options.body) {
    headers.set('Content-Type', 'application/json')
  }

  if (token) {
    headers.set('Authorization', `Bearer ${token}`)
  }

  const response = await fetch(`${API_URL}${path}`, {
    ...options,
    headers
  })

  if (!response.ok) {
    if (response.status === 401) {
      throw new Error('Unauthorized. Please log in again.')
    }

    const detail = await response.text()
    throw new Error(detail || `Request failed: ${response.status}`)
  }

  if (response.status === 204) return null

  return response.json() as Promise<T>
}
