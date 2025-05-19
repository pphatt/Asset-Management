import path from '@/constants/path';
import { AppContext } from '@/context/app.context.tsx';
import { lazy, useContext } from 'react';
import { Navigate, Outlet, RouteObject, useRoutes } from 'react-router-dom';
import LoginLayout from './layouts/LoginLayout';
import MainLayout from './layouts/MainLayout';
import Home from './pages/Home';

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

const Login = lazy(() => import('./pages/Login'));
const Asset = lazy(() => import('./pages/Asset'));

const User = lazy(() => import('./pages/User'));

export default function useRouteElements() {
  const routeElements = useRoutes([...ProtectedRoutes, ...RejectedRoutes, ...OtherRoutes]);

  return routeElements;
}

const ProtectedRoutes: RouteObject[] = [
  {
    path: '',
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
    ],
  },
];

const RejectedRoutes: RouteObject[] = [
  {
    path: '',
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
    path: '*',
    element: (
      <MainLayout>
        <></>
        {/* Not Found */}
      </MainLayout>
    ),
  },
];
