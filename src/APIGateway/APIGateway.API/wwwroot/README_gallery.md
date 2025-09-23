# Media Gallery Patch for /api-docs

Drop these files into `src/APIGateway/APIGateway.API/wwwroot/`:
- `openapi.json` (with GET endpoints)
- `index.html` (with preview gallery)

## What’s new
- Auto preview of images when response contains `MediaFileDto.url` or `MemoryDto.mediaFiles[].url`
- Works for:
  - **GetMediaById** — single preview
  - **GetMemoryById** — gallery for the memory
  - **ListByUser** — aggregated gallery from all memories on the page

If a media item is not an image, a link is shown instead.