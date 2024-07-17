namespace ToDoList_Test.Models
{
    /// <summary>
    /// Интерфейс хранилища данных
    /// </summary>
    public interface IRepository
    {
        IQueryable<ToDoItem> Tasks { get; }
        IEnumerable<Priority> Priorities { get; }
        IEnumerable<User> Users { get; }

        Task<ToDoItem?> GetTask(int id);
        Task<IEnumerable<ToDoItem>> GetCompleteTasks(int userid);
        Task CreateTaskAsync(ToDoItem t);
        Task DeleteTaskAsync(ToDoItem t);
        Task DeleteTaskRangeAsync(IEnumerable<ToDoItem> tasks);
        Task SaveTaskAsync(ToDoItem t);

        Task<User?> GetUser(int id);
        Task<User?> GetUserByName(string name);
        Task CreateUserAsync(User u);
        Task DeleteUserAsync(User u);

        Task<Priority?> GetPriority(int level);
    }
}
