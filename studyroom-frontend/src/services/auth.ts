import { apiRequest } from './api'

type LoginResponse = {
  token: string
  expiresAt: string
}

export async function loginUser(
  email: string,
  password: string
): Promise<string> {
  try {
    const result = await apiRequest<LoginResponse>(
      '/api/auth/login',
      {
        method: 'POST',
        body: JSON.stringify({ email, password })
      }
    )

    if (!result?.token) {
      throw new Error('Login response did not include a token.')
    }

    return result.token
  } catch (error) {
    if (
      error instanceof Error &&
      error.message === 'Unauthorized. Please log in again.'
    ) {
      throw new Error('Invalid email or password.')
    }

    throw error
  }
}
