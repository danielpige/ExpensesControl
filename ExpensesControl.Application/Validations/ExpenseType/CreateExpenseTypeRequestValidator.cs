using ExpensesControl.Application.Dtos.ExpenseType;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpensesControl.Application.Validations.ExpenseType
{
    public class CreateExpenseTypeRequestValidator : AbstractValidator<CreateExpenseTypeRequestDto>
    {
        public CreateExpenseTypeRequestValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .MaximumLength(100);
        }
    }
}
