using Application.DTOs;
using Application.Features.Todos.Query;
using Application.Interfaces.Repositories;
using Application.Mappings;
using Application.Wrappers;
using AutoMapper;
using Domain.Entities;
using Moq;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TodoApiTest.Mocks;
using Xunit;
using static Application.Features.Todos.Query.GetTodosListQuery;

namespace TodoApiTest.Todos.Queries
{
    public class GetTodosListQueryHandlerTest
    {
        private readonly IMapper _mapper;
        private readonly Mock<ITodoRepositoryAsync> _mockRepo;
        public GetTodosListQueryHandlerTest()
        {
            _mockRepo = MockTodoRepository.GetTodosRepository();

            //mapping
            var mapperConfig = new MapperConfiguration(c =>
            {
                c.AddProfile<TodoProfile>();
            });

            _mapper = mapperConfig.CreateMapper();
        }


        [Fact]
        public async Task GetTodosListTest()
        {
            var handler = new GetTodosListQueryHandler(_mockRepo.Object, _mapper);
            var result = await handler.Handle(new GetTodosListQuery(), CancellationToken.None);
            result.ShouldBeOfType<Response<IEnumerable<TodoDto>>>();
            
        }
    }
}
