import { NavLink } from "react-router-dom"
import { cn } from "@/lib/utils"
import { Building2, Users, MapPin, FileText } from "lucide-react"

const navItems = [
  {
    label: "Institutions",
    path: "/institutions",
    icon: Building2,
  },
  {
    label: "Customers",
    path: "/customers",
    icon: Users,
  },
  {
    label: "Addresses",
    path: "/addresses",
    icon: MapPin,
  },
  {
    label: "Documents",
    path: "/documents",
    icon: FileText,
  },
]

export function Sidebar() {
  return (
    <aside className="w-64 border-r bg-background min-h-screen">
      <nav className="flex flex-col gap-1 p-4">
        {navItems.map((item) => {
          const Icon = item.icon
          return (
            <NavLink
              key={item.path}
              to={item.path}
              className={({ isActive }) =>
                cn(
                  "flex items-center gap-3 rounded-lg px-3 py-2 text-sm font-medium transition-colors",
                  isActive
                    ? "bg-primary text-primary-foreground"
                    : "text-muted-foreground hover:bg-accent hover:text-accent-foreground"
                )
              }
            >
              <Icon className="h-4 w-4" />
              {item.label}
            </NavLink>
          )
        })}
      </nav>
    </aside>
  )
}
