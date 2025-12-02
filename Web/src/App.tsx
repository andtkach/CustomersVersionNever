import { RouterProvider } from "react-router-dom"
import { router } from "./router"
import { AppProviders } from "./providers/AppProviders"
import { GlobalLoadingSpinner } from "./components/GlobalLoadingSpinner"

function App() {
  return (
    <AppProviders>
      <GlobalLoadingSpinner />
      <RouterProvider router={router} />
    </AppProviders>
  )
}

export default App
