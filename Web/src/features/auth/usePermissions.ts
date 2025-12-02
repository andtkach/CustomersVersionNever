import { useUserInfo } from "./hooks"
import type { Permission } from "./types"

export function usePermissions() {
  const { data: userInfo } = useUserInfo()

  // Normalize permissions to an array
  const getPermissionsArray = (): string[] => {
    if (!userInfo?.permission) return []

    // If it's already an array, return it
    if (Array.isArray(userInfo.permission)) {
      return userInfo.permission
    }

    // If it's a string, split by comma
    return userInfo.permission.split(',').map(p => p.trim())
  }

  const permissions = getPermissionsArray()

  const hasPermission = (permission: Permission): boolean => {
    return permissions.includes(permission)
  }

  const canRead = hasPermission("data:read")
  const canWrite = hasPermission("data:write")
  const canRemove = hasPermission("data:remove")

  return {
    hasPermission,
    canRead,
    canWrite,
    canRemove,
    permissions,
  }
}
