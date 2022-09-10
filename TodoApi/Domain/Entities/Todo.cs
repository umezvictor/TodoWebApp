using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Todo : BaseEntity
    {
        
        public string Title { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public bool Completed { get; set; } 
      
       
    }
}
