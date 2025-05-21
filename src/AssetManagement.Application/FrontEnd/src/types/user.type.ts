// Define user types for the application

import { UserType } from "../constants/user-params";

export type IUserType = "Admin" | "Staff";

export interface IUser {
  staffCode: string;
  firstName: string;
  lastName: string;
  username: string;
  joinedDate: string; // Format: ISO 8601
  type: IUserType;
  isPasswordUpdated: boolean;
}

// Extension of the base IUser interface for creating a new user
export interface ICreateUserRequest {
  firstName: string;
  lastName: string;
  dateOfBirth: string; // Format: YYYY-MM-DD
  joinedDate: string; // Format: YYYY-MM-DD
  gender: "Male" | "Female";
  type: IUserType;
  location?: string; // Will be set to current admin's location by default
}

export interface IUpdateUserRequest {
  id: number;
  dateOfBirth?: string;
  gender?: "Male" | "Female";
  joinedDate?: string;
  type?: IUserType;
}

export interface IUserParams {
  searchTerm: string;
  sortBy?: string; // In format 'fieldName:asc' or 'fieldName:desc'
  _apiSortBy?: string; // Used internally for API communication
  userType?: UserType;
  pageNumber?: number;
  pageSize?: number;
  location?: string; // Add location for filtering by admin location
}
type Role = "Staff" | "Admin";

export interface User {
  staffCode: string;
  type: Role;
  firstName: string;
  lastName: string;
  username: string;
  joinedDate: string;
  isPasswordUpdated: boolean;
}
