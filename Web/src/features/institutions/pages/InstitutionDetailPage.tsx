import { useParams, useNavigate } from "react-router-dom"
import { ArrowLeft, Pencil } from "lucide-react"
import { Button } from "@/components/ui/button"
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card"
import { useInstitution } from "../hooks"
import { usePermissions } from "@/features/auth/usePermissions"

export function InstitutionDetailPage() {
  const { id } = useParams<{ id: string }>()
  const navigate = useNavigate()
  const { canWrite } = usePermissions()
  const { data: institution, isLoading } = useInstitution(id!, true)

  if (isLoading) {
    return <div>Loading...</div>
  }

  if (!institution) {
    return <div>Institution not found</div>
  }

  return (
    <div className="space-y-4">
      <div className="flex justify-between items-center">
        <div className="flex items-center gap-4">
          <Button variant="ghost" size="icon" onClick={() => navigate("/institutions")}>
            <ArrowLeft className="h-4 w-4" />
          </Button>
          <h1 className="text-3xl font-bold">Institution Details</h1>
        </div>
        {canWrite && (
          <Button onClick={() => navigate(`/institutions/${id}/edit`)}>
            <Pencil className="mr-2 h-4 w-4" />
            Edit
          </Button>
        )}
      </div>

      <Card>
        <CardHeader>
          <CardTitle>{institution.name}</CardTitle>
          <CardDescription>Institution ID: {institution.id}</CardDescription>
        </CardHeader>
        <CardContent className="space-y-4">
          <div>
            <div className="text-sm font-medium text-muted-foreground">Description</div>
            <div className="text-base">{institution.description}</div>
          </div>

          {institution.customers && institution.customers.length > 0 && (
            <div>
              <div className="text-sm font-medium text-muted-foreground mb-2">
                Customers ({institution.customers.length})
              </div>
              <div className="space-y-2">
                {institution.customers.map((customer) => (
                  <div
                    key={customer.id}
                    className="p-3 border rounded-md cursor-pointer hover:bg-accent"
                    onClick={() => navigate(`/customers/${customer.id}`)}
                  >
                    <div className="font-medium">
                      {customer.firstName} {customer.lastName}
                    </div>
                    <div className="text-sm text-muted-foreground">ID: {customer.id}</div>
                  </div>
                ))}
              </div>
            </div>
          )}
        </CardContent>
      </Card>
    </div>
  )
}
