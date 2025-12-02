import { fetcher } from "@/lib/fetcher"
import { env } from "@/lib/env"
import type { LoginRequest, LoginResponse, RegisterRequest, RegisterResponse, UserInfo } from "./types"

export const authApi = {
  login: (data: LoginRequest) =>
    fetcher<LoginResponse>("/login", {
      method: "POST",
      body: JSON.stringify(data),
      baseUrl: env.authApiUrl,
      requiresAuth: false,
    }),

  register: (data: RegisterRequest) =>
    fetcher<RegisterResponse>("/register", {
      method: "POST",
      body: JSON.stringify(data),
      baseUrl: env.authApiUrl,
      requiresAuth: false,
    }),

  getMe: () =>
    fetcher<UserInfo>("/me", {
      method: "GET",
      baseUrl: env.authApiUrl,
      requiresAuth: true,
    }),
}
