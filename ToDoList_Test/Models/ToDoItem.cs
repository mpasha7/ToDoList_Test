using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace ToDoList_Test.Models
{
    /// <summary>
    /// Задача
    /// </summary>
    public class ToDoItem
    {
        /// <summary>
        /// Идентификатор задачи в базе данных (первичный ключ)
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Название задачи
        /// </summary>
        [Required(ErrorMessage = "Пожалуйста, введите название задачи")]
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Описание задачи
        /// </summary>
        [Required(ErrorMessage = "Пожалуйста, введите описание задачи")]
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Является ли задача завершенной
        /// </summary>
        public bool IsCompleted { get; set; } = false;

        /// <summary>
        /// Срок завершения задачи
        /// </summary>
        [Required(ErrorMessage = "Пожалуйста, введите срок выполнения задачи")]
        public DateTime? DueDate { get; set; } = null!;

        /// <summary>
        /// Навигационное свойство для связи с объектом приоритета задачи (внешний ключ)
        /// </summary>
        [ValidateNever]
        public virtual Priority Priority { get; set; } = null!;

        /// <summary>
        /// Навигационное свойство для связи с объектом пользователя - владельца задачи (внешний ключ)
        /// </summary>
        [ValidateNever]
        public virtual User User { get; set; } = null!;

        /// <summary>
        /// Является ли задача просроченной
        /// </summary>
        public bool Overdue => !IsCompleted && DueDate < DateTime.Today;
    }
}
