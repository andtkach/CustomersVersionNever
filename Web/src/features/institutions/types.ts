export interface Institution {
  id: string
  name: string
  description: string
  customers?: Customer[]
}

export interface Customer {
  id: string
  firstName: string
  lastName: string
  institutionId: string
}

export interface InstitutionCreateRequest {
  name: string
  description: string
}

export interface InstitutionUpdateRequest {
  name: string
  description: string
}

export interface InstitutionsListResponse {
  institutions: Institution[]
}
