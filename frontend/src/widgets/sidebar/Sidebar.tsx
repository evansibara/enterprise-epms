import { NavLink } from "react-router-dom";
import { LayoutDashboard, FolderKanban, Settings } from "lucide-react";
import { cn } from "@/shared/lib/cn";

const navItems = [
  { to: "/", label: "Dashboard", icon: LayoutDashboard },
  { to: "/projects", label: "Projects", icon: FolderKanban },
  { to: "/settings", label: "Settings", icon: Settings },
];

export function Sidebar() {
  return (
    <aside className="hidden w-60 flex-col border-r border-slate-200 bg-white p-4 md:flex">
      <div className="mb-6 px-2 text-lg font-bold text-primary-600">EPMS</div>
      <nav className="space-y-1">
        {navItems.map(({ to, label, icon: Icon }) => (
          <NavLink
            key={to}
            to={to}
            className={({ isActive }) =>
              cn(
                "flex items-center gap-3 rounded-lg px-3 py-2 text-sm font-medium text-slate-600 hover:bg-slate-100",
                isActive && "bg-primary-50 text-primary-700",
              )
            }
          >
            <Icon className="h-4 w-4" />
            {label}
          </NavLink>
        ))}
      </nav>
    </aside>
  );
}
