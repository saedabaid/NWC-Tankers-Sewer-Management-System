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
using System.Threading.Tasks;

namespace NWC.BLL.Validators
{
    public class ContractTermsValidator : AbstractValidator<ContractTermDTO>
    {
        IRepository<NWC_ContractTerms> _ContractTermsRepository;

        public ContractTermsValidator(ValidationMode mode, IRepository<NWC_ContractTerms> ContractTermsRepository)
        {
            this._ContractTermsRepository = ContractTermsRepository;
            switch (mode)
            {
                case ValidationMode.Create:
                    Initialize();
                    break;
                case ValidationMode.Update:
                    Initialize();
                    RuleFor(a => a.index).Must(IsStationWithCategoryTermExist).WithMessage(ValidationMessagesKeys.TermAlreadyExist);
                    break;
                case ValidationMode.CheckIfExist:
                    RuleFor(a => a.index).Must(IsStationWithCategoryTermExist).WithMessage(ValidationMessagesKeys.TermAlreadyExist);
                    break;
            }
        }


        private void Initialize()
        {
            RuleFor(a => a).NotEmpty().WithMessage(ValidationMessagesKeys.EmptyContract);
            RuleFor(a => a.StationIDs).NotEmpty().WithMessage(ValidationMessagesKeys.ChooseStation);
            RuleFor(a => a.ContractID).NotEmpty().WithMessage(ValidationMessagesKeys.ContractIDMissed);
            RuleFor(a => a.ContractTermCode).NotEmpty().WithMessage(ValidationMessagesKeys.ContractTermCodeMissed);
            RuleFor(a => a.TermsCategoryID).NotEmpty().WithMessage(ValidationMessagesKeys.ChooseCategory);
            RuleFor(a => a.ContractTermName).NotEmpty().WithMessage(ValidationMessagesKeys.ContractNameMissed);
            // RuleFor(a => a.ContractTermCode).Must(IsCodeUnique).WithMessage(ValidationMessagesKeys.CodeUnique);
            RuleFor(a => a.ContractTermCode).Must(IsCodeUnique).WithMessage(ValidationMessagesKeys.CodeUnique);
            RuleFor(a => a).Must(IsValidValueUnit).WithMessage(ValidationMessagesKeys.UnitIdIsRequiredAsTotalValueIsExist);

        }


        private bool IsCodeUnique(ContractTermDTO model, string TermCode)
        {
            if (model == null || string.IsNullOrEmpty(TermCode)) return true;

            var searchCode = TermCode.Trim();
            return !this._ContractTermsRepository.GetQuery().Any(s => s.Code == searchCode && s.ContractID == model.ContractID && s.ID != model.ID );
        }

        private bool IsStationWithCategoryTermExist(ContractTermDTO model,int index)
        {
            var stationId = model.StationIDs[index];
            return !_ContractTermsRepository.GetQuery().Any(s =>
                                                            s.Code == model.ContractTermCode && 
                                                            s.StationID == stationId && 
                                                            s.ID != model.ID && 
                                                            s.ContractID == model.ContractID &&
                                                            s.TermsCategoryID == model.TermsCategoryID);
        }

        private bool IsValidValueUnit(ContractTermDTO model)
        {
            if (model == null || model.TotalValue == null || model.TotalValue == 0) return true;

            return (model.TotalValueUnitId != null && model.TotalValue > 0);
        }



        //private bool IsStationWithCategoryTermExistForEditting(ContractTermDTO model, int index)
        //{
        //    var TermID = _ContractTermsRepository.FindById(model.ID).ID;
        //  //  var CurrentStationID = model.StationIDs[0];
        //   // return _ContractStationsRepository.GetQuery().Any(s => (CurrentStationID == s.StationID) && (s.StationID != LastStationID) && (s.ContractID == model.ContractID) && (s.IsDeleted != true));

        //    var stationId = model.StationIDs[0];

        //    return !_ContractTermsRepository.GetQuery().Any(s =>
        //                                                    s.Code == model.ContractTermCode &&
        //                                                    s.StationID == stationId &&
        //                                                    s.ID != model.ID &&
        //                                                    s.ContractID != model.ContractID &&
        //                                                    s.TermsCategoryID == model.TermsCategoryID);
        //}

    }
}
