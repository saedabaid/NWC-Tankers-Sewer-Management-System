using FluentValidation;
using Infrastructure;
using NWC.BLL.Interfaces;
using NWC.DAL.NWCEntities;
using NWC.DTO.Enums;
using NWC.DTO.Models;
using NWC.DTO.Resources;
using System;
using System.Linq;

namespace NWC.BLL.Validators
{
    public class ContractTermViolationValidator : AbstractValidator<ContractTermsViolationsDTO>
    {
        ILoggedInUserService _loggedInUserService;
        IRepository<NWC_ContractTerms> _contractTermsRepository;

        public ContractTermViolationValidator(ValidationMode mode, ILoggedInUserService loggedInUser, IRepository<NWC_ContractTerms> contractTermsRepository)
        {
            this._loggedInUserService = loggedInUser;
            this._contractTermsRepository = contractTermsRepository;

            switch (mode)
            {
                case ValidationMode.Create:
                    Initialize();
                    break;
                case ValidationMode.Update:
                    Initialize();
                    RuleFor(a => a.Id).NotEmpty();
                    break;
            }
        }


        private void Initialize()
        {
            RuleFor(a => a).NotEmpty().WithMessage(ValidationMessagesKeys.EmptyContractTermViolation);
            RuleFor(a => a.ContractTermId).NotEmpty().WithMessage(ValidationMessagesKeys.ContractTermRequired);

            RuleFor(a => a.ViolationLocation)//.NotEmpty().WithMessage(ValidationMessagesKeys.ViolationLocationRequired)
                .MaximumLength(300).WithMessage(ValidationMessagesKeys.ExceedMaxCharViolationLocation);

            RuleFor(a => a.ViolationDescription).MaximumLength(1000).WithMessage(ValidationMessagesKeys.ExceedMaxCharViolationDescription);

            RuleFor(a => a.IncidentTime).GreaterThan(DateTime.MinValue).WithMessage(ValidationMessagesKeys.IncidentTimeRequired);
            RuleFor(a => a.IssueDate).GreaterThan(DateTime.MinValue).WithMessage(ValidationMessagesKeys.IssueDateRequired);
            //RuleFor(a => a.PaymentDueDate).GreaterThan(DateTime.MinValue).WithMessage(ValidationMessagesKeys.PaymentDueDateRequired);

            RuleFor(a => a.PaymentStatusId).NotEmpty().WithMessage(ValidationMessagesKeys.PaymentStatusIdRequired);
            
            RuleFor(a => a.StatusId).NotEmpty().WithMessage(ValidationMessagesKeys.StatusIdRequired);
                
            RuleFor(a => a).Must(IsCancellationReasonValid).WithMessage(ValidationMessagesKeys.CancellationReasonRequired);
            RuleFor(a => a).Must(IsNumberOfDaysValid).WithMessage(ValidationMessagesKeys.NumberOfDaysNotValid);

        }

        private bool IsCancellationReasonValid(ContractTermsViolationsDTO model)
        {
            if (model == null || !model.StatusId.HasValue || model.StatusId != 3) return true;
            return model.CancelReasonId.HasValue;
        }

        private bool IsNumberOfDaysValid(ContractTermsViolationsDTO model)
        {
            if (model == null || model.ContractTermId < 1) return true;

            var required = this._contractTermsRepository.GetQuery()
                .Any(s => s.ID == model.ContractTermId && s.TotalValueUnitId == 2); //per day

            return !required || (model.TermUnitNoOfDays > 0);
        }


    }
}
