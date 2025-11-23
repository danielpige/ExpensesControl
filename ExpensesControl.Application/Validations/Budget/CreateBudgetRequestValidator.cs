using ExpensesControl.Application.Dtos.Budget;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpensesControl.Application.Validations.Budget
{
    public class CreateBudgetRequestValidator : AbstractValidator<CreateBudgetRequestDto>
    {
        public CreateBudgetRequestValidator()
        {
            RuleFor(x => x.ExpenseTypeId)
                .GreaterThan(0);

            RuleFor(x => x.Year)
                .InclusiveBetween(2000, 2100);

            RuleFor(x => x.Month)
                .InclusiveBetween(1, 12);

            RuleFor(x => x.Amount)
                .GreaterThan(0);
        }
    }
}
