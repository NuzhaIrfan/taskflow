import { useState } from 'react';
import type { CreateTaskRequest } from '../api';

interface Props {
  onClose: () => void;
  onCreate: (data: CreateTaskRequest) => Promise<void>;
}

export default function CreateTaskModal({ onClose, onCreate }: Props) {
  const [title, setTitle] = useState('');
  const [description, setDescription] = useState('');
  const [priority, setPriority] = useState(1);
  const [dueDate, setDueDate] = useState('');
  const [loading, setLoading] = useState(false);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!title.trim()) return;
    setLoading(true);
    try {
      await onCreate({
        title: title.trim(),
        description: description.trim() || undefined,
        priority,
        dueDate: dueDate ? new Date(dueDate).toISOString() : undefined,
      });
      onClose();
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="fixed inset-0 bg-black/60 flex items-center justify-center z-50 p-4">
      <div className="bg-dark-800 border border-dark-700 rounded-xl w-full max-w-md p-6">
        <div className="flex justify-between items-center mb-4">
          <h2 className="text-xl font-bold text-neon-400">New Task</h2>
          <button onClick={onClose} className="text-gray-500 hover:text-white">
            ✕
          </button>
        </div>

        <form onSubmit={handleSubmit} className="space-y-3">
          <input
            type="text"
            placeholder="Task title *"
            value={title}
            onChange={(e) => setTitle((e.target as HTMLInputElement).value)}
            required
            autoFocus
            className="w-full p-3 bg-dark-900 border border-dark-700 rounded-lg text-white focus:outline-none focus:border-neon-400"
          />

          <textarea
            placeholder="Description (optional)"
            value={description}
            onChange={(e) => setDescription((e.target as HTMLTextAreaElement).value)}
            rows={3}
            className="w-full p-3 bg-dark-900 border border-dark-700 rounded-lg text-white focus:outline-none focus:border-neon-400"
          />

          <div className="grid grid-cols-2 gap-3">
            <select
              value={priority}
              onChange={(e) => setPriority(Number((e.target as HTMLSelectElement).value))}
              className="p-3 bg-dark-900 border border-dark-700 rounded-lg text-white focus:outline-none focus:border-neon-400"
            >
              <option value={0}>🟢 Low</option>
              <option value={1}>🟡 Medium</option>
              <option value={2}>🔴 High</option>
            </select>

            <input
              type="date"
              value={dueDate}
              onChange={(e) => setDueDate((e.target as HTMLInputElement).value)}
              className="p-3 bg-dark-900 border border-dark-700 rounded-lg text-white focus:outline-none focus:border-neon-400"
            />
          </div>

          <div className="flex gap-3 pt-2">
            <button
              type="button"
              onClick={onClose}
              className="flex-1 py-2 border border-dark-700 rounded-lg text-gray-400 hover:text-white"
            >
              Cancel
            </button>
            <button
              type="submit"
              disabled={loading || !title.trim()}
              className="flex-1 py-2 bg-neon-400 text-dark-950 font-semibold rounded-lg hover:bg-neon-500 disabled:opacity-50"
            >
              {loading ? 'Creating...' : 'Create Task'}
            </button>
          </div>
        </form>
      </div>
    </div>
  );
}