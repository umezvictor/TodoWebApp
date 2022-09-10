
using Application.Features.Todos.Commands.UpdateTodo;
using Application.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Todos.Command
{

    public class UpdateTodoCommandValidator : AbstractValidator<UpdateTodoCommand>
    {

        public UpdateTodoCommandValidator()
        {

            RuleFor(p => p.Id)
                .NotEmpty().WithMessage("{Id} is required.")
                .NotNull();

            RuleFor(p => p.UserId)
                .NotEmpty().WithMessage("{User Id} is required.")
                .NotNull();

            RuleFor(p => p.Completed)
               .NotEmpty().WithMessage("{Completed} is required.")
               .NotNull();

        }


    }
}
