using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace ToDoList_Test.Models.ViewModels
{
    /// <summary>
    /// Модель представления для создания новой задачи
    /// </summary>
    public class AddTaskViewModel
    {
        /// <summary>
        /// Создаваемая задача
        /// </summary>
        public ToDoItem ToDoItem { get; set; } = null!;

        /// <summary>
        /// Id пользователя - владельца задачи
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// Приоритет задачи
        /// </summary>
        public int PriorityLevel { get; set; }

        /// <summary>
        /// Список существующих пользователей (для выбора)
        /// </summary>
        [ValidateNever]
        public IEnumerable<User> Users { get; set; } = null!;

        /// <summary>
        /// Список существующих приоритетов (для выбора)
        /// </summary>
        [ValidateNever]
        public IEnumerable<Priority> Priorities { get; set; } = null!;

    }
}
