import { fetcher } from "@/lib/fetcher"
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
