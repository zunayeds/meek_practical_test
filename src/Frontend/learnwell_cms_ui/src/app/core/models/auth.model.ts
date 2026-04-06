export interface LoginRequest {
  userName: string;
  password: string;
}

export interface LoginResponse {
  email: string;
  roles: string[];
  token: string;
  expiresAt: string;
}

export interface CurrentUser {
  userId: string;
  email: string;
  firstName: string;
  lastName: string;
  role: string;
  studentId?: string;
}
