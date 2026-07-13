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
    public class CustomerValidator : AbstractValidator<CustomerDTO>
    {
        IRepository<NWC_Customer> _customerRepository;
        ILoggedInUserService _loggedInUser;

        public CustomerValidator(ValidationMode mode, ILoggedInUserService loggedInUser, IRepository<NWC_Customer> customerRepository)
        {
            this._customerRepository = customerRepository;
            this._loggedInUser = loggedInUser;
            switch (mode)
            {
                case ValidationMode.Create:
                    Initialize();
                    break;
                case ValidationMode.CreateLogic2:
                    CreateFromNWC();
                    break;
            }
        }

        private void Initialize()
        {
            RuleFor(a => a).NotEmpty().WithMessage(ValidationMessagesKeys.ModelEmpety);

            RuleFor(a => a.IntegrationId).NotEmpty().WithMessage(ValidationMessagesKeys.CustomerIntegartionIdRequired)
                .MaximumLength(50).WithMessage(ValidationMessagesKeys.ExceedMaxCharCustomerIntegartionId);
                //.Must(IsIntegrationIdUnique).WithMessage(ValidationMessagesKeys.CustomerIntegrationIdNotUnique);

            //RuleFor(a => a.Code).NotEmpty().WithMessage(ValidationMessagesKeys.CustomerCodeRequired)
            //    .MaximumLength(50).WithMessage(ValidationMessagesKeys.ExceedMaxCharCustomerCode)
            //    .Must(IsCodeUnique).WithMessage(ValidationMessagesKeys.CustomerCodeNotUnique);

            RuleFor(a => a.FullName).NotEmpty().WithMessage(ValidationMessagesKeys.FullNameRequired)
               .MaximumLength(500).WithMessage(ValidationMessagesKeys.ExceedMaxCharFullName);

            RuleFor(a => a.IDTypeID).NotEmpty().WithMessage(ValidationMessagesKeys.IDTypeIDRequired);

            RuleFor(a => a.IDNumber).NotEmpty().WithMessage(ValidationMessagesKeys.IDNumberRequired)
               .MaximumLength(10).WithMessage(ValidationMessagesKeys.ExceedMaxCharIDNumber);

            RuleFor(a => a.Email).NotEmpty().WithMessage(ValidationMessagesKeys.EmailRequired)
               .EmailAddress().WithMessage(ValidationMessagesKeys.InvalidEmail);
               //.Must(IsEmailUnique).WithMessage(ValidationMessagesKeys.EmailNotUnique);

            //RuleFor(a => a).Must(IsIDNumberTypeUnique).WithMessage(ValidationMessagesKeys.IDNumberTypeNotUnique);
        }

        private void CreateFromNWC()
        {
            RuleFor(a => a).NotEmpty().WithMessage(ValidationMessagesKeys.ModelEmpety);

            RuleFor(a => a.IntegrationId).NotNull().WithMessage(ValidationMessagesKeys.CustomerIntegartionIdRequired);
            //    .MaximumLength(50).WithMessage(ValidationMessagesKeys.ExceedMaxCharCustomerIntegartionId);
            //.Must(IsIntegrationIdUnique).WithMessage(ValidationMessagesKeys.CustomerIntegrationIdNotUnique);

            //RuleFor(a => a.Code).NotEmpty().WithMessage(ValidationMessagesKeys.CustomerCodeRequired)
            //    .MaximumLength(50).WithMessage(ValidationMessagesKeys.ExceedMaxCharCustomerCode)
            //    .Must(IsCodeUnique).WithMessage(ValidationMessagesKeys.CustomerCodeNotUnique);

            RuleFor(a => a.FullName).NotEmpty().WithMessage(ValidationMessagesKeys.FullNameRequired)
               .MaximumLength(500).WithMessage(ValidationMessagesKeys.ExceedMaxCharFullName);

            RuleFor(a => a.IDTypeID).NotEmpty().WithMessage(ValidationMessagesKeys.IDTypeIDRequired);

            RuleFor(a => a.IDNumber).NotEmpty().WithMessage(ValidationMessagesKeys.IDNumberRequired)
               .MaximumLength(10).WithMessage(ValidationMessagesKeys.ExceedMaxCharIDNumber);

            RuleFor(a => a.Email).NotNull().WithMessage(ValidationMessagesKeys.EmailRequired);
               //.EmailAddress().WithMessage(ValidationMessagesKeys.InvalidEmail);
            //.Must(IsEmailUnique).WithMessage(ValidationMessagesKeys.EmailNotUnique);

            RuleFor(a => a).Must(IsIDNumberTypeUnique).WithMessage(ValidationMessagesKeys.IDNumberTypeNotUnique);
            RuleFor(a => a).Must(IsValidLocationList).WithMessage(ValidationMessagesKeys.RedundantAddress);

            RuleFor(a => a.customerAccounts).NotNull().Must(a => a.Any()).WithMessage(ValidationMessagesKeys.AtLeastOneLocationIsRequired);

        }

        private bool IsIntegrationIdUnique(CustomerDTO model, string IntegrationId)
        {
            if (model == null || string.IsNullOrEmpty(IntegrationId)) return true;

            var search = IntegrationId.Trim();
            return !_customerRepository.GetQuery().Any(s => s.ID != model.ID && s.IntegrationId == search && s.IsDeleted == false
                                                                && s.SubID == _loggedInUser.LoggedInUser.SubscriberId);
        }

        private bool IsCodeUnique(CustomerDTO model, string Code)
        {
            if (model == null || string.IsNullOrEmpty(Code)) return true;

            var searchCode = Code.Trim();
            return !_customerRepository.GetQuery().Any(s => s.ID != model.ID && s.Code == searchCode && s.IsDeleted == false
                                                                && s.SubID == _loggedInUser.LoggedInUser.SubscriberId);
        }

        private bool IsEmailUnique(CustomerDTO model, string email)
        {
            if (model == null || string.IsNullOrEmpty(email)) return true;

            var search = email.Trim();
            return !_customerRepository.GetQuery().Any(s => s.ID != model.ID && s.Email == search && s.IsDeleted == false
                                                                && s.SubID == _loggedInUser.LoggedInUser.SubscriberId);
        }

        private bool IsIDNumberTypeUnique(CustomerDTO model)
        {
            if (model == null || string.IsNullOrEmpty(model.IDNumber) || model.IDTypeID <= 0) return true;

            var searchIDNumber = model.IDNumber.Trim();
            return !_customerRepository.GetQuery().Any(s => s.ID != model.ID  && s.IsDeleted == false
                                                                && s.SubID == _loggedInUser.LoggedInUser.SubscriberId
                                                                && (s.IDNumber == searchIDNumber && s.IDTypeID == model.IDTypeID) );
        }


        private bool IsValidLocationList(CustomerDTO model)
        {
            if (model == null || !model.customerAccounts.Any()) return true;

            foreach (var item in model.customerAccounts)
            {
                var noOfExist = model.customerAccounts.Count(a =>
                                                   a.CL_Address.ToLower() == item.CL_Address.ToLower()
                                                && a.CL_ZoneID == item.CL_ZoneID
                                                && a.CL_ClassID == item.CL_ClassID
                                                && a.ServiceTypeId == item.ServiceTypeId
                                                && a.AccountId_Integration.ToLower() == item.AccountId_Integration.ToLower()

                                                //&& a.CL_Code.ToLower() == item.CL_Code.ToLower()
                                                //&& a.CL_CategoryID == item.CL_CategoryID
                                                //&& a.CL_PriorityID == item.CL_PriorityID
                                                //&& a.CL_StatusID == item.CL_StatusID
                                                );
                if (noOfExist > 1)
                {
                    return false;
                }

            }

            return true;
            
        }

    }
}
