export const API = import.meta.env.VITE_GATEWAY_BASE ?? "";

/** Базовый fetch с токеном */
async function authFetch(path, { method = "GET", headers = {}, body } = {}, token) {
  const res = await fetch(`${API}${path}`, {
    method,
    headers: {
      ...(body instanceof FormData ? {} : { "Content-Type": "application/json" }),
      ...(token ? { Authorization: `Bearer ${token}` } : {}),
      ...headers,
    },
    body: body instanceof FormData ? body : body ? JSON.stringify(body) : undefined,
  });
  if (!res.ok) {
    const text = await res.text().catch(() => "");
    throw new Error(`${res.status} ${res.statusText}: ${text}`);
  }
  const ct = res.headers.get("content-type") || "";
  return ct.includes("application/json") ? res.json() : res.text();
}

/** Логин */
export async function login(username, password) {
  return authFetch("/identity/login", {
    method: "POST",
    body: { username, password },
  });
}

/** Список воспоминаний пользователя */
export async function listMemoriesByUser(userId, { page = 1, pageSize = 10, accessLevel } = {}, token) {
  const q = new URLSearchParams({ page: String(page), pageSize: String(pageSize) });
  if (accessLevel) q.set("accessLevel", accessLevel);
  return authFetch(`/memory/api/memory/user/${userId}?${q.toString()}`, {}, token);
}

/** Создать память (мультизагрузка файлов) */
export async function createMemory({
  ownerId,
  title,
  description,
  mediaType = "Image",
  accessLevel = "Private",
  tags = [],
  files = [],
  fileName, // опционально (для единичного “логического” имени)
}, token) {
  const fd = new FormData();
  if (ownerId) fd.append("OwnerId", ownerId);
  if (title) fd.append("Title", title);
  if (description) fd.append("Description", description);
  if (mediaType) fd.append("MediaType", mediaType);
  if (accessLevel) fd.append("AccessLevel", accessLevel);
  if (tags?.length) tags.forEach(t => fd.append("Tags", t));
  if (files?.length) {
    for (const f of files) {
      fd.append("File", f, f.name || "upload.bin"); // повторяющийся ключ File
    }
  }
  if (fileName) fd.append("FileName", fileName);

  return authFetch("/memory/api/memory", { method: "POST", body: fd }, token);
}

/** Удалить память */
export async function deleteMemory(memoryId, token) {
  return authFetch(`/memory/api/memory/${memoryId}`, { method: "DELETE" }, token);
}
