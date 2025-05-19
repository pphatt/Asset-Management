import { User } from "src/types/user.type";
import { SuccessResponse } from "src/types/utils.type";

export type AuthResponse = SuccessResponse<{
  access_token: string;
  expires: string;
  user: User;
}>;
