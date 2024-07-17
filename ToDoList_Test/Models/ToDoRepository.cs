
using Microsoft.EntityFrameworkCore;

namespace ToDoList_Test.Models
{
    /// <summary>
    /// Реализация хранилища данных
    /// </summary>
    public class ToDoRepository : IRepository
    {
        /// <summary>
        /// Контекст базы данных
        /// </summary>
        private ToDoContext context;

        /// <summary>
        /// Инициализирует новый объект класса ToDoRepository, используя контекст базы данных ToDoContext
        /// </summary>
        /// <param name="context"></param>
        public ToDoRepository(ToDoContext context)
        {
            this.context = context;
        }

        /// <summary>
        /// Возвращает объект, запрашивающий список задач из базы данных (с учетом связанных данных о пользователях и приоритетах)
        /// </summary>
        public IQueryable<ToDoItem> Tasks => context.ToDoItems.Include(t => t.User).Include(t => t.Priority);

        /// <summary>
        /// Возвращает из базы данных перечисляемый список приоритетов
        /// </summary>
        public IEnumerable<Priority> Priorities => context.Priorities.ToList();

        /// <summary>
        /// Возвращает из базы данных перечисляемый список пользователей
        /// </summary>
        public IEnumerable<User> Users => context.Users.ToList();

        /// <summary>
        /// Возвращает из базы данных объект задачи по её Id
        /// </summary>
        /// <param name="id">Id задачи</param>
        /// <returns></returns>
        public async Task<ToDoItem?> GetTask(int id)
        {
            return await context.ToDoItems.FirstOrDefaultAsync(t => t.Id == id);
        }

        /// <summary>
        /// Возвращает из базы данных перечисляемый список завершенных задач пользователя по его Id
        /// </summary>
        /// <param name="userid">Id пользователя</param>
        /// <returns></returns>
        public async Task<IEnumerable<ToDoItem>> GetCompleteTasks(int userid)
        {
            IQueryable<ToDoItem> tasks = context.ToDoItems.Where(t => t.User.Id == userid && t.IsCompleted);
            return await tasks.ToListAsync();
        }

        /// <summary>
        /// Создает и сохраняет в базе данных новый объект задачи
        /// </summary>
        /// <param name="t">Новая задача</param>
        /// <returns></returns>
        public async Task CreateTaskAsync(ToDoItem t)
        {
            await context.AddAsync(t);
            await context.SaveChangesAsync();
        }

        /// <summary>
        /// Удаляет из базы данных объект задачи
        /// </summary>
        /// <param name="t">Удаляемая задача</param>
        /// <returns></returns>
        public async Task DeleteTaskAsync(ToDoItem t)
        {
            context.Remove(t);
            await context.SaveChangesAsync();
        }

        /// <summary>
        /// Удаляет из базы данных список задач
        /// </summary>
        /// <param name="tasks">Список удаляемых задач</param>
        /// <returns></returns>
        public async Task DeleteTaskRangeAsync(IEnumerable<ToDoItem> tasks)
        {
            context.RemoveRange(tasks);
            await context.SaveChangesAsync();
        }

        /// <summary>
        /// Сохраняет в базе данных изменения, совершенные над объектом задачи
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public async Task SaveTaskAsync(ToDoItem t)
        {
            await context.SaveChangesAsync();
        }

        /// <summary>
        /// Возвращает из базы данных объект пользователя по его Id
        /// </summary>
        /// <param name="id">Id пользователя</param>
        /// <returns></returns>
        public async Task<User?> GetUser(int id)
        {
            return await context.Users.FirstOrDefaultAsync(u => u.Id == id);
        }

        /// <summary>
        /// Возвращает из базы данных объект пользователя по его имени
        /// </summary>
        /// <param name="name">Имя пользователя</param>
        /// <returns></returns>
        public async Task<User?> GetUserByName(string name)
        {
            return await context.Users.FirstOrDefaultAsync(u => u.Name == name);
        }

        /// <summary>
        /// Создает и сохраняет в базе данных новый объект пользователя
        /// </summary>
        /// <param name="u">Новый пользователь</param>
        /// <returns></returns>
        public async Task CreateUserAsync(User u)
        {
            await context.AddAsync(u);
            await context.SaveChangesAsync();
        }

        /// <summary>
        /// Удаляет из базы данных объект пользователя
        /// </summary>
        /// <param name="u">Удаляемый пользователь</param>
        /// <returns></returns>
        public async Task DeleteUserAsync(User u)
        {
            context.Remove(u);
            await context.SaveChangesAsync();
        }

        /// <summary>
        /// Возвращает из базы данных объект приоритета по его уровню
        /// </summary>
        /// <param name="level">Уровень приоритета</param>
        /// <returns></returns>
        public async Task<Priority?> GetPriority(int level)
        {
            return await context.Priorities.FirstOrDefaultAsync(p => p.Level == level);
        }
    }
}
