import { fetcher } from "@/lib/fetcher"
import type { Address, AddressCreateRequest, AddressUpdateRequest } from "./types"

export const addressesApi = {
  getAll: () => fetcher<{ addresses: Address[] }>("/addresses"),

  getById: (id: string) => fetcher<Address>(`/addresses/${id}`),

  create: (data: AddressCreateRequest) =>
    fetcher<Address>("/addresses", {
      method: "POST",
      body: JSON.stringify(data),
    }),

  update: (id: string, data: AddressUpdateRequest) =>
    fetcher<void>(`/addresses/${id}`, {
      method: "PUT",
      body: JSON.stringify(data),
    }),

  delete: (id: string) =>
    fetcher<void>(`/addresses/${id}`, {
      method: "DELETE",
    }),
}
