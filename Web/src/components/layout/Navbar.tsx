import { LogOut } from "lucide-react"
import { Button } from "@/components/ui/button"
import { logout } from "@/lib/auth-utils"

export function Navbar() {
  const handleLogout = () => {
    logout()
  }

  return (
    <nav className="border-b bg-background">
      <div className="flex h-16 items-center px-4 gap-4">
        <div className="font-semibold text-lg">Customers UI</div>
        <div className="ml-auto flex items-center gap-4">
          <Button variant="ghost" size="sm" onClick={handleLogout}>
            <LogOut className="mr-2 h-4 w-4" />
            Logout
          </Button>
        </div>
      </div>
    </nav>
  )
}
