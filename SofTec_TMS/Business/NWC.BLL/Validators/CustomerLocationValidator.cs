using FluentValidation;
using Infrastructure;
using NWC.BLL.Interfaces;
using NWC.DAL.NWCEntities;
using NWC.DTO.Enums;
using NWC.DTO.Models;
using NWC.DTO.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NWC.BLL.Validators
{
    public class CustomerLocationValidator : AbstractValidator<CustomerLocationDTO>
    {
        IRepository<NWC_CustomerLocation> _customerLocationRepository;
        ILoggedInUserService _loggedInUser;

        public CustomerLocationValidator(ValidationMode mode, ILoggedInUserService loggedInUser, IRepository<NWC_CustomerLocation> customerLocationRepository)
        {
            this._customerLocationRepository = customerLocationRepository;
            this._loggedInUser = loggedInUser;
            switch (mode)
            {
                case ValidationMode.Create:
                    Initialize();
                    break;
            }
        }

        private void Initialize()
        {
            RuleFor(a => a).NotEmpty().WithMessage(ValidationMessagesKeys.ModelEmpety);

            RuleFor(a => a.IntegrationId).NotEmpty().WithMessage(ValidationMessagesKeys.CustomerLocationIntegartionIdRequired)
                .MaximumLength(50).WithMessage(ValidationMessagesKeys.ExceedMaxCharCustomerLocationIntegartionId);
                //.Must(IsIntegrationIdUnique).WithMessage(ValidationMessagesKeys.CustomerIntegrationLocationIdNotUnique);

            //RuleFor(a => a.Code).NotEmpty().WithMessage(ValidationMessagesKeys.CustomerLocationCodeRequired)
            //    .MaximumLength(50).WithMessage(ValidationMessagesKeys.ExceedMaxCharCustomerCodeLocation)
            //    .Must(IsCodeUnique).WithMessage(ValidationMessagesKeys.CustomerCodeLocationNotUnique);

            //RuleFor(a => a.CustomerID).NotEmpty().WithMessage(ValidationMessagesKeys.CustomerIdRequired);

            RuleFor(a => a.ZoneID).NotEmpty().WithMessage(ValidationMessagesKeys.ZoneIdRequired);

            RuleFor(a => a.ClassID).NotEmpty().WithMessage(ValidationMessagesKeys.ClassIdRequired);

            RuleFor(a => a.PriorityID).NotEmpty().WithMessage(ValidationMessagesKeys.PriorityIdRequired);

            RuleFor(a => a.CategoryID).NotEmpty().WithMessage(ValidationMessagesKeys.ChooseCategoryIdRequired);

            RuleFor(a => a.StatusID).NotEmpty().WithMessage(ValidationMessagesKeys.StatusIdRequired);

        }

        private bool IsIntegrationIdUnique(CustomerLocationDTO model, string IntegrationId)
        {
            if (model == null || string.IsNullOrEmpty(IntegrationId)) return true;

            var search = IntegrationId.Trim();
            return !_customerLocationRepository.GetQuery().Any(s => s.ID != model.ID && s.IntegrationId == search && s.IsDeleted == false
                                                                && s.SubID == _loggedInUser.LoggedInUser.SubscriberId);
        }

        private bool IsCodeUnique(CustomerLocationDTO model, string Code)
        {
            if (model == null || string.IsNullOrEmpty(Code)) return true;

            var searchCode = Code.Trim();
            return !_customerLocationRepository.GetQuery().Any(s => s.ID != model.ID && s.Code == searchCode && s.IsDeleted == false
                                                                && s.SubID == _loggedInUser.LoggedInUser.SubscriberId);
        }


    }
}
