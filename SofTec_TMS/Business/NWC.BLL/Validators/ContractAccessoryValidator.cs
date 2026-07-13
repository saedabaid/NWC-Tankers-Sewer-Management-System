using FluentValidation;
using Infrastructure;
using NWC.DAL.NWCEntities;
using NWC.DTO.Enums;
using NWC.DTO.Models;
using NWC.DTO.Resources;
using System.Linq;

namespace NWC.BLL.Validators
{
    public class ContractAccessoryValidator : AbstractValidator<ContractAccessoryDTO>
    {
        IRepository<NWC_ContractAccessory> _contractAccessoryRepository;

        public ContractAccessoryValidator(ValidationMode mode, IRepository<NWC_ContractAccessory> contractAccessoryRepository)
        {
            this._contractAccessoryRepository = contractAccessoryRepository;

            Initialize(mode);
        }

        private void Initialize(ValidationMode mode)
        {
            switch (mode)
            {
                case ValidationMode.Create:
                    RuleFor(a => a.StationIDs).NotEmpty().WithMessage(ValidationMessagesKeys.ChooseStation);
                    RuleFor(a => a.AccessoryIDs).NotEmpty().WithMessage(ValidationMessagesKeys.ChooseAccessory);
                    RuleFor(a => a.Charge).NotNull().WithMessage(ValidationMessagesKeys.EnterCharge);
                    RuleFor(a => a.Charge).GreaterThanOrEqualTo(0).WithMessage(ValidationMessagesKeys.InvalidCharge);
                    //RuleFor(a => a.ID).Must(IsNotExist).WithMessage(ValidationMessagesKeys.ChargeAlreadyExistForAccessory);
                    break;
                case ValidationMode.Update:
                    RuleFor(a => a.StationID).NotEmpty().WithMessage(ValidationMessagesKeys.ChooseStation);
                    RuleFor(a => a.AccessoryID).NotEmpty().WithMessage(ValidationMessagesKeys.ChooseAccessory);
                    RuleFor(a => a.Charge).NotNull().WithMessage(ValidationMessagesKeys.EnterCharge);
                    RuleFor(a => a.Charge).GreaterThanOrEqualTo(0).WithMessage(ValidationMessagesKeys.InvalidCharge);
                    RuleFor(a => a.ID).NotEmpty();
                    RuleFor(a => a.ID).Must(IsNotExist).WithMessage(ValidationMessagesKeys.ChargeAlreadyExistForAccessory);
                    break;
            }
        }

        private bool IsNotExist(ContractAccessoryDTO model, long id)
        {
            var result = false;

            switch (id)
            {
                case 0:
                    result = !this._contractAccessoryRepository.GetQuery().Any(a =>
                         a.ContractID == model.ContractID &&
                         model.StationIDs.Contains(a.StationID) &&
                         model.AccessoryIDs.Contains(a.AccessoryID) && 
                         a.IsDeleted != true);
                    break;
                default:
                    result = !this._contractAccessoryRepository.GetQuery().Any(a =>
                         a.ID != id &&
                         a.ContractID == model.ContractID &&
                         a.StationID == model.StationID &&
                         a.AccessoryID == model.AccessoryID &&
                         a.IsDeleted != true);
                    break;
            }

            return result;
        }
    }
}
