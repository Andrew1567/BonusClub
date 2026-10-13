# sr02-audit-Прізвище

Самостійна робота № 2 «Аудит якості та рефакторинг успадкованого коду», варіант 16, проєкт FleetMaintenance.

- Варіант: 16. Модуль-донор: `MaintenancePlanner.Plan`. Профільний дефект: дублювання перевірок пробігу й дати.
- Джерело коду: А (пакет успадкованого коду, 9 файлів, 600 непорожніх рядків, .NET 8).

## Як запустити
```
dotnet build
dotnet run --project src
dotnet test
dotnet test --collect:"XPlat Code Coverage"
```

## Теги
`baseline` — вихідний код; `safety-net` — сітка тестів готова; `after-refactoring` — усі 12 рефакторингів виконано.

## Порядок роботи
1. Коміт `baseline: вихідний код до аудиту` (теки `src`, `README.md`, `.gitignore`), тег `baseline`.
2. Коміт `test: шов для часу` — замінити `src/MaintenancePlanner.cs` файлом із `_reference/step-seam/`.
3. Коміт `test: страхувальна сітка характеризаційних тестів` (тека `Legacy.Tests`), тег `safety-net`.
4. Коміти `refactor(...)` у порядку таблиці розділу 5 звіту; результат кожного кроку можна звіряти з `_reference/after-src/`.
5. Після R11 замінити `Legacy.Tests/ReportTests.cs` файлом із `_reference/after-tests/` окремим комітом `test:`.
6. Тег `after-refactoring`; `docs/metrics-after.csv`, `docs/out-after.txt`.

Тека `_reference` — довідкова: її в репозиторій комітити не потрібно.
