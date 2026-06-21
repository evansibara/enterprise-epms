import { useParams } from "react-router-dom";
import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { AppLayout } from "@/shared/ui/AppLayout";
import { KanbanBoard } from "@/widgets/kanban-board/KanbanBoard";
import { taskApi } from "@/features/task-management/api/task-api";
import type { Task, TaskStatus } from "@/entities/task/task.types";

export function KanbanBoardPage() {
  const { projectId } = useParams<{ projectId: string }>();
  const queryClient = useQueryClient();
  const taskQueryKey = ["tasks", projectId];

  const { data: tasks, isLoading } = useQuery({
    queryKey: taskQueryKey,
    queryFn: () => taskApi.listByProject(projectId!),
    enabled: Boolean(projectId),
  });

  const updateStatusMutation = useMutation({
    mutationFn: ({ taskId, status }: { taskId: string; status: TaskStatus }) =>
      taskApi.updateStatus(taskId, status),

    onMutate: async ({ taskId, status }) => {
      await queryClient.cancelQueries({ queryKey: taskQueryKey });

      const previousTasks = queryClient.getQueryData<Task[]>(taskQueryKey);

      queryClient.setQueryData<Task[]>(taskQueryKey, (old) =>
        old?.map((task) => (task.id === taskId ? { ...task, status } : task)),
      );

      return { previousTasks };
    },

    onError: (_err, _variables, context) => {
      if (context?.previousTasks) {
        queryClient.setQueryData(taskQueryKey, context.previousTasks);
      }
    },

    onSettled: () => {
      queryClient.invalidateQueries({ queryKey: taskQueryKey });
    },
  });

  const handleTaskStatusChange = (taskId: string, newStatus: TaskStatus) => {
    updateStatusMutation.mutate({ taskId, status: newStatus });
  };

  return (
    <AppLayout>
      <h1 className="mb-6 text-2xl font-semibold text-slate-900">
        Task Board
      </h1>

      {isLoading && <p className="text-sm text-slate-500">Memuat tugas...</p>}
      {tasks && (
        <KanbanBoard tasks={tasks} onTaskStatusChange={handleTaskStatusChange} />
      )}
    </AppLayout>
  );
}