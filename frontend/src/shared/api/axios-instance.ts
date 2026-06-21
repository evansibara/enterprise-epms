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
//
// Khusus respons file (responseType: "blob", lihat attachment-api.ts
// download()), body BUKAN format ApiResponse di atas — jadi dibiarkan apa
// adanya, jangan dibongkar.
apiClient.interceptors.response.use(
  (response) => {
    if (
      response.data &&
      typeof response.data === "object" &&
      !(response.data instanceof Blob) &&
      "success" in response.data &&
      "data" in response.data
    ) {
      response.data = response.data.data;
    }
    return response;
  },
  async (error) => {
    const originalRequest = error.config;

    // Hanya coba refresh sekali per request (flag _retry), dan jangan coba
    // refresh untuk request /auth/* itu sendiri (login/refresh gagal = sesi
    // benar-benar habis, bukan token expired biasa).
    const isAuthEndpoint = originalRequest?.url?.startsWith("/auth/");

    if (error.response?.status === 401 && !originalRequest?._retry && !isAuthEndpoint) {
      originalRequest._retry = true;

      try {
        // Refresh-token cookie (HttpOnly) terkirim otomatis lewat withCredentials.
        // Backend mengembalikan access token baru + data user.
        const refreshed = await refreshAccessToken();
        originalRequest.headers.Authorization = `Bearer ${refreshed.accessToken}`;
        return apiClient(originalRequest);
      } catch (refreshError) {
        useAuthStore.getState().clearSession();
        window.location.href = "/login";
        return Promise.reject(refreshError);
      }
    }

    return Promise.reject(error);
  },
);

// --- Refresh queue ---------------------------------------------------------
// Mencegah beberapa request yang gagal bersamaan (401 di waktu yang sama)
// memicu beberapa kali panggilan /auth/refresh secara paralel. Request
// pertama yang memicu refresh, request berikutnya menumpang hasil yang sama.
//
// Catatan: endpoint /auth/refresh backend HANYA mengembalikan access token
// baru (lihat RefreshTokenResponseDto), TIDAK ada data user di dalamnya —
// jadi data user yang sudah tersimpan di auth-store tidak disentuh sama
// sekali di sini, hanya token-nya yang diganti lewat setAccessToken().
interface RefreshResponse {
  accessToken: string;
  accessTokenExpiresAt: string;
}

let refreshPromise: Promise<RefreshResponse> | null = null;

async function refreshAccessToken(): Promise<RefreshResponse> {
  if (!refreshPromise) {
    refreshPromise = apiClient
      .post<RefreshResponse>("/auth/refresh")
      .then((res) => {
        useAuthStore.getState().setAccessToken(res.data.accessToken);
        return res.data;
      })
      .finally(() => {
        refreshPromise = null;
      });
  }

  return refreshPromise;
}
