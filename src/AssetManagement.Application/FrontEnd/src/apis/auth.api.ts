import {
  AuthResponse,
  LoginRequest,
  PasswordUpdateRequest,
} from "../types/auth.type";
import http from "../utils/http";

const authApi = {
  loginAccount: async (
    body: LoginRequest,
  ): Promise<HttpResponse<AuthResponse>> => {
    const { data } = await http.post("auth/login", body);
    return data;
  },
  changePassword: async (
    body: PasswordUpdateRequest,
  ): Promise<HttpResponse<AuthResponse>> => {
    const { data } = await http.post("auth/password/change", body);
    return data;
  },
};

export default authApi;
