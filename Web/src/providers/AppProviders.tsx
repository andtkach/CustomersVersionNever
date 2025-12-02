import { ReactNode } from "react"
import { QueryProvider } from "./QueryProvider"
import { ToastProvider } from "@/components/ui/toast"

export function AppProviders({ children }: { children: ReactNode }) {
  return (
    <QueryProvider>
      <ToastProvider>{children}</ToastProvider>
    </QueryProvider>
  )
}
