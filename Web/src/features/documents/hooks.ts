import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query"
import { documentsApi } from "./api"
import type { DocumentCreateRequest, DocumentUpdateRequest } from "./types"

export const useDocuments = () => {
  return useQuery({
    queryKey: ["documents"],
    queryFn: () => documentsApi.getAll(),
  })
}

export const useDocument = (id: string) => {
  return useQuery({
    queryKey: ["documents", id],
    queryFn: () => documentsApi.getById(id),
    enabled: !!id,
  })
}

export const useCreateDocument = () => {
  const queryClient = useQueryClient()
  return useMutation({
    mutationFn: (data: DocumentCreateRequest) => documentsApi.create(data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["documents"] })
    },
  })
}

export const useUpdateDocument = () => {
  const queryClient = useQueryClient()
  return useMutation({
    mutationFn: ({ id, data }: { id: string; data: DocumentUpdateRequest }) =>
      documentsApi.update(id, data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["documents"] })
    },
  })
}

export const useDeleteDocument = () => {
  const queryClient = useQueryClient()
  return useMutation({
    mutationFn: (id: string) => documentsApi.delete(id),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["documents"] })
    },
  })
}
