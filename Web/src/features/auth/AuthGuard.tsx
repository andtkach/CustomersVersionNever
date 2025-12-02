import { Navigate } from "react-router-dom"
import { ReactNode } from "react"

interface AuthGuardProps {
  children: ReactNode
}

export function AuthGuard({ children }: AuthGuardProps) {
  const token = localStorage.getItem("auth_token")

  if (!token) {
    return <Navigate to="/login" replace />
  }

  return <>{children}</>
}
