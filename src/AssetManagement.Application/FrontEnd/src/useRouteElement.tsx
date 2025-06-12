import path from "@/constants/path";
import { lazy, useContext } from "react";
import { Navigate, Outlet, RouteObject, useRoutes } from "react-router-dom";
import { AppContext } from "./contexts/app.context";
import LoginLayout from "./layouts/Login";
import MainLayout from "./layouts/Main";
import Home from "./pages/Home";
import CreateUser from "./pages/User/CreateUser";
import EditUser from "./pages/User/EditUser";
import UpdateAsset from "./pages/Asset/UpdateAsset";

function ProtectedRoute() {
  const { isAuthenticated } = useContext(AppContext);
  // const isAuthenticated = true;
  return isAuthenticated ? <Outlet /> : <Navigate to="/login" />;
}

function RejectedRoute() {
  const { isAuthenticated } = useContext(AppContext);
  // const isAuthenticated = true; // Simulate unauthenticated state
  return !isAuthenticated ? <Outlet /> : <Navigate to="/" />;
}

const Login = lazy(() => import("./pages/Login"));
const Asset = lazy(() => import("./pages/Asset"));
const CreateAsset = lazy(() => import("./pages/Asset/CreateAsset"));
const User = lazy(() => import("./pages/User"));
const Assignment = lazy(() => import("./pages/Assignment"));
const NotFound = lazy(() => import("./pages/NotFound"));
const Report = lazy(() => import("./pages/Report"));
const ReturnRequest = lazy(() => import("./pages/ReturnRequest"));
const Dashboard = lazy(() => import("./pages/Dashboard"));

// Assignment components
const CreateAssignment = lazy(
  () => import("./pages/Assignment/CreateAssignment")
);
const EditAssignment = lazy(() => import("./pages/Assignment/EditAssignment"));

export default function useRouteElements() {
  const routeElements = useRoutes([
    ...ProtectedRoutes,
    ...RejectedRoutes,
    ...OtherRoutes,
  ]);

  return routeElements;
}

const ProtectedRoutes: RouteObject[] = [
  {
    path: "",
    element: <ProtectedRoute />,
    children: [
      {
        path: path.home,
        element: (
          <MainLayout>
            <Home />
          </MainLayout>
        ),
      },
      {
        path: path.user,
        element: (
          <MainLayout>
            <User />
          </MainLayout>
        ),
      },
      {
        path: path.userCreate,
        element: (
          <MainLayout>
            <CreateUser />
          </MainLayout>
        ),
      },
      {
        path: path.userEdit,
        element: (
          <MainLayout>
            <EditUser />
          </MainLayout>
        ),
      },
      {
        path: path.assignment,
        element: (
          <MainLayout>
            <Assignment />
          </MainLayout>
        ),
      },
      {
        path: path.asset,
        element: (
          <MainLayout>
            <Asset />
          </MainLayout>
        ),
      },
      {
        path: path.assetCreate,
        element: (
          <MainLayout>
            <CreateAsset />
          </MainLayout>
        ),
      },
      {
        path: path.assetEdit,
        element: (
          <MainLayout>
            <UpdateAsset />
          </MainLayout>
        ),
      },
      {
        path: path.assignmentCreate,
        element: (
          <MainLayout>
            <CreateAssignment />
          </MainLayout>
        ),
      },
      {
        path: path.assignmentEdit,
        element: (
          <MainLayout>
            <EditAssignment />
          </MainLayout>
        ),
      },
      {
        path: path.request,
        element: (
          <MainLayout>
            <ReturnRequest />
          </MainLayout>
        ),
      },
      {
        path: path.report,
        element: (
          <MainLayout>
            <Report />
          </MainLayout>
        ),
      },
      {
        path: path.dashboard,
        element: (
          <MainLayout>
            <Dashboard />
          </MainLayout>
        )
      },
    ],
  },
];

const RejectedRoutes: RouteObject[] = [
  {
    path: "",
    element: <RejectedRoute />,
    children: [
      {
        path: path.login,
        index: true,
        element: (
          <LoginLayout>
            <Login />
          </LoginLayout>
        ),
      },
    ],
  },
];

const OtherRoutes: RouteObject[] = [
  {
    path: "*",
    element: <NotFound />,
  },
];
