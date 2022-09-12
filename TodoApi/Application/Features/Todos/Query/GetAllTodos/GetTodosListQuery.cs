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

    public class GetTodosListQuery : IRequest<Response<IEnumerable<TodoDto>>>
    {
       
        public class GetTodosListQueryHandler : IRequestHandler<GetTodosListQuery, Response<IEnumerable<TodoDto>>>
        {
            private readonly ITodoRepositoryAsync _todoRepository;
            private readonly IMapper _mapper;
          

            public GetTodosListQueryHandler(ITodoRepositoryAsync TodoRepository, IMapper mapper)
            {
                _todoRepository = TodoRepository;
                _mapper = mapper;
                
            }
            public async Task<Response<IEnumerable<TodoDto>>> Handle(GetTodosListQuery query, CancellationToken cancellationToken)
            {

               
                var todos = await _todoRepository.GetAllAsync();
                var response = _mapper.Map<IEnumerable<TodoDto>>(todos);

                return new Response<IEnumerable<TodoDto>>(response);
            }
        }
    }
}
