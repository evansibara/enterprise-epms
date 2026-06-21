import { useEffect } from "react";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { z } from "zod";
import { useMutation, useQueryClient } from "@tanstack/react-query";
import { Modal } from "@/shared/ui/modal/Modal";
import { Input } from "@/shared/ui/input/Input";
import { Button } from "@/shared/ui/button/Button";
import { projectApi } from "@/features/project-management/api/project-api";
import type { Project } from "@/entities/project/project.types";

const projectSchema = z.object({
  name: z.string().min(1, "Nama project wajib diisi").max(200),
  description: z.string().max(2000).optional(),
  deadline: z.string().optional(), // input type="date" -> "YYYY-MM-DD"
});

type ProjectFormValues = z.infer<typeof projectSchema>;

interface ProjectFormModalProps {
  open: boolean;
  onClose: () => void;
  /** Jika diisi, modal dalam mode edit; jika kosong, mode create. */
  project?: Project | null;
}

export function ProjectFormModal({ open, onClose, project }: ProjectFormModalProps) {
  const queryClient = useQueryClient();
  const isEditMode = Boolean(project);

  const {
    register,
    handleSubmit,
    reset,
    formState: { errors },
  } = useForm<ProjectFormValues>({ resolver: zodResolver(projectSchema) });

  // Re-isi form setiap kali modal dibuka dengan project yang berbeda
  // (atau dikosongkan untuk mode create).
  useEffect(() => {
    if (open) {
      reset({
        name: project?.name ?? "",
        description: project?.description ?? "",
        deadline: project?.deadline ? project.deadline.slice(0, 10) : "",
      });
    }
  }, [open, project, reset]);

  const saveMutation = useMutation({
    mutationFn: (values: ProjectFormValues) => {
      const payload = {
        name: values.name,
        description: values.description ?? "",
        deadline: values.deadline ? new Date(values.deadline).toISOString() : null,
      };

      return isEditMode && project
        ? projectApi.update(project.id, { ...payload, status: project.status })
        : projectApi.create(payload);
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["projects"] });
      onClose();
    },
  });

  return (
    <Modal
      open={open}
      onClose={onClose}
      title={isEditMode ? "Edit Project" : "Buat Project Baru"}
    >
      <form
        onSubmit={handleSubmit((values) => saveMutation.mutate(values))}
        className="space-y-4"
      >
        <div>
          <label className="mb-1 block text-sm font-medium text-slate-700">
            Nama Project
          </label>
          <Input placeholder="Mis. Migrasi Sistem ERP" {...register("name")} />
          {errors.name && (
            <p className="mt-1 text-xs text-red-600">{errors.name.message}</p>
          )}
        </div>

        <div>
          <label className="mb-1 block text-sm font-medium text-slate-700">
            Deskripsi
          </label>
          <textarea
            rows={3}
            placeholder="Deskripsi singkat project..."
            className="w-full rounded-lg border border-slate-300 bg-white px-3 py-2 text-sm outline-none focus:border-primary-500 focus:ring-2 focus:ring-primary-100"
            {...register("description")}
          />
          {errors.description && (
            <p className="mt-1 text-xs text-red-600">{errors.description.message}</p>
          )}
        </div>

        <div>
          <label className="mb-1 block text-sm font-medium text-slate-700">
            Tenggat Waktu (opsional)
          </label>
          <Input type="date" {...register("deadline")} />
        </div>

        {saveMutation.isError && (
          <p className="text-sm text-red-600">
            Gagal menyimpan project. Periksa kembali data Anda.
          </p>
        )}

        <div className="flex justify-end gap-2 pt-2">
          <Button type="button" variant="secondary" onClick={onClose}>
            Batal
          </Button>
          <Button type="submit" disabled={saveMutation.isPending}>
            {saveMutation.isPending ? "Menyimpan..." : isEditMode ? "Simpan Perubahan" : "Buat Project"}
          </Button>
        </div>
      </form>
    </Modal>
  );
}
