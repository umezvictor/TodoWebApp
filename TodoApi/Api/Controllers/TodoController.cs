using Application.DTOs;
using Application.Features.Todos.Command;
using Application.Features.Todos.Commands.UpdateTodo;
using Application.Features.Todos.Query;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Authorize("ClientIdPolicy")]
    [ApiController]
    public class TodoController : ControllerBase
    {
        private readonly IMediator _mediator;

        public TodoController(IMediator mediator)
        {
            _mediator = mediator;
        }
        // GET: api/<TodoController>
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(Response<IEnumerable<TodoDto>>))]
        public async Task<IActionResult> Get()
        {
            return Ok(await _mediator.Send(new GetTodosListQuery()));
        }

        // GET api/<TodoController>/5
        [HttpGet("{id}")]
        [ProducesResponseType(200, Type = typeof(Response<TodoDto>))]
        public async Task<IActionResult>  Get(int id)
        {
            return Ok(await _mediator.Send(new GetTodoQuery { Id = id}));
        }

        // POST api/<TodoController>
        [HttpPost]
        [ProducesResponseType(200, Type = typeof(Response<TodoDto>))]
        public async Task<IActionResult> Post([FromBody] TodoCommand request)
        {
            return Ok(await _mediator.Send(request));
        }

        // PUT api/<TodoController>/5
        
        [HttpPut("{id}")]
        [ProducesResponseType(200, Type = typeof(Response<TodoDto>))]
        public async Task<IActionResult> Put(int id, [FromBody] UpdateTodoCommand request)
        {           
            request.Id = id;
            return Ok(await _mediator.Send(request));
        }

        // DELETE api/<TodoController>/5
        [HttpDelete("{id}")]
        [ProducesResponseType(200, Type = typeof(Response<TodoDto>))]
        public async Task<IActionResult> Delete(int id)
        {
            return Ok(await _mediator.Send(new DeleteTodoCommand { Id = id }));
        }
    }
}
