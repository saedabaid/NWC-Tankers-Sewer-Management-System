using FluentValidation;
using Infrastructure;
using NWC.BLL.Interfaces;
using NWC.DAL.NWCEntities;
using NWC.DTO.Enums;
using NWC.DTO.Models;
using NWC.DTO.Resources;
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace NWC.BLL.Validators
{
    public class ContractorValidator: AbstractValidator<ContractorDTO>
    {
        IRepository<NWC_Contractor> _contractorRepository;
        ILoggedInUserService _loggedInUser;

        public ContractorValidator(ValidationMode mode, ILoggedInUserService loggedInUser, IRepository<NWC_Contractor> contractorRepository)
        {
            this._contractorRepository = contractorRepository;
            this._loggedInUser = loggedInUser;
            switch (mode)
            {
                case ValidationMode.Create:
                    Initialize();
                    break;
                case ValidationMode.Update:
                    Initialize();
                    RuleFor(a => a.ID).NotEmpty();
                    break;
            }
        }


        private void Initialize()
        {
            RuleFor(a => a).NotEmpty().WithMessage(ValidationMessagesKeys.EmptyContractor);
            RuleFor(a => a.ContractorFullName).NotEmpty().WithMessage(ValidationMessagesKeys.ContractorNameRequired);

            RuleFor(a => a.Code).NotEmpty().WithMessage(ValidationMessagesKeys.ContractorCodeRequired)
                .MaximumLength(50).WithMessage(ValidationMessagesKeys.ExceedMaxCharContractorCode)
                .Must(IsCodeUnique).WithMessage(ValidationMessagesKeys.ContractorCodeNotUnique);

            RuleFor(a => a.CommercialIDNumber).NotEmpty().WithMessage(ValidationMessagesKeys.CommercialIDNumberRequired)
                .MaximumLength(15).WithMessage(ValidationMessagesKeys.ExceedMaxCharCommercialID)
                .Must(IsCommericalIdUnique).WithMessage(ValidationMessagesKeys.CommericalIdNotUnique);

            RuleFor(a => a.TaxNumber).NotEmpty().WithMessage(ValidationMessagesKeys.TaxNumberRequired)
                .MaximumLength(15).WithMessage(ValidationMessagesKeys.ExceedMaxCharTaxNumber)
                .Must(IsTaxumberUnique).WithMessage(ValidationMessagesKeys.TaxNumberNotUnique);

            RuleFor(a => a.MOI).NotEmpty().WithMessage(ValidationMessagesKeys.MOIRequired)
                .MaximumLength(10).WithMessage(ValidationMessagesKeys.ExceedMaxCharMOI)
                .Must(IsMOIUnique).WithMessage(ValidationMessagesKeys.MOINotUnique);

            RuleFor(a => a.CompanyAddressCityID).NotEmpty().WithMessage(ValidationMessagesKeys.ChooseCity)
                .Must(IsPermittedCity).WithMessage(ValidationMessagesKeys.CityNotPermitted);

            RuleFor(a => a).Must(ValidateFristNameContactPerson).WithMessage(ValidationMessagesKeys.InsertFristName);
            RuleFor(a => a).Must(ValidateLastNameContactPerson).WithMessage(ValidationMessagesKeys.InsertLastName);
            RuleFor(a => ExceedMaxCharFristName(a)).Equal(false).WithMessage(ValidationMessagesKeys.ExceedMaxCharFristName);
            RuleFor(a => ExceedMaxCharLastName(a)).Equal(false).WithMessage(ValidationMessagesKeys.ExceedMaxCharLastName);
            RuleFor(a => ExceedMaxCharSecondName(a)).Equal(false).WithMessage(ValidationMessagesKeys.ExceedMaxCharSecondName);
            RuleFor(a => ExceedMaxCharPosition(a)).Equal(false).WithMessage(ValidationMessagesKeys.ExceedMaxCharPosition);
            RuleFor(a => ExceedMaxCharPersonAddressPostalCode(a)).Equal(false).WithMessage(ValidationMessagesKeys.ExceedMaxCharPersonAddressPostalCode);
            RuleFor(a => InvalidEmail(a)).Equal(false).WithMessage(ValidationMessagesKeys.InvalidEmail);
            RuleFor(a => InvalidMobileNumber(a)).Equal(false).WithMessage(ValidationMessagesKeys.InvalidMobileNumber);
            RuleFor(a => InvalidLandlineNumber(a)).Equal(false).WithMessage(ValidationMessagesKeys.InvalidLandlineNumber);
            RuleFor(a => a.PersonalIDNumber).MaximumLength(10).WithMessage(ValidationMessagesKeys.ExceedMaxCharIDNumber);

        }


        private bool IsCodeUnique(ContractorDTO model, string contractorCode)
        {
            if (model == null || string.IsNullOrEmpty(contractorCode)) return true;

            var searchCode = contractorCode.Trim();
            return !_contractorRepository.GetQuery().Any(s => s.ID != model.ID && s.Code == searchCode && s.IsDeleted == false
                                                                && s.SubID == _loggedInUser.LoggedInUser.SubscriberId );
        }

        private bool IsCommericalIdUnique(ContractorDTO model, string commericalId)
        {
            if (model == null || string.IsNullOrEmpty(commericalId)) return true;

            var searchCommericalId = commericalId.Trim();
            return !_contractorRepository.GetQuery()
                .Any(s => s.ID != model.ID && s.CommercialIDNumber == searchCommericalId && s.IsDeleted == false
                           && s.SubID == _loggedInUser.LoggedInUser.SubscriberId );
        }

        private bool IsMOIUnique(ContractorDTO model, string moi)
        {
            if (model == null || string.IsNullOrEmpty(moi)) return true;

            var searchMOI = moi.Trim();
            return !_contractorRepository.GetQuery().Any(s => s.ID != model.ID && s.MOI == searchMOI && s.IsDeleted == false
                                                               && s.SubID == _loggedInUser.LoggedInUser.SubscriberId );
        }

        private bool IsTaxumberUnique(ContractorDTO model, string taxNumber)
        {
            if (model == null || string.IsNullOrEmpty(taxNumber)) return true;

            var searchTaxNumber = taxNumber.Trim();
            return !_contractorRepository.GetQuery().Any(s => s.ID != model.ID && s.TaxNumber == searchTaxNumber && s.IsDeleted == false
                                                                && s.SubID == _loggedInUser.LoggedInUser.SubscriberId );
        }

        private bool IsPermittedCity(ContractorDTO model, Guid CompanyAddressCityID)
        {
            if (model == null || CompanyAddressCityID != null) return true;

            return this._loggedInUser.SubBranches.Contains(CompanyAddressCityID);
        }



        // Contact Person
        private bool ValidateFristNameContactPerson(ContractorDTO model)
        {
            if (model == null || !IsContractPerson(model)) return true;

            return !string.IsNullOrEmpty(model.FirstName);
        }

        private bool ValidateLastNameContactPerson(ContractorDTO model)
        {
            if (model == null || !IsContractPerson(model)) return true;
            
            return !string.IsNullOrEmpty(model.LastName);
        }

        private bool ExceedMaxCharFristName(ContractorDTO model)
        {
            if (model == null)
                return false;
            if (IsContractPerson(model))
            {
                if (!string.IsNullOrEmpty(model.FirstName))
                    return (model.FirstName.Count() > 100);
            }
            return false;
        }

        private bool ExceedMaxCharLastName(ContractorDTO model)
        {
            if (model == null)
                return false;
            if (IsContractPerson(model))
            {
                if (!string.IsNullOrEmpty(model.LastName))
                    return (model.LastName.Count() > 100);
            }
            return false;
        }

        private bool ExceedMaxCharSecondName(ContractorDTO model)
        {
            if (model == null)
                return false;
            if (IsContractPerson(model))
            {
                if (!string.IsNullOrEmpty(model.SecondName))
                    return (model.SecondName.Count() > 100);
            }
            return false;
        }

        private bool ExceedMaxCharPosition(ContractorDTO model)
        {
            if (model == null)
                return false;
            if (IsContractPerson(model))
            {
                if (!string.IsNullOrEmpty(model.Position))
                    return (model.Position.Count() > 100);
            }
            return false;
        }

        private bool ExceedMaxCharPersonAddressPostalCode(ContractorDTO model)
        {
            if (model == null)
                return false;
            if (IsContractPerson(model))
            {
                if (!string.IsNullOrEmpty(model.PersonAddressPostalCode))
                    return (model.PersonAddressPostalCode.Count() > 50);
            }
            return false;
        }

        private bool InvalidEmail(ContractorDTO model)
        {
            if (model == null)
                return false;
            if (IsContractPerson(model))
            {
                if (!string.IsNullOrEmpty(model.Email))
                {
                    Regex r = new Regex(@"^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}$");
                    return !r.IsMatch(model.Email);
                }

            }
            return false;
        }

        private bool InvalidMobileNumber(ContractorDTO model)
        {
            if (model == null)
                return false;
            if (IsContractPerson(model))
            {
                if (!string.IsNullOrEmpty(model.Mobile))
                {
                    Regex r = new Regex(@"^[+]*[-\s\./0-9]*$");
                    return !r.IsMatch(model.Mobile);
                }

            }
            return false;
        }

        private bool InvalidLandlineNumber(ContractorDTO model)
        {
            if (model == null)
                return false;
            if (IsContractPerson(model))
            {
                if (!string.IsNullOrEmpty(model.LandlineNumber))
                {
                    Regex r = new Regex(@"^[-\s\./0-9]*$");
                    return !r.IsMatch(model.LandlineNumber);
                }

            }
            return false;
        }



        #region Helper
        private bool? _IsContractPerson;
        public bool IsContractPerson(ContractorDTO model)
        {
            if (!_IsContractPerson.HasValue)
            {
                if (!string.IsNullOrEmpty(model.FirstName) ||
                   !string.IsNullOrEmpty(model.LastName) ||
                   !string.IsNullOrEmpty(model.Mobile) ||
                   !string.IsNullOrEmpty(model.SecondName) ||
                   !string.IsNullOrEmpty(model.Position) ||
                   !string.IsNullOrEmpty(model.PersonAddressPostalCode) ||
                   !string.IsNullOrEmpty(model.Email) ||
                   !string.IsNullOrEmpty(model.LandlineNumber))
                    _IsContractPerson = true;
                else
                    _IsContractPerson = false;
            }
            return _IsContractPerson.Value;
        }

        #endregion


    }
}
