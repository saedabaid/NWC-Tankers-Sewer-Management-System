using FluentValidation;
using NWC.DTO.Common;
using NWC.DTO.Enums;
using NWC.DTO.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NWC.BLL.Validators
{
    public class ContractPriceValidator : AbstractValidator<List<ContractPriceDTO>>
    {
        public ContractPriceValidator(ValidationMode mode)
        {

            switch (mode)
            {

                case ValidationMode.Update:

                    RuleFor(a =>a.Any(s=>s.PriceCharge < 0)).Equal(false).WithMessage(ValidationMessagesKeys.priceMustBePositiveNum);
                    break;



            }
        }
    }
}
