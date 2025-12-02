import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query"
import { institutionsApi } from "./api"
import type { InstitutionCreateRequest, InstitutionUpdateRequest } from "./types"

export const useInstitutions = (includeCustomers = false) => {
  return useQuery({
    queryKey: ["institutions", { includeCustomers }],
    queryFn: () => institutionsApi.getAll(includeCustomers),
  })
}

export const useInstitution = (id: string, includeCustomers = false) => {
  return useQuery({
    queryKey: ["institutions", id, { includeCustomers }],
    queryFn: () => institutionsApi.getById(id, includeCustomers),
    enabled: !!id,
  })
}

export const useCreateInstitution = () => {
  const queryClient = useQueryClient()
  return useMutation({
    mutationFn: (data: InstitutionCreateRequest) => institutionsApi.create(data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["institutions"] })
    },
  })
}

export const useUpdateInstitution = () => {
  const queryClient = useQueryClient()
  return useMutation({
    mutationFn: ({ id, data }: { id: string; data: InstitutionUpdateRequest }) =>
      institutionsApi.update(id, data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["institutions"] })
    },
  })
}

export const useDeleteInstitution = () => {
  const queryClient = useQueryClient()
  return useMutation({
    mutationFn: (id: string) => institutionsApi.delete(id),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["institutions"] })
    },
  })
}
