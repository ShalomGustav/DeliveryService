# DeliveryService
**Автор:** Фатихов Максим Сергеевич

## Описание
**DeliveryService** — это консольное приложение для получения и обработки заказов на доставку. Приложение позволяет фильтровать заказы по региону и времени доставки, выполнять валидацию данных и записывать результаты и ошибки в лог-файл.

### Содержание
- [Требования](#требования)
- [Структура проекта](#структура-проекта)
  - [ConfigurationFiles](#1-папка-configurationfiles)
  - [Models](#2-папка-models)
  - [Service](#3-папка-service)
- [Особенности](#особенности)
- [Примеры конфигурации и данных](#примеры-конфигурации-и-данных)
  - [Конфигурация через командную строку](#конфигурация-через-командную-строку)
    - [Формат запуска через командную строку](#формат-запуска-через-командную-строку)
  - [Пример конфигурации из файла формата Json](#пример-конфигурации)
    - [Пример данных в формате JSON](#пример-данных-в-формате-json)
    - [Пример данных в формате TXT](#пример-данных-в-формате-txt)
- [Логирование и обработка ошибок](#логирование-и-обработка-ошибок)
  - [Основные типы логируемых событий](#основные-типы-логируемых-событий)
  - [Пример логов](#пример-логов)

## Требования
- .NET 6 или выше
- Newtonsoft.Json для работы с JSON файлами

## Структура проекта

Проект **DeliveryService** организован в несколько папок для упрощения навигации и разделения ответственности между компонентами:

### 1. Папка `ConfigurationFiles`
Эта папка содержит файлы конфигурации и примеры данных для тестирования:

- `config.json`: Основной файл конфигурации приложения, содержащий пути для логирования и заказов, код региона и время первой доставки.
- `test_orders.json`: Пример файла заказов в формате JSON для тестирования.
- `test_orders.txt`: Пример файла заказов в формате текста (CSV) для тестирования.

### 2. Папка `Models`
Папка содержит классы моделей данных приложения:

- `FileConfig.cs`: Класс для представления конфигурации приложения, включающий настройки, такие как пути для логирования, файл заказов, код региона и время доставки.
- `Order.cs`: Класс, представляющий заказ, с полями `OrderId`, `Weight`, `IndexRegion` и `TimeDelivery`. Также содержит метод `ToString()` для упрощенного вывода данных.

### 3. Папка `Service`
Содержит основные сервисные классы для бизнес-логики, логирования и загрузки конфигурации:

- `ConfigLoader.cs`: Класс для загрузки конфигурации из `config.json`, с обработкой ошибок десериализации.
- `FileLogger.cs`: Класс для логирования событий и ошибок, сохраняющий логи в указанный файл.
- `OrderService.cs`: Основной класс обработки заказов, включает методы для загрузки, валидации, фильтрации и сохранения данных заказов.

---

## Особенности
Приложение **DeliveryService** поддерживает обработку заказов из командной строки, а также из файлов в формате JSON, так и в текстовом формате (CSV). В зависимости от расширения файла конфигурации (`.json` или `.txt`), приложение автоматически определяет, какой формат использовать.

- **Формат JSON**: Заказы читаются и десериализуются из JSON-массива объектов, содержащих свойства `OrderId`, `Weight`, `IndexRegion`, `TimeDelivery`.
- **Формат TXT**: Заказы читаются как строки с разделителем, указанным в конфигурации (например, `,`), и парсятся для дальнейшей обработки.

## Примеры конфигурации и данных

### Конфигурация через командную строку

## Пример конфигурации
Приложение **DeliveryService** поддерживает возможность передавать конфигурационные параметры через командную строку. Это позволяет гибко управлять параметрами запуска, не редактируя конфигурационный файл `config.json`.

### Примечание
Будет использован `config.json`, если параметры командной строки не переданы.

### Формат запуска через командную строку

Для запуска приложения с передачей параметров через командную строку используйте следующий формат:

```bash
dotnet run -- [DeliveryOrders] [IndexRegion] [FirstDeliveryTime] [Separator]
```
###Дополнительный вариант запуска через ярлык приложения **DeliveryService**:

1. Создаем ярлык приложения `DeliveryService.exe`.
2. В свойствах ярлыка в разделе `Объект` можно передать строку следующего формата:

``` bash
C:\Полный путь\DeliveryService.exe log.txt test_orders.json 423330 "2024-10-25 09:44:07"
```

`DeliveryService.exe`: Название приложения в данном примере **DeliveryService**.

`log.txt`: Лог-файл.

`test_orders.json`: Список заказов в формате JSON. Разделитель указывать необязательно, в логике сервиса это обрабатывается.

`423330`: Индекс района доставки.

`"2024-10-25 09:44:07"`: Время доставки.

### Параметры командной строки

| Параметр           | Описание                                                                                                 |
|--------------------|----------------------------------------------------------------------------------------------------------|
| `DeliveryOrders`   | Путь к файлу с заказами (может быть JSON или TXT).                                                       |
| `IndexRegion`      | Целое шестизначное число, представляющее код региона для фильтрации заказов.                             |
| `FirstDeliveryTime`| Дата и время первой доставки для фильтрации заказов в формате `yyyy-MM-dd HH:mm:ss`.                     |
| `Separator`        | Символ-разделитель для файлов в формате TXT (например, `,`). Если файл JSON, этот параметр игнорируется. |

### Описание работы приложения при запуске с параметрами командной строки

Инициализация: Приложение инициализируется с использованием переданных параметров.

Проверка и Валидация: Каждый параметр проверяется на соответствие ожидаемому формату:

`DeliveryOrders` — проверяется, что указанный файл существует.

`IndexRegion` — проверяется, что значение является шестизначным числом.

`FirstDeliveryTime` — проверяется формат даты и времени.

`Separator` — используется для разделения полей в TXT-файле заказов (если применимо).

Загрузка и Фильтрация: Приложение загружает заказы из указанного файла, проводит валидацию и фильтрацию, а затем сохраняет результаты в файл, указанный в `ResultFilePath` (по умолчанию путь к файлу может быть указан в `config.json`, если не используется в командной строке).

Логирование: Все операции и возможные ошибки записываются в файл логов.

### Примечания

Если параметры командной строки не переданы, приложение попытается загрузить конфигурацию из файла `config.json`.

В случае указания неправильных значений в параметрах командной строки, приложение запишет ошибку в лог и завершит выполнение.

### Пример конфигурации из файла формата JSON

### Пример конфигурации JSON

Файл `config.json` используется для указания путей и настроек приложения. Пример:

```json
{
  "DeliveryOrders": "ConfigurationFiles\\test_orders.txt",
  "ResultFilePath": "ConfigurationFiles\\filtered_orders.txt",
  "IndexRegion": 423330,
  "FirstDeliveryTime": "2024-10-22 02:31:44"
}

```
### Описание конфигурации

`DeliveryOrders`: Путь к файлу заказов (может быть JSON или TXT).

`ResultFilePath`: Путь для записи отфильтрованных заказов.

`IndexRegion`: Код региона для фильтрации заказов.

`FirstDeliveryTime`: Время первой доставки для фильтрации заказов.

### Пример данных в формате JSON

```json
[
    {
        "OrderId": "fc5fe60c-19c6-4d9c-a582-3d99f3c271a3",
        "Weight": 31.35,
        "IndexRegion": -1,
        "TimeDelivery": "2024-10-25 09:44:07"
    },
    {
        "OrderId": "fc5fd60c-93ca-4f3d-8394-4974008e3aeb",
        "Weight": 11.11,
        "IndexRegion": 423630,
        "TimeDelivery": "2024-10-25 09:47:07"
     },
    {
        "OrderId": "6d358f7e-19b8-4ec4-8717-20c5cd6f52e2",
        "Weight": 6.89,
        "IndexRegion": 423330,
        "TimeDelivery": "2024-10-25 09:46:07"
    },
    {
        "OrderId": "6e83bb34-faa3-4418-abed-f5c91625a0bc",
        "Weight": 6.44,
        "IndexRegion": 814881,
        "TimeDelivery": "2024-11-25 09:47:07"
    },
    {
        "OrderId": "a8cf3387-f4ea-44fb-bcb3-0804923868c8",
        "Weight": 28.34,
        "IndexRegion": 244787,
        "TimeDelivery": "2024-10-25 09:54:07"
    },
    {
        "OrderId": "d5fa030a-ddda-47b7-b69b-8a752862d7b9",
        "Weight": 47.26,
        "IndexRegion": 174969,
        "TimeDelivery": "2024-10-25 09:44:07"
    },
    {
        "OrderId": "4f0d76cb-9ef6-4ce9-abbb-643757bac386",
        "Weight": 32.7,
        "IndexRegion": 961531,
        "TimeDelivery": "2024-10-24 09:44:07"
    },
    {
        "OrderId": "a91d0c5a-98f7-440c-bf26-3e2790d7c17d",
        "Weight": 18.32,
        "IndexRegion": 367530,
        "TimeDelivery": "2024-10-30 09:44:07"
    },
    {
        "OrderId": "0be0f60e-a0b4-4422-a71f-b23afded78c1",
        "Weight": 2.21,
        "IndexRegion": 582426,
        "TimeDelivery": "2024-11-01 09:44:07"
    },
    {
        "OrderId": "1aa418fe-c1fa-4da9-8cc0-d3c3d02f76a0",
        "Weight": 24.35,
        "IndexRegion": 927261,
        "TimeDelivery": "2024-10-28 09:44:07"
    }
]
```
### Описание 

`OrderId`: Уникальный идентификатор заказа в формате `Guid`.

`Weight`:  Вес товара в формате `double`.

`IndexRegion`: Шестизначный индекс региона в формате `int`.

`TimeDelivery`: Дата и время доставки в формате `yyyy-MM-dd HH:mm:ss`.

### Пример данных в формате TXT

```txt
df204a7a-73a4-42ff-8c70-8caaa5e56a6b,30.03,423330,2024-10-22 02:31:44
11e91275-bd54-4560-ba78-475db9809e7a,23.89,423330,2024-10-22 02:54:43
4a1df1cb-00b4-457f-a445-5526d20c77b5,7.17,423330,2024-10-22 15:40:43
0e23475e-b3a9-4471-b407-02eec6ecc32c,13.09,389836,2024-10-22 01:46:43
f7bbecac-685b-418e-9845-b82c91622417,5.15,270261,2024-10-22 05:11:43
984e4b5d-b274-4e07-808a-7e1fab21e07d,1.32,654289,2024-10-21 22:57:43
fe532d05-e9f8-4ddf-9000-f6b1af6ed2c1,8.16,557036,2024-10-22 10:01:43
121eca92-b33d-429f-aa9b-0d19af8b9b5b,35.04,411166,2024-10-21 22:27:43
7fdb40eb-e7a8-4d73-bed1-ad8769897ff6,29.96,565688,2024-10-22 04:29:43
7d5e33ee-62dd-4c7f-9b0f-6bb1c0c970fb,25.71,123424,2024-10-21 21:43:43
```
### Описание

`df204a7a-73a4-42ff-8c70-8caaa5e56a6b`: Уникальный идентификатор заказа (OrderId).

`30.03`: Вес товара.

`423330`: Шестизначный индекс региона (IndexRegion).

`2024-10-22 02:31:44`: Дата и время доставки (TimeDelivery).

### Логирование и обработка ошибок

### Основные типы логируемых событий

Приложение DeliveryService реализует функциональность логирования всех основных операций, включая ошибки валидации и загрузки данных.

`FileLogger`: Класс для логирования событий и ошибок. 

Логи записываются в файл, указанный в конфигурации, или в файл по умолчанию `default_log.txt`.

### Основные типы логируемых событий

Инициализация: Логирование успешного запуска приложения и загрузки конфигурации.

Ошибки валидации: Например, если заказ имеет некорректный `OrderId` или неверный вес.

Ошибки загрузки данных: Включает ошибки формата JSON, отсутствующие или поврежденные файлы, и ошибки десериализации.

### Пример логов

```plaintext 
2024-10-27 18:34:51:Program started...
2024-10-27 18:34:51:Reading parameters from config file...
2024-10-27 18:34:51:Initializing order processing...
2024-10-27 18:34:51:Starting order processing...
2024-10-27 18:34:51:Loaded 10 orders from ConfigurationFiles\test_orders.txt
2024-10-27 18:34:51:Filtered 2 orders for region 423330
2024-10-27 18:34:51:Successfully saved 2 orders to ConfigurationFiles\filtered_orders.txt.
2024-10-27 18:34:51:Filtered orders saved to ConfigurationFiles\filtered_orders.txt
2024-10-27 18:34:51:Order processing completed
```
### Валидация данных



