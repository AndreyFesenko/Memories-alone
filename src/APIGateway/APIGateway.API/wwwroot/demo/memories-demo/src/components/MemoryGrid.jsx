import { resolveMediaUrl } from "../lib/api";

export default function MemoryGrid({ paged, onThumbClick, onOpenAll, onCopyIds }) {
  const items = Array.isArray(paged?.items) ? paged.items : [];

  // плоский список всех медиа с пометкой memoryId
  const flat = [];
  for (const mem of items) {
    if (Array.isArray(mem.mediaFiles)) {
      for (const m of mem.mediaFiles) {
        flat.push({
          ...m,
          url: resolveMediaUrl(m.url),
          _memoryId: mem.id,
        });
      }
    }
  }

  if (!flat.length) {
    return <div className="text-sm text-slate-400">Нет медиа для отображения.</div>;
  }

  return (
    <div className="space-y-3">
      <div className="flex items-center gap-2">
        <button
          className="rounded-md border border-slate-600 bg-slate-800 px-3 py-1 text-sm"
          onClick={() => onOpenAll(flat)}
        >
          Открыть всё
        </button>
        <button
          className="rounded-md border border-slate-600 bg-slate-800 px-3 py-1 text-sm"
          onClick={() => onCopyIds(flat)}
        >
          Скопировать mediaId
        </button>
        <div className="text-xs text-slate-400 ml-auto">
          Всего: {flat.length} • Памятей: {items.length}
        </div>
      </div>

      <div className="grid grid-cols-[repeat(auto-fill,minmax(170px,1fr))] gap-3">
        {flat.map((m, i) => {
          const mt = (m.mediaType || "").toLowerCase();
          const isImg = mt.includes("image");
          const isVideo = mt.includes("video");
          const isAudio = mt.includes("audio");
          return (
            <div key={`${m.id || m.url}-${i}`} className="border border-slate-700 rounded-lg p-2">
              <div className="aspect-[4/3] bg-slate-800 rounded-md overflow-hidden flex items-center justify-center">
                {isImg && (
                  <img
                    src={m.url}
                    alt={m.fileName || m.id || "image"}
                    className="w-full h-full object-cover cursor-zoom-in"
                    onClick={() => onThumbClick(m, i, flat)}
                  />
                )}
                {isVideo && (
                  <video
                    src={m.url}
                    controls
                    className="w-full h-full object-cover"
                  />
                )}
                {isAudio && <audio src={m.url} controls className="w-full" />}
                {!isImg && !isVideo && !isAudio && (
                  <a
                    href={m.url}
                    target="_blank"
                    rel="noreferrer"
                    className="underline text-blue-300 text-sm"
                  >
                    Открыть файл
                  </a>
                )}
              </div>
              <div className="mt-2 text-xs text-slate-300 break-words">
                {m.fileName || "—"}
                <div className="text-slate-500">{m.mediaType || ""}</div>
                {m.id && <div className="text-slate-500">id: {m.id}</div>}
                {m._memoryId && <div className="text-slate-500">memoryId: {m._memoryId}</div>}
              </div>
            </div>
          );
        })}
      </div>
    </div>
  );
}
