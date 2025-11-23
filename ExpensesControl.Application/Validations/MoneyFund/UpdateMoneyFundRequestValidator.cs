using ExpensesControl.Application.Dtos.MoneyFund;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpensesControl.Application.Validations.MoneyFund
{
    public class UpdateMoneyFundRequestValidator : AbstractValidator<UpdateMoneyFundRequestDto>
    {
        public UpdateMoneyFundRequestValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .MaximumLength(100);
        }
    }
}
