export interface User {
  id: string;
  name: string;
  email: string;
}

export interface AuthResponse {
  token: string;
  user: User;
}

export interface Project {
  id: string;
  name: string;
  description: string;
  startDate: string;
  expectedEndDate: string;
  state: string;
}

export interface KanbanTask {
  id: string;
  title: string;
  description: string;
  priority: string;
  order: number;
  createdAt: string;
  columnId: string;
  assigneeId: string;
}

export interface KanbanColumn {
  id: string;
  name: string;
  order: number;
  projectId: string;
  tasks?: KanbanTask[];
}
