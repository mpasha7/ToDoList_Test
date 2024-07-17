using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace ToDoList_Test.Models.ViewModels
{
    /// <summary>
    /// Модель представления для делегирования задачи другому пользователю
    /// </summary>
    public class ChangeUserViewModel
    {
        /// <summary>
        /// Id делегируемой задачи
        /// </summary>
        public int TaskId { get; set; }

        /// <summary>
        /// Id пользователя - нового владельца задачи
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// Список существующих пользователей (для выбора)
        /// </summary>
        [ValidateNever]
        public IEnumerable<User> Users { get; set; } = null!;
    }
}
