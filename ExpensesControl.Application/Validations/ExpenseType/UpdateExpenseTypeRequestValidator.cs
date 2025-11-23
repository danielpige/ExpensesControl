using ExpensesControl.Application.Dtos.ExpenseType;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpensesControl.Application.Validations.ExpenseType
{
    public class UpdateExpenseTypeRequestValidator : AbstractValidator<UpdateExpenseTypeRequestDto>
    {
        public UpdateExpenseTypeRequestValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .MaximumLength(100);
        }
    }
}
