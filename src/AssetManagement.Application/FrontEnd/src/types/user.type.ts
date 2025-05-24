// Define user types for the application

import { UserType } from "../constants/user-params";

export type IUserType = "Admin" | "Staff";

export enum LocationEnum {
  HCM = 1,
  DN = 2,
  HN = 3,
}

export enum GenderEnum {
  Male = 1,
  Female = 2,
}

export enum UserTypeEnum {
  Admin = 1,
  Staff = 2,
}

export interface IUser {
  staffCode: string;
  firstName: string;
  lastName: string;
  username: string;
  joinedDate: string; // Format: ISO 8601
  type: IUserType;
  isPasswordUpdated: boolean;
}

export interface IUserDetails {
  staffCode: string;
  firstName: string;
  lastName: string;
  username: string;
  dateOfBirth: string;
  gender: GenderEnum;
  joinedDate: string;
  type: UserTypeEnum;
  location: LocationEnum;
}

// Extension of the base IUser interface for creating a new user
export interface ICreateUserRequest {
  firstName: string;
  lastName: string;
  dateOfBirth: string; // Format: YYYY-MM-DD
  joinedDate: string; // Format: YYYY-MM-DD
  gender: GenderEnum;
  type: UserTypeEnum;
}

export interface IUpdateUserRequest {
  dateOfBirth?: string;
  gender?: GenderEnum;
  joinedDate?: string;
  type?: UserTypeEnum;
}

export interface IUserParams {
  searchTerm: string;
  sortBy?: string;
  _apiSortBy?: string;
  userType?: UserType;
  pageNumber?: number;
  pageSize?: number;
  location?: string;
}

export type Role = "Staff" | "Admin";

export interface User {
  staffCode: string;
  type: Role;
  firstName: string;
  lastName: string;
  username: string;
  joinedDate: string;
  isPasswordUpdated: boolean;
}
