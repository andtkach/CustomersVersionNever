// Utility function to handle logout
export function logout() {
  // Clear auth token from localStorage
  localStorage.removeItem("auth_token")

  // Redirect to login page (this will reload the page and clear all React state)
  window.location.href = "/login"
}
