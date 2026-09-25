import type { Task } from '../api';

interface Props {
  task: Task;
  onToggle: (id: number, isDone: boolean) => void;
  onDelete: (id: number) => void;
}

const priorityColors: Record<number, string> = {
  0: 'bg-green-500/20 text-green-400 border-green-500/40',
  1: 'bg-yellow-500/20 text-yellow-400 border-yellow-500/40',
  2: 'bg-red-500/20 text-red-400 border-red-500/40',
};

const priorityLabels: Record<number, string> = { 0: 'Low', 1: 'Med', 2: 'High' };

export default function TaskCard({ task, onToggle, onDelete }: Props) {
  const overdue =
    task.dueDate &&
    !task.isDone &&
    new Date(task.dueDate) < new Date();

  return (
    <div
      className={`bg-dark-800 border rounded-lg p-4 flex items-start gap-3 transition-all ${
        task.isDone ? 'border-dark-700 opacity-60' : 'border-dark-700 hover:border-neon-400'
      }`}
    >
      <input
        type="checkbox"
        checked={task.isDone}
        onChange={(e) => onToggle(task.id, (e.target as HTMLInputElement).checked)}
        className="mt-1 w-5 h-5 accent-cyan-400 cursor-pointer"
      />

      <div className="flex-1 min-w-0">
        <h3
          className={`font-medium ${
            task.isDone ? 'line-through text-gray-500' : 'text-white'
          }`}
        >
          {task.title}
        </h3>

        {task.description && (
          <p className="text-sm text-gray-400 mt-1">{task.description}</p>
        )}

        <div className="flex items-center gap-2 mt-2 flex-wrap">
          <span
            className={`text-xs px-2 py-0.5 rounded border ${
              priorityColors[task.priority] || priorityColors[1]
            }`}
          >
            {priorityLabels[task.priority] || 'Med'}
          </span>

          {task.dueDate && (
            <span
              className={`text-xs ${
                overdue ? 'text-red-400' : 'text-gray-500'
              }`}
            >
              📅 {new Date(task.dueDate).toLocaleDateString()}
              {overdue && ' (overdue)'}
            </span>
          )}
        </div>
      </div>

      <button
        onClick={() => onDelete(task.id)}
        className="text-gray-500 hover:text-red-400 transition-colors text-sm px-2"
        title="Delete"
      >
        ✕
      </button>
    </div>
  );
}