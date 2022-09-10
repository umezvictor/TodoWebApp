using Application.DTOs;
using Application.Interfaces.Repositories;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Todos.Command
{


    public class TodoCommand : IRequest<Response<TodoDto>>
    {
        public string Title { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
       
    }


    public class TodoCommandHandler : IRequestHandler<TodoCommand, Response<TodoDto>>
    {
        private readonly ITodoRepositoryAsync _todoRepository;
        private readonly IMapper _mapper;

        public TodoCommandHandler(ITodoRepositoryAsync TodoRepositoryAsync, IMapper mapper)
        {
            _todoRepository = TodoRepositoryAsync;
            _mapper = mapper;
        }

        public async Task<Response<TodoDto>> Handle(TodoCommand request, CancellationToken cancellationToken)
        {
            var todo = _mapper.Map<Todo>(request);
            todo.Completed = false;
            await _todoRepository.AddAsync(todo);
            var response = _mapper.Map<TodoDto>(todo);
            //return created todo
            return new Response<TodoDto>(response);
        }

    }
}
