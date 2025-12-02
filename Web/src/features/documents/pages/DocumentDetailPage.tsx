import { useParams, useNavigate } from "react-router-dom"
import { ArrowLeft, Pencil } from "lucide-react"
import { Button } from "@/components/ui/button"
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card"
import { useDocument } from "../hooks"
import { usePermissions } from "@/features/auth/usePermissions"

export function DocumentDetailPage() {
  const { id } = useParams<{ id: string }>()
  const navigate = useNavigate()
  const { canWrite } = usePermissions()
  const { data: document, isLoading } = useDocument(id!)

  if (isLoading) {
    return <div>Loading...</div>
  }

  if (!document) {
    return <div>Document not found</div>
  }

  return (
    <div className="space-y-4">
      <div className="flex justify-between items-center">
        <div className="flex items-center gap-4">
          <Button variant="ghost" size="icon" onClick={() => navigate("/documents")}>
            <ArrowLeft className="h-4 w-4" />
          </Button>
          <h1 className="text-3xl font-bold">Document Details</h1>
        </div>
        {canWrite && (
          <Button onClick={() => navigate(`/documents/${id}/edit`)}>
            <Pencil className="mr-2 h-4 w-4" />
            Edit
          </Button>
        )}
      </div>

      <Card>
        <CardHeader>
          <CardTitle>{document.title}</CardTitle>
          <CardDescription>Document ID: {document.id}</CardDescription>
        </CardHeader>
        <CardContent className="space-y-4">
          <div>
            <div className="text-sm font-medium text-muted-foreground">Title</div>
            <div className="text-base">{document.title}</div>
          </div>

          <div>
            <div className="text-sm font-medium text-muted-foreground">Content</div>
            <div className="text-base whitespace-pre-wrap">{document.content}</div>
          </div>

          <div>
            <div className="text-sm font-medium text-muted-foreground">Status</div>
            <div className="text-base">{document.isActive ? "Active" : "Inactive"}</div>
          </div>

          <div>
            <div className="text-sm font-medium text-muted-foreground">Customer ID</div>
            <div
              className="text-base text-primary cursor-pointer hover:underline"
              onClick={() => navigate(`/customers/${document.customerId}`)}
            >
              {document.customerId}
            </div>
          </div>
        </CardContent>
      </Card>
    </div>
  )
}
