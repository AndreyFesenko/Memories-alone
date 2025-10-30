// src/MemoryArchiveService/MemoryArchiveService.Application/Commands/CreateMemoryCommand.cs
using MediatR;
using MemoryArchiveService.Application.DTOs;
using System.IO;

namespace MemoryArchiveService.Application.Commands;

public class CreateMemoryCommand : IRequest<MemoryDto>
{
    public string OwnerId { get; set; } = default!;
    public string Title { get; set; } = default!;
    public string Description { get; set; } = default!;
    public string AccessLevel { get; set; } = "Private";
    public List<string>? Tags { get; set; }

    /// <summary>
    /// Мультизагрузка: набор входных файлов (каждый со своим именем/типом/медиатипом/потоком)
    /// ВАЖНО: контроллер обязан собрать это из multipart (повторяющегося ключа "File")
    /// </summary>
    public List<IncomingFile> Files { get; set; } = new();
}

/// <summary>
/// Единица загружаемого файла для CreateMemory (слой Application — без ASP.NET зависимостей)
/// </summary>
public sealed class IncomingFile
{
    public string FileName { get; init; } = default!;
    public string ContentType { get; init; } = "application/octet-stream";
    /// <summary>
    /// Логический тип в терминах домена: Image | Video | Audio | Document
    /// (при необходимости контроллер/маппер может положить сюда осмысленное значение,
    /// либо хендлер определит по ContentType)
    /// </summary>
    public string MediaType { get; init; } = "Image";
    public Stream FileStream { get; init; } = default!;
}
