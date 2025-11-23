using ExpensesControl.Application.Dtos.Expense;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpensesControl.Application.Validations.Expense
{
    public class CreateExpenseRequestValidator : AbstractValidator<CreateExpenseRequestDto>
    {
        public CreateExpenseRequestValidator()
        {
            RuleFor(x => x.Date)
                .LessThanOrEqualTo(DateTime.Today);

            RuleFor(x => x.MoneyFundId)
                .GreaterThan(0);

            RuleFor(x => x.Details)
                .NotEmpty().WithMessage("At least one detail is required.");

            RuleForEach(x => x.Details).ChildRules(detail =>
            {
                detail.RuleFor(d => d.ExpenseTypeId)
                    .GreaterThan(0);
                detail.RuleFor(d => d.Amount)
                    .GreaterThan(0);
            });
        }
    }
}
