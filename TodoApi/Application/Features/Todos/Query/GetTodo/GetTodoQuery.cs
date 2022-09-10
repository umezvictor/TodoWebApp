using Application.DTOs;
using Application.Interfaces.Repositories;
using Application.Wrappers;
using AutoMapper;
using Domain.Entities;
using MediatR;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Todos.Query
{

    public class GetTodoQuery : IRequest<Response<TodoDto>>
    {
        public int Id { get; set; }

        public class GetTodoQueryHandler : IRequestHandler<GetTodoQuery, Response<TodoDto>>
        {
            private readonly ITodoRepositoryAsync _todoRepository;
            private readonly IMapper _mapper;
            private readonly IMemoryCache _memoryCache;

            public GetTodoQueryHandler(ITodoRepositoryAsync TodoRepository, IMemoryCache memoryCache, IMapper mapper)
            {
                _todoRepository = TodoRepository;
                _mapper = mapper;
                _memoryCache = memoryCache;
            }
            public async Task<Response<TodoDto>> Handle(GetTodoQuery query, CancellationToken cancellationToken)
            {

                var todo = await _todoRepository.GetByIdAsync(query.Id);
                var response = _mapper.Map<TodoDto>(todo);

                return new Response<TodoDto>(response);
            }
        }
    }
}
