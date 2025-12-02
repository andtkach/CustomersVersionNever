export interface LoginRequest {
  email: string
  password: string
}

export interface LoginResponse {
  id: string
  token: string
  email?: string
  company?: string
}

export interface RegisterRequest {
  email: string
  company: string
  password: string
}

export interface RegisterResponse {
  id: string
  email: string
  company: string
}

export interface UserInfo {
  aud: string
  iss: string
  permission: string | string[]
  [key: string]: unknown
}

export type Permission = "data:read" | "data:write" | "data:remove"
