using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.Client.ActionFilters;
using Web.Client.DTOs;
using Web.Client.Services;

namespace Web.Client.ApiController
{
    [Route("api/[controller]")]
    [ApiController]
    [ServiceFilter(typeof(AuthenticationFilter))]
    public class TodoController : ControllerBase
    {
        private readonly ITodoService _todoService;
        private readonly ILogger<TodoController> _logger;

        public TodoController(ITodoService todoService, ILogger<TodoController> logger)
        {
            _todoService = todoService;
            _logger = logger;
        }
        // GET: api/<TodoController>
        [HttpGet]
        public IActionResult Get()
        {
            _logger.LogInformation("Hello");
            var response = _todoService.GetTodos();
            if(response.Success)            
                return Ok(response);           
            return NotFound(response);
        }

        

        // POST api/<TodoController>
        [HttpPost]
        public IActionResult Post([FromBody] Todo todo)
        {
            var userId = HttpContext.Session.GetString("Id");
            todo.UserId = userId!;
            var response = _todoService.AddTodo(todo);
            if(response.Success)
                return Ok(response);
            return BadRequest(response);
        }

        // PUT api/<TodoController>/5
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] Todo todo)
        {
            var userId = HttpContext.Session.GetString("Id");
            todo.UserId = userId!;
            var response = _todoService.UpdateTodo(id, todo);
            if (response.Success)
                return Ok(response);
            return BadRequest(response);
        }

        // DELETE api/<TodoController>/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var response = _todoService.DeleteTodo(id);
            if (response.Success)
                return Ok(response);
            return BadRequest(response);

        }
    }
}
