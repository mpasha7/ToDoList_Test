using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using ToDoList_Test.Models;
using ToDoList_Test.Models.ViewModels;

namespace ToDoList_Test.Controllers
{
    /// <summary>
    /// Контроллер, определяющий бизнес-логику приложения ToDoList
    /// </summary>
    public class HomeController : Controller
    {
        /// <summary>
        /// Хранилище данных
        /// </summary>
        private readonly IRepository _repository;

        /// <summary>
        /// Инициализирует новый объект класса HomeController, используя объект хранилища данных
        /// </summary>
        /// <param name="repo">Объект хранилища данных</param>
        public HomeController(IRepository repo)
        {
            _repository = repo;
        }

        /// <summary>
        /// Предоставляет клиенту интерфейс для входа в приложение
        /// </summary>
        /// <returns></returns>
        public IActionResult Login()
        {
            if (HttpContext.Session.Keys.Contains("userid"))
            {
                return RedirectToAction(nameof(Index), new { filters = GetFiltersFromSession() });
            }
            return View(new LoginViewModel());
        }

        /// <summary>
        /// Осуществляет вход в приложение по имени пользователя
        /// </summary>
        /// <param name="model">Модель представления для входа в приложение</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                User? user = await _repository.GetUserByName(model.UserName);
                if (user != null)
                {
                    HttpContext.Session.SetInt32("userid", user.Id);
                    return RedirectToAction(nameof(Index), new { filters = GetFiltersFromSession() });
                }
                else
                {
                    ModelState.AddModelError("UserName", "Пользователя с таким именем не существует");
                }
            }
            return View(model);
        }

        /// <summary>
        /// Осуществляет выход из приложения
        /// </summary>
        /// <returns></returns>
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction(nameof(Login));
        }

        /// <summary>
        /// Предоставляет клиенту основной интерфейс для работы с приложением, включая работу со списком задач и фильтрами
        /// </summary>
        /// <param name="filters">Фильтры задач</param>
        /// <returns></returns>
        public IActionResult Index(int[] filters)
        {
            if (HttpContext.Session.Keys.Contains("userid"))
            {
                if (filters == null || filters.Length == 0)
                    filters = new[] { 0, 0, 0 };
                filters[0] = HttpContext.Session.GetInt32("userid") ?? -1;
                IQueryable<ToDoItem> tasks = _repository.Tasks;
                Filter filter = new Filter(filters);
                IndexViewModel model = new IndexViewModel
                {
                    Tasks = filter.FilterTasks(tasks),
                    Priorities = _repository.Priorities,
                    Filter = filter
                };
                return View(model);
            }
            return RedirectToAction(nameof(Logout));
        }

        /// <summary>
        /// Принимает от клиента фильтры задач и передает их дейстию Index
        /// </summary>
        /// <param name="filters">Фильтры задач</param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Filter(int[] filters)
        {
            HttpContext.Session.SetInt32("priority_level", filters[1]);
            HttpContext.Session.SetInt32("complete_status", filters[2]);
            return RedirectToAction(nameof(Index), new { filters = filters });
        }

        /// <summary>
        /// Очищает фильтры задач
        /// </summary>
        /// <returns></returns>
        public IActionResult CleanFilters()
        {
            HttpContext.Session.SetInt32("priority_level", 0);
            HttpContext.Session.SetInt32("complete_status", 0);
            return RedirectToAction(nameof(Index), new { filters = new int[] { -1, 0, 0 } });
        }

        /// <summary>
        /// Предоставляет клиенту интерфейс для создания новой задачи пользователя
        /// </summary>
        /// <param name="userid">Id пользователя</param>
        /// <returns></returns>
        public IActionResult AddTask(int userid)
        {
            AddTaskViewModel model = new AddTaskViewModel
            {
                ToDoItem = new ToDoItem(),
                UserId = userid,
                Users = _repository.Users,
                Priorities = _repository.Priorities
            };
            return View(model);
        }

        /// <summary>
        /// Создает новую задачу по данным, переданным от клиента
        /// </summary>
        /// <param name="model">Модель представления для создания новой задачи</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> AddTask(AddTaskViewModel model)
        {
            if (ModelState.IsValid)
            {
                model.ToDoItem.User = await _repository.GetUser(model.UserId);
                model.ToDoItem.Priority = await _repository.GetPriority(model.PriorityLevel);
                if (model.ToDoItem.User == null || model.ToDoItem.Priority == null)
                {
                    return NotFound();
                }
                await _repository.CreateTaskAsync(model.ToDoItem);
                return RedirectToAction(nameof(Index), new { filters = GetFiltersFromSession()});
            }
            model.Users = _repository.Users;
            model.Priorities = _repository.Priorities;
            return View(model);
        }

        /// <summary>
        /// Завершает выбранную задачу
        /// </summary>
        /// <param name="taskid">Id завершаемой задачи</param>
        /// <returns></returns>
        public async Task<IActionResult> CompleteTask(int taskid)
        {
            ToDoItem? task = await _repository.GetTask(taskid);
            if (task == null)
            {
                return NotFound();
            }
            task.IsCompleted = true;
            await _repository.SaveTaskAsync(task);
            return RedirectToAction(nameof(Index), new { filters = GetFiltersFromSession() });
        }

        /// <summary>
        /// Предоставляет клиенту интерфейс для создания нового пользователя
        /// </summary>
        /// <returns></returns>
        public IActionResult AddUser()
        {
            return View(new User());
        }

        /// <summary>
        /// Создает нового пользователя по данным, переданным от клиента
        /// </summary>
        /// <param name="user">Объект нового пользователя, формируемый из тела POST-запроса</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> AddUser(User user)
        {
            if (ModelState.IsValid)
            {
                await _repository.CreateUserAsync(user);
                return RedirectToAction(nameof(Index), new { filters = GetFiltersFromSession()});
            }
            return View(user);
        }

        /// <summary>
        /// Предоставляет клиенту интерфейс для делегирования выбранной задачи другому пользователю
        /// </summary>
        /// <param name="taskid">Id делегируемой задачи</param>
        /// <returns></returns>
        public IActionResult ChangeUser(int taskid)
        {
            ChangeUserViewModel model = new ChangeUserViewModel
            {
                TaskId = taskid,
                Users = _repository.Users
            };
            return View(model);
        }

        /// <summary>
        /// Делегирует задачу выбранному пользователю
        /// </summary>
        /// <param name="model">Модель представления для делегирования задачи другому пользователю</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> ChangeUser(ChangeUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                ToDoItem? task = await _repository.GetTask(model.TaskId);
                if (task == null)
                {
                    return NotFound();
                }
                task.User = await _repository.GetUser(model.UserId);
                if (task.User == null)
                {
                    return NotFound();
                }
                await _repository.SaveTaskAsync(task);
                return RedirectToAction(nameof(Index), new { filters = GetFiltersFromSession() });
            }
            model.Users = _repository.Users;
            return View(model);
        }

        /// <summary>
        /// Удаляет завершенные задачи пользователя
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> CleanCompletedTasks()
        {
            if (HttpContext.Session.Keys.Contains("userid"))
            {
                int userid = HttpContext.Session.GetInt32("userid") ?? -1;
                var tasks = await _repository.GetCompleteTasks(userid);
                await _repository.DeleteTaskRangeAsync(tasks);
                return RedirectToAction(nameof(Index), new { filters = GetFiltersFromSession() });
            }
            return RedirectToAction(nameof(Logout));
        }

        /// <summary>
        /// Описывает делали необработанного исключения (в режиме разработки)
        /// </summary>
        /// <returns></returns>
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        /// <summary>
        /// Формирует фильтры задач, используя информацию из текущей сессии
        /// </summary>
        /// <returns></returns>
        private int[] GetFiltersFromSession()
        {
            return new int[]
            {
                -1,
                HttpContext.Session.GetInt32("priority_level") ?? 0,
                HttpContext.Session.GetInt32("complete_status") ?? 0
            };
        }
    }
}
