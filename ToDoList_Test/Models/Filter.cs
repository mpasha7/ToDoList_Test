namespace ToDoList_Test.Models
{
    /// <summary>
    /// Фильтратор задач
    /// </summary>
    public class Filter
    {
        /// <summary>
        /// Фильтр по Id пользователя
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// Фильтр по уровню приоритета
        /// </summary>
        public int PriorityLevel { get; set; }

        /// <summary>
        /// Фильтр по статусу завершенности
        /// </summary>
        public int CompleteStatus { get; set; }

        /// <summary>
        /// Массив фильтров
        /// </summary>
        public int[] Filters { get; set; }

        /// <summary>
        /// Инициализирует новый объект класса HomeController, используя массив фильтров
        /// </summary>
        /// <param name="filters">Массив фильтров</param>
        public Filter(int[] filters)
        {
            if (filters == null || filters.Length == 0)
                filters = new[] { 0, 0, 0 };
            Filters = filters;
            UserId = filters[0];
            PriorityLevel = filters[1];
            CompleteStatus = filters[2];
        }

        /// <summary>
        /// Фильтрует переданный список задач с применением своих фильтров
        /// </summary>
        /// <param name="tasks">Список фильтруемых задач</param>
        /// <returns></returns>
        public IEnumerable<ToDoItem> FilterTasks(IQueryable<ToDoItem> tasks)
        {
            if (UserId > 0)
            {
                tasks = tasks.Where(t => t.User.Id == UserId);
            }
            if (PriorityLevel > 0)
            {
                tasks = tasks.Where(t => t.Priority.Level == PriorityLevel);
            }
            if (CompleteStatus > 0)
            {
                tasks = tasks.Where(t => t.IsCompleted == (CompleteStatus == 2));
            }
            return tasks.OrderBy(t => t.DueDate).ThenBy(t => t.Priority.Level).ToList();
        }
    }
}
