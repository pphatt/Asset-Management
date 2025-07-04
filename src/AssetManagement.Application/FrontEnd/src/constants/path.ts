const path = {
  home: '/',
  user: '/user',
  userCreate: "/user/create",
  userEdit: "/user/edit/:staffCode",
  asset: '/asset',
  assetCreate: "/asset/create",
  assetEdit: "/asset/edit/:assetId",
  assignment: '/assignment',
  assignmentCreate: '/assignment/create',
  assignmentEdit: '/assignment/edit/:assignmentId',
  request: '/request',
  report: '/report',
  login: '/login',
  register: '/register',
  logout: '/logout',
  dashboard: '/dashboard',
} as const;

export default path;
