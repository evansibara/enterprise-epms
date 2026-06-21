import { useState } from "react";
import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { Modal } from "@/shared/ui/modal/Modal";
import { Input } from "@/shared/ui/input/Input";
import { Button } from "@/shared/ui/button/Button";
import { taskApi } from "@/features/task-management/api/task-api";
import { userApi } from "@/features/user-management/api/user-api";
import type { TaskPriority } from "@/entities/task/task.types";

interface CreateTaskModalProps {
  open: boolean;
  onClose: () => void;
  projectId: string;
}

const priorityOptions: TaskPriority[] = ["Low", "Medium", "High", "Urgent"];

export function CreateTaskModal({ open, onClose, projectId }: CreateTaskModalProps) {
  const queryClient = useQueryClient();

  const [title, setTitle] = useState("");
  const [description, setDescription] = useState("");
  const [priority, setPriority] = useState<TaskPriority>("Medium");
  const [assigneeId, setAssigneeId] = useState("");
  const [dueDate, setDueDate] = useState("");

  const { data: usersResult } = useQuery({
    queryKey: ["users", "lookup"],
    queryFn: () => userApi.list(),
    enabled: open,
  });

  const resetForm = () => {
    setTitle("");
    setDescription("");
    setPriority("Medium");
    setAssigneeId("");
    setDueDate("");
  };

  const createMutation = useMutation({
    mutationFn: () =>
      taskApi.create(projectId, {
        title,
        description,
        priority,
        assigneeId: assigneeId || null,
        dueDate: dueDate ? new Date(dueDate).toISOString() : null,
      }),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["tasks", projectId] });
      resetForm();
      onClose();
    },
  });

  return (
    <Modal open={open} onClose={onClose} title="Tambah Task Baru">
      <form
        onSubmit={(e) => {
          e.preventDefault();
          if (title.trim()) createMutation.mutate();
        }}
        className="space-y-4"
      >
        <div>
          <label className="mb-1 block text-sm font-medium text-slate-700">Judul</label>
          <Input
            placeholder="Mis. Desain wireframe halaman login"
            value={title}
            onChange={(e) => setTitle(e.target.value)}
            required
          />
        </div>

        <div>
          <label className="mb-1 block text-sm font-medium text-slate-700">Deskripsi</label>
          <textarea
            rows={3}
            value={description}
            onChange={(e) => setDescription(e.target.value)}
            className="w-full rounded-lg border border-slate-300 bg-white px-3 py-2 text-sm outline-none focus:border-primary-500 focus:ring-2 focus:ring-primary-100"
          />
        </div>

        <div className="grid grid-cols-1 gap-3 sm:grid-cols-3">
          <div>
            <label className="mb-1 block text-sm font-medium text-slate-700">Priority</label>
            <select
              value={priority}
              onChange={(e) => setPriority(e.target.value as TaskPriority)}
              className="w-full rounded-lg border border-slate-300 bg-white px-3 py-2 text-sm outline-none focus:border-primary-500 focus:ring-2 focus:ring-primary-100"
            >
              {priorityOptions.map((p) => (
                <option key={p} value={p}>{p}</option>
              ))}
            </select>
          </div>

          <div>
            <label className="mb-1 block text-sm font-medium text-slate-700">Assignee</label>
            <select
              value={assigneeId}
              onChange={(e) => setAssigneeId(e.target.value)}
              className="w-full rounded-lg border border-slate-300 bg-white px-3 py-2 text-sm outline-none focus:border-primary-500 focus:ring-2 focus:ring-primary-100"
            >
              <option value="">Belum ditugaskan</option>
              {usersResult?.items.map((u) => (
                <option key={u.id} value={u.id}>{u.name}</option>
              ))}
            </select>
          </div>

          <div>
            <label className="mb-1 block text-sm font-medium text-slate-700">Due Date</label>
            <Input type="date" value={dueDate} onChange={(e) => setDueDate(e.target.value)} />
          </div>
        </div>

        {createMutation.isError && (
          <p className="text-sm text-red-600">Gagal membuat task. Coba lagi.</p>
        )}

        <div className="flex justify-end gap-2 pt-2">
          <Button type="button" variant="secondary" onClick={onClose}>
            Batal
          </Button>
          <Button type="submit" disabled={createMutation.isPending || !title.trim()}>
            {createMutation.isPending ? "Membuat..." : "Buat Task"}
          </Button>
        </div>
      </form>
    </Modal>
  );
}
