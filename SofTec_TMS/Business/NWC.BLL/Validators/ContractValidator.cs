using NWC.DAL.NWCEntities;
using FluentValidation;
using NWC.DTO.Resources;
using Infrastructure;
using System.Linq;
using NWC.DTO.Enums;
using NWC.DTO.Models;
using NWC.DTO.Helpers;
using NWC.BLL.Interfaces;

namespace NWC.BLL.Validators
{
    public class ContractValidator: AbstractValidator<ContractDTO>
    {
        IRepository<NWC_Contract> _contractRepository;
        ILoggedInUserService _loggedInUserService;
        IRepository<NWC_Contractor> _contractorRepository;

        public ContractValidator(ValidationMode mode, ILoggedInUserService loggedInUser, IRepository<NWC_Contract> contractRepository, IRepository<NWC_Contractor> contractorRepository)
        {
            this._contractRepository = contractRepository;
            this._loggedInUserService = loggedInUser;
            this._contractorRepository = contractorRepository;
            switch (mode)
            {
                case ValidationMode.Create:
                    Initialize();
                    RuleFor(a => a.ContractEndDate.Date).GreaterThan(DateTimeHelper.GetDateTimeNow().Date).WithMessage(ValidationMessagesKeys.EndDateMustBeInTheFuture);
                    break;
                case ValidationMode.Update:
                    Initialize();
                    RuleFor(a => a.ID).NotEmpty();
                    break;
            }
        }


        private void Initialize()
        {
            RuleFor(a => a).NotEmpty().WithMessage(ValidationMessagesKeys.EmptyContract);
            RuleFor(a => a.Code).NotEmpty().WithMessage(ValidationMessagesKeys.InsertCode)
                .MaximumLength(50).WithMessage(ValidationMessagesKeys.ExceedMaxCharCode);
            RuleFor(a => a.ContractTypeID).NotEmpty().WithMessage(ValidationMessagesKeys.ChooseContractType)
                .GreaterThan(0).WithMessage(ValidationMessagesKeys.ChooseContractType);
            RuleFor(a => a.ContractorID).NotEmpty().WithMessage(ValidationMessagesKeys.ChooseContractor)
                .Must(IsActiveContractor).WithMessage(ValidationMessagesKeys.ActiveContractorRequired);


            RuleFor(a => a.ContractStartDate).NotEmpty().WithMessage(ValidationMessagesKeys.InsertStartDate);
            RuleFor(a => a.ContractEndDate).NotEmpty().WithMessage(ValidationMessagesKeys.InsertEndDate);
            RuleFor(a => a.ContractStartDate).LessThan(s => s.ContractEndDate).WithMessage(ValidationMessagesKeys.StartDateMustBeAfterEndDate);
            //RuleFor(a => a.ContractStatusID).NotEmpty().WithMessage(ValidationMessagesKeys.ChooseContractStatus)
            //    .NotEqual(0).WithMessage(ValidationMessagesKeys.ChooseContractStatus);
            RuleFor(a => a.Description).MaximumLength(500).WithMessage(ValidationMessagesKeys.ExceedMaxCharDescription);

            RuleFor(a => a.Code).Must(IsCodeUnique).WithMessage(ValidationMessagesKeys.ContractCodeNotUnique);
            RuleFor(a => a.AwardLetterNo).MaximumLength(24).WithMessage(ValidationMessagesKeys.ExceedMaxCharAwardLetterNo)
                .Must(IsAwardLetterNumUnique).WithMessage(ValidationMessagesKeys.AwardLetterNumNotUnique);
            RuleFor(a => a.ConfirmationNo).MaximumLength(24).WithMessage(ValidationMessagesKeys.ExceedMaxCharConfirmationNo)
                .Must(IsConfirmationNumUnique).WithMessage(ValidationMessagesKeys.ConfirmationNumNotUnique);
        }


        private bool IsCodeUnique(ContractDTO model, string contractCode)
        {
            if (model == null || string.IsNullOrEmpty(contractCode)) return true;

            var searchCode = contractCode.Trim();
            return !_contractRepository.GetQuery().Any(s => s.ID != model.ID && s.Code == searchCode && s.IsDeleted == false
                                                            && s.SubID == _loggedInUserService.LoggedInUser.SubscriberId);
        }

        private bool IsAwardLetterNumUnique(ContractDTO model, string awardNum)
        {
            if (model == null || string.IsNullOrEmpty(awardNum)) return true;

            var searchNum = awardNum.Trim();
            return !_contractRepository.GetQuery().Any(s => s.AwardLetterNo == searchNum && s.ID != model.ID && s.IsDeleted == false
                                                            && s.SubID == _loggedInUserService.LoggedInUser.SubscriberId);
        }

        private bool IsConfirmationNumUnique(ContractDTO model, string ConfirmationNum)
        {
            if (model == null || string.IsNullOrEmpty(ConfirmationNum)) return true;

            var searchNum = ConfirmationNum.Trim();
            return !_contractRepository.GetQuery().Any(s => s.ConfirmationNo == searchNum && s.ID != model.ID && s.IsDeleted == false
                                                            && s.SubID == _loggedInUserService.LoggedInUser.SubscriberId);
        }

        private bool IsActiveContractor(ContractDTO model, long contractorId)
        {
            if (model == null) return true;

            return _contractorRepository.GetQuery().Any(s => s.ID == contractorId && s.IsActive && !s.IsBlackListed && !s.IsDeleted);
        }

    }
}
