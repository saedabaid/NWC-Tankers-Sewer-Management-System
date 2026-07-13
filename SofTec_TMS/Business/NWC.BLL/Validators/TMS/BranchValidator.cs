using FluentValidation;
using Infrastructure;
using NWC.BLL.Interfaces;
using NWC.DAL.NWCEntities;
using NWC.DTO.Enums;
using NWC.DTO.Models.TMS;
using NWC.DTO.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NWC.BLL.Validators.TMS
{
    public class BranchValidator: AbstractValidator<AreaDTO>
    {
        IRepository<Branch> _BranchRepository;

        public BranchValidator(ValidationMode mode, ILoggedInUserService loggedInUser,
            IRepository<Branch> branchRepository)
        {
            _BranchRepository = branchRepository;

            switch (mode)
            {
                case ValidationMode.Create:
                    Initialize();
                    break;
                case ValidationMode.Update:
                    break;
                case ValidationMode.Delete:
                    break;
                case ValidationMode.CheckIfExist:
                    break;
                case ValidationMode.CreateLogic2:
                    break;
                case ValidationMode.CreateExcel:
                    break;
                default:
                    break;
            }
        }

        private void Initialize()
        {
            RuleFor(a => a).NotEmpty().WithMessage(ValidationMessagesKeys.branchModelEmpety);
            RuleFor(a => a.name).NotEmpty().WithMessage(ValidationMessagesKeys.ChooseArea);
        }
    }
}
