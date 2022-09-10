using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class TodoDto
    {
        public int Id { get; set; } 
        public string Title { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public bool Completed { get; set; }
    }
}
