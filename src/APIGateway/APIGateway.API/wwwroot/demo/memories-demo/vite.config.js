import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'

// В dev открываем по http://localhost:5173/
// В prod билд будет лежать на /demo/memories-demo/ (через Gateway)
export default defineConfig(({ command }) => ({
  plugins: [react()],
  base: command === 'build' ? '/demo/memories-demo/' : '/',
}))
