const path = {
  home: '/',
  user: '/user',
  userCreate: "/user/create",
  userEdit: "/user/edit/:staffCode",
  asset: '/asset',
  assetCreate: "/asset/create",
  assignment: '/assignment',
  assignmentCreate: '/assignment/create',
  assignmentEdit: '/assignment/edit/:assignmentId',
  request: '/request',
  report: '/report',
  login: '/login',
  register: '/register',
  logout: '/logout',
} as const;

export default path;
