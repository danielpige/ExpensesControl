using ExpensesControl.Application.Dtos.Deposit;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpensesControl.Application.Validations.Deposit
{
    public class CreateDepositRequestValidator : AbstractValidator<CreateDepositRequestDto>
    {
        public CreateDepositRequestValidator()
        {
            RuleFor(x => x.Date)
                .NotEmpty()
                .LessThanOrEqualTo(DateTime.Today)
                .WithMessage("Deposit date cannot be in the future.");

            RuleFor(x => x.MoneyFundId)
                .GreaterThan(0);

            RuleFor(x => x.Amount)
                .GreaterThan(0)
                .WithMessage("Amount must be greater than zero.");
        }
    }
}
