export function isAdminToken(token: string): boolean {
  if (!token) return false

  try {
    const part = token.split('.')[1]
    if (!part) return false

    const base64 = part.replace(/-/g, '+').replace(/_/g, '/')
    const padded = base64.padEnd(
      Math.ceil(base64.length / 4) * 4, '='
    )

    const payload = JSON.parse(atob(padded))

    if (typeof payload.exp !== 'number' ||
        payload.exp * 1000 <= Date.now()) {
      return false
    }

    const role =
      payload.role ??
      payload.roles ??
      payload['http://schemas.microsoft.com/ws/2008/06/identity/claims/role']

    return Array.isArray(role)
      ? role.includes('Admin')
      : role === 'Admin'
  } catch {
    return false
  }
}
