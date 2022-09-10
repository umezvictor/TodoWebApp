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
            private readonly IMemoryCache _memoryCache;

            public GetTodosListQueryHandler(ITodoRepositoryAsync TodoRepository, IMemoryCache memoryCache, IMapper mapper)
            {
                _todoRepository = TodoRepository;
                _mapper = mapper;
                _memoryCache = memoryCache;
            }
            public async Task<Response<IEnumerable<TodoDto>>> Handle(GetTodosListQuery query, CancellationToken cancellationToken)
            {

                var cacheKey = nameof(Todo);

                if (!_memoryCache.TryGetValue(cacheKey, out IReadOnlyList<Todo> todos))
                {
                    todos = await _todoRepository.GetAllAsync();
                    var cacheExpiryOptions = new MemoryCacheEntryOptions
                    {
                        AbsoluteExpiration = DateTime.Now.AddMinutes(5),
                        Priority = CacheItemPriority.High,
                        SlidingExpiration = TimeSpan.FromMinutes(2)
                    };
                    _memoryCache.Set(cacheKey, todos, cacheExpiryOptions);
                }

                var response = _mapper.Map<IEnumerable<TodoDto>>(todos);

                return new Response<IEnumerable<TodoDto>>(response);
            }
        }
    }
}
