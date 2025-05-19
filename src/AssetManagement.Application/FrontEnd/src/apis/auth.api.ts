import { AuthResponse } from "src/types/auth.type";
import http from "src/utils/http";

const authApi = {
  loginAccount: (body: { email: string; password: string }) =>
    http.post<AuthResponse>("/login", body),
};

export default authApi;
