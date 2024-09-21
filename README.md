# Цель
Разработать симулятор гонок.
# Задача
Спроектировать классовую модель видов транспорта, и реализовать симуляцию разных видов гонок.
# Выполнение
Все виды транспортных делятся на две абстракции: *ВОЗДУШНЫЕ* и *НАЗЕМНЫЕ*

Наземные виды транспорта имеют следующие характеристики:
- скорость движения (в условных единицах);
- время движения до необходимого отдыха (в условных единицах);
- длительность отдыха, которая зависит от порядкового номера остановки (условных единицах).

Воздушные виды транспорта характеризуются:
- скоростью движения (в условных единицах);
- коэффициентом ускорения (задается формулой, зависит от расстояния).

Гонки бывают трех видов:
1. только для наземного транспорта;
2. только для воздушного транспорта;
3. для всех типов транспортных средств.

*На гонку для наземного транспорта нельзя зарегистрировать воздушное транспортное средство и наоборот*

Также добавлены разные погодные условия, которые так или иначе влияют на типы транспорта:
- Солнечно: не влияет на время прохождения дистанции;
- Дождь: увеличивает время прохождения дистанции на *10%* для наземного транспорта и на *15%* для воздушного;
- Ветер: увеличивает время прохождения дистанции на *5%* для наземного транспорта и на *20%* для воздушного;
- Туман: не влияет на наземный транспорт, но увеличивает время прохождения дистанции на *10%* для воздушного транспорта.

Разработанное решение:
- Использует **Spectre.Console** для улучшения внешнего вида консольного приложения.  
Чтобы начать использовать **Spectre.Console** в своем проекте .NET, нужно добавить соответствующий пакет NuGet:

```
Install-Package Spectre.Console
Install-Package Spectre.Console.Cli
```

Также можно установить его с помощью диспетчера пакетов NuGet в Visual Studio;
- Соблюдает принципы SOLID;
- При попытке добавить неподходящее ТС гонка не запускается;
- Характеристики ТС задаются разными формулами;
- Пользователь имеет возможность выбрать тип гонки, её дистанцию, погодные условия, ТС, которые в ней будут участвовать и запустить гонку.
