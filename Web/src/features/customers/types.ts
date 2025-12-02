export interface Customer {
  id: string
  firstName: string
  lastName: string
  institutionId: string
}

export interface CustomerCreateRequest {
  firstName: string
  lastName: string
  institutionId: string
}

export interface CustomerUpdateRequest {
  firstName: string
  lastName: string
  institutionId: string
}
