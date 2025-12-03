import { fetcher } from "@/lib/fetcher"
import { env } from "@/lib/env"
import type { Document, DocumentCreateRequest, DocumentUpdateRequest } from "./types"

export const documentsApi = {
  getAll: () => fetcher<{ documents: Document[] }>("/documents"),

  getById: (id: string) => fetcher<Document>(`/documents/${id}`),

  create: (data: DocumentCreateRequest) =>
    fetcher<Document>("/documents", {
      method: "POST",
      body: JSON.stringify(data),
    }),

  update: (id: string, data: DocumentUpdateRequest) =>
    fetcher<void>(`/documents/${id}`, {
      method: "PUT",
      body: JSON.stringify(data),
    }),

  delete: (id: string) =>
    fetcher<void>(`/documents/${id}`, {
      method: "DELETE",
    }),
}

export const storageApi = {
  upload: async (documentId: string, file: File) => {
    const token = localStorage.getItem("auth_token")
    const formData = new FormData()
    formData.append("file", file)

    const response = await fetch(`${env.storageApiUrl}/storage/${documentId}`, {
      method: "POST",
      headers: {
        Authorization: `Bearer ${token}`,
      },
      body: formData,
    })

    if (!response.ok) {
      throw new Error(`Upload failed: ${response.statusText}`)
    }

    return response
  },

  download: async (documentId: string) => {
    const token = localStorage.getItem("auth_token")

    const response = await fetch(`${env.storageApiUrl}/storage/${documentId}`, {
      method: "GET",
      headers: {
        Authorization: `Bearer ${token}`,
      },
    })

    if (!response.ok) {
      if (response.status === 404) {
        return null
      }
      throw new Error(`Download failed: ${response.statusText}`)
    }

    return response
  },

  delete: async (documentId: string) => {
    const token = localStorage.getItem("auth_token")

    const response = await fetch(`${env.storageApiUrl}/storage/${documentId}`, {
      method: "DELETE",
      headers: {
        Authorization: `Bearer ${token}`,
      },
    })

    if (!response.ok) {
      throw new Error(`Delete failed: ${response.statusText}`)
    }

    return response
  },

  checkExists: async (documentId: string) => {
    const token = localStorage.getItem("auth_token")

    try {
      const response = await fetch(`${env.storageApiUrl}/storage/${documentId}`, {
        method: "GET",
        headers: {
          Authorization: `Bearer ${token}`,
        },
      })

      return response.ok
    } catch {
      return false
    }
  },
}
