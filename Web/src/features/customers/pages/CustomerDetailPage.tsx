import { useParams, useNavigate } from "react-router-dom"
import { ArrowLeft, Pencil } from "lucide-react"
import { Button } from "@/components/ui/button"
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card"
import { useCustomer } from "../hooks"
import { usePermissions } from "@/features/auth/usePermissions"

export function CustomerDetailPage() {
  const { id } = useParams<{ id: string }>()
  const navigate = useNavigate()
  const { canWrite } = usePermissions()
  const { data: customer, isLoading } = useCustomer(id!)

  if (isLoading) {
    return <div>Loading...</div>
  }

  if (!customer) {
    return <div>Customer not found</div>
  }

  return (
    <div className="space-y-4">
      <div className="flex justify-between items-center">
        <div className="flex items-center gap-4">
          <Button variant="ghost" size="icon" onClick={() => navigate("/customers")}>
            <ArrowLeft className="h-4 w-4" />
          </Button>
          <h1 className="text-3xl font-bold">Customer Details</h1>
        </div>
        {canWrite && (
          <Button onClick={() => navigate(`/customers/${id}/edit`)}>
            <Pencil className="mr-2 h-4 w-4" />
            Edit
          </Button>
        )}
      </div>

      <Card>
        <CardHeader>
          <CardTitle>
            {customer.firstName} {customer.lastName}
          </CardTitle>
          <CardDescription>Customer ID: {customer.id}</CardDescription>
        </CardHeader>
        <CardContent className="space-y-4">
          <div>
            <div className="text-sm font-medium text-muted-foreground">First Name</div>
            <div className="text-base">{customer.firstName}</div>
          </div>

          <div>
            <div className="text-sm font-medium text-muted-foreground">Last Name</div>
            <div className="text-base">{customer.lastName}</div>
          </div>

          <div>
            <div className="text-sm font-medium text-muted-foreground">Institution ID</div>
            <div
              className="text-base text-primary cursor-pointer hover:underline"
              onClick={() => navigate(`/institutions/${customer.institutionId}`)}
            >
              {customer.institutionId}
            </div>
          </div>
        </CardContent>
      </Card>
    </div>
  )
}
