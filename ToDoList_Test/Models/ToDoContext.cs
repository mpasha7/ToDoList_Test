using Microsoft.EntityFrameworkCore;

namespace ToDoList_Test.Models
{
    /// <summary>
    /// Контекст базы данных
    /// </summary>
    public class ToDoContext : DbContext
    {
        /// <summary>
        /// Инициализирует новый объект класса ToDoContext, используя указанные опции
        /// </summary>
        /// <param name="options">Опции контекта базы данных</param>
        public ToDoContext(DbContextOptions<ToDoContext> options) : base(options) { }

        /// <summary>
        /// Коллекция всех объектов ToDoItem, определенных в соответствующей таблице базы данных
        /// </summary>
        public DbSet<ToDoItem> ToDoItems { get; set; } = null!;

        /// <summary>
        /// Коллекция всех объектов Priority, определенных в соответствующей таблице базы данных
        /// </summary>
        public DbSet<Priority> Priorities { get; set; } = null!;

        /// <summary>
        /// Коллекция всех объектов User, определенных в соответствующей таблице базы данных
        /// </summary>
        public DbSet<User> Users { get; set; } = null!;

        /// <summary>
        /// Инициализирует базу данных начальными данными при её создании
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Priority>().HasData(
                new Priority { Level = 1 },
                new Priority { Level = 2 },
                new Priority { Level = 3 },
                new Priority { Level = 4 },
                new Priority { Level = 5 }
                );

            modelBuilder.Entity<User>().HasData(
                new User { Id = 1, Name = "Павел" }
                );
        }
    }
}
