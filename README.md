# Admin Panel SPA

SPA-приложение для администрирования с React/Vite фронтендом и ASP.NET Core бэкендом.

## Требования

- .NET 6+
- Node.js 16+
- Docker (для PostgreSQL)

## Быстрый запуск

1. Откройте корневую папку проекта в терминале

2. Запустите бэкенд (API на порту 5000):
   ```bash
   dotnet run
   ```

3. В новом терминале запустите фронтенд (React на порту 5173):
   ```bash
   npm run dev
   ```

**Данные для входа:**  
Email: `admin@mirra.dev`  
Пароль: `admin123`

## Запуск с PostgreSQL

1. Запустите приложение Docker

2. Откройте корневую папку проекта в терминале

3. Используйте команду:
```docker
docker-compose up
```

## Примеры API запросов

Авторизация:
```bash
curl -X POST "http://localhost:5000/auth/login" \
-H "Content-Type: application/json" \
-d '{"email":"admin@mirra.dev","password":"admin123"}'
```

Получение списка клиентов (требует авторизации):
```bash
curl -X GET "http://localhost:5000/clients" \
-H "Authorization: Bearer YOUR_JWT_TOKEN"
```
