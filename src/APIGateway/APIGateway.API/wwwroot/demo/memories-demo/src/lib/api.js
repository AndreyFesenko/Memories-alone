export const API = import.meta.env.VITE_GATEWAY_BASE ?? "";

export async function apiJson(path, { method = "GET", token, body } = {}) {
  const res = await fetch(`${API}${path}`, {
    method,
    headers: {
      "Content-Type": "application/json",
      ...(token ? { Authorization: `Bearer ${token}` } : {}),
    },
    body: body ? JSON.stringify(body) : undefined,
  });
  if (!res.ok) {
    const text = await res.text().catch(() => "");
    throw new Error(`${res.status} ${res.statusText}\n${text}`);
  }
  return res.json();
}

export async function apiForm(path, { token, formData, method = "POST" }) {
  const res = await fetch(`${API}${path}`, {
    method,
    headers: { ...(token ? { Authorization: `Bearer ${token}` } : {}) },
    body: formData,
  });
  if (!res.ok) {
    const text = await res.text().catch(() => "");
    throw new Error(`${res.status} ${res.statusText}\n${text}`);
  }
  return res.json();
}

export function resolveMediaUrl(url) {
  // если API отдает относительные ссылки — нормализуем
  if (!url) return url;
  if (url.startsWith("http://") || url.startsWith("https://")) return url;
  return `${API}${url}`;
}
