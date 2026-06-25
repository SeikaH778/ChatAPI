# ChatAPI — Backend

**[RU]** Бэкенд для мессенджера реального времени на ASP.NET Core 10 с SignalR, Clean Architecture и PostgreSQL.

**[EN]** Real-time messaging backend built with ASP.NET Core 10, SignalR, Clean Architecture and PostgreSQL.

---

## Stack / Стек

| | |
|---|---|
| ASP.NET Core 10 | Web API, middleware |
| SignalR | Real-time messaging / Сообщения в реальном времени |
| Entity Framework Core | ORM, migrations / ORM, миграции |
| PostgreSQL | Database / База данных |
| JWT (HMAC-SHA256) | Authentication / Аутентификация |
| BCrypt.Net | Password hashing / Хэширование паролей |
| Docker + Nginx | Deployment / Деплой |

---

## Architecture / Архитектура

**[EN]** The project follows Clean Architecture principles, separated into 4 layers:

**[RU]** Проект построен по принципам Clean Architecture и разделён на 4 слоя:

```
ChatDomain/        — Models, interfaces, DTOs
                     Модели, интерфейсы, DTO

ChatApplication/   — Business logic: services, JWT provider, password hasher
                     Бизнес-логика: сервисы, JWT провайдер, хэшер паролей

ChatPersistance/   — Repositories, DbContext, EF Core migrations
                     Репозитории, DbContext, миграции EF Core

TestChatAPI/       — Controllers, SignalR Hub, Program.cs
                     Контроллеры, SignalR хаб, Program.cs
```

---

## Domain Models / Доменные модели

| Model / Модель | Fields / Поля |
|---|---|
| `User` | Id, UserName, PasswordHash |
| `Chat` | Id, ChatName, Members |
| `ChatMember` | ChatId, UserId, Role, JoinedAt |
| `Message` | Id, ChatId, UserId, Content, CreatedAt |
| `JoinRequest` | Id, ChatId, UserId, Status |

### Roles / Роли

| Role / Роль | Value / Значение | Permissions / Права |
|---|---|---|
| `Member` | 0 | Read and send messages / Читать и писать |
| `Moderator` | 1 | Reserved / Зарезервировано |
| `Owner` | 2 | Full access / Полный доступ |

---

## API Reference

### Auth

| Method | Endpoint | EN | RU |
|---|---|---|---|
| `POST` | `/api/Auth/login` | Sign in, returns JWT in cookie | Вход, возвращает JWT в куке |
| `POST` | `/api/Auth/register` | Register new user | Регистрация нового пользователя |

### Chat

| Method | Endpoint | EN | RU |
|---|---|---|---|
| `POST` | `/api/Chat/CreateChat` | Create chat room (caller becomes Owner) | Создать чат (создатель становится Owner) |
| `GET` | `/api/Chat/GetChats` | Get current user's chats | Список своих чатов |
| `GET` | `/api/Chat/GetAllChatMessages?chatId=` | Get message history | История сообщений |
| `GET` | `/api/Chat/GetChatByName?chatName=` | Search chats by name | Поиск чата по названию |
| `POST` | `/api/Chat/CreateJoinRequest?chatId=` | Submit join request | Подать заявку на вступление |
| `GET` | `/api/Chat/GetAllPendingRequest?chatId=` | Get pending requests (Owner) | Ожидающие заявки (Owner) |
| `GET` | `/api/Chat/GetJoinRequestById?requestId=` | Get request by ID | Получить заявку по ID |
| `POST` | `/api/Chat/ApproveRequest?requestId=` | Approve request (Owner) | Принять заявку (Owner) |
| `POST` | `/api/Chat/RejectRequest?requestId=` | Reject request (Owner) | Отклонить заявку (Owner) |
| `POST` | `/api/Chat/KickUser?chatId=&targetUserId=` | Kick member (Owner) | Кикнуть участника (Owner) |
| `POST` | `/api/Chat/LeaveChat?chatId=` | Leave chat | Покинуть чат |
| `DELETE` | `/api/Chat/DeleteChat?chatId=` | Delete chat permanently (Owner) | Удалить чат полностью (Owner) |

### SignalR Hub — `/chat`

**Client → Server:**

| Method | Parameters | EN | RU |
|---|---|---|---|
| `JoinChat` | `{ chatId, userName }` | Join chat group | Войти в группу чата |
| `SendMessage` | `chatId, content` | Send message | Отправить сообщение |

**Server → Client:**

| Event | Parameters | EN | RU |
|---|---|---|---|
| `ReceiveMessage` | `userName, content` | New message | Новое сообщение |
| `ChatListUpdated` | — | Chat list changed | Список чатов обновился |
| `RemovedFromChat` | `chatId` | User was kicked | Пользователь кикнут |
| `RequestRejected` | `chatId` | Join request rejected | Заявка отклонена |

---

## Getting Started / Запуск

### Prerequisites / Требования

- .NET 10 SDK
- PostgreSQL 16+

### Configuration / Конфигурация

**[EN]** Fill in `appsettings.json`:

**[RU]** Заполни `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=chatdb;Username=postgres;Password=yourpassword"
  },
  "JWTOptions": {
    "SecretKey": "your-super-secret-key-minimum-32-characters",
    "ExpiresHours": 24
  }
}
```

> **[EN]** `SecretKey` must be at least 32 characters — HMAC-SHA256 requires a 256-bit key minimum.
>
> **[RU]** `SecretKey` должен быть минимум 32 символа — HMAC-SHA256 требует ключ не короче 256 бит.

