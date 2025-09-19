// src/MemoryArchiveService/MemoryArchiveService.Application/DTOs/MediaFileDto.cs
using System;

namespace MemoryArchiveService.Application.DTOs
{
    public sealed class MediaFileDto
    {
        public Guid Id { get; set; }

        // Имя файла в хранилище
        public string FileName { get; set; } = null!;

        // Публичный URL (если есть)
        public string Url { get; set; } = null!;

        // Полный путь/URL в хранилище (S3/Supabase)
        public string StorageUrl { get; set; } = null!;

        /// <summary>Image / Video / Document / Audio / Other</summary>
        public string MediaType { get; set; } = "Image";

        // Владелец файла (как у тебя заведено)
        public string OwnerId { get; set; } = null!;

        // Когда создана запись/загружен файл
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Размер файла в байтах (из метаданных S3). Может быть 0, если метаданные не получены.
        /// </summary>
        public long Size { get; set; }

        // ===== Поля, которых не хватало хэндлеру =====

        /// <summary>
        /// Связанный Memory.Id (если медиа принадлежит конкретному Memory).
        /// </summary>
        public Guid MemoryId { get; set; }

        /// <summary>
        /// Время загрузки (для совместимости с кодом, можно маппить из CreatedAt).
        /// </summary>
        public DateTime UploadedAt { get; set; }
    }
}
