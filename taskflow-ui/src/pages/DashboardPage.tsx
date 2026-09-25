import { useEffect, useState } from 'react';
import { useAuth } from '../authContext';
import { tasksApi } from '../api';
import type { Task, CreateTaskRequest } from '../api';
import TaskCard from '../components/TaskCard';
import CreateTaskModal from '../components/CreateTaskModal';

type Filter = 'all' | 'active' | 'done';

export default function DashboardPage() {
  const { email, logout } = useAuth();
  const [tasks, setTasks] = useState<Task[]>([]);
  const [loading, setLoading] = useState(true);
  const [filter, setFilter] = useState<Filter>('all');
  const [showModal, setShowModal] = useState(false);
  const [error, setError] = useState('');

  const loadTasks = async () => {
    try {
      setError('');
      const res = await tasksApi.list();
      setTasks(res.data);
    } catch (err: any) {
      setError(err.response?.data?.error || 'Failed to load tasks');
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    loadTasks();
  }, []);

  const handleCreate = async (data: CreateTaskRequest) => {
    await tasksApi.create(data);
    await loadTasks();
  };

  const handleToggle = async (id: number, isDone: boolean) => {
    setTasks((prev) =>
      prev.map((t) => (t.id === id ? { ...t, isDone } : t))
    );
    try {
      await tasksApi.update(id, { isDone });
    } catch {
      await loadTasks();
    }
  };

  const handleDelete = async (id: number) => {
    if (!confirm('Delete this task?')) return;
    setTasks((prev) => prev.filter((t) => t.id !== id));
    try {
      await tasksApi.delete(id);
    } catch {
      await loadTasks();
    }
  };

  const filtered = tasks.filter((t) => {
    if (filter === 'active') return !t.isDone;
    if (filter === 'done') return t.isDone;
    return true;
  });

  const stats = {
    total: tasks.length,
    done: tasks.filter((t) => t.isDone).length,
    active: tasks.filter((t) => !t.isDone).length,
    overdue: tasks.filter(
      (t) => t.dueDate && !t.isDone && new Date(t.dueDate) < new Date()
    ).length,
  };

  return (
    <div className="min-h-screen bg-dark-900 p-4 md:p-8">
      <div className="max-w-4xl mx-auto">
        {/* Header */}
        <div className="flex justify-between items-center mb-8">
          <h1 className="text-3xl font-bold text-neon-400">
            TaskFlow<span className="text-white">.</span>
          </h1>
          <div className="flex items-center gap-4">
            <span className="text-gray-400 text-sm hidden sm:inline">{email}</span>
            <button
              onClick={logout}
              className="px-4 py-2 border border-dark-700 rounded-lg text-gray-400 hover:text-neon-400 hover:border-neon-400 text-sm"
            >
              Logout
            </button>
          </div>
        </div>

        {/* Stats */}
        <div className="grid grid-cols-2 md:grid-cols-4 gap-4 mb-8">
          {[
            { label: 'Total', value: stats.total, color: 'text-white' },
            { label: 'Active', value: stats.active, color: 'text-neon-400' },
            { label: 'Done', value: stats.done, color: 'text-green-400' },
            { label: 'Overdue', value: stats.overdue, color: 'text-red-400' },
          ].map((s) => (
            <div
              key={s.label}
              className="bg-dark-800 border border-dark-700 rounded-lg p-4"
            >
              <p className="text-xs text-gray-500 uppercase tracking-wide">{s.label}</p>
              <p className={`text-2xl font-bold mt-1 ${s.color}`}>{s.value}</p>
            </div>
          ))}
        </div>

        {/* Controls */}
        <div className="flex justify-between items-center mb-4 gap-4 flex-wrap">
          <div className="flex gap-2">
            {(['all', 'active', 'done'] as Filter[]).map((f) => (
              <button
                key={f}
                onClick={() => setFilter(f)}
                className={`px-4 py-2 rounded-lg text-sm font-medium transition-colors ${
                  filter === f
                    ? 'bg-neon-400 text-dark-950'
                    : 'bg-dark-800 text-gray-400 hover:text-white'
                }`}
              >
                {f.charAt(0).toUpperCase() + f.slice(1)}
              </button>
            ))}
          </div>
          <button
            onClick={() => setShowModal(true)}
            className="px-4 py-2 bg-neon-400 text-dark-950 font-semibold rounded-lg hover:bg-neon-500"
          >
            + New Task
          </button>
        </div>

        {/* Error */}
        {error && (
          <div className="mb-4 p-3 bg-red-900/40 border border-red-700 text-red-300 rounded">
            {error}
          </div>
        )}

        {/* Tasks */}
        {loading ? (
          <div className="text-center text-gray-500 py-12">Loading...</div>
        ) : filtered.length === 0 ? (
          <div className="text-center text-gray-500 py-12 bg-dark-800 border border-dark-700 rounded-xl">
            {filter === 'all' ? 'No tasks yet. Create one!' : `No ${filter} tasks.`}
          </div>
        ) : (
          <div className="space-y-3">
            {filtered.map((t) => (
              <TaskCard
                key={t.id}
                task={t}
                onToggle={handleToggle}
                onDelete={handleDelete}
              />
            ))}
          </div>
        )}
      </div>

      {showModal && (
        <CreateTaskModal
          onClose={() => setShowModal(false)}
          onCreate={handleCreate}
        />
      )}
    </div>
  );
}