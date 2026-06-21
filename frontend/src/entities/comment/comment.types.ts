export interface Comment {
  id: string;
  taskId: string;
  content: string;
  createdByUserId: string;
  createdByUserName: string | null;
  createdAt: string;
}
