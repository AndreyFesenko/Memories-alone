import { useEffect, useMemo, useState } from "react";
import Lightbox from "./components/Lightbox.jsx";
import MemoryGrid from "./components/MemoryGrid.jsx";
import { API, apiJson, apiForm } from "./lib/api";

const LS_JWT = "memories.jwt";
const DEF_USER = "fdd2eee1-7871-45da-a3b4-c6a214ef4928";

export default function App() {
  const [token, setToken] = useState(localStorage.getItem(LS_JWT) || "");
  const [busy, setBusy] = useState(false);
  const [err, setErr] = useState("");

  // auth
  const [username, setUsername] = useState("andrey");
  const [password, setPassword] = useState("andrey256!");

  // paging & data
  const [userId, setUserId] = useState(DEF_USER);
  const [page, setPage] = useState(1);
  const [pageSize, setPageSize] = useState(12);
  const [paged, setPaged] = useState(null);

  // lightbox
  const [lbOpen, setLbOpen] = useState(false);
  const [lbItems, setLbItems] = useState([]);
  const [lbIndex, setLbIndex] = useState(0);

  const isAuthed = !!token;

  const headersNote = useMemo(() => {
    return `API: ${API || "(не задан VITE_GATEWAY_BASE)"}\nJWT: ${
      token ? "есть" : "нет"
    }`;
  }, [token]);

  async function doLogin(e) {
    e?.preventDefault();
    setBusy(true);
    setErr("");
    try {
      const res = await apiJson("/identity/login", {
        method: "POST",
        body: { username, password },
      });
      const t = res?.accessToken || "";
      if (!t) throw new Error("Нет accessToken в ответе");
      localStorage.setItem(LS_JWT, t);
      setToken(t);
    } catch (e) {
      setErr(String(e.message || e));
    } finally {
      setBusy(false);
    }
  }

  function logout() {
    localStorage.removeItem(LS_JWT);
    setToken("");
  }

  async function loadMemories() {
    if (!token) return;
    setBusy(true);
    setErr("");
    try {
      const data = await apiJson(
        `/memory/api/memory/user/${userId}?page=${page}&pageSize=${pageSize}`,
        { token }
      );
      setPaged(data);
    } catch (e) {
      setErr(String(e.message || e));
    } finally {
      setBusy(false);
    }
  }

  useEffect(() => {
    loadMemories();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [token, userId, page, pageSize]);

  // upload new memory (CreateMemory multipart)
  const [ownerId, setOwnerId] = useState(DEF_USER);
  const [title, setTitle] = useState("New memory");
  const [description, setDescription] = useState("");
  const [mediaType, setMediaType] = useState("Image");
  const [accessLevel, setAccessLevel] = useState("Private");
  const [tags, setTags] = useState("demo,upload");
  const [files, setFiles] = useState([]);

  async function uploadMemory(e) {
    e?.preventDefault();
    if (!token) return;
    if (!files?.length) {
      setErr("Выберите хотя бы один файл");
      return;
    }
    setBusy(true);
    setErr("");
    try {
      const fd = new FormData();
      if (ownerId) fd.append("OwnerId", ownerId);
      if (title) fd.append("Title", title);
      if (description) fd.append("Description", description);
      if (mediaType) fd.append("MediaType", mediaType);
      if (accessLevel) fd.append("AccessLevel", accessLevel);
      if (tags)
        tags
          .split(",")
          .map((t) => t.trim())
          .filter(Boolean)
          .forEach((t) => fd.append("Tags", t));
      for (const f of files) fd.append("File", f, f.name || "upload.bin");

      await apiForm("/memory/api/memory", { token, formData: fd, method: "POST" });
      setFiles([]);
      await loadMemories();
    } catch (e) {
      setErr(String(e.message || e));
    } finally {
      setBusy(false);
    }
  }

  // delete memory
  async function deleteMemory(memoryId) {
    if (!token || !memoryId) return;
    if (!confirm("Удалить память целиком?")) return;
    setBusy(true);
    setErr("");
    try {
      await apiJson(`/memory/api/memory/${memoryId}`, {
        method: "DELETE",
        token,
      });
      await loadMemories();
    } catch (e) {
      setErr(String(e.message || e));
    } finally {
      setBusy(false);
    }
  }

  // lightbox handlers
  function openLightboxFromFlat(item, index, flat) {
    setLbItems(flat);
    setLbIndex(index);
    setLbOpen(true);
  }
  function lbPrev() {
    setLbIndex((i) => (i - 1 + lbItems.length) % lbItems.length);
  }
  function lbNext() {
    setLbIndex((i) => (i + 1) % lbItems.length);
  }

  // toolbar helpers
  function openAll(flat) {
    for (const m of flat) {
      if (m.url) window.open(m.url, "_blank", "noopener");
    }
  }
  async function copyIds(flat) {
    const ids = flat.map((m) => m.id).filter(Boolean).join("\n");
    if (ids) await navigator.clipboard.writeText(ids);
  }

  return (
    <div className="min-h-screen">
      <header className="sticky top-0 z-10 border-b border-slate-800 bg-slate-900/90 backdrop-blur px-4 py-3">
        <div className="mx-auto max-w-7xl flex items-center gap-3">
          <h1 className="text-base font-semibold">Memories — Demo Front</h1>
          <div className="text-xs text-slate-400 ml-auto whitespace-pre">
            {headersNote}
          </div>
        </div>
      </header>

      <main className="mx-auto max-w-7xl p-4 space-y-6">
        {/* AUTH */}
        <section className="rounded-xl border border-slate-800 bg-slate-900 p-4">
          <h2 className="text-sm font-semibold mb-3">1) Аутентификация</h2>
          {!isAuthed ? (
            <form onSubmit={doLogin} className="grid gap-3 md:grid-cols-3">
              <label className="grid gap-1 text-sm">
                <span>Username</span>
                <input
                  className="bg-slate-800 border border-slate-700 rounded-md px-3 py-2"
                  value={username}
                  onChange={(e) => setUsername(e.target.value)}
                  required
                />
              </label>
              <label className="grid gap-1 text-sm">
                <span>Password</span>
                <input
                  type="password"
                  className="bg-slate-800 border border-slate-700 rounded-md px-3 py-2"
                  value={password}
                  onChange={(e) => setPassword(e.target.value)}
                  required
                />
              </label>
              <div className="flex items-end gap-2">
                <button
                  type="submit"
                  className="rounded-md bg-emerald-500 text-emerald-950 font-semibold px-4 py-2"
                  disabled={busy}
                >
                  {busy ? "Вход…" : "Войти"}
                </button>
              </div>
            </form>
          ) : (
            <div className="flex items-center gap-2">
              <span className="text-sm text-emerald-300">Вы авторизованы</span>
              <button
                className="rounded-md border border-slate-600 bg-slate-800 px-3 py-2 text-sm"
                onClick={logout}
              >
                Выйти
              </button>
            </div>
          )}
        </section>

        {/* LIST */}
        <section className="rounded-xl border border-slate-800 bg-slate-900 p-4">
          <div className="flex flex-wrap items-end gap-2 mb-3">
            <h2 className="text-sm font-semibold">2) Воспоминания пользователя</h2>
            <div className="ml-auto flex items-end gap-2">
              <label className="grid text-xs">
                <span className="text-slate-400 mb-1">UserId</span>
                <input
                  className="bg-slate-800 border border-slate-700 rounded-md px-3 py-1.5 w-[340px]"
                  value={userId}
                  onChange={(e) => setUserId(e.target.value)}
                />
              </label>
              <label className="grid text-xs">
                <span className="text-slate-400 mb-1">Page</span>
                <input
                  type="number"
                  min={1}
                  className="bg-slate-800 border border-slate-700 rounded-md px-3 py-1.5 w-20"
                  value={page}
                  onChange={(e) => setPage(+e.target.value || 1)}
                />
              </label>
              <label className="grid text-xs">
                <span className="text-slate-400 mb-1">PageSize</span>
                <input
                  type="number"
                  min={1}
                  className="bg-slate-800 border border-slate-700 rounded-md px-3 py-1.5 w-24"
                  value={pageSize}
                  onChange={(e) => setPageSize(+e.target.value || 12)}
                />
              </label>
              <button
                className="rounded-md border border-slate-600 bg-slate-800 px-3 py-2 text-sm"
                onClick={loadMemories}
                disabled={!token || busy}
              >
                Обновить
              </button>
            </div>
          </div>

          {err && (
            <div className="mb-3 text-sm text-red-300 whitespace-pre-wrap">{err}</div>
          )}

          {!paged ? (
            <div className="text-sm text-slate-400">
              {token ? "Загружаю…" : "Авторизуйтесь, чтобы загрузить список"}
            </div>
          ) : (
            <>
              <MemoryGrid
                paged={paged}
                onThumbClick={openLightboxFromFlat}
                onOpenAll={openAll}
                onCopyIds={copyIds}
              />

              {/* Кнопки удаления по MemoryId (каждый блок) */}
              <div className="mt-4 grid gap-2">
                {Array.isArray(paged.items) &&
                  paged.items.map((m) => (
                    <div
                      key={m.id}
                      className="flex items-center justify-between rounded-md border border-slate-800 bg-slate-950 px-3 py-2 text-xs"
                    >
                      <div className="text-slate-300 break-all">
                        {m.title || "(без названия)"} •{" "}
                        <span className="text-slate-500">{m.id}</span>
                      </div>
                      <button
                        className="rounded-md border border-red-600/60 bg-red-900/40 px-3 py-1.5"
                        onClick={() => deleteMemory(m.id)}
                      >
                        Удалить память
                      </button>
                    </div>
                  ))}
              </div>
            </>
          )}
        </section>

        {/* UPLOAD */}
        <section className="rounded-xl border border-slate-800 bg-slate-900 p-4">
          <h2 className="text-sm font-semibold mb-3">3) Создать новую память</h2>
          <form onSubmit={uploadMemory} className="grid gap-3 md:grid-cols-2">
            <label className="grid gap-1 text-sm">
              <span>OwnerId</span>
              <input
                className="bg-slate-800 border border-slate-700 rounded-md px-3 py-2"
                value={ownerId}
                onChange={(e) => setOwnerId(e.target.value)}
                required
              />
            </label>
            <label className="grid gap-1 text-sm">
              <span>Title</span>
              <input
                className="bg-slate-800 border border-slate-700 rounded-md px-3 py-2"
                value={title}
                onChange={(e) => setTitle(e.target.value)}
              />
            </label>
            <label className="grid gap-1 text-sm md:col-span-2">
              <span>Description</span>
              <textarea
                className="bg-slate-800 border border-slate-700 rounded-md px-3 py-2 min-h-[72px]"
                value={description}
                onChange={(e) => setDescription(e.target.value)}
              />
            </label>
            <label className="grid gap-1 text-sm">
              <span>MediaType</span>
              <select
                className="bg-slate-800 border border-slate-700 rounded-md px-3 py-2"
                value={mediaType}
                onChange={(e) => setMediaType(e.target.value)}
              >
                <option>Image</option>
                <option>Video</option>
                <option>Audio</option>
                <option>Document</option>
              </select>
            </label>
            <label className="grid gap-1 text-sm">
              <span>AccessLevel</span>
              <select
                className="bg-slate-800 border border-slate-700 rounded-md px-3 py-2"
                value={accessLevel}
                onChange={(e) => setAccessLevel(e.target.value)}
              >
                <option>Private</option>
                <option>Public</option>
                <option>FriendsOnly</option>
              </select>
            </label>
            <label className="grid gap-1 text-sm md:col-span-2">
              <span>Tags (через запятую)</span>
              <input
                className="bg-slate-800 border border-slate-700 rounded-md px-3 py-2"
                value={tags}
                onChange={(e) => setTags(e.target.value)}
              />
            </label>
            <label className="grid gap-1 text-sm md:col-span-2">
              <span>Файлы</span>
              <input
                type="file"
                multiple
                onChange={(e) => setFiles([...e.target.files])}
                className="bg-slate-800 border border-slate-700 rounded-md px-3 py-2"
              />
              <div className="text-xs text-slate-400">
                {files?.length
                  ? `${files.length} файл(ов): ` + files.map((f) => f.name).join(", ")
                  : "Не выбрано"}
              </div>
            </label>
            <div className="md:col-span-2 flex gap-2">
              <button
                className="rounded-md bg-emerald-500 text-emerald-950 font-semibold px-4 py-2"
                type="submit"
                disabled={!token || busy}
              >
                {busy ? "Загружаю…" : "Создать"}
              </button>
            </div>
          </form>
        </section>
      </main>

      {lbOpen && (
        <Lightbox
          items={lbItems}
          index={lbIndex}
          onClose={() => setLbOpen(false)}
          onPrev={lbPrev}
          onNext={lbNext}
        />
      )}
    </div>
  );
}
