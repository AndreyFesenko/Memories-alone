
import React, { useState } from "react";
import {
  login,
  register,
  refreshToken as apiRefreshToken,
  getClaims,
} from "./api";
import { useDarkMode } from "./theme";

function pretty(obj) {
  return <pre className="bg-gray-100 dark:bg-gray-800 p-2 rounded">{JSON.stringify(obj, null, 2)}</pre>;
}

function ThemeSwitcher({ theme, setTheme }) {
  return (
    <button
      className="absolute top-4 right-4 text-xl p-2 rounded-full hover:bg-gray-100 dark:hover:bg-gray-700"
      title={theme === "dark" ? "Светлая тема" : "Тёмная тема"}
      onClick={() => setTheme(theme === "dark" ? "light" : "dark")}
    >
      {theme === "dark" ? "🌞" : "🌙"}
    </button>
  );
}

export default function App() {
  const [theme, setTheme] = useDarkMode();
  const [tab, setTab] = useState("login");
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [accessToken, setAccessToken] = useState("");
  const [refresh, setRefresh] = useState("");
  const [claims, setClaims] = useState(null);
  const [result, setResult] = useState(null);
  const [error, setError] = useState(null);

  async function onLogin(e) {
    e.preventDefault();
    setError(null);
    try {
      const res = await login(email, password);
      setAccessToken(res.accessToken);
      setRefresh(res.refreshToken);
      setResult(res);
      setTab("me");
    } catch (err) {
      setError(err);
    }
  }

  async function onRegister(e) {
    e.preventDefault();
    setError(null);
    try {
      const res = await register(email, password);
      setResult(res);
      setTab("login");
    } catch (err) {
      setError(err);
    }
  }

  async function onRefresh(e) {
    e.preventDefault();
    setError(null);
    try {
      const res = await apiRefreshToken(refresh);
      setAccessToken(res.accessToken);
      setRefresh(res.refreshToken);
      setResult(res);
    } catch (err) {
      setError(err);
    }
  }

  async function onGetClaims(e) {
    e.preventDefault();
    setError(null);
    try {
      const res = await getClaims(accessToken);
      setClaims(res);
    } catch (err) {
      setError(err);
    }
  }

  function logout() {
    setAccessToken("");
    setRefresh("");
    setClaims(null);
    setTab("login");
    setResult(null);
    setError(null);
  }

  return (
    <div className="min-h-screen flex flex-col items-center justify-center bg-gradient-to-br from-blue-50 to-purple-100 dark:from-gray-900 dark:to-gray-800 p-4">
      <ThemeSwitcher theme={theme} setTheme={setTheme} />
      <div className="w-full max-w-md bg-white dark:bg-gray-900 rounded-2xl shadow-2xl p-8 space-y-6">
        <div className="flex justify-center mb-2 space-x-4">
          <button className={tab === "login" ? "font-bold underline" : ""} onClick={() => setTab("login")}>Вход</button>
          <button className={tab === "register" ? "font-bold underline" : ""} onClick={() => setTab("register")}>Регистрация</button>
          {accessToken && (
            <button className={tab === "me" ? "font-bold underline" : ""} onClick={() => setTab("me")}>Профиль</button>
          )}
        </div>

        {tab === "login" && (
          <form className="space-y-4" onSubmit={onLogin}>
            <input type="email" placeholder="Email" className="w-full input input-bordered dark:bg-gray-800 dark:text-white" value={email} onChange={e => setEmail(e.target.value)} />
            <input type="password" placeholder="Пароль" className="w-full input input-bordered dark:bg-gray-800 dark:text-white" value={password} onChange={e => setPassword(e.target.value)} />
            <button className="btn btn-primary w-full">Войти</button>
            {error && <div className="text-red-400">{error.message ?? (typeof error === "string" ? error : JSON.stringify(error))}</div>}
          </form>
        )}

        {tab === "register" && (
          <form className="space-y-4" onSubmit={onRegister}>
            <input type="email" placeholder="Email" className="w-full input input-bordered dark:bg-gray-800 dark:text-white" value={email} onChange={e => setEmail(e.target.value)} />
            <input type="password" placeholder="Пароль" className="w-full input input-bordered dark:bg-gray-800 dark:text-white" value={password} onChange={e => setPassword(e.target.value)} />
            <button className="btn btn-primary w-full">Зарегистрироваться</button>
            {error && <div className="text-red-400">{error.message ?? (typeof error === "string" ? error : JSON.stringify(error))}</div>}
          </form>
        )}

        {tab === "me" && accessToken && (
          <div className="space-y-4">
            <div className="flex justify-between">
              <button className="btn btn-secondary" onClick={onGetClaims}>Мои claims</button>
              <button className="btn btn-outline" onClick={logout}>Выйти</button>
            </div>
            <div>
              <div className="font-mono text-xs break-all">accessToken: {accessToken.slice(0, 50)}...</div>
              <div className="font-mono text-xs break-all">refreshToken: {refresh.slice(0, 12)}...</div>
            </div>
            <button className="btn btn-accent w-full" onClick={onRefresh}>Обновить accessToken (refresh)</button>
            {claims && <div><div className="font-semibold">Claims:</div>{pretty(claims)}</div>}
            {result && <div><div className="font-semibold">Результат:</div>{pretty(result)}</div>}
            {error && <div className="text-red-400">{error.message ?? (typeof error === "string" ? error : JSON.stringify(error))}</div>}
          </div>
        )}
      </div>
      <div className="mt-6 text-center text-gray-400 text-xs">
        Memories IdentityService Front &copy; 2025 • <span>{theme === "dark" ? "Тёмная" : "Светлая"} тема</span>
      </div>
    </div>
  );
}
