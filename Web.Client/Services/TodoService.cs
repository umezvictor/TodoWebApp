using Newtonsoft.Json;
using Web.Client.DTOs;
using Web.Client.Responses;
using Web.Client.Utils;

namespace Web.Client.Services
{
    public class TodoService : ITodoService
    {
        private readonly IConfiguration _configuration;
        private readonly ITokenService _tokenService;

        public TodoService(IConfiguration configuration, ITokenService tokenService)
        {
            _configuration = configuration;
            _tokenService = tokenService;
        }


        public ApiResponse<List<Todo>> GetTodos()
        {
            ApiResponse<List<Todo>> apiResponse = new ApiResponse<List<Todo>>();
            try
            {
                var todoUrl = _configuration.GetSection("TodoUrl").Value;
                var tokenResponse = _tokenService.GetApiToken();
                if (tokenResponse != null)
                {
                    var token = tokenResponse.access_token;
                    var response = ApiClient.GetAsync(todoUrl, token);
                    var res = JsonConvert.DeserializeObject<GetTodosResponse>(response);
                    if (res!.Data != null)
                    {
                        apiResponse.Payload = res.Data;
                        apiResponse.Success = true;
                        return apiResponse;
                    }

                    apiResponse.Message = "No record found";
                    apiResponse.Success = false;
                    return apiResponse;
                }
                apiResponse.Message = "An error occured";
                apiResponse.Success = false;
                return apiResponse;
            }
            catch (Exception)
            {
                //log here
                apiResponse.Message = "An internal server error occured";
                apiResponse.Success = false;
                return apiResponse;
            }

        }


        public ApiResponse<Todo> AddTodo(AddTodoDto todo)
        {
            ApiResponse<Todo> apiResponse = new ApiResponse<Todo>();

            try
            {
                var todoUrl = _configuration.GetSection("TodoUrl").Value;
                var tokenResponse = _tokenService.GetApiToken();
                if (tokenResponse != null)
                {
                    var data = JsonConvert.SerializeObject(todo);
                    var token = tokenResponse.access_token;
                    var response = ApiClient.PostAsync(todoUrl, data, token);
                    var res = JsonConvert.DeserializeObject<TodoResponse>(response);
                    if (res!.Data != null)
                    {

                        apiResponse.Message = "Todo added successfully";
                        apiResponse.Success = true;
                        return apiResponse;
                    }

                    apiResponse.Message = "Todo was not saved successfully";
                    apiResponse.Success = false;
                    return apiResponse;
                }
                apiResponse.Message = "An error occured";
                apiResponse.Success = false;
                return apiResponse;
            }
            catch (Exception)
            {

                apiResponse.Message = "An internal server error occured";
                apiResponse.Success = false;
                return apiResponse;
            }

        }


        public ApiResponse<Todo> UpdateTodo(int id, Todo todo)
        {
            ApiResponse<Todo> apiResponse = new ApiResponse<Todo>();
            try
            {
                var todoUrl = _configuration.GetSection("TodoUrl").Value;
                var updateUrl = $"{todoUrl}/{id}";
                var tokenResponse = _tokenService.GetApiToken();
                if (tokenResponse != null)
                {
                    var data = JsonConvert.SerializeObject(todo);
                    var token = tokenResponse.access_token;
                    var response = ApiClient.PutAsync(updateUrl, data, token);
                    var res = JsonConvert.DeserializeObject<TodoResponse>(response);
                    if (res!.Data != null)
                    {
                        apiResponse.Message = "Todo updated successfully";
                        apiResponse.Success = true;
                        return apiResponse;
                    }

                    apiResponse.Message = "Todo was not updated successfully";
                    apiResponse.Success = false;
                    return apiResponse;
                }
                apiResponse.Message = "An error occured";
                apiResponse.Success = false;
                return apiResponse;
            }
            catch (Exception)
            {

                apiResponse.Message = "An internal server error occured";
                apiResponse.Success = false;
                return apiResponse;
            }

        }


        public ApiResponse<Todo> DeleteTodo(int Id)
        {
            ApiResponse<Todo> apiResponse = new ApiResponse<Todo>();
            try
            {
                var todoUrl = _configuration.GetSection("TodoUrl").Value;
                var deleteUrl = $"{todoUrl}/{Id}";
                var tokenResponse = _tokenService.GetApiToken();
                if (tokenResponse != null)
                {
                    var data = JsonConvert.SerializeObject(deleteUrl);
                    var token = tokenResponse.access_token;
                    var response = ApiClient.DeleteAsync(deleteUrl, token);
                    var res = JsonConvert.DeserializeObject<TodoResponse>(response);
                    if (res!.Data != null)
                    {
                        apiResponse.Message = "Todo deleted successfully";
                        apiResponse.Success = true;
                        return apiResponse;
                    }

                    apiResponse.Message = "Todo was not deleted successfully";
                    apiResponse.Success = false;
                    return apiResponse;
                }
                apiResponse.Message = "An error occured";
                apiResponse.Success = false;
                return apiResponse;
            }
            catch (Exception)
            {

                apiResponse.Message = "An internal server error occured";
                apiResponse.Success = false;
                return apiResponse;
            }

        }
    }
}
