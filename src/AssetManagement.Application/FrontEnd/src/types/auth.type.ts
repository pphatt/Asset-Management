import { IUser } from "../types/user.type";

export type LoginRequest = {
  username: string;
  password: string;
};

export type PasswordUpdateRequest = {
  password?: string;
  newPassword: string;
};

export type AuthResponse = {
  accessToken: string;
  expires: string;
  userInfo: IUser;
};
