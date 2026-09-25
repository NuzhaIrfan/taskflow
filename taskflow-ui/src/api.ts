import axios from 'axios';

export const API_BASE = import.meta.env.VITE_API_BASE || 'http://localhost:5050';

export const api = axios.create({
  baseURL: API_BASE,
  headers: { 'Content-Type': 'application/json' },
});

api.interceptors.request.use((config) => {
  const token = localStorage.getItem('token');
  if (token) config.headers.Authorization = `Bearer ${token}`;
  return config;
});

api.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.response?.status === 401) {
      localStorage.removeItem('token');
      localStorage.removeItem('email');
      if (window.location.pathname !== '/login') {
        window.location.href = '/login';
      }
    }
    return Promise.reject(error);
  }
);

export interface Task {
  id: number;
  title: string;
  description: string | null;
  isDone: boolean;
  priority: number;
  dueDate: string | null;
  createdAt: string;
  userEmail: string;
}

export interface CreateTaskRequest {
  title: string;
  description?: string;
  priority?: number;
  dueDate?: string;
}

export interface AuthResponse {
  token: string;
  email: string;
  userId: number;
}

export const authApi = {
  register: (email: string, password: string) =>
    api.post<AuthResponse>('/auth/register', { email, password }),
  login: (email: string, password: string) =>
    api.post<AuthResponse>('/auth/login', { email, password }),
};

export const tasksApi = {
  list: () => api.get<Task[]>('/tasks'),
  create: (data: CreateTaskRequest) => api.post('/tasks', data),
  update: (id: number, data: Partial<CreateTaskRequest & { isDone: boolean }>) =>
    api.put(`/tasks/${id}`, data),
  delete: (id: number) => api.delete(`/tasks/${id}`),
};