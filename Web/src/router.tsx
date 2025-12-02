import { createBrowserRouter, Navigate } from "react-router-dom"
import { Layout } from "./components/layout/Layout"
import { AuthGuard } from "./features/auth/AuthGuard"
import { LoginPage } from "./features/auth/pages/LoginPage"

// Institutions
import { InstitutionsListPage } from "./features/institutions/pages/InstitutionsListPage"
import { InstitutionDetailPage } from "./features/institutions/pages/InstitutionDetailPage"
import { InstitutionCreatePage } from "./features/institutions/pages/InstitutionCreatePage"
import { InstitutionEditPage } from "./features/institutions/pages/InstitutionEditPage"

// Customers
import { CustomersListPage } from "./features/customers/pages/CustomersListPage"
import { CustomerDetailPage } from "./features/customers/pages/CustomerDetailPage"
import { CustomerCreatePage } from "./features/customers/pages/CustomerCreatePage"
import { CustomerEditPage } from "./features/customers/pages/CustomerEditPage"

// Addresses
import { AddressesListPage } from "./features/addresses/pages/AddressesListPage"
import { AddressDetailPage } from "./features/addresses/pages/AddressDetailPage"
import { AddressCreatePage } from "./features/addresses/pages/AddressCreatePage"
import { AddressEditPage } from "./features/addresses/pages/AddressEditPage"

// Documents
import { DocumentsListPage } from "./features/documents/pages/DocumentsListPage"
import { DocumentDetailPage } from "./features/documents/pages/DocumentDetailPage"
import { DocumentCreatePage } from "./features/documents/pages/DocumentCreatePage"
import { DocumentEditPage } from "./features/documents/pages/DocumentEditPage"

export const router = createBrowserRouter([
  {
    path: "/login",
    element: <LoginPage />,
  },
  {
    path: "/",
    element: (
      <AuthGuard>
        <Layout />
      </AuthGuard>
    ),
    children: [
      {
        index: true,
        element: <Navigate to="/institutions" replace />,
      },
      // Institutions routes
      {
        path: "institutions",
        element: <InstitutionsListPage />,
      },
      {
        path: "institutions/new",
        element: <InstitutionCreatePage />,
      },
      {
        path: "institutions/:id",
        element: <InstitutionDetailPage />,
      },
      {
        path: "institutions/:id/edit",
        element: <InstitutionEditPage />,
      },
      // Customers routes
      {
        path: "customers",
        element: <CustomersListPage />,
      },
      {
        path: "customers/new",
        element: <CustomerCreatePage />,
      },
      {
        path: "customers/:id",
        element: <CustomerDetailPage />,
      },
      {
        path: "customers/:id/edit",
        element: <CustomerEditPage />,
      },
      // Addresses routes
      {
        path: "addresses",
        element: <AddressesListPage />,
      },
      {
        path: "addresses/new",
        element: <AddressCreatePage />,
      },
      {
        path: "addresses/:id",
        element: <AddressDetailPage />,
      },
      {
        path: "addresses/:id/edit",
        element: <AddressEditPage />,
      },
      // Documents routes
      {
        path: "documents",
        element: <DocumentsListPage />,
      },
      {
        path: "documents/new",
        element: <DocumentCreatePage />,
      },
      {
        path: "documents/:id",
        element: <DocumentDetailPage />,
      },
      {
        path: "documents/:id/edit",
        element: <DocumentEditPage />,
      },
    ],
  },
])
