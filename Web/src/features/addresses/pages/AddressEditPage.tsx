import { useParams, useNavigate } from "react-router-dom"
import { useForm } from "react-hook-form"
import { zodResolver } from "@hookform/resolvers/zod"
import { z } from "zod"
import { useEffect } from "react"
import { ArrowLeft } from "lucide-react"
import { Button } from "@/components/ui/button"
import { Input } from "@/components/ui/input"
import { Label } from "@/components/ui/label"
import { Select } from "@/components/ui/select"
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card"
import { useToast } from "@/components/ui/toast"
import { useAddress, useUpdateAddress } from "../hooks"
import { useCustomers } from "@/features/customers/hooks"

const addressSchema = z.object({
  customerId: z.string().min(1, "Customer is required"),
  country: z.string().min(1, "Country is required"),
  city: z.string().min(1, "City is required"),
  street: z.string().min(1, "Street is required"),
  isCurrent: z.boolean().default(false),
})

type AddressFormData = z.infer<typeof addressSchema>

export function AddressEditPage() {
  const { id } = useParams<{ id: string }>()
  const navigate = useNavigate()
  const { toast } = useToast()
  const { data: address, isLoading } = useAddress(id!)
  const updateMutation = useUpdateAddress()
  const { data: customersData } = useCustomers()

  const {
    register,
    handleSubmit,
    reset,
    formState: { errors },
  } = useForm<AddressFormData>({
    resolver: zodResolver(addressSchema),
  })

  useEffect(() => {
    if (address) {
      reset({
        customerId: address.customerId,
        country: address.country,
        city: address.city,
        street: address.street,
        isCurrent: address.isCurrent,
      })
    }
  }, [address, reset])

  const onSubmit = async (data: AddressFormData) => {
    try {
      await updateMutation.mutateAsync({ id: id!, data })
      toast({
        title: "Success",
        description: "Address updated successfully",
      })
      navigate(`/addresses/${id}`)
    } catch (error) {
      toast({
        title: "Error",
        description: error instanceof Error ? error.message : "Failed to update address",
        variant: "destructive",
      })
    }
  }

  if (isLoading) {
    return <div>Loading...</div>
  }

  if (!address) {
    return <div>Address not found</div>
  }

  return (
    <div className="space-y-4">
      <div className="flex items-center gap-4">
        <Button variant="ghost" size="icon" onClick={() => navigate(`/addresses/${id}`)}>
          <ArrowLeft className="h-4 w-4" />
        </Button>
        <h1 className="text-3xl font-bold">Edit Address</h1>
      </div>

      <Card>
        <CardHeader>
          <CardTitle>Edit Address</CardTitle>
          <CardDescription>Update address information</CardDescription>
        </CardHeader>
        <CardContent>
          <form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
            <div className="space-y-2">
              <Label htmlFor="customerId">Customer</Label>
              <Select id="customerId" {...register("customerId")}>
                <option value="">Select a customer</option>
                {customersData?.customers.map((customer) => (
                  <option key={customer.id} value={customer.id}>
                    {customer.firstName} {customer.lastName}
                  </option>
                ))}
              </Select>
              {errors.customerId && (
                <p className="text-sm text-destructive">{errors.customerId.message}</p>
              )}
            </div>

            <div className="space-y-2">
              <Label htmlFor="country">Country</Label>
              <Input id="country" {...register("country")} />
              {errors.country && (
                <p className="text-sm text-destructive">{errors.country.message}</p>
              )}
            </div>

            <div className="space-y-2">
              <Label htmlFor="city">City</Label>
              <Input id="city" {...register("city")} />
              {errors.city && (
                <p className="text-sm text-destructive">{errors.city.message}</p>
              )}
            </div>

            <div className="space-y-2">
              <Label htmlFor="street">Street</Label>
              <Input id="street" {...register("street")} />
              {errors.street && (
                <p className="text-sm text-destructive">{errors.street.message}</p>
              )}
            </div>

            <div className="flex items-center space-x-2">
              <input
                type="checkbox"
                id="isCurrent"
                className="h-4 w-4 rounded border-input"
                {...register("isCurrent")}
              />
              <Label htmlFor="isCurrent" className="cursor-pointer">
                Current Address
              </Label>
            </div>

            <div className="flex gap-2">
              <Button type="submit" disabled={updateMutation.isPending}>
                {updateMutation.isPending ? "Updating..." : "Update"}
              </Button>
              <Button
                type="button"
                variant="outline"
                onClick={() => navigate(`/addresses/${id}`)}
              >
                Cancel
              </Button>
            </div>
          </form>
        </CardContent>
      </Card>
    </div>
  )
}
