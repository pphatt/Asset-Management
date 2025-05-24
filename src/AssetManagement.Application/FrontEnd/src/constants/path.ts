const path = {
  home: "/",
  user: "/user",
  userCreate: '/user/create',
  userEdit: '/user/edit/:staffCode',
  asset: "/asset",
  assignment: "/assignment",
  request: "/request",
  report: "/report",
  login: "/login",
  register: "/register",
  logout: "/logout",
} as const;

export default path;
