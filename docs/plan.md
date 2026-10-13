# План рефакторингу

Індекс = В ÷ Р; сортування за спаданням, за однакових індексів вище менш трудомісткий пункт.

| ID | Рефакторинг | В | Р | Т, хв | Індекс | Черга |
|---|---|---|---|---|---|---|
| D-03 | R04 Replace Magic Number with Constant | 4 | 1 | 20 | 4.00 | 1 |
| D-01 | R02 Rename (km → kmSince, mo → monthsSince, st → state, v → vehicle) | 3 | 1 | 15 | 3.00 | 2 |
| D-01 | R05 Decompose Conditional (StateFor, IsReached) | 5 | 2 | 40 | 2.50 | 3 |
| D-02 | R01 Extract Method (MonthsBetween, CheckService, Log, ReportedCost, TryParse, Line) | 5 | 2 | 60 | 2.50 | 4 |
| D-10 | R17 Remove Dead Code | 2 | 1 | 5 | 2.00 | 5 |
| D-01 | R03 Extract Variable (byKm, byMonths) | 2 | 1 | 10 | 2.00 | 6 |
| D-11 | R08 Replace Conditional with Table (інтервали, ціни, NextDueKm) | 4 | 2 | 45 | 2.00 | 7 |
| D-08 | R10 Move Method (ReportedCost → ServiceTask) | 3 | 2 | 20 | 1.50 | 8 |
| D-12 | R06 Replace Nested Conditional with Guard Clauses (Parse) | 3 | 2 | 25 | 1.50 | 9 |
| D-08 | R13 Encapsulate Collection (FleetRepository.Items → All) | 3 | 2 | 25 | 1.50 | 10 |
| D-04 | R09 Extract Class (ServicePolicy) | 4 | 3 | 50 | 1.33 | 11 |
| D-05 | R11 Introduce Parameter Object (ReportOptions) | 3 | 3 | 30 | 1.00 | 12 |
| D-07 | R12 Replace Primitive with Value Object (вид і стан) | 4 | 4 | 120 | 1.00 | відхилено |
| D-09 | R20 Replace Error Code with Exception | 4 | 5 | 60 | 0.80 | відхилено |
| D-06 | R09 Extract Class (ServiceHistory із Vehicle) | 3 | 4 | 90 | 0.75 | відхилено |

## Правила безпеки

1. Один рефакторинг — один коміт.
2. Крок довший за 20 хвилин розбивається на менші.
3. Після кожного кроку — dotnet build і dotnet test.
4. Червоні тести понад 10 хвилин — git checkout . і спочатку меншими порціями.
5. Спочатку кроки з низьким ризиком.
6. Знайдену помилку не виправляти в refactor-коміті, а вносити в docs/debt.md.
7. Очікувані значення в тестах не змінювати.
8. Два коди рефакторингу в одному коміті не змішувати.