using System.ComponentModel.DataAnnotations;

namespace ToDoList_Test.Models
{
    /// <summary>
    /// Приоритет задачи
    /// </summary>
    public class Priority
    {
        /// <summary>
        /// Уровень приоритета (первичный ключ)
        /// </summary>
        [Key]
        public int Level { get; set; }

        /// <summary>
        /// Навигационное свойство для связи со списком задач, имеющих данный приоритет
        /// </summary>
        public virtual IEnumerable<ToDoItem> ToDoItems { get; set; }

        /// <summary>
        /// Инициализирует новый объект класса Priority
        /// </summary>
        public Priority()
        {
            ToDoItems = new List<ToDoItem>();
        }
    }
}
