import { env } from "./env"
import { logout } from "./auth-utils"

export class ApiError extends Error {
  constructor(
    public status: number,
    message: string,
    public data?: unknown
  ) {
    super(message)
    this.name = "ApiError"
  }
}

interface FetcherOptions extends RequestInit {
  baseUrl?: string
  requiresAuth?: boolean
}

export async function fetcher<T = unknown>(
  endpoint: string,
  options: FetcherOptions = {}
): Promise<T> {
  const {
    baseUrl = env.dataApiUrl,
    requiresAuth = true,
    headers = {},
    ...restOptions
  } = options

  const token = localStorage.getItem("auth_token")

  const finalHeaders: Record<string, string> = {
    "Content-Type": "application/json",
    ...(headers as Record<string, string>),
  }

  if (requiresAuth && token) {
    finalHeaders["Authorization"] = `Bearer ${token}`
  }

  const url = `${baseUrl}${endpoint}`

  try {
    // Add 1 second delay before making the request
    await new Promise(resolve => setTimeout(resolve, 100))

    const response = await fetch(url, {
      ...restOptions,
      headers: finalHeaders,
    })

    if (!response.ok) {
      let errorMessage = `HTTP ${response.status}: ${response.statusText}`
      let errorData: unknown

      try {
        errorData = await response.json()
        if (errorData && typeof errorData === "object" && "message" in errorData) {
          errorMessage = (errorData as { message: string }).message
        }
      } catch {
        // Response body is not JSON
      }

      // Handle 401 Unauthorized - logout and redirect
      if (response.status === 401) {
        logout()
      }

      throw new ApiError(response.status, errorMessage, errorData)
    }

    // Handle 204 No Content or empty responses
    if (response.status === 204) {
      return undefined as T
    }

    // Check if response has content before parsing JSON
    const contentType = response.headers.get("content-type")
    const contentLength = response.headers.get("content-length")

    // If no content or content-length is 0, return undefined
    if (contentLength === "0" || !contentType?.includes("application/json")) {
      return undefined as T
    }

    // Try to parse JSON, but handle empty responses gracefully
    const text = await response.text()
    if (!text || text.trim() === "") {
      return undefined as T
    }

    return JSON.parse(text)
  } catch (error) {
    if (error instanceof ApiError) {
      throw error
    }
    throw new Error(
      error instanceof Error ? error.message : "An unknown error occurred"
    )
  }
}
