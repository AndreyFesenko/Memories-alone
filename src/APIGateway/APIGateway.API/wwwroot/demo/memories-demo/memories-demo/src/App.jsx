import { useEffect, useMemo, useState } from "react";
import { API, login, listMemoriesByUser, createMemory, deleteMemory } from "./api";

export default function App() {
  const [username, setUsername] = useState("andrey");
  const [password, setPassword] = useState("andrey256!");
  const [token, setToken] = useState(localStorage.getItem("mem.jwt") || "");
  const [ownerId, setOwnerId] = useState("fdd2eee1-7871-45da-a3b4-c6a214ef4928");

  const [title, setTitle] = useState("Trip to Alps");
  const [description, setDescription] = useState("Ski weekend");
  const [mediaType, setMediaType] = useState("Image");
  const [accessLevel, setAccessLevel] = useState("Private");
  const [tagsText, setTagsText] = useState("alps,trip");
  const tags = useMemo(
    () => tagsText.split(",").map(s => s.trim()).filter(Boolean),
    [tagsText]
  );

  const [files, setFiles] = useState([]);
  const [page, setPage] = useState(1);
  const [pageSize] = useState(10);
  const [list, setList] = useState(null);
  const [loading, setLoading] = useState(false);
  const authed = !!token;

  useEffect(() => {
    if (!token) return;
    localStorage.setItem("mem.jwt", token);
  }, [token]);

  async function doLogin() {
    try {
      setLoading(true);
      const data = await login(username, password);
      setToken(data.accessToken);
    } catch (e) {
      alert(`Ошибка входа: ${e.message}`);
    } finally {
      setLoading(false);
    }
  }

  async function doList() {
    if (!ownerId) {
      alert("Укажите OwnerId (userId)");
      return;
    }
    try {
      setLoading(true);
      const data = await listMemoriesByUser(ownerId, { page, pageSize }, token);
      setList(data);
    } catch (e) {
      alert(`Ошибка загрузки: ${e.message}`);
    } finally {
      setLoading(false);
    }
  }

  async function doCreate() {
    if (!files.length) {
      alert("Выберите хотя бы один файл");
      return;
    }
    try {
      setLoading(true);
      const data = await createMemory(
        {
          ownerId,
          title,
          description,
          mediaType,
          accessLevel,
          tags,
          files,
          fileName: files.length === 1 ? files[0]?.name : undefined,
        },
        token
      );
      alert("Создано!");
      // после создания обновим список
      await doList();
    } catch (e) {
      alert(`Ошибка создания: ${e.message}`);
    } finally {
      setLoading(false);
    }
  }

  async function doDelete(id) {
    if (!confirm("Удалить память?")) return;
    try {
      setLoading(true);
      await deleteMemory(id, token);
      await doList();
    } catch (e) {
      alert(`Не удалось удалить: ${e.message}`);
    } finally {
      setLoading(false);
    }
  }

  return (
    <div className="min-h-screen bg-slate-900 text-slate-100">
      <header className="sticky top-0 z-10 border-b border-slate-700 bg-slate-900/80 backdrop-blur">
        <div className="max-w-6xl mx-auto px-4 py-3 flex items-center justify-between">
          <div>
            <h1 className="text-lg font-semibold">Memories Demo Front</h1>
            <div className="text-xs text-slate-400">Gateway: <span className="font-mono">{API}</span></div>
          </div>

          <div className="flex items-center gap-2">
            {!authed ? (
              <>
                <input
                  className="px-2 py-1 bg-slate-800 rounded border border-slate-700 text-sm"
                  placeholder="username"
                  value={username}
                  onChange={e => setUsername(e.target.value)}
                />
                <input
                  className="px-2 py-1 bg-slate-800 rounded border border-slate-700 text-sm"
                  placeholder="password"
                  type="password"
                  value={password}
                  onChange={e => setPassword(e.target.value)}
                />
                <button
                  onClick={doLogin}
                  className="px-3 py-1.5 rounded bg-emerald-400 text-emerald-950 font-semibold text-sm"
                  disabled={loading}
                >
                  {loading ? "..." : "Login"}
                </button>
              </>
            ) : (
              <>
                <span className="text-xs text-emerald-400">JWT получен</span>
                <button
                  onClick={() => { setToken(""); localStorage.removeItem("mem.jwt"); }}
                  className="px-3 py-1.5 rounded bg-slate-800 border border-slate-700 text-sm"
                >
                  Logout
                </button>
              </>
            )}
          </div>
        </div>
      </header>

      <main className="max-w-6xl mx-auto px-4 py-6 grid gap-6">
        <section className="rounded-xl border border-slate-700 bg-slate-900">
          <div className="p-4 border-b border-slate-800">
            <h2 className="text-sm font-semibold text-slate-200">Создать память (мульти-аплоад)</h2>
            <p className="text-xs text-slate-400">Эндпоинт: <code className="font-mono">POST /memory/api/memory</code></p>
          </div>
          <div className="p-4 grid gap-3">
            <div className="grid md:grid-cols-2 gap-3">
              <label className="text-xs">
                OwnerId
                <input
                  className="mt-1 w-full px-2 py-2 bg-slate-800 rounded border border-slate-700 text-sm"
                  placeholder="userId (guid)"
                  value={ownerId}
                  onChange={e => setOwnerId(e.target.value)}
                />
              </label>
              <label className="text-xs">
                Title
                <input
                  className="mt-1 w-full px-2 py-2 bg-slate-800 rounded border border-slate-700 text-sm"
                  value={title}
                  onChange={e => setTitle(e.target.value)}
                />
              </label>
              <label className="text-xs md:col-span-2">
                Description
                <textarea
                  className="mt-1 w-full px-2 py-2 bg-slate-800 rounded border border-slate-700 text-sm"
                  value={description}
                  onChange={e => setDescription(e.target.value)}
                />
              </label>
              <label className="text-xs">
                MediaType
                <select
                  className="mt-1 w-full px-2 py-2 bg-slate-800 rounded border border-slate-700 text-sm"
                  value={mediaType}
                  onChange={e => setMediaType(e.target.value)}
                >
                  <option>Image</option>
                  <option>Video</option>
                  <option>Audio</option>
                  <option>Document</option>
                </select>
              </label>
              <label className="text-xs">
                AccessLevel
                <select
                  className="mt-1 w-full px-2 py-2 bg-slate-800 rounded border border-slate-700 text-sm"
                  value={accessLevel}
                  onChange={e => setAccessLevel(e.target.value)}
                >
                  <option>Private</option>
                  <option>Public</option>
                  <option>FriendsOnly</option>
                </select>
              </label>
              <label className="text-xs md:col-span-2">
                Tags (через запятую)
                <input
                  className="mt-1 w-full px-2 py-2 bg-slate-800 rounded border border-slate-700 text-sm"
                  placeholder="alps,trip"
                  value={tagsText}
                  onChange={e => setTagsText(e.target.value)}
                />
              </label>
              <label className="text-xs md:col-span-2">
                Файлы (множественный выбор)
                <input
                  type="file"
                  multiple
                  onChange={e => setFiles([...e.target.files])}
                  className="mt-1 block w-full text-sm file:mr-3 file:py-2 file:px-3 file:rounded file:border-0 file:bg-slate-700 file:text-slate-100 hover:file:bg-slate-600"
                />
                <div className="text-[11px] text-slate-400 mt-1">
                  {files?.length ? `${files.length} файл(ов): ${files.map(f => f.name).join(", ")}` : "—"}
                </div>
              </label>
            </div>
            <div>
              <button
                onClick={doCreate}
                disabled={!authed || loading}
                className="px-4 py-2 rounded bg-emerald-400 text-emerald-950 font-semibold disabled:opacity-60"
              >
                Создать
              </button>
            </div>
          </div>
        </section>

        <section className="rounded-xl border border-slate-700 bg-slate-900">
          <div className="p-4 border-b border-slate-800">
            <h2 className="text-sm font-semibold text-slate-200">Список пользовательских воспоминаний</h2>
            <p className="text-xs text-slate-400">Эндпоинт: <code className="font-mono">GET /memory/api/memory/user/{`{userId}`}</code></p>
          </div>
          <div className="p-4 grid gap-3">
            <div className="flex gap-2 items-end">
              <label className="text-xs flex-1">
                UserId
                <input
                  className="mt-1 w-full px-2 py-2 bg-slate-800 rounded border border-slate-700 text-sm"
                  value={ownerId}
                  onChange={e => setOwnerId(e.target.value)}
                />
              </label>
              <label className="text-xs">
                Page
                <input
                  type="number"
                  className="mt-1 w-24 px-2 py-2 bg-slate-800 rounded border border-slate-700 text-sm"
                  value={page}
                  onChange={e => setPage(Number(e.target.value) || 1)}
                  min={1}
                />
              </label>
              <button
                onClick={doList}
                disabled={!authed || loading}
                className="px-4 py-2 rounded bg-slate-700"
              >
                Загрузить
              </button>
            </div>

            {/* Результаты */}
            {list ? (
              <div className="grid gap-4">
                <div className="text-xs text-slate-400">
                  Всего: {list.totalCount ?? "?"}, Страница: {list.page ?? page}
                </div>

                <div className="grid gap-4">
                  {(list.items || []).map(mem => (
                    <div key={mem.id} className="rounded-lg border border-slate-800 p-3">
                      <div className="flex items-center justify-between">
                        <div>
                          <div className="font-semibold">{mem.title || "(без названия)"}</div>
                          <div className="text-xs text-slate-400">{mem.description}</div>
                          <div className="text-[11px] text-slate-500 mt-1">
                            {mem.tags?.length ? `#${mem.tags.join(" #")}` : null}
                          </div>
                        </div>
                        <button
                          onClick={() => doDelete(mem.id)}
                          className="px-3 py-1.5 rounded bg-rose-500 text-rose-950 text-sm font-semibold"
                        >
                          Удалить
                        </button>
                      </div>

                      {/* Превью всех медиа (исправляет проблему “только одного медиа”) */}
                      <div className="mt-3 grid grid-cols-2 sm:grid-cols-3 md:grid-cols-4 gap-2">
                        {(mem.mediaFiles || []).map(m => {
                          const mt = (m.mediaType || "").toLowerCase();
                          return (
                            <div key={m.id} className="rounded border border-slate-800 p-2">
                              {mt.includes("image") ? (
                                <img src={m.url} alt={m.fileName || m.id} className="aspect-video object-cover rounded" />
                              ) : mt.includes("video") ? (
                                <video src={m.url} className="aspect-video rounded" controls />
                              ) : mt.includes("audio") ? (
                                <audio src={m.url} controls className="w-full" />
                              ) : (
                                <a className="text-sky-300 underline text-sm" href={m.url} target="_blank" rel="noopener noreferrer">
                                  Открыть файл
                                </a>
                              )}
                              <div className="text-[10px] text-slate-400 mt-1 break-all">{m.fileName || m.id}</div>
                            </div>
                          );
                        })}
                      </div>
                    </div>
                  ))}
                </div>
              </div>
            ) : (
              <div className="text-sm text-slate-400">Нажмите “Загрузить”, чтобы получить список.</div>
            )}
          </div>
        </section>
      </main>
    </div>
  );
}