### Run / Запуск

```bash
dotnet run --project TestChatAPI
```

**[EN]** Migrations are applied automatically on startup via `db.Database.Migrate()`.

**[RU]** Миграции применяются автоматически при старте через `db.Database.Migrate()`.

### CORS

**[EN]** Add your frontend origin to `WithOrigins` in `Program.cs`:

**[RU]** Добавь адрес фронтенда в `WithOrigins` в `Program.cs`:

```csharp
policy.WithOrigins(
    "http://localhost:5173",
    "https://your-domain.com"
)
.AllowAnyHeader()
.AllowAnyMethod()
.AllowCredentials();
```

---

## Docker Deployment / Деплой через Docker

### Dockerfile

```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 5126

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["TestChatAPI/TestChatAPI.csproj", "TestChatAPI/"]
COPY ["ChatApplication/ChatApplication.csproj", "ChatApplication/"]
COPY ["ChatDomain/ChatDomain.csproj", "ChatDomain/"]
COPY ["ChatPersistance/ChatPersistance.csproj", "ChatPersistance/"]
RUN dotnet restore "TestChatAPI/TestChatAPI.csproj"
COPY . .
WORKDIR "/src/TestChatAPI"
RUN dotnet publish -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "TestChatAPI.dll"]
```

### docker-compose.yaml

```yaml
services:
  chat-api:
    build:
      context: .
      dockerfile: Dockerfile
    container_name: chat-api
    ports:
      - "5126:5126"
    environment:
      - ConnectionStrings__DefaultConnection=Host=db;Database=chatdb;Username=user;Password=pass
      - JWTOptions__SecretKey=your-super-secret-key-minimum-32-characters
      - JWTOptions__ExpiresHours=24
      - ASPNETCORE_URLS=http://+:5126
    depends_on:
      db:
        condition: service_healthy
    restart: unless-stopped
    networks:
      - chat-net

  db:
    image: postgres:16-alpine
    container_name: chat-db
    environment:
      - POSTGRES_USER=user
      - POSTGRES_PASSWORD=pass
      - POSTGRES_DB=chatdb
    volumes:
      - pgdata:/var/lib/postgresql/data
    healthcheck:
      test: ["CMD-SHELL", "pg_isready -U user -d chatdb"]
      interval: 5s
      timeout: 5s
      retries: 5
    restart: unless-stopped
    networks:
      - chat-net

volumes:
  pgdata:

networks:
  chat-net:
    driver: bridge
```

### Nginx

**[EN]** Required headers for SignalR WebSocket support:

**[RU]** Обязательные заголовки для работы SignalR через WebSocket:

```nginx
location /chat {
    proxy_pass http://localhost:5126;
    proxy_http_version 1.1;
    proxy_set_header Upgrade $http_upgrade;
    proxy_set_header Connection "upgrade";
    proxy_set_header Host $host;
}

location /api {
    proxy_pass http://localhost:5126;
    proxy_set_header Host $host;
    proxy_set_header X-Real-IP $remote_addr;
}
```

> **[EN]** Without `proxy_http_version 1.1` and `Upgrade` headers SignalR cannot establish a WebSocket connection and falls back to long-polling.
>
> **[RU]** Без `proxy_http_version 1.1` и заголовков `Upgrade` SignalR не сможет установить WebSocket соединение и упадёт на long-polling.

---

## Implementation Notes / Технические решения

**`ReferenceHandler.IgnoreCycles`**

**[EN]** Enabled globally in `Program.cs`. EF Core navigation properties create circular references (`Chat → Members → ChatMember → Chat`). Without this the JSON serializer enters infinite recursion and throws a `JsonException`.

**[RU]** Включён глобально в `Program.cs`. Навигационные свойства EF Core создают циклические ссылки (`Chat → Members → ChatMember → Chat`). Без этого сериализатор уходит в бесконечную рекурсию и бросает `JsonException`.

```csharp
options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
```

---

**JWT via query string / JWT через query string**

**[EN]** SignalR cannot send cookies during the WebSocket handshake. The token is passed as `?access_token=` and extracted in `OnMessageReceived`:

**[RU]** SignalR не может отправить куки при WebSocket handshake. Токен передаётся через `?access_token=` и читается в `OnMessageReceived`:

```csharp
OnMessageReceived = context =>
{
    if (string.IsNullOrEmpty(context.Token))
        context.Token = context.Request.Cookies["jwtToken"];

    var accessToken = context.Request.Query["access_token"];
    var path = context.HttpContext.Request.Path;
    if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/chat"))
        context.Token = accessToken;

    return Task.CompletedTask;
}
```

---

**Database name case sensitivity / Регистрозависимость имён баз данных**

**[EN]** PostgreSQL database names are case-sensitive. `chatdb` and `ChatDB` are two different databases. Always verify the actual connection string used by the running container:

**[RU]** PostgreSQL различает регистр в именах баз данных. `chatdb` и `ChatDB` — две разные базы. Всегда проверяй реальную строку подключения запущенного контейнера:

```bash
docker exec -it chat-api env | grep Connection
```

---

## Live Demo

**[EN]** A live instance is available at:

**[RU]** Живой экземпляр доступен по адресу:

**http://chat.declensice.dpdns.org/**

**[EN]** Feel free to register an account and test the full functionality — real-time messaging, join requests, chat management.

**[RU]** Можно зарегистрироваться и протестировать весь функционал — сообщения в реальном времени, заявки на вступление, управление чатами.
