using ExpensesControl.Application.Dtos.Movements;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpensesControl.Application.Validations.Movement
{
    public class MovementQueryRequestValidator : AbstractValidator<MovementQueryRequestDto>
    {
        public MovementQueryRequestValidator()
        {
            RuleFor(x => x.From)
                .NotEmpty();

            RuleFor(x => x.To)
                .NotEmpty();

            RuleFor(x => x)
                .Must(x => x.From <= x.To)
                .WithMessage("'From' date must be less than or equal to 'To' date.");
        }
    }
}
