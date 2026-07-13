using FluentValidation;
using Infrastructure;
using NWC.DAL.NWCEntities;
using NWC.DTO.Enums;
using NWC.DTO.Models;
using NWC.DTO.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace NWC.BLL.Validators
{
    class ContractStationValidator : AbstractValidator<ContractStationDTO>
    {
        IRepository<NWC_ContractStations> _ContractStationsRepository;
        IRepository<NWC_Contract> _ContractRepository;
        public ContractStationValidator(ValidationMode mode, IRepository<NWC_ContractStations> ContractStationsRepository = null, IRepository<NWC_Contract> ContractRepository = null)
        {
            this._ContractStationsRepository = ContractStationsRepository;
            this._ContractRepository = ContractRepository;

            switch (mode)
            {
                case ValidationMode.Create:
                    Initialize();
                    // 
                    break;
                case ValidationMode.Update:
                    Initialize();
                    //RuleFor(a => IsStationSavedBeforeForEdite(a)).Equal(false).WithMessage(ValidationMessagesKeys.StationSavedBefore);
                    break;
                case ValidationMode.CheckIfExist:
                    RuleFor(a => IsStationSavedBefore(a)).Equal(false).WithMessage(ValidationMessagesKeys.StationSavedBefore);
                    RuleFor(a => !IsStationSavedBefore(a) && IsOverlap(a)).Equal(false).WithMessage(ValidationMessagesKeys.StationContractOverlap);
                    break;
            }
        }


        private void Initialize()
        {
            RuleFor(a => a).NotEmpty().WithMessage(ValidationMessagesKeys.ModelEmpety);
            RuleFor(a => a.StationIDs).NotEmpty().WithMessage(ValidationMessagesKeys.ChooseStation);
            RuleFor(a => a.ContractID).NotEmpty().WithMessage(ValidationMessagesKeys.ContractIDMissed);
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
        }

        private bool ValidateFristNameContactPerson(ContractStationDTO model)
        {
            if (model == null || model.ContactPerson == null)
                return true;
            if (IsContractPerson(model))
            {
                return !string.IsNullOrEmpty(model.ContactPerson.FirstName);
            }
          return true;
        }

        private bool ValidateLastNameContactPerson(ContractStationDTO model)
        {
            if (model == null || model.ContactPerson == null)
                return true;
            if (IsContractPerson(model))
            {
                return !string.IsNullOrEmpty(model.ContactPerson.LastName);
            }
            return true;
        }

        private bool ExceedMaxCharFristName(ContractStationDTO model)
        {
            if (model == null || model.ContactPerson == null)
                return false;
            if (IsContractPerson(model))
            {
                if(!string.IsNullOrEmpty(model.ContactPerson.FirstName))
                    return  (model.ContactPerson.FirstName.Count() > 100);
            }
            return false;
        }

        private bool ExceedMaxCharLastName(ContractStationDTO model)
        {
            if (model == null || model.ContactPerson == null)
                return false;
            if (IsContractPerson(model))
            {
               if( !string.IsNullOrEmpty(model.ContactPerson.LastName))
                    return (model.ContactPerson.LastName.Count() > 100);
            }
            return false;
        }

        private bool ExceedMaxCharSecondName(ContractStationDTO model)
        {
            if (model == null || model.ContactPerson == null)
                return false;
            if (IsContractPerson(model))
            {
                if (!string.IsNullOrEmpty(model.ContactPerson.SecondName))
                    return (model.ContactPerson.SecondName.Count() > 100);
            }
            return false;
        }

        private bool ExceedMaxCharPosition(ContractStationDTO model)
        {
            if (model == null || model.ContactPerson == null)
                return false;
            if (IsContractPerson(model))
            {
                if (!string.IsNullOrEmpty(model.ContactPerson.Position))
                    return (model.ContactPerson.Position.Count() > 100);
            }
            return false;
        }

        private bool ExceedMaxCharPersonAddressPostalCode(ContractStationDTO model)
        {
            if (model == null || model.ContactPerson == null)
                return false;
            if (IsContractPerson(model))
            {
                if (!string.IsNullOrEmpty(model.ContactPerson.PersonAddressPostalCode))
                    return (model.ContactPerson.PersonAddressPostalCode.Count() > 50);
            }
            return false;
        }

        private bool InvalidEmail(ContractStationDTO model)
        {
            if (model == null || model.ContactPerson == null)
                return false;
            if (IsContractPerson(model))
            {
                if (!string.IsNullOrEmpty(model.ContactPerson.Email))
                {
                    Regex r = new Regex(@"^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}$");
                    return ! r.IsMatch(model.ContactPerson.Email);
                }
                    
            }
            return false;
        }

        private bool InvalidMobileNumber(ContractStationDTO model)
        {
            if (model == null || model.ContactPerson == null)
                return false;
            if (IsContractPerson(model))
            {
                if (!string.IsNullOrEmpty(model.ContactPerson.Mobile))
                {
                    Regex r = new Regex(@"^[+]*[-\s\./0-9]*$");
                    return ! r.IsMatch(model.ContactPerson.Mobile);
                }

            }
            return false;
        }
        private bool InvalidLandlineNumber(ContractStationDTO model)
        {
            if (model == null || model.ContactPerson == null)
                return false;
            if (IsContractPerson(model))
            {
                if (!string.IsNullOrEmpty(model.ContactPerson.LandlineNumber))
                {
                    Regex r = new Regex(@"^[-\s\./0-9]*$");
                    return ! r.IsMatch(model.ContactPerson.LandlineNumber);
                }

            }
            return false;
        }

        private bool? _IsContractPerson;
        public bool IsContractPerson(ContractStationDTO model)
        {
            if (!_IsContractPerson.HasValue)
            {
                if(!string.IsNullOrEmpty(model.ContactPerson.FirstName) ||
                   !string.IsNullOrEmpty(model.ContactPerson.LastName)||
                   !string.IsNullOrEmpty(model.ContactPerson.Mobile)||
                   !string.IsNullOrEmpty(model.ContactPerson.SecondName) ||
                   !string.IsNullOrEmpty(model.ContactPerson.Position) ||
                   !string.IsNullOrEmpty(model.ContactPerson.PersonAddressPostalCode) ||
                   !string.IsNullOrEmpty(model.ContactPerson.Email) ||
                   !string.IsNullOrEmpty(model.ContactPerson.LandlineNumber) )
                    _IsContractPerson = true;
                else
                    _IsContractPerson = false;
            }
            return _IsContractPerson.Value;
        }


        private bool IsStationSavedBefore(ContractStationDTO model)
        {
            var StationId = model.StationIDs[model.index];
            var result = _ContractStationsRepository.GetQuery().Any(s => StationId == s.StationID &&
             s.ContractID == model.ContractID && s.IsDeleted != true);
            return result;
        }

        private bool IsOverlap(ContractStationDTO model)
        {
            var StationId = model.StationIDs[model.index];
            var newContract = this._ContractRepository.FindById(model.ContractID);
            var result = _ContractStationsRepository.GetQuery().Any(s => s.NWC_Contract.ContractStartDate < newContract.ContractEndDate
            && newContract.ContractStartDate < s.NWC_Contract.ContractEndDate
            && s.StationID == StationId && s.IsDeleted != true && s.IsActive !=false );
            return result;

        }
        private bool IsStationSavedBeforeForEdite(ContractStationDTO model)
        {
            var LastStationID = _ContractStationsRepository.FindById(model.ContractStationID).StationID;
            var CurrentStationID = model.StationIDs[0];
            return _ContractStationsRepository.GetQuery().Any(s => (CurrentStationID == s.StationID) && (s.StationID != LastStationID) && (s.ContractID == model.ContractID) && (s.IsDeleted != true));

        }
    }
}
