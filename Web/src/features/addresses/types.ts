export interface Address {
  id: string
  customerId: string
  country: string
  city: string
  street: string
  isCurrent: boolean
}

export interface AddressCreateRequest {
  customerId: string
  country: string
  city: string
  street: string
  isCurrent: boolean
}

export interface AddressUpdateRequest {
  customerId: string
  country: string
  city: string
  street: string
  isCurrent: boolean
}
