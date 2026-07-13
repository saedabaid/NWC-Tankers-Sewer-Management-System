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
    public class StaffValidator : AbstractValidator<StaffDTO>
    {
        IRepository<Staff> _StaffRepository;
        ILoggedInUserService _loggedInUserService;
        List<StaffDTO> _entryList;
        public StaffValidator(ValidationMode mode, ILoggedInUserService loggedInUser,
            IRepository<Staff> StaffRepository, List<StaffDTO> entryList = null)
        {
            this._StaffRepository = StaffRepository;
            this._loggedInUserService = loggedInUser;
            this._entryList = entryList;

            switch (mode)
            {
                case ValidationMode.Create:
                    Initialize();
                    RuleFor(a => a.Username).NotEmpty().WithMessage(ValidationMessagesKeys.InsertUsername);
                    RuleFor(a => a.Password).NotEmpty().WithMessage(ValidationMessagesKeys.InsertPassword);
                    break;
                case ValidationMode.Update:
                    RuleFor(a => a.ID).NotEmpty().WithMessage(ValidationMessagesKeys.IDNotExist);
                    Initialize();
                    RuleFor(a => a.Username).NotEmpty().WithMessage(ValidationMessagesKeys.InsertUsername);
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
            RuleFor(a => a).NotEmpty().WithMessage(ValidationMessagesKeys.StaffModelEmpty);
            RuleFor(a => a.FirstName).NotEmpty().WithMessage(ValidationMessagesKeys.InsertFirstName);
            RuleFor(a => a.MiddleName).NotEmpty().WithMessage(ValidationMessagesKeys.InsertMiddleName);
            RuleFor(a => a.personalID).NotEmpty().WithMessage(ValidationMessagesKeys.InsertPersonalID);
            RuleFor(a => a.code).NotEmpty().WithMessage(ValidationMessagesKeys.InsertCode);
            RuleFor(a => a.staffRoleCategoryID).NotEmpty().WithMessage(ValidationMessagesKeys.ChooseRoleCategory);
            RuleFor(a => a.staffRoleID).NotEmpty().WithMessage(ValidationMessagesKeys.ChooseStaffRole);
            RuleFor(a => a.AllocatedSubBranch).NotEmpty().WithMessage(ValidationMessagesKeys.ChooseBranch);
            RuleFor(a => a.PermittedBranch.Count()).GreaterThan(0).WithMessage(ValidationMessagesKeys.ChoosePermittedBranch);
            RuleFor(a => a.PermittedSubBranch.Count()).GreaterThan(0).WithMessage(ValidationMessagesKeys.ChoosePermittedBranch);
        }
    }
}
