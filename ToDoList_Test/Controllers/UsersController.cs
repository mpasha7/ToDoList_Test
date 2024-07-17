using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ToDoList_Test.Models;

namespace ToDoList_Test.Controllers
{
    /// <summary>
    /// API-контроллер для действий над объектами пользователей (User)
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        /// <summary>
        /// Контекст базы данных
        /// </summary>
        private readonly ToDoContext _context;

        /// <summary>
        /// Инициализирует новый объект класса UsersController, используя контекст базы данных ToDoContext
        /// </summary>
        /// <param name="context">Объект контекста базы данных</param>
        public UsersController(ToDoContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Возвращает из базы данных список всех объектов пользователей (User)
        /// </summary>
        /// <returns></returns>
        // GET: api/Users
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            return await _context.Users.ToListAsync();
        }

        /// <summary>
        /// Возвращает из базы данных объект пользователя (User) по его Id
        /// </summary>
        /// <param name="id">Id пользователя</param>
        /// <returns></returns>
        // GET: api/Users/5
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }
            return user;
        }

        /// <summary>
        /// Заменяет данные выбранного объекта пользователя (User) на данные представленные в теле запроса
        /// </summary>
        /// <param name="id">Id пользователя</param>
        /// <param name="user">Объект пользователя (User), формируемый из тела запроса</param>
        /// <returns></returns>
        // PUT: api/Users/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> PutUser(int id, User user)
        {
            if (id != user.Id)
            {
                return BadRequest();
            }

            _context.Entry(user).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
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
        /// Размещает в базе данных новый объект пользователя (User)
        /// </summary>
        /// <param name="user">Объект пользователя (User), формируемый из тела запроса</param>
        /// <returns></returns>
        // POST: api/Users
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<User>> PostUser(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
        }

        /// <summary>
        /// Удаляет из базы данных объект пользователя (User) с выбранным Id
        /// </summary>
        /// <param name="id">Id удаляемого пользователя</param>
        /// <returns></returns>
        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
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
    }
}
