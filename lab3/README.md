
# ЛР3 — Система управления заказами (C# / .NET 8, xUnit)

## Запуск тестов
```bash
# В корне архива:
dotnet test FoodDelivery.Tests/FoodDelivery.Tests.csproj -v n
```

## Что внутри
- **FoodDelivery.Domain** — доменная библиотека с паттернами (State, Strategy, Decorator, Chain of Responsibility, Factory Method, Builder, Repository, Observer).
- **FoodDelivery.Tests** — юнит‑тесты на xUnit.

См. комментарии в коде по каждому паттерну.
