import { useEffect } from "react";

export default function Lightbox({ items, index, onClose, onPrev, onNext }) {
  const item = items[index];

  useEffect(() => {
    const onKey = (e) => {
      if (e.key === "Escape") onClose();
      if (e.key === "ArrowLeft") onPrev();
      if (e.key === "ArrowRight") onNext();
    };
    window.addEventListener("keydown", onKey);
    return () => window.removeEventListener("keydown", onKey);
  }, [onClose, onPrev, onNext]);

  if (!item) return null;
  const isImg = (item.mediaType || "").toLowerCase().includes("image");
  const isVideo = (item.mediaType || "").toLowerCase().includes("video");
  const isAudio = (item.mediaType || "").toLowerCase().includes("audio");

  return (
    <div
      className="fixed inset-0 z-50 flex items-center justify-center bg-black/80 p-4"
      onClick={onClose}
    >
      <div className="relative max-w-[95vw] max-h-[95vh]" onClick={(e) => e.stopPropagation()}>
        <button
          className="absolute -top-3 -right-3 rounded-full bg-slate-800 px-3 py-1 text-sm border border-slate-600"
          onClick={onClose}
          title="Закрыть"
        >
          ✕
        </button>

        <div className="flex items-center gap-3">
          <button
            className="rounded-md border border-slate-600 bg-slate-800 px-3 py-2"
            onClick={onPrev}
            title="Назад"
          >
            ←
          </button>

          <div className="max-w-[80vw] max-h-[85vh]">
            {isImg && (
              <img
                src={item.url}
                alt={item.fileName || item.id}
                className="max-w-[80vw] max-h-[85vh] rounded-lg shadow-xl"
              />
            )}
            {isVideo && (
              <video
                src={item.url}
                controls
                className="max-w-[80vw] max-h-[85vh] rounded-lg shadow-xl"
              />
            )}
            {isAudio && <audio src={item.url} controls className="w-[70vw]" />}
            {!isImg && !isVideo && !isAudio && (
              <a
                href={item.url}
                target="_blank"
                rel="noreferrer"
                className="underline text-blue-300"
              >
                Открыть файл
              </a>
            )}
            <div className="mt-2 text-center text-sm text-slate-300">
              {item.fileName || ""} {item.mediaType ? `• ${item.mediaType}` : ""}
              {item.id ? ` • id: ${item.id}` : ""}
            </div>
          </div>

          <button
            className="rounded-md border border-slate-600 bg-slate-800 px-3 py-2"
            onClick={onNext}
            title="Вперёд"
          >
            →
          </button>
        </div>
      </div>
    </div>
  );
}
