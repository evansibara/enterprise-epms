import { apiClient } from "@/shared/api/axios-instance";
import type { User } from "@/entities/user/user.types";

export interface LoginPayload {
  email: string;
  password: string;
}

export interface RegisterPayload {
  name: string;
  email: string;
  password: string;
}

interface AuthResponse {
  user: User;
  accessToken: string;
}

// Mencerminkan EPMS.Application.DTOs.Auth.RefreshTokenResponseDto — backend
// HANYA mengembalikan access token baru di endpoint ini, TIDAK ada data user
// (berbeda dari login). Jangan disamakan dengan AuthResponse di atas.
interface RefreshResponse {
  accessToken: string;
  accessTokenExpiresAt: string;
}

interface RegisterResponse {
  id: string;
  name: string;
  email: string;
  role: string;
}

export const authApi = {
  login: (payload: LoginPayload) =>
    apiClient.post<AuthResponse>("/auth/login", payload).then((res) => res.data),

  register: (payload: RegisterPayload) =>
    apiClient.post<RegisterResponse>("/auth/register", payload).then((res) => res.data),

  logout: () => apiClient.post("/auth/logout"),

  refresh: () =>
    apiClient.post<RefreshResponse>("/auth/refresh").then((res) => res.data),
};