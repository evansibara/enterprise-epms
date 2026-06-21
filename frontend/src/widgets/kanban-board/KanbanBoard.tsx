import { useState } from "react";
import {
  DndContext,
  DragOverlay,
  PointerSensor,
  useDraggable,
  useDroppable,
  useSensor,
  useSensors,
  type DragEndEvent,
  type DragStartEvent,
} from "@dnd-kit/core";
import { Card } from "@/shared/ui/card/Card";
import { Badge } from "@/shared/ui/badge/Badge";
import type { Task, TaskStatus } from "@/entities/task/task.types";

const columns: { status: TaskStatus; label: string }[] = [
  { status: "ToDo", label: "To Do" },
  { status: "InProgress", label: "In Progress" },
  { status: "Review", label: "Review" },
  { status: "Done", label: "Done" },
];

const priorityTone = {
  Low: "neutral",
  Medium: "info",
  High: "warning",
  Urgent: "danger",
} as const;

interface KanbanBoardProps {
  tasks: Task[];
  onTaskStatusChange: (taskId: string, newStatus: TaskStatus) => void;
}

function TaskCard({ task }: { task: Task }) {
  const { attributes, listeners, setNodeRef, transform, isDragging } = useDraggable({
    id: task.id,
  });

  const style = transform
    ? {
        transform: `translate3d(${transform.x}px, ${transform.y}px, 0)`,
        zIndex: 10,
      }
    : undefined;

  return (
    <div
      ref={setNodeRef}
      style={style}
      {...listeners}
      {...attributes}
      className={isDragging ? "opacity-50" : undefined}
    >
      <Card className="cursor-grab active:cursor-grabbing">
        <div className="mb-2 flex items-start justify-between gap-2">
          <p className="text-sm font-medium text-slate-900">{task.title}</p>
          <Badge tone={priorityTone[task.priority]}>{task.priority}</Badge>
        </div>
        <p className="text-xs text-slate-500 line-clamp-2">{task.description}</p>
      </Card>
    </div>
  );
}

function KanbanColumn({
  status,
  label,
  tasks,
}: {
  status: TaskStatus;
  label: string;
  tasks: Task[];
}) {
  const { setNodeRef, isOver } = useDroppable({ id: status });

  return (
    <div
      ref={setNodeRef}
      className={`flex flex-col gap-3 rounded-lg p-2 transition-colors ${
        isOver ? "bg-primary-50" : ""
      }`}
    >
      <div className="flex items-center justify-between px-1">
        <h3 className="text-sm font-semibold text-slate-700">{label}</h3>
        <span className="text-xs text-slate-400">{tasks.length}</span>
      </div>

      <div className="flex min-h-[60px] flex-col gap-2">
        {tasks.map((task) => (
          <TaskCard key={task.id} task={task} />
        ))}
      </div>
    </div>
  );
}

export function KanbanBoard({ tasks, onTaskStatusChange }: KanbanBoardProps) {
  const [activeTask, setActiveTask] = useState<Task | null>(null);

  const sensors = useSensors(
    useSensor(PointerSensor, { activationConstraint: { distance: 8 } }),
  );

  const handleDragStart = (event: DragStartEvent) => {
    const task = tasks.find((t) => t.id === event.active.id);
    setActiveTask(task ?? null);
  };

  const handleDragEnd = (event: DragEndEvent) => {
    setActiveTask(null);

    const { active, over } = event;
    if (!over) return;

    const taskId = String(active.id);
    const newStatus = over.id as TaskStatus;

    const task = tasks.find((t) => t.id === taskId);
    if (!task || task.status === newStatus) {
      return;
    }

    onTaskStatusChange(taskId, newStatus);
  };

  return (
    <DndContext
      sensors={sensors}
      onDragStart={handleDragStart}
      onDragEnd={handleDragEnd}
    >
      <div className="grid grid-cols-1 gap-4 md:grid-cols-4">
        {columns.map((column) => (
          <KanbanColumn
            key={column.status}
            status={column.status}
            label={column.label}
            tasks={tasks.filter((task) => task.status === column.status)}
          />
        ))}
      </div>

      <DragOverlay>
        {activeTask && (
          <Card className="cursor-grabbing shadow-lg">
            <div className="mb-2 flex items-start justify-between gap-2">
              <p className="text-sm font-medium text-slate-900">{activeTask.title}</p>
              <Badge tone={priorityTone[activeTask.priority]}>
                {activeTask.priority}
              </Badge>
            </div>
            <p className="text-xs text-slate-500 line-clamp-2">
              {activeTask.description}
            </p>
          </Card>
        )}
      </DragOverlay>
    </DndContext>
  );
}