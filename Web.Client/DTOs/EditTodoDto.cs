﻿namespace Web.Client.DTOs
{
   

    public class EditTodoDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public bool Completed { get; set; }
        

    }
}
