
using Application.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Todos.Command
{

    public class TodoCommandValidator : AbstractValidator<TodoCommand>
    {
       

        public TodoCommandValidator()
        {
            
            RuleFor(p => p.UserId)
                .NotEmpty().WithMessage("{User Id} is required.")
                .NotNull();
            RuleFor(p => p.Title)
                .NotEmpty().WithMessage("{Title} is required.")
                .NotNull();           
        }


    }
}
