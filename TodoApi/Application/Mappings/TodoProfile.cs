using Application.DTOs;
using Application.Features.Todos.Command;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Mappings
{
    public class TodoProfile : Profile
    {
        public TodoProfile()
        {
            CreateMap<TodoCommand, Todo>().ReverseMap();
            CreateMap<TodoDto, Todo>().ReverseMap();
            
        }
    }
}
