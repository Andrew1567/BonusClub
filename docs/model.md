# Класова модель варіанта: FleetMaintenance

## Сутності
| Сутність | Роль | Незмінний | Інваріант |
| :--- | :--- | :--- | :--- |
| `WorkLine` | позиція заявки | так, record | Quantity > 0; UnitPrice >= 0; ServiceCode не порожній |
| `WorkOrder` | документ зі станом | ні, агрегат | Id > 0; ClientId > 0; позиції додають лише у стані Draft; зі стану Completed і Cancelled повернення немає |
| `FleetClient` | довідник учасників | так, record | Id > 0; Name не порожнє; Email містить @ |
| `Vehicle` | довідник об’єктів обліку | ні (змінюється MileageKm) | Id > 0; PlateNumber не порожній; MileageKm >= 0 і не зменшується |

## Стани заявки та дозволені переходи
| Зі стану | До стану | Умова |
| :--- | :--- | :--- |
| Draft | Scheduled | є хоча б одна позиція |
| Draft | Cancelled | завжди |
| Scheduled | InProgress | завжди |
| Scheduled | Cancelled | завжди |
| InProgress | Completed | завжди |
| Completed, Cancelled | — | кінцеві стани, переходів немає |