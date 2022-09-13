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

namespace Application.Features.Todos.Command
{
   
    public class DeleteTodoCommand : IRequest<Response<TodoDto>>
    {
        public int Id { get; set; }
      
        public class DeleteTodoCommandHandler : IRequestHandler<DeleteTodoCommand, Response<TodoDto>>
        {
            private readonly ITodoRepositoryAsync _todoRepository;
            private readonly IMapper _mapper;

            public DeleteTodoCommandHandler(ITodoRepositoryAsync todoRepository, IMapper mapper)
            {
                _todoRepository = todoRepository;
                _mapper = mapper;
            }
            public async Task<Response<TodoDto>> Handle(DeleteTodoCommand command, CancellationToken cancellationToken)
            {
                var todo = await _todoRepository.GetByIdAsync(command.Id);
                if (todo == null) throw new ApiException($"Todo item not found.");                             
                    todo.IsDeleted = true;
                    await _todoRepository.DeleteAsync(todo);
                    var response = _mapper.Map<TodoDto>(todo);
                    return new Response<TodoDto>(response);
            }
        }
    }
}
