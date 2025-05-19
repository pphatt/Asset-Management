type Role = "Staff" | "Admin";

export interface User {
  id: string;
  role: Role;
  email: string;
  name: string;
  date_of_birth: string;
  address: string;
  avatar: string;
  phone: string;
  createdAt: string;
  updateAt?: string;
}
