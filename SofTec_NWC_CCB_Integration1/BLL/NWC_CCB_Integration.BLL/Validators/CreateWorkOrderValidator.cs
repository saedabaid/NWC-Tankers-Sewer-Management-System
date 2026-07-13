using FluentValidation;
using NWC_CCB_Integration.DAL;
using NWC_CCB_Integration.DTO.Models;
using NWC_CCB_Integration.DTO.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NWC_CCB_Integration.BLL.Validators
{
    public class CreateWorkOrderValidator : AbstractValidator<WorkOrderRequestDTO>
    {
        public NWC_CCBEntities Context { get; private set; }
        public bool ValidateOrderNumberExists { get; set; }

        public CreateWorkOrderValidator(bool _validateOrderNumberExists)
        {
            Context = new NWC_CCBEntities();

            this.ValidateOrderNumberExists = _validateOrderNumberExists;

            Initialize();
        }

        private void Initialize()
        {
            RuleFor(a => a.OrderNumber).NotEmpty().WithMessage(ValidationMessagesKeys.OrderNumberRequired);

            if (this.ValidateOrderNumberExists)
                RuleFor(a => a.OrderNumber).Must(IsOrderNumberNotExist).WithMessage(ValidationMessagesKeys.OrderNumberAlreadyExist);

            RuleFor(a => a.CISDivision).NotEmpty().WithMessage(ValidationMessagesKeys.CisDivisionRequired);
            RuleFor(a => a.CreationTime).NotEmpty().WithMessage(ValidationMessagesKeys.CreationTimeRequired);
            RuleFor(a => a.ScheduledDeliveryTime).NotEmpty().WithMessage(ValidationMessagesKeys.ScheduledDeliveryTimeRequired);
            RuleFor(a => a.ServiceTypeCode).NotEmpty().WithMessage(ValidationMessagesKeys.ServiceTypeCodeRequired);
            RuleFor(a => a.ReceiverName).NotEmpty().WithMessage(ValidationMessagesKeys.ContactNameRequired);
            RuleFor(a => a.ReceiverMobile).NotEmpty().WithMessage(ValidationMessagesKeys.ContactMobileRequired);
            RuleFor(a => a.OrderQuantity).NotEmpty().WithMessage(ValidationMessagesKeys.OrderQuantityRequired);
            RuleFor(a => a.ConfirmationCode).NotEmpty().WithMessage(ValidationMessagesKeys.ConfirmationCodeRequired);
            RuleFor(a => a.AccountID).NotEmpty().WithMessage(ValidationMessagesKeys.AccountIDRequired);
            RuleFor(a => a.ClassID).NotEmpty().WithMessage(ValidationMessagesKeys.ClassIDRequired);
            RuleFor(a => a.CustomerCode).NotEmpty().WithMessage(ValidationMessagesKeys.CustomerCodeRequired);
            RuleFor(a => a.PersonName).NotEmpty().WithMessage(ValidationMessagesKeys.PersonNameRequired);
            RuleFor(a => a.IDTypeID).NotEmpty().WithMessage(ValidationMessagesKeys.IDTypeIDRequired);
            RuleFor(a => a.IDNumber).NotEmpty().WithMessage(ValidationMessagesKeys.IDNumberRequired);
            RuleFor(a => a.PremiseID).NotEmpty().WithMessage(ValidationMessagesKeys.PremiseIDRequired);
            RuleFor(a => a.PremiseCoordinates).NotEmpty().WithMessage(ValidationMessagesKeys.PremiseCoordinatesRequired);
        }

        private bool IsOrderNumberNotExist(WorkOrderRequestDTO model, string orderNumber)
        {
            return !this.Context.NWC_Int_ObjectStatus
                .Any(s => s.OrderNumber == orderNumber);
        }
    }
}
