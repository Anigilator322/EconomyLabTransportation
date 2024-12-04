using Google.OrTools.ConstraintSolver;

class Test
{
    public void Main()
    {
        // 1. Данные задачи
        int[,] distanceMatrix = {
            { 0, 10, 15, 20 },  // От склада к точкам и обратно
            { 10, 0, 35, 25 },
            { 15, 35, 0, 30 },
            { 20, 25, 30, 0 }
        };
        int[] demands = { 0, 10, 20, 30 }; // Потребности (0 для склада)
        int vehicleCount = 2;             // Количество машин
        int depot = 0;                    // Склад

        long[] vehicleCapacities = { 40, 50 }; // Вместимость машин
        int[] vehicleBaseCosts = { 100, 120 }; // Базовая стоимость аренды машин
        int overtimeCost = 20;                // Стоимость переработки в час

        // 2. Создание модели маршрутизации
        RoutingIndexManager manager = new RoutingIndexManager(distanceMatrix.GetLength(0), vehicleCount, depot);
        RoutingModel routing = new RoutingModel(manager);

        // 3. Функция расстояний
        int transitCallbackIndex = routing.RegisterTransitCallback((long fromIndex, long toIndex) => {
            int fromNode = manager.IndexToNode(fromIndex);
            int toNode = manager.IndexToNode(toIndex);
            return distanceMatrix[fromNode, toNode];
        });
        routing.SetArcCostEvaluatorOfAllVehicles(transitCallbackIndex);

        // 4. Ограничение по вместимости
        int demandCallbackIndex = routing.RegisterUnaryTransitCallback((long fromIndex) => {
            int fromNode = manager.IndexToNode(fromIndex);
            return demands[fromNode];
        });
        routing.AddDimensionWithVehicleCapacity(
            demandCallbackIndex,    // Потребности точек
            0,                      // Нулевое переполнение
            vehicleCapacities,      // Вместимость машин
            true,                   // Запрет переполнения
            "Capacity"
        );

        // 5. Ограничение по времени (или стоимости)
        // Здесь можно добавить аналогично дополнительные параметры

        // 6. Задание поиска
        RoutingSearchParameters searchParameters = operations_research_constraint_solver.DefaultRoutingSearchParameters();
        searchParameters.FirstSolutionStrategy = FirstSolutionStrategy.Types.Value.PathCheapestArc;
        searchParameters.LocalSearchMetaheuristic = LocalSearchMetaheuristic.Types.Value.GuidedLocalSearch;
        searchParameters.TimeLimit = new Google.Protobuf.WellKnownTypes.Duration { Seconds = 10 };

        // 7. Решение
        Assignment solution = routing.SolveWithParameters(searchParameters);
        if (solution != null)
        {
            PrintSolution(routing, manager, solution);
        }
        else
        {
            Console.WriteLine("Нет решения.");
        }
    }

    static void PrintSolution(RoutingModel routing, RoutingIndexManager manager, Assignment solution)
    {
        Console.WriteLine("Решение:");
        long totalDistance = 0;
        for (int i = 0; i < routing.Vehicles(); i++)
        {
            Console.WriteLine($"Маршрут для машины {i + 1}:");
            long routeDistance = 0;
            var index = routing.Start(i);
            while (!routing.IsEnd(index))
            {
                Console.Write($"{manager.IndexToNode(index)} -> ");
                var previousIndex = index;
                index = solution.Value(routing.NextVar(index));
                routeDistance += routing.GetArcCostForVehicle(previousIndex, index, i);
            }
            Console.WriteLine($"{manager.IndexToNode(index)}");
            Console.WriteLine($"Расстояние маршрута: {routeDistance}");
            totalDistance += routeDistance;
        }
        Console.WriteLine($"Общее расстояние: {totalDistance}");
    }
}
