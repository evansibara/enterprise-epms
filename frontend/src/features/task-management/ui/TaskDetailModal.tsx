import { useEffect, useRef, useState } from "react";
import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { Paperclip, Download, Trash2, Send, Loader2 } from "lucide-react";
import { Modal } from "@/shared/ui/modal/Modal";
import { Button } from "@/shared/ui/button/Button";
import { Input } from "@/shared/ui/input/Input";
import { Avatar } from "@/shared/ui/avatar/Avatar";
import { formatDate, formatRelativeTime } from "@/shared/lib/format-date";
import { useAuthStore } from "@/features/auth/model/auth-store";
import { taskApi } from "@/features/task-management/api/task-api";
import { attachmentApi } from "@/features/task-management/api/attachment-api";
import { commentApi } from "@/features/task-management/api/comment-api";
import { activityLogApi } from "@/features/project-management/api/activity-log-api";
import { userApi } from "@/features/user-management/api/user-api";
import {
  ALLOWED_ATTACHMENT_MIME_TYPES,
  MAX_ATTACHMENT_SIZE_BYTES,
} from "@/entities/attachment/attachment.types";
import type { TaskPriority, TaskStatus } from "@/entities/task/task.types";

interface TaskDetailModalProps {
  open: boolean;
  onClose: () => void;
  taskId: string | null;
  projectId: string;
}

const priorityOptions: TaskPriority[] = ["Low", "Medium", "High", "Urgent"];
const statusOptions: { value: TaskStatus; label: string }[] = [
  { value: "ToDo", label: "To Do" },
  { value: "InProgress", label: "In Progress" },
  { value: "Review", label: "Review" },
  { value: "Done", label: "Done" },
];

function describeActivity(action: string): string {
  switch (action) {
    case "Created":
      return "membuat task ini";
    case "Updated":
      return "memperbarui detail task";
    case "StatusChanged":
      return "mengubah status task";
    case "Assigned":
      return "menugaskan task ini";
    case "Deleted":
      return "menghapus task ini";
    case "AttachmentAdded":
      return "menambahkan lampiran";
    case "AttachmentRemoved":
      return "menghapus lampiran";
    case "CommentAdded":
      return "menambahkan komentar";
    case "CommentRemoved":
      return "menghapus komentar";
    default:
      return action;
  }
}

function formatFileSize(bytes: number): string {
  if (bytes < 1024) return `${bytes} B`;
  if (bytes < 1024 * 1024) return `${(bytes / 1024).toFixed(1)} KB`;
  return `${(bytes / (1024 * 1024)).toFixed(1)} MB`;
}

