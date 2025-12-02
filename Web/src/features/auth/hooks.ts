import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query"
import { authApi } from "./api"
import type { LoginRequest, RegisterRequest } from "./types"

export const useLogin = () => {
  const queryClient = useQueryClient()
  return useMutation({
    mutationFn: (data: LoginRequest) => authApi.login(data),
    onSuccess: (data) => {
      localStorage.setItem("auth_token", data.token)
      // Invalidate user info cache to fetch fresh data with new token
      queryClient.invalidateQueries({ queryKey: ["user-info"] })
    },
  })
}

export const useRegister = () => {
  return useMutation({
    mutationFn: (data: RegisterRequest) => authApi.register(data),
  })
}

export const useUserInfo = () => {
  const token = localStorage.getItem("auth_token")
  return useQuery({
    queryKey: ["user-info", token],
    queryFn: () => authApi.getMe(),
    enabled: !!token,
    staleTime: 0, // Always fetch fresh data
    retry: false, // Don't retry on errors (especially 401)
  })
}
