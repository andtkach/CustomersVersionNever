import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query"
import { documentsApi, storageApi } from "./api"
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
    mutationFn: async (id: string) => {
      await documentsApi.delete(id)
      try {
        await storageApi.delete(id)
      } catch {
        // Ignore storage delete errors
      }
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["documents"] })
    },
  })
}

export const useUploadDocument = () => {
  const queryClient = useQueryClient()
  return useMutation({
    mutationFn: ({ documentId, file }: { documentId: string; file: File }) =>
      storageApi.upload(documentId, file),
    onSuccess: (_, variables) => {
      queryClient.invalidateQueries({ queryKey: ["document-file", variables.documentId] })
    },
  })
}

export const useDocumentFileExists = (documentId: string) => {
  return useQuery({
    queryKey: ["document-file", documentId],
    queryFn: () => storageApi.checkExists(documentId),
    enabled: !!documentId,
  })
}
