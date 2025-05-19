import { Navigate, Outlet, RouteObject, useRoutes } from "react-router-dom";
import { lazy, useContext } from "react";
import { AppContext } from "./context/app.context";
import LoginLayout from "./layouts/LoginLayout";
import path from "./constant/path";
import MainLayout from "./layouts/MainLayout";

function ProtectedRoute() {
  const { isAuthenticated } = useContext(AppContext);
  return isAuthenticated ? <Outlet /> : <Navigate to="/login" />;
}

function RejectedRoute() {
  const { isAuthenticated } = useContext(AppContext);
  return !isAuthenticated ? <Outlet /> : <Navigate to="/" />;
}

const Login = lazy(() => import("./pages/Login"));
const Asset = lazy(() => import("./pages/Asset"));

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
        path: path.asset,
        element: (
          <MainLayout>
            <Asset />
          </MainLayout>
        ),
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
    element: (
      <MainLayout>
        <></>
        {/* Not Found */}
      </MainLayout>
    ),
  },
];
