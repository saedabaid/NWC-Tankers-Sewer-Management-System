using FluentValidation;
using Infrastructure;
using NWC.BLL.Interfaces;
using NWC.DAL.NWCEntities;
using NWC.DTO.Enums;
using NWC.DTO.Models;
using NWC.DTO.Resources;
using System.Collections.Generic;
using System.Linq;

namespace NWC.BLL.Validators
{
    public class TransporterValidator : AbstractValidator<TransporterDTO>
    {
        IRepository<Transporter> _repository;
        ILoggedInUserService _loggedInUser;
        List<TransporterExcelDTO> _entryList;

        public TransporterValidator(ValidationMode mode, ILoggedInUserService loggedInUser, IRepository<Transporter> repository, List<TransporterExcelDTO> entryList = null)
        {
            _repository = repository;
            _loggedInUser = loggedInUser;
            _entryList = entryList;
            switch (mode)
            {
                case ValidationMode.Create:
                    Initialize();
                    break;
                case ValidationMode.Update:
                    Initialize();
                    RuleFor(a => a.Id).NotEmpty();
                    break;
                case ValidationMode.CreateLogic2:
                    Initialize();
                    break;
            }
        }


        private void Initialize()
        {
        }
        
        private bool IsRedundantInEntryList(TransporterExcelDTO model)
        {
            if (model == null) return true;

            return !_entryList.Any(a =>
                        a.ExcelSheetRowId != model.ExcelSheetRowId &&
                        a.Code == model.Code &&
                        a.ChassisNo == model.ChassisNo
                        );
        }
    }
}
