
# Memory Retrieval (Выгрузка медиа) — How to Test via Gateway

## Updated Docs/UI
- Replace your `wwwroot/openapi.json` with `openapi.modified.json`.
- Replace your `wwwroot/index.html` with `index.modified.html`.
- Open http://localhost:8080/api-docs and use new presets:
  - **GetMemoryById**
  - **ListByUser**
  - **GetMediaById**

> Поле **OwnerId** используется как `userId` для запроса ListByUser.

## Gateway Endpoints

### Get memory by ID
```
GET http://localhost:8080/memory/api/memory/{memoryId}
Authorization: Bearer <JWT>
```

**curl:**
```bash
curl -X 'GET'   'http://localhost:8080/memory/api/memory/{memoryId}'   -H 'Authorization: Bearer REPLACE_ME_JWT'
```

### List memories by user (paged)
```
GET http://localhost:8080/memory/api/memory/user/{userId}?page=1&pageSize=10&accessLevel=Public
Authorization: Bearer <JWT>
```

**curl:**
```bash
curl -X 'GET'   'http://localhost:8080/memory/api/memory/user/{userId}?page=1&pageSize=10'   -H 'Authorization: Bearer REPLACE_ME_JWT'
```

### Get media metadata by ID
```
GET http://localhost:8080/memory/api/media/{mediaId}
Authorization: Bearer <JWT>
```

**curl:**
```bash
curl -X 'GET'   'http://localhost:8080/memory/api/media/{mediaId}'   -H 'Authorization: Bearer REPLACE_ME_JWT'
```

## Notes
- Эти маршруты соответствуют контроллерам в **MemoryArchiveService**:
  - `MemoryController`: `[HttpGet("{id:guid}")]` и `[HttpGet("user/{userId:guid}")]`
  - `MediaController`: `[HttpGet("{id}")]`
- Gateway уже проксирует `/memory/*` на сервис, поэтому GET будет работать так же, как POST/PUT/DELETE.
- Если файлы отдаются по **presigned URL** через `MediaFileDto.Url`, отображение/скачивание выполняется клиентом по этой ссылке.
