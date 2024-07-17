using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using ToDoList_Test.Models;

namespace ToDoList_Test.Controllers
{
    /// <summary>
    /// API-контроллер для действий над объектами задач (ToDoItem)
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class TasksController : ControllerBase
    {
        /// <summary>
        /// Контекст базы данных
        /// </summary>
        private readonly ToDoContext _context;

        /// <summary>
        /// Инициализирует новый объект класса TasksController, используя контекст базы данных ToDoContext
        /// </summary>
        /// <param name="context">Объект контекста базы данных</param>
        public TasksController(ToDoContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Возвращает из базы данных объект пользователя (User) с его списком задач (ToDoItem)
        /// </summary>
        /// <param name="userid">Идентификатор пользователя в базе данных</param>
        /// <returns></returns>
        // GET: api/Tasks/all/1
        [HttpGet("all/{userid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<User>> GetTasks(int userid)
        {
            User? userWithTasks = await _context.Users.Include(u => u.ToDoItems).ThenInclude(t => t.Priority).FirstOrDefaultAsync(u => u.Id == userid);
            if (userWithTasks != null)
            {
                if (userWithTasks.ToDoItems != null && userWithTasks.ToDoItems.Count() > 0)
                {
                    foreach (var task in userWithTasks.ToDoItems)
                    {
                        task.Priority.ToDoItems = null!;
                        task.User = null!;
                    }
                }
                return userWithTasks;
            }
            return NotFound();
        }

        /// <summary>
        /// Возвращает объект пользователя (User) с его списком задач (ToDoItem)
        /// </summary>
        /// <param name="userid">Идентификатор пользователя (User.Id) в базе данных</param>
        /// <param name="priority">Уровень приоритета для фильтрации задач (Priority.Level)</param>
        /// <param name="status">Статус выполнения для фильтрации задач (0 - все, 1 - открытые, 2 - закрытые)</param>
        /// <returns></returns>
        // GET: api/Tasks/filtred/1?priority=1&status=1
        [HttpGet("filtred/{userid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<User>> GetFiltredTasks(int userid, int priority = 0, int status = 0)
        {
            User? user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userid);
            if (user == null || !PriorityExists(priority) || !StatusExists(status))
            {
                return NotFound();
            }

            var tasks = _context.ToDoItems.Where(t => t.User.Id == userid).Include(t => t.Priority).Include(t => t.User);
            Filter filter = new Filter(new int[] { userid, priority, status });
            user.ToDoItems = filter.FilterTasks(tasks);
            foreach (var task in user.ToDoItems)
            {
                task.Priority.ToDoItems = null!;
                task.User = null!;
            }
            return user;
        }

        /// <summary>
        /// Возвращает объект задачи по её Id (без связанных данных)
        /// </summary>
        /// <param name="id">Id задачи</param>
        /// <returns></returns>
        // GET: api/Tasks/5
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ToDoItem>> GetTask(int id)
        {
            var toDoItem = await _context.ToDoItems.FindAsync(id);

            if (toDoItem == null)
            {
                return NotFound();
            }

            return toDoItem;
        }

        /// <summary>
        /// Возвращает объект задачи по её Id (с учетом связанных данных)
        /// </summary>
        /// <param name="id">Id задачи</param>
        /// <returns></returns>
        // GET: api/Tasks/full/5
        [HttpGet("full/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ToDoItem>> GetTaskFullInfo(int id)
        {
            var task = await _context.ToDoItems.Include(t => t.Priority).Include(t => t.User).FirstOrDefaultAsync(t => t.Id == id);

            if (task == null)
            {
                return NotFound();
            }
            task.Priority.ToDoItems = null!;
            task.User.ToDoItems = null!;
            return task;
        }

        /// <summary>
        /// Заменяет данные выбранного объекта задачи (ToDoItem) на данные представленные в теле запроса
        /// </summary>
        /// <param name="id">Id задачи</param>
        /// <param name="task">Объект задачи (ToDoItem), формируемый из тела запроса</param>
        /// <param name="userid">Id пользователя (указывается при делегировании задачи другому пользователю)</param>
        /// <param name="priority">Уровень приоритета задачи (указывается при смене приоритета задачи)</param>
        /// <returns></returns>
        // PUT: api/Tasks/5?userid=1&priority=1
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> PutTask(int id, ToDoItem task, int userid = 0, int priority = 0)
        {
            if (id != task.Id)
            {
                return BadRequest();
            }

            if (userid > 0)
            {
                User? user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userid);
                if (user == null)
                {
                    return NotFound();
                }
                task.User = user;
            }
            if (priority > 0)
            {
                Priority? priorityObject = await _context.Priorities.FirstOrDefaultAsync(p => p.Level == priority);
                if (priorityObject == null)
                {
                    return NotFound();
                }
                task.Priority = priorityObject;
            }
            _context.Entry(task).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TaskExists(id))
                {
                    return NotFound();
                }
                else
                {
                    return Conflict();
                }
            }
            return NoContent();
        }

        /// <summary>
        /// Размещает в базе данных новый объект задачи (ToDoItem)
        /// </summary>
        /// <param name="task">Объект задачи (ToDoItem), формируемый из тела запроса</param>
        /// <param name="userid">Id пользователя, владеющего новой задачей</param>
        /// <param name="priority">Уровень приоритета новой задачи</param>
        /// <returns></returns>
        // POST: api/Tasks?userid=1&priority=1
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ToDoItem>> PostTask(ToDoItem task, int userid, int priority)
        {
            User? user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userid);
            Priority? priorityObject = await _context.Priorities.FirstOrDefaultAsync(p => p.Level == priority);
            if (user == null || priorityObject == null)
            {
                return NotFound();
            }

            task.User = user;
            task.Priority = priorityObject;
            _context.ToDoItems.Add(task);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTask), new { id = task.Id }, task);
        }

        /// <summary>
        /// Удаляет из базы данных объект задачи (ToDoItem) с выбранным Id
        /// </summary>
        /// <param name="id">Id удаляемой задачи</param>
        /// <returns></returns>
        // DELETE: api/Tasks/5
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteTask(int id)
        {
            var task = await _context.ToDoItems.FindAsync(id);
            if (task == null)
            {
                return NotFound();
            }

            _context.ToDoItems.Remove(task);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>
        /// Проверяет наличие в базе данных задачи (ToDoItem) с указанным Id
        /// </summary>
        /// <param name="id">Id искомой задачи</param>
        /// <returns></returns>
        private bool TaskExists(int id)
        {
            return _context.ToDoItems.Any(e => e.Id == id);
        }

        /// <summary>
        /// Проверяет наличие в базе данных пользователя (User) с указанным Id
        /// </summary>
        /// <param name="id">Id искомого пользователя</param>
        /// <returns></returns>
        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.Id == id);
        }

        /// <summary>
        /// Проверяет корректность параметра фильтрации задач по уровню их приоритета (Priority.Level)
        /// </summary>
        /// <param name="level">Параметр фильтрации задач по уровню их приоритета</param>
        /// <returns></returns>
        private bool PriorityExists(int level)
        {
            if (level == 0)
                return true;
            return _context.Priorities.Any(e => e.Level == level);
        }

        /// <summary>
        /// Проверяет корректность параметра фильтрации задач по статусу их завершенности (0 - все, 1 - открытые, 2 - закрытые)
        /// </summary>
        /// <param name="id">Параметр фильтрации задач по статусу их завершенности</param>
        /// <returns></returns>
        private bool StatusExists(int id)
        {
            return id >= 0 & id < 3;
        }
    }
}
