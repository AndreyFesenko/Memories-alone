# AccessControlService.API

- REST API дл€ управлени€ доступом пользователей к ресурсам.
- »спользует Scalar + OpenAPI (без Swagger UI).

## Ёндпоинты

- `POST /api/access/grant` Ч выдать доступ
- `POST /api/access/check` Ч проверить доступ
- `POST /api/access/revoke` Ч отозвать доступ

## «апуск

```sh
dotnet run
