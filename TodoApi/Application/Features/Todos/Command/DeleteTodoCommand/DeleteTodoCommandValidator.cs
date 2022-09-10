
using Application.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Todos.Command
{

    public class DeleteTodoCommandValidator : AbstractValidator<DeleteTodoCommand>
    {

        public DeleteTodoCommandValidator()
        {

            RuleFor(p => p.Id)
                .NotEmpty().WithMessage("{PropertyName} is required.")
                .NotNull();
           
           

        }


    }
}
