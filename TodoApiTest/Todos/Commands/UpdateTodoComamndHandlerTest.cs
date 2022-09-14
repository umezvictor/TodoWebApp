using Application.DTOs;
using Application.Features.Todos.Commands.UpdateTodo;
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
using static Application.Features.Todos.Commands.UpdateTodo.UpdateTodoCommand;

namespace TodoApiTest.Todos.Commands
{
    public class UpdateTodoComamndHandlerTest
    {
        private readonly IMapper _mapper;
        private readonly Mock<ITodoRepositoryAsync> _mockRepo;
        public UpdateTodoComamndHandlerTest()
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
        public async Task UpdateTodoTest()
        {
             var handler = new UpdateTodoCommandHandler(_mockRepo.Object, _mapper);
             //await handler.Handle(new UpdateTodoCommand(), CancellationToken.None);
             var result = await handler.Handle(new UpdateTodoCommand(), CancellationToken.None);
             result.ShouldBeOfType<Response<TodoDto>>();

        }
    }
}