export function TaskDetailModal({ open, onClose, taskId, projectId }: TaskDetailModalProps) {
  const queryClient = useQueryClient();
  const currentUser = useAuthStore((state) => state.user);
  const fileInputRef = useRef<HTMLInputElement>(null);

  const [title, setTitle] = useState("");
  const [description, setDescription] = useState("");
  const [priority, setPriority] = useState<TaskPriority>("Medium");
  const [dueDate, setDueDate] = useState("");
  const [assigneeId, setAssigneeId] = useState("");
  const [commentText, setCommentText] = useState("");
  const [fileError, setFileError] = useState<string | null>(null);

  const taskQueryKey = ["tasks", "detail", taskId];
  const tasksByProjectKey = ["tasks", projectId];

  const { data: task, isLoading: isTaskLoading } = useQuery({
    queryKey: taskQueryKey,
    queryFn: () => taskApi.getById(taskId!),
    enabled: open && Boolean(taskId),
  });

  const { data: usersResult } = useQuery({
    queryKey: ["users", "lookup"],
    queryFn: () => userApi.list(),
    enabled: open,
  });

  const { data: attachments, isLoading: isAttachmentsLoading } = useQuery({
    queryKey: ["attachments", taskId],
    queryFn: () => attachmentApi.listByTask(taskId!),
    enabled: open && Boolean(taskId),
  });

  const { data: comments, isLoading: isCommentsLoading } = useQuery({
    queryKey: ["comments", taskId],
    queryFn: () => commentApi.listByTask(taskId!),
    enabled: open && Boolean(taskId),
  });

  const { data: activityLogs, isLoading: isLogsLoading } = useQuery({
    queryKey: ["activity-logs", "task", taskId],
    queryFn: () => activityLogApi.listByTask(taskId!),
    enabled: open && Boolean(taskId),
  });

  // Isi form setiap kali task yang dibuka berubah.
  useEffect(() => {
    if (task) {
      setTitle(task.title);
      setDescription(task.description ?? "");
      setPriority(task.priority);
      setDueDate(task.dueDate ? task.dueDate.slice(0, 10) : "");
      setAssigneeId(task.assigneeId ?? "");
    }
  }, [task]);

  const invalidateTaskViews = () => {
    queryClient.invalidateQueries({ queryKey: taskQueryKey });
    queryClient.invalidateQueries({ queryKey: tasksByProjectKey });
  };

  const updateMutation = useMutation({
    mutationFn: () =>
      taskApi.update(taskId!, {
        title,
        description,
        priority,
        dueDate: dueDate ? new Date(dueDate).toISOString() : null,
      }),
    onSuccess: invalidateTaskViews,
  });

  const statusMutation = useMutation({
    mutationFn: (newStatus: TaskStatus) => taskApi.updateStatus(taskId!, newStatus),
    onSuccess: invalidateTaskViews,
  });

  const assignMutation = useMutation({
    mutationFn: (newAssigneeId: string) => taskApi.assign(taskId!, newAssigneeId),
    onSuccess: invalidateTaskViews,
  });

  const uploadMutation = useMutation({
    mutationFn: (file: File) => attachmentApi.upload(taskId!, file),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["attachments", taskId] });
      queryClient.invalidateQueries({ queryKey: ["activity-logs", "task", taskId] });
    },
  });

  const deleteAttachmentMutation = useMutation({
    mutationFn: (attachmentId: string) => attachmentApi.remove(attachmentId),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["attachments", taskId] });
      queryClient.invalidateQueries({ queryKey: ["activity-logs", "task", taskId] });
    },
  });

  const addCommentMutation = useMutation({
    mutationFn: (content: string) => commentApi.create(taskId!, content),
    onSuccess: () => {
      setCommentText("");
      queryClient.invalidateQueries({ queryKey: ["comments", taskId] });
      queryClient.invalidateQueries({ queryKey: ["activity-logs", "task", taskId] });
    },
  });

  const deleteCommentMutation = useMutation({
    mutationFn: (commentId: string) => commentApi.remove(commentId),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["comments", taskId] });
      queryClient.invalidateQueries({ queryKey: ["activity-logs", "task", taskId] });
    },
  });

  const handleFileSelect = (file: File | undefined) => {
    setFileError(null);
    if (!file) return;

    if (!ALLOWED_ATTACHMENT_MIME_TYPES.includes(file.type as never)) {
      setFileError("Tipe file tidak didukung. Hanya PDF, PNG, atau JPG.");
      return;
    }

    if (file.size > MAX_ATTACHMENT_SIZE_BYTES) {
      setFileError("Ukuran file maksimal 5MB.");
      return;
    }

    uploadMutation.mutate(file);
  };

  if (!open) return null;

  return (
    <Modal open={open} onClose={onClose} title="Detail Task" widthClassName="max-w-2xl">
      {isTaskLoading && <p className="text-sm text-slate-500">Memuat task...</p>}

      {task && (
        <div className="space-y-6">
          {/* --- Metadata --- */}
          <div className="space-y-4">
            <div>
              <label className="mb-1 block text-sm font-medium text-slate-700">Judul</label>
              <Input value={title} onChange={(e) => setTitle(e.target.value)} />
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

            <div className="grid grid-cols-2 gap-3 sm:grid-cols-4">
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
                <label className="mb-1 block text-sm font-medium text-slate-700">Status</label>
                <select
                  value={task.status}
                  onChange={(e) => statusMutation.mutate(e.target.value as TaskStatus)}
                  className="w-full rounded-lg border border-slate-300 bg-white px-3 py-2 text-sm outline-none focus:border-primary-500 focus:ring-2 focus:ring-primary-100"
                >
                  {statusOptions.map((s) => (
                    <option key={s.value} value={s.value}>{s.label}</option>
                  ))}
                </select>
              </div>

              <div>
                <label className="mb-1 block text-sm font-medium text-slate-700">Assignee</label>
                <select
                  value={assigneeId}
                  onChange={(e) => {
                    setAssigneeId(e.target.value);
                    if (e.target.value) assignMutation.mutate(e.target.value);
                  }}
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

            <div className="flex justify-end">
              <Button onClick={() => updateMutation.mutate()} disabled={updateMutation.isPending}>
                {updateMutation.isPending ? "Menyimpan..." : "Simpan Perubahan"}
              </Button>
            </div>
          </div>

          <hr className="border-slate-100" />

          {/* --- Attachments --- */}
          <div>
            <h3 className="mb-2 text-sm font-semibold text-slate-700">Lampiran</h3>

            <div
              onDragOver={(e) => e.preventDefault()}
              onDrop={(e) => {
                e.preventDefault();
                handleFileSelect(e.dataTransfer.files?.[0]);
              }}
              onClick={() => fileInputRef.current?.click()}
              className="flex cursor-pointer flex-col items-center justify-center rounded-lg border-2 border-dashed border-slate-200 px-4 py-6 text-center hover:border-primary-300"
            >
              <Paperclip className="mb-2 h-5 w-5 text-slate-400" />
              <p className="text-sm text-slate-500">
                Drag &amp; drop file di sini, atau klik untuk memilih
              </p>
              <p className="text-xs text-slate-400">PDF, PNG, JPG — maks 5MB</p>
              <input
                ref={fileInputRef}
                type="file"
                className="hidden"
                accept=".pdf,.png,.jpg,.jpeg"
                onChange={(e) => handleFileSelect(e.target.files?.[0])}
              />
            </div>

            {fileError && <p className="mt-2 text-xs text-red-600">{fileError}</p>}
            {uploadMutation.isPending && (
              <p className="mt-2 flex items-center gap-1 text-xs text-slate-500">
                <Loader2 className="h-3 w-3 animate-spin" /> Mengupload file...
              </p>
            )}

            <div className="mt-3 space-y-2">
              {isAttachmentsLoading && <p className="text-xs text-slate-400">Memuat lampiran...</p>}
              {attachments?.length === 0 && (
                <p className="text-xs text-slate-400">Belum ada file dilampirkan.</p>
              )}
              {attachments?.map((attachment) => (
                <div
                  key={attachment.id}
                  className="flex items-center justify-between rounded-lg border border-slate-100 px-3 py-2 text-sm"
                >
                  <div className="min-w-0 flex-1">
                    <p className="truncate font-medium text-slate-700">{attachment.fileName}</p>
                    <p className="text-xs text-slate-400">{formatFileSize(attachment.sizeBytes)}</p>
                  </div>
                  <div className="flex items-center gap-1">
                    <button
                      type="button"
                      onClick={() => attachmentApi.download(attachment.id, attachment.fileName)}
                      className="rounded-md p-1.5 text-slate-400 hover:bg-slate-100 hover:text-slate-600"
                      aria-label="Download"
                    >
                      <Download className="h-4 w-4" />
                    </button>
                    <button
                      type="button"
                      onClick={() => deleteAttachmentMutation.mutate(attachment.id)}
                      className="rounded-md p-1.5 text-slate-400 hover:bg-red-50 hover:text-red-600"
                      aria-label="Hapus"
                    >
                      <Trash2 className="h-4 w-4" />
                    </button>
                  </div>
                </div>
              ))}
            </div>
          </div>

          <hr className="border-slate-100" />

          {/* --- Activity log + comments timeline --- */}
          <div>
            <h3 className="mb-2 text-sm font-semibold text-slate-700">Aktivitas &amp; Komentar</h3>

            <div className="max-h-64 space-y-3 overflow-y-auto pr-1">
              {(isLogsLoading || isCommentsLoading) && (
                <p className="text-xs text-slate-400">Memuat riwayat...</p>
              )}

              {[
                ...(activityLogs ?? []).map((log) => ({
                  kind: "activity" as const,
                  id: log.id,
                  timestamp: log.timestamp,
                  actorName: log.performedByUserName,
                  text: describeActivity(log.action),
                  authorId: undefined as string | undefined,
                })),
                ...(comments ?? []).map((comment) => ({
                  kind: "comment" as const,
                  id: comment.id,
                  timestamp: comment.createdAt,
                  actorName: comment.createdByUserName,
                  text: comment.content,
                  authorId: comment.createdByUserId as string | undefined,
                })),
              ]
                .sort((a, b) => new Date(a.timestamp).getTime() - new Date(b.timestamp).getTime())
                .map((item) => (
                  <div key={`${item.kind}-${item.id}`} className="flex gap-2">
                    <Avatar name={item.actorName} className="mt-0.5 shrink-0" />
                    <div className="min-w-0 flex-1">
                      <p className="text-sm text-slate-700">
                        <span className="font-medium text-slate-900">
                          {item.actorName ?? "Seseorang"}
                        </span>{" "}
                        {item.kind === "activity" ? (
                          item.text
                        ) : (
                          <span className="text-slate-600">&ldquo;{item.text}&rdquo;</span>
                        )}
                      </p>
                      <div className="flex items-center gap-2">
                        <p className="text-xs text-slate-400">{formatRelativeTime(item.timestamp)}</p>
                        {item.kind === "comment" && item.authorId === currentUser?.id && (
                          <button
                            type="button"
                            onClick={() => deleteCommentMutation.mutate(item.id)}
                            className="text-xs text-slate-400 hover:text-red-600"
                          >
                            Hapus
                          </button>
                        )}
                      </div>
                    </div>
                  </div>
                ))}

              {!isLogsLoading &&
                !isCommentsLoading &&
                (activityLogs?.length ?? 0) === 0 &&
                (comments?.length ?? 0) === 0 && (
                  <p className="text-xs text-slate-400">Belum ada aktivitas.</p>
                )}
            </div>

            <form
              onSubmit={(e) => {
                e.preventDefault();
                if (commentText.trim()) addCommentMutation.mutate(commentText.trim());
              }}
              className="mt-3 flex gap-2"
            >
              <Input
                placeholder="Tulis komentar..."
                value={commentText}
                onChange={(e) => setCommentText(e.target.value)}
              />
              <Button type="submit" disabled={addCommentMutation.isPending || !commentText.trim()}>
                <Send className="h-4 w-4" />
              </Button>
            </form>
          </div>

          <p className="text-right text-xs text-slate-400">
            Dibuat {formatDate(task.createdAt)}
          </p>
        </div>
      )}
    </Modal>
  );
}
