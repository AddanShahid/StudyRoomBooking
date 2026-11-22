import type { ReactNode } from 'react'
import { isAdminToken } from './services/roles'

type Props = {
  token: string
  children: ReactNode
}

/** Client-side route guard. All privileged writes are also authorized on the API. */
export default function ProtectedRoute({ token, children }: Props) {
  if (!token) {
    return <section className="card" role="alert"><h2>Login required</h2><p>Please log in to access this page.</p></section>
  }
  if (!isAdminToken(token)) {
    return <section className="card" role="alert"><h2>Admin access required</h2><p>You do not have permission to manage study rooms, or your session has expired.</p></section>
  }
  return <>{children}</>
}
