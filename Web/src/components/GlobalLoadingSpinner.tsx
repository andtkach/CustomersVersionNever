import { useIsFetching, useIsMutating } from "@tanstack/react-query"
import { Spinner } from "./ui/spinner"

export function GlobalLoadingSpinner() {
  const isFetching = useIsFetching()
  const isMutating = useIsMutating()

  const isLoading = isFetching > 0 || isMutating > 0

  if (!isLoading) return null

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/50 backdrop-blur-sm">
      <div className="rounded-lg bg-background p-6 shadow-xl">
        <div className="flex flex-col items-center gap-3">
          <Spinner size="lg" />
          <p className="text-sm font-medium text-muted-foreground">
            Loading...
          </p>
        </div>
      </div>
    </div>
  )
}
