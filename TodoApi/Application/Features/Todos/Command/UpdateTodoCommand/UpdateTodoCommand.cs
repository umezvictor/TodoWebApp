using Application.DTOs;
using Application.Exceptions;
using Application.Interfaces.Repositories;
using Application.Wrappers;
using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Todos.Commands.UpdateTodo
{
   
    public class UpdateTodoCommand : IRequest<Response<TodoDto>>
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public bool Completed { get; set; } 

        public class UpdateTodoCommandHandler : IRequestHandler<UpdateTodoCommand, Response<TodoDto>>
        {
            private readonly ITodoRepositoryAsync _todoRepository;
            private readonly IMapper _mapper;

            public UpdateTodoCommandHandler(ITodoRepositoryAsync todoRepository, IMapper mapper)
            {
                _todoRepository = todoRepository;
                _mapper = mapper;
            }
            public async Task<Response<TodoDto>> Handle(UpdateTodoCommand command, CancellationToken cancellationToken)
            {
                var todo = await _todoRepository.GetByIdAsync(command.Id);

                if (todo == null)
                {
                    throw new ApiException($"Todo item not found.");
                }
                else
                {
                    todo.Completed = command.Completed;
                    todo.Title = command.Title;
                    todo.UserId = command.UserId;
                                     
                    await _todoRepository.UpdateAsync(todo);
                    var response = _mapper.Map<TodoDto>(todo);
                    return new Response<TodoDto>(response);
                }


            }
        }
    }
}
