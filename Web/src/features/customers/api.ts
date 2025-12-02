import { fetcher } from "@/lib/fetcher"
import type { Customer, CustomerCreateRequest, CustomerUpdateRequest } from "./types"

export const customersApi = {
  getAll: () => fetcher<{ customers: Customer[] }>("/customers"),

  getById: (id: string) => fetcher<Customer>(`/customers/${id}`),

  create: (data: CustomerCreateRequest) =>
    fetcher<Customer>("/customers", {
      method: "POST",
      body: JSON.stringify(data),
    }),

  update: (id: string, data: CustomerUpdateRequest) =>
    fetcher<void>(`/customers/${id}`, {
      method: "PUT",
      body: JSON.stringify(data),
    }),

  delete: (id: string) =>
    fetcher<void>(`/customers/${id}`, {
      method: "DELETE",
    }),
}
