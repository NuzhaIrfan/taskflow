import { useState } from 'react';
import { useNavigate, Link } from 'react-router-dom';
import { authApi } from '../api';
import { useAuth } from '../authContext';

export default function LoginPage() {
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [error, setError] = useState('');
  const [loading, setLoading] = useState(false);
  const navigate = useNavigate();
  const { login } = useAuth();

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError('');
    setLoading(true);
    try {
      const res = await authApi.login(email, password);
      login(res.data.token, res.data.email);
      navigate('/dashboard');
    } catch (err: any) {
      setError(err.response?.data?.error || 'Login failed');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="min-h-screen flex items-center justify-center bg-dark-900 px-6">
      <div className="w-full max-w-md bg-dark-800 border border-dark-700 rounded-xl p-8">
        <h1 className="text-3xl font-bold text-neon-400 text-center mb-2">TaskFlow</h1>
        <p className="text-center text-gray-400 mb-8">Sign in to your account</p>

        {error && (
          <div className="mb-4 p-3 bg-red-900/40 border border-red-700 text-red-300 rounded">
            {error}
          </div>
        )}

        <form onSubmit={handleSubmit} className="space-y-4">
          <input
            type="email"
            placeholder="Email"
            value={email}
            onChange={(e) => setEmail((e.target as HTMLInputElement).value)}
            required
            className="w-full p-3 bg-dark-900 border border-dark-700 rounded-lg text-white focus:outline-none focus:border-neon-400"
          />
          <input
            type="password"
            placeholder="Password"
            value={password}
            onChange={(e) => setPassword((e.target as HTMLInputElement).value)}
            required
            className="w-full p-3 bg-dark-900 border border-dark-700 rounded-lg text-white focus:outline-none focus:border-neon-400"
          />
          <button
            type="submit"
            disabled={loading}
            className="w-full py-3 bg-neon-400 text-dark-950 font-semibold rounded-lg hover:bg-neon-500 disabled:opacity-50"
          >
            {loading ? 'Signing in...' : 'Sign In'}
          </button>
        </form>

        <p className="text-center text-gray-400 mt-6 text-sm">
          No account?{' '}
          <Link to="/register" className="text-neon-400 hover:underline">
            Create one
          </Link>
        </p>
      </div>
    </div>
  );
}