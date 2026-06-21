import { Routes, Route, Navigate } from "react-router-dom";
import { LoginPage } from "@/pages/auth/login/LoginPage";
import { RegisterPage } from "@/pages/auth/register/RegisterPage";
import { DashboardPage } from "@/pages/dashboard/DashboardPage";
import { ProjectListPage } from "@/pages/projects/ProjectListPage";
import { ProjectDetailPage } from "@/pages/projects/ProjectDetailPage";
import { KanbanBoardPage } from "@/pages/tasks/KanbanBoardPage";
import { SettingsPage } from "@/pages/settings/SettingsPage";
import { ProtectedRoute } from "@/features/rbac/ProtectedRoute";

export function AppRouter() {
  return (
    <Routes>
      {/* Public routes */}
      <Route path="/login" element={<LoginPage />} />
      <Route path="/register" element={<RegisterPage />} />

      {/* Protected routes — semua role yang sudah login. Nilai role
          HARUS PascalCase ("Admin"/"Manager"/"Employee"), sama dengan
          hasil serialisasi enum UserRole dari backend. */}
      <Route element={<ProtectedRoute allowedRoles={["Admin", "Manager", "Employee"]} />}>
        <Route path="/" element={<DashboardPage />} />
        <Route path="/projects" element={<ProjectListPage />} />
        <Route path="/projects/:projectId" element={<ProjectDetailPage />} />
        <Route path="/projects/:projectId/board" element={<KanbanBoardPage />} />
        <Route path="/settings" element={<SettingsPage />} />
      </Route>

      {/* Contoh route khusus Admin/Manager, lihat RBAC di section 4.2 spesifikasi */}
      <Route element={<ProtectedRoute allowedRoles={["Admin", "Manager"]} />}>
        {/* <Route path="/projects/new" element={<CreateProjectPage />} /> */}
      </Route>

      <Route path="*" element={<Navigate to="/" replace />} />
    </Routes>
  );
}