import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'

// В DEV работаем c base = '/', чтобы не было 404 по /src/*.jsx
// В PROD передадим base флагом при билде (--base=/demo/memories-demo/)
export default defineConfig({
  plugins: [react()],
  base: '/', // важно для dev
})
