using Web.Client.DTOs;
using Web.Client.Responses;

namespace Web.Client.Services
{
    public interface ITodoService
    {
        ApiResponse<Todo> AddTodo(AddTodoDto todo);
        ApiResponse<Todo> DeleteTodo(int Id);
        ApiResponse<List<Todo>> GetTodos();
        ApiResponse<Todo> UpdateTodo(int id, EditTodoDto todo);
    }
}