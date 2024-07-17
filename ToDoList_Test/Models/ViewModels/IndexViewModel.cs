namespace ToDoList_Test.Models.ViewModels
{
    /// <summary>
    /// Модель представления для основного интерфейса приложения
    /// </summary>
    public class IndexViewModel
    {
        /// <summary>
        /// Список отображаемых задач
        /// </summary>
        public IEnumerable<ToDoItem> Tasks { get; set; } = null!;

        /// <summary>
        /// Список приоритетов (для выбора и фильтрации)
        /// </summary>
        public IEnumerable<Priority> Priorities { get; set; } = null!;

        /// <summary>
        /// Список возможных статусов завершенности (для выбора и фильтрации)
        /// </summary>
        public Dictionary<int, string> Statuses { get; set; } = new Dictionary<int, string>
        {
            { 0, "Все" },
            { 1, "Открытые" },
            { 2, "Закрытые" }
        };

        /// <summary>
        /// Фильтратор задач
        /// </summary>
        public Filter Filter { get; set; } = null!;
    }
}
