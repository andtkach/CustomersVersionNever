import { useParams, useNavigate } from "react-router-dom"
import { ArrowLeft, Pencil, Download } from "lucide-react"
import { Button } from "@/components/ui/button"
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card"
import { useDocument, useDocumentFileExists } from "../hooks"
import { usePermissions } from "@/features/auth/usePermissions"
import { storageApi } from "../api"
import { useToast } from "@/components/ui/toast"

export function DocumentDetailPage() {
  const { id } = useParams<{ id: string }>()
  const navigate = useNavigate()
  const { toast } = useToast()
  const { canWrite } = usePermissions()
  const { data: documentData, isLoading } = useDocument(id!)
  const { data: fileExists, isLoading: isCheckingFile } = useDocumentFileExists(id!)

  const handleDownload = async () => {
    try {
      const response = await storageApi.download(id!)
      if (!response) {
        toast({
          title: "Error",
          description: "File not found",
          variant: "destructive",
        })
        return
      }

      const blob = await response.blob()
      const contentDisposition = response.headers.get("content-disposition")
      let filename = `document-${id}`

      if (contentDisposition) {
        const filenameMatch = contentDisposition.match(/filename[^;=\n]*=((['"]).*?\2|[^;\n]*)/)
        if (filenameMatch && filenameMatch[1]) {
          filename = filenameMatch[1].replace(/['"]/g, "")
        }
      }

      const url = window.URL.createObjectURL(blob)
      const a = window.document.createElement("a")
      a.href = url
      a.download = filename
      window.document.body.appendChild(a)
      a.click()
      window.URL.revokeObjectURL(url)
      window.document.body.removeChild(a)

      toast({
        title: "Success",
        description: "File downloaded successfully",
      })
    } catch (error) {
      toast({
        title: "Error",
        description: error instanceof Error ? error.message : "Failed to download file",
        variant: "destructive",
      })
    }
  }

  if (isLoading) {
    return <div>Loading...</div>
  }

  if (!documentData) {
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
        <div className="flex gap-2">
          {fileExists && !isCheckingFile && (
            <Button variant="outline" onClick={handleDownload}>
              <Download className="mr-2 h-4 w-4" />
              Download File
            </Button>
          )}
          {canWrite && (
            <Button onClick={() => navigate(`/documents/${id}/edit`)}>
              <Pencil className="mr-2 h-4 w-4" />
              Edit
            </Button>
          )}
        </div>
      </div>

      <Card>
        <CardHeader>
          <CardTitle>{documentData.title}</CardTitle>
          <CardDescription>Document ID: {documentData.id}</CardDescription>
        </CardHeader>
        <CardContent className="space-y-4">
          <div>
            <div className="text-sm font-medium text-muted-foreground">Title</div>
            <div className="text-base">{documentData.title}</div>
          </div>

          <div>
            <div className="text-sm font-medium text-muted-foreground">Content</div>
            <div className="text-base whitespace-pre-wrap">{documentData.content}</div>
          </div>

          <div>
            <div className="text-sm font-medium text-muted-foreground">Status</div>
            <div className="text-base">{documentData.isActive ? "Active" : "Inactive"}</div>
          </div>

          <div>
            <div className="text-sm font-medium text-muted-foreground">Customer ID</div>
            <div
              className="text-base text-primary cursor-pointer hover:underline"
              onClick={() => navigate(`/customers/${documentData.customerId}`)}
            >
              {documentData.customerId}
            </div>
          </div>

          {fileExists && !isCheckingFile && (
            <div>
              <div className="text-sm font-medium text-muted-foreground">Attached File</div>
              <div className="text-base text-green-600">File available for download</div>
            </div>
          )}
        </CardContent>
      </Card>
    </div>
  )
}
