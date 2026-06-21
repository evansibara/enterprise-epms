import { ReactNode } from "react";
import { QueryClientProvider } from "@/app/providers/query-client";
import { BrowserRouter } from "react-router-dom";

interface AppProvidersProps {
  children: ReactNode;
}

/**
 * Pusat semua provider global aplikasi.
 * Tambahkan provider baru (Theme, Auth Context, dsb) di sini
 * agar App.tsx tetap bersih.
 */
export function AppProviders({ children }: AppProvidersProps) {
  return (
    <QueryClientProvider>
      <BrowserRouter>{children}</BrowserRouter>
    </QueryClientProvider>
  );
}
