using ExpensesControl.Application.Dtos.MoneyFund;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpensesControl.Application.Validations.MoneyFund
{
    public class CreateMoneyFundRequestValidator : AbstractValidator<CreateMoneyFundRequestDto>
    {
        public CreateMoneyFundRequestValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .MaximumLength(100);

            RuleFor(x => x.InitialBalance)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Initial balance cannot be negative.");
        }
    }
}
