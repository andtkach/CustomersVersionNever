export interface Document {
  id: string
  customerId: string
  title: string
  content: string
  isActive: boolean
}

export interface DocumentCreateRequest {
  customerId: string
  title: string
  content: string
  isActive: boolean
}

export interface DocumentUpdateRequest {
  customerId: string
  title: string
  content: string
  isActive: boolean
}
