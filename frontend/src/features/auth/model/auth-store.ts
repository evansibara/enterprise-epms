import { create } from "zustand";
import type { User } from "@/entities/user/user.types";

interface AuthState {
  user: User | null;
  accessToken: string | null;
  isAuthenticated: boolean;
  setSession: (user: User, accessToken: string) => void;
  clearSession: () => void;
}

/**
 * Client state untuk sesi login.
 * Access token disimpan di memori (state ini),
 * Refresh token TIDAK disimpan di sini — ia hidup di
 * HttpOnly cookie yang diatur oleh backend (lihat section 3.4 spesifikasi).
 */
export const useAuthStore = create<AuthState>((set) => ({
  user: null,
  accessToken: null,
  isAuthenticated: false,
  setSession: (user, accessToken) =>
    set({ user, accessToken, isAuthenticated: true }),
  clearSession: () =>
    set({ user: null, accessToken: null, isAuthenticated: false }),
}));
