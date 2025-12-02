import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query"
import { addressesApi } from "./api"
import type { AddressCreateRequest, AddressUpdateRequest } from "./types"

export const useAddresses = () => {
  return useQuery({
    queryKey: ["addresses"],
    queryFn: () => addressesApi.getAll(),
  })
}

export const useAddress = (id: string) => {
  return useQuery({
    queryKey: ["addresses", id],
    queryFn: () => addressesApi.getById(id),
    enabled: !!id,
  })
}

export const useCreateAddress = () => {
  const queryClient = useQueryClient()
  return useMutation({
    mutationFn: (data: AddressCreateRequest) => addressesApi.create(data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["addresses"] })
    },
  })
}

export const useUpdateAddress = () => {
  const queryClient = useQueryClient()
  return useMutation({
    mutationFn: ({ id, data }: { id: string; data: AddressUpdateRequest }) =>
      addressesApi.update(id, data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["addresses"] })
    },
  })
}

export const useDeleteAddress = () => {
  const queryClient = useQueryClient()
  return useMutation({
    mutationFn: (id: string) => addressesApi.delete(id),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["addresses"] })
    },
  })
}
