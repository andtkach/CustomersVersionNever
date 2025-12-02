import { useParams, useNavigate } from "react-router-dom"
import { ArrowLeft, Pencil } from "lucide-react"
import { Button } from "@/components/ui/button"
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card"
import { useAddress } from "../hooks"
import { usePermissions } from "@/features/auth/usePermissions"

export function AddressDetailPage() {
  const { id } = useParams<{ id: string }>()
  const navigate = useNavigate()
  const { canWrite } = usePermissions()
  const { data: address, isLoading } = useAddress(id!)

  if (isLoading) {
    return <div>Loading...</div>
  }

  if (!address) {
    return <div>Address not found</div>
  }

  return (
    <div className="space-y-4">
      <div className="flex justify-between items-center">
        <div className="flex items-center gap-4">
          <Button variant="ghost" size="icon" onClick={() => navigate("/addresses")}>
            <ArrowLeft className="h-4 w-4" />
          </Button>
          <h1 className="text-3xl font-bold">Address Details</h1>
        </div>
        {canWrite && (
          <Button onClick={() => navigate(`/addresses/${id}/edit`)}>
            <Pencil className="mr-2 h-4 w-4" />
            Edit
          </Button>
        )}
      </div>

      <Card>
        <CardHeader>
          <CardTitle>
            {address.street}, {address.city}
          </CardTitle>
          <CardDescription>Address ID: {address.id}</CardDescription>
        </CardHeader>
        <CardContent className="space-y-4">
          <div>
            <div className="text-sm font-medium text-muted-foreground">Country</div>
            <div className="text-base">{address.country}</div>
          </div>

          <div>
            <div className="text-sm font-medium text-muted-foreground">City</div>
            <div className="text-base">{address.city}</div>
          </div>

          <div>
            <div className="text-sm font-medium text-muted-foreground">Street</div>
            <div className="text-base">{address.street}</div>
          </div>

          <div>
            <div className="text-sm font-medium text-muted-foreground">Current Address</div>
            <div className="text-base">{address.isCurrent ? "Yes" : "No"}</div>
          </div>

          <div>
            <div className="text-sm font-medium text-muted-foreground">Customer ID</div>
            <div
              className="text-base text-primary cursor-pointer hover:underline"
              onClick={() => navigate(`/customers/${address.customerId}`)}
            >
              {address.customerId}
            </div>
          </div>
        </CardContent>
      </Card>
    </div>
  )
}
