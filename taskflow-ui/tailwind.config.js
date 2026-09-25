/** @type {import('tailwindcss').Config} */
export default {
  content: [
    "./index.html",
    "./src/**/*.{js,ts,jsx,tsx}",
  ],
  theme: {
    extend: {
      colors: {
        neon: {
          400: '#22d3ee',
          500: '#06b6d4',
        },
        dark: {
          700: '#1e293b',
          800: '#0f172a',
          900: '#020617',
          950: '#000000',
        }
      }
    },
  },
  plugins: [],
}