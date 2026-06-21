import { AppProviders } from "@/app/providers";
import { AppRouter } from "@/app/providers/router";

export function App() {
  return (
    <AppProviders>
      <AppRouter />
    </AppProviders>
  );
}
