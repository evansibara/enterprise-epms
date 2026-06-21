import { useQuery } from "@tanstack/react-query";
import { AppLayout } from "@/shared/ui/AppLayout";
import { Card } from "@/shared/ui/card/Card";
import { dashboardApi } from "@/features/dashboard/api/dashboard-api";

export function DashboardPage() {
  const { data, isLoading } = useQuery({
    queryKey: ["dashboard", "summary"],
    queryFn: () => dashboardApi.getSummary(),
  });

  const metrics = [
    { label: "Total Proyek", value: data?.totalProjects },
    { label: "Tugas Aktif", value: data?.activeTasks },
    { label: "Tugas Selesai", value: data?.completedTasks },
    { label: "Anggota Tim", value: data?.teamMembers },
  ];

  return (
    <AppLayout>
      <h1 className="mb-6 text-2xl font-semibold text-slate-900">Dashboard</h1>
      <div className="grid grid-cols-1 gap-4 sm:grid-cols-2 lg:grid-cols-4">
        {metrics.map((metric) => (
          <Card key={metric.label}>
            <p className="text-sm text-slate-500">{metric.label}</p>
            <p className="mt-2 text-3xl font-bold text-slate-900">
              {isLoading ? "—" : metric.value}
            </p>
          </Card>
        ))}
      </div>
    </AppLayout>
  );
}