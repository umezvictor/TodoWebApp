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
using static Application.Features.Todos.Command.DeleteTodoCommand;

namespace TodoApiTest.Todos.Commands
{
    public class DeleteCommandHandlerTest
    {
        private readonly IMapper _mapper;
        private readonly Mock<ITodoRepositoryAsync> _mockRepo;
        public DeleteCommandHandlerTest()
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
        public async Task DeleteTodoTest()
        {
            var handler = new DeleteTodoCommandHandler(_mockRepo.Object, _mapper);
            var result = await handler.Handle(new DeleteTodoCommand(), CancellationToken.None);
            result.ShouldBeOfType<Response<TodoDto>>();

        }
    }
}
