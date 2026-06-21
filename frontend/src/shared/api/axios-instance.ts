import axios from "axios";
import { useAuthStore } from "@/features/auth/model/auth-store";

export const apiClient = axios.create({
  baseURL: import.meta.env.VITE_API_BASE_URL ?? "http://localhost:8080/api/v1",
  withCredentials: true, // wajib agar HttpOnly refresh-token cookie terkirim
});

// Sisipkan access token (short-lived, ~15 menit) di setiap request
apiClient.interceptors.request.use((config) => {
  const token = useAuthStore.getState().accessToken;
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});

// Backend selalu membungkus response sukses dalam format konsisten:
//   { success: true, data: <payload asli>, message, errors: null }
// Interceptor ini "membongkar" pembungkus tersebut secara terpusat,
// sehingga res.data di seluruh *-api.ts (auth-api, project-api, task-api,
// dst) langsung berisi <payload asli> tanpa perlu res.data.data manual
// di setiap tempat — dan tidak rawan lupa/salah di salah satu file.
apiClient.interceptors.response.use(
  (response) => {
    if (
      response.data &&
      typeof response.data === "object" &&
      "success" in response.data &&
      "data" in response.data
    ) {
      response.data = response.data.data;
    }
    return response;
  },
  async (error) => {
    if (error.response?.status === 401) {
      // TODO: implementasikan refresh-token rotation di sini.
      // Saat backend mengembalikan 401, panggil endpoint /auth/refresh
      // (cookie HttpOnly dikirim otomatis), lalu retry request asli.
      useAuthStore.getState().clearSession();
    }
    return Promise.reject(error);
  },
);