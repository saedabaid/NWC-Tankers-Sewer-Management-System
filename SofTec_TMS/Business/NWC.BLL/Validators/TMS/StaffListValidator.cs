using FluentValidation;
using Infrastructure;
using NWC.BLL.Interfaces;
using NWC.DAL.NWCEntities;
using NWC.DTO.Enums;
using NWC.DTO.Models;
using NWC.DTO.Models.TMS;
using NWC.DTO.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NWC.BLL.Validators.TMS
{
    public class StaffListValidator : AbstractValidator<StaffExcelDTO>
    {
        IRepository<Staff> _StaffRepository;
        ILoggedInUserService _loggedInUserService;
        List<StaffExcelDTO> _entryList;
        public StaffListValidator(ValidationMode mode, ILoggedInUserService loggedInUser,
            IRepository<Staff> StaffRepository, List<StaffExcelDTO> entryList = null)
        {
            this._StaffRepository = StaffRepository;
            this._loggedInUserService = loggedInUser;
            this._entryList = entryList;

            switch (mode)
            {
                case ValidationMode.Create:
                    Initialize();
                    break;
                case ValidationMode.Update:
                    Initialize();
                    //RuleFor(a => a.IDs).NotEmpty();
                    break;
                case ValidationMode.CreateLogic2:
                    Initialize();
                    break;
            }
        }

        private void Initialize()
        {
        
        }
    }
}
