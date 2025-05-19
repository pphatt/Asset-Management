import { User } from '../types/user.type';

export type LoginRequest = {
  username: string;
  password: string;
};

// export type AuthResponse = SuccessResponse<{
//   accessToken: string;
//   expires: string;
//   userInfo: User;
// }>;

export type AuthResponse = {
  accessToken: string;
  expires: string;
  userInfo: User;
};
