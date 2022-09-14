using Application.DTOs;
using Application.Features.Todos.Command;
using Application.Interfaces.Repositories;
using Application.Mappings;
using Application.Wrappers;
using AutoMapper;
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

namespace TodoApiTest.Todos.Commands
{
    public class CreateTodoCommandHandlerTest
    {
        private readonly IMapper _mapper;
        private readonly Mock<ITodoRepositoryAsync> _mockRepo;
        public CreateTodoCommandHandlerTest()
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
        public async Task AddTodoTest()
        {
            var handler = new TodoCommandHandler(_mockRepo.Object, _mapper);
            var result = await handler.Handle(new TodoCommand(), CancellationToken.None);
            result.ShouldBeOfType<Response<TodoDto>>();

        }
    }
}
