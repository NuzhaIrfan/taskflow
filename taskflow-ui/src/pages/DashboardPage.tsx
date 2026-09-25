import { useAuth } from '../authContext';

export default function DashboardPage() {
  const { email, logout } = useAuth();

  return (
    <div className="min-h-screen bg-dark-900 p-8">
      <div className="max-w-5xl mx-auto">
        <div className="flex justify-between items-center mb-8">
          <h1 className="text-3xl font-bold text-neon-400">TaskFlow</h1>
          <div className="flex items-center gap-4">
            <span className="text-gray-400">{email}</span>
            <button
              onClick={logout}
              className="px-4 py-2 border border-dark-700 rounded-lg text-gray-400 hover:text-neon-400 hover:border-neon-400"
            >
              Logout
            </button>
          </div>
        </div>
        <div className="bg-dark-800 border border-dark-700 rounded-xl p-8 text-center text-gray-400">
          Dashboard coming next...
        </div>
      </div>
    </div>
  );
}