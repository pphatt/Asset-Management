import {
  ICreateUserRequest,
  IUpdateUserRequest,
  IUser,
  IUserDetails,
  IUserParams,
  IUserType,
} from '../types/user.type';
import http from '../utils/http';

const userApi = {
  getUsers: async (params: IUserParams): Promise<HttpResponse<PaginatedResult<IUser>>> => {
    const { data } = await http.get('/users', { params });
    return data;
  },

  getUserTypes: async (): Promise<HttpResponse<IUserType[]>> => {
    const { data } = await http.get('/users/types');
    return data;
  },

  createUser: async (userData: ICreateUserRequest): Promise<HttpResponse<IUser>> => {
    const { data } = await http.post('/users', userData);
    return data;
  },

  getUserByStaffCode: async (staffCode: string): Promise<HttpResponse<IUserDetails>> => {
    const { data } = await http.get(`/users/${staffCode}`);
    return data;
  },

  updateUser: async (staffCode: string, data: IUpdateUserRequest): Promise<HttpResponse<IUser>> => {
    const { data: responseData } = await http.put(`/users/${staffCode}`, data);
    return responseData;
  },

  deleteUser: async (staffCode: string): Promise<HttpResponse<void>> => {
    const { data } = await http.delete(`/users/${staffCode}`);
    return data;
  },

  changePassword: async (
    staffCode: string,
    oldPassword: string,
    newPassword: string
  ): Promise<HttpResponse<void>> => {
    const { data } = await http.post(`/users/${staffCode}/change-password`, {
      oldPassword,
      newPassword,
    });
    return data;
  },
};

export default userApi;
