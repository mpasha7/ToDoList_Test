using System.ComponentModel.DataAnnotations;

namespace ToDoList_Test.Models
{
    /// <summary>
    /// Пользователь
    /// </summary>
    public class User
    {
        /// <summary>
        /// Идентификатор пользователя в базе данных (первичный ключ)
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Имя пользователя
        /// </summary>
        [Required(ErrorMessage = "Пожалуйста, введите имя пользователя")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Навигационное свойство для связи со списком задач данного пользователя
        /// </summary>
        public virtual IEnumerable<ToDoItem> ToDoItems { get; set; }

        /// <summary>
        /// Инициализирует новый объект класса User
        /// </summary>
        public User()
        {
            ToDoItems = new List<ToDoItem>();
        }
    }
}
