using ExpensesControl.Application.Dtos.Budget;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpensesControl.Application.Validations.Budget
{
    public class UpdateBudgetRequestValidator : AbstractValidator<UpdateBudgetRequestDto>
    {
        public UpdateBudgetRequestValidator()
        {
            RuleFor(x => x.Amount)
                .GreaterThan(0);
        }
    }
}
