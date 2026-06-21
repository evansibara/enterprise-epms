import { ReactNode } from "react";
import { Sidebar } from "@/widgets/sidebar/Sidebar";
import { Navbar } from "@/widgets/navbar/Navbar";

export function AppLayout({ children }: { children: ReactNode }) {
  return (
    <div className="flex min-h-screen bg-slate-50">
      <Sidebar />
      <div className="flex flex-1 flex-col">
        <Navbar />
        <main className="flex-1 p-6">{children}</main>
      </div>
    </div>
  );
}
