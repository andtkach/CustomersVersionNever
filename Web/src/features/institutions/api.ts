import { fetcher } from "@/lib/fetcher"
import type {
  Institution,
  InstitutionCreateRequest,
  InstitutionUpdateRequest,
  InstitutionsListResponse,
} from "./types"

export const institutionsApi = {
  getAll: (includeCustomers = false) =>
    fetcher<InstitutionsListResponse>(
      `/institutions${includeCustomers ? "?includeCustomers=true" : ""}`
    ),

  getById: (id: string, includeCustomers = false) =>
    fetcher<Institution>(
      `/institutions/${id}${includeCustomers ? "?includeCustomers=true" : ""}`
    ),

  create: (data: InstitutionCreateRequest) =>
    fetcher<Institution>("/institutions", {
      method: "POST",
      body: JSON.stringify(data),
    }),

  update: (id: string, data: InstitutionUpdateRequest) =>
    fetcher<void>(`/institutions/${id}`, {
      method: "PUT",
      body: JSON.stringify(data),
    }),

  patch: (id: string, data: Partial<InstitutionUpdateRequest>) =>
    fetcher<void>(`/institutions/${id}`, {
      method: "PATCH",
      body: JSON.stringify(data),
    }),

  delete: (id: string) =>
    fetcher<void>(`/institutions/${id}`, {
      method: "DELETE",
    }),
}
