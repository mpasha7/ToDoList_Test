using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using ToDoList_Test.Models;
using ToDoList_Test.Models.ViewModels;

namespace ToDoList_Test.Controllers
{
    /// <summary>
    /// ����������, ������������ ������-������ ���������� ToDoList
    /// </summary>
    public class HomeController : Controller
    {
        /// <summary>
        /// ��������� ������
        /// </summary>
        private readonly IRepository _repository;

        /// <summary>
        /// �������������� ����� ������ ������ HomeController, ��������� ������ ��������� ������
        /// </summary>
        /// <param name="repo">������ ��������� ������</param>
        public HomeController(IRepository repo)
        {
            _repository = repo;
        }

        /// <summary>
        /// ������������� ������� ��������� ��� ����� � ����������
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
        /// ������������ ���� � ���������� �� ����� ������������
        /// </summary>
        /// <param name="model">������ ������������� ��� ����� � ����������</param>
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
                    ModelState.AddModelError("UserName", "������������ � ����� ������ �� ����������");
                }
            }
            return View(model);
        }

        /// <summary>
        /// ������������ ����� �� ����������
        /// </summary>
        /// <returns></returns>
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction(nameof(Login));
        }

        /// <summary>
        /// ������������� ������� �������� ��������� ��� ������ � �����������, ������� ������ �� ������� ����� � ���������
        /// </summary>
        /// <param name="filters">������� �����</param>
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
        /// ��������� �� ������� ������� ����� � �������� �� ������� Index
        /// </summary>
        /// <param name="filters">������� �����</param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Filter(int[] filters)
        {
            HttpContext.Session.SetInt32("priority_level", filters[1]);
            HttpContext.Session.SetInt32("complete_status", filters[2]);
            return RedirectToAction(nameof(Index), new { filters = filters });
        }

        /// <summary>
        /// ������� ������� �����
        /// </summary>
        /// <returns></returns>
        public IActionResult CleanFilters()
        {
            HttpContext.Session.SetInt32("priority_level", 0);
            HttpContext.Session.SetInt32("complete_status", 0);
            return RedirectToAction(nameof(Index), new { filters = new int[] { -1, 0, 0 } });
        }

        /// <summary>
        /// ������������� ������� ��������� ��� �������� ����� ������ ������������
        /// </summary>
        /// <param name="userid">Id ������������</param>
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
        /// ������� ����� ������ �� ������, ���������� �� �������
        /// </summary>
        /// <param name="model">������ ������������� ��� �������� ����� ������</param>
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
        /// ��������� ��������� ������
        /// </summary>
        /// <param name="taskid">Id ����������� ������</param>
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
        /// ������������� ������� ��������� ��� �������� ������ ������������
        /// </summary>
        /// <returns></returns>
        public IActionResult AddUser()
        {
            return View(new User());
        }

        /// <summary>
        /// ������� ������ ������������ �� ������, ���������� �� �������
        /// </summary>
        /// <param name="user">������ ������ ������������, ����������� �� ���� POST-�������</param>
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
        /// ������������� ������� ��������� ��� ������������� ��������� ������ ������� ������������
        /// </summary>
        /// <param name="taskid">Id ������������ ������</param>
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
        /// ���������� ������ ���������� ������������
        /// </summary>
        /// <param name="model">������ ������������� ��� ������������� ������ ������� ������������</param>
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
        /// ������� ����������� ������ ������������
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
        /// ��������� ������ ��������������� ���������� (� ������ ����������)
        /// </summary>
        /// <returns></returns>
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        /// <summary>
        /// ��������� ������� �����, ��������� ���������� �� ������� ������
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
