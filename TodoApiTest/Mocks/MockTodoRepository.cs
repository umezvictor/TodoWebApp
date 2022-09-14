using Application.Interfaces.Repositories;
using Domain.Entities;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TodoApiTest.Mocks
{
    public static class MockTodoRepository
    {
        public static Mock<ITodoRepositoryAsync> GetTodosRepository()
        {
            var todos = new List<Todo>
            {
                new Todo
                {
                    Id = 1,
                    Title = "test todo",
                    UserId = "1",
                    Completed = false,
                },
                new Todo
                {
                    Id = 2,
                    Title = "test todo2",
                    UserId = "2",
                    Completed = true,
                }
            };



            var mockRepo = new Mock<ITodoRepositoryAsync>();  
            //for get all
            mockRepo.Setup(x => x.GetAllAsync()).ReturnsAsync(todos);
            //for add
            mockRepo.Setup(x => x.AddAsync(It.IsAny<Todo>())).ReturnsAsync((Todo todo) =>
            {
                todos.Add(todo);
                return todo;
            });

            mockRepo.Setup(m => m.UpdateAsync(It.IsAny<Todo>()))
                .Callback(() => { return; });

            //update
            //mockRepo.Setup(x => x.UpdateAsync(It.IsAny<Todo>()));
            //delete
            mockRepo.Setup(x => x.DeleteAsync(It.IsAny<Todo>()));

            return mockRepo;
        }
    }
}
