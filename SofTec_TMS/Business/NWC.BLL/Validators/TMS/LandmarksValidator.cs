using FluentValidation;
using Infrastructure;
using NWC.BLL.Interfaces;
using NWC.DAL.NWCEntities;
using NWC.DTO.Enums;
using NWC.DTO.Models;
using System.Collections.Generic;

namespace NWC.BLL.Validators
{
    public class LandmarksValidator : AbstractValidator<LandmarkDto>
    {
        IRepository<Landmark> _repository;
        ILoggedInUserService _loggedInUserService;
        List<LandmarkDto> _entryList;

        public LandmarksValidator(ValidationMode mode, ILoggedInUserService loggedInUser, IRepository<Landmark> repository, List<LandmarkDto> entryList = null)
        {
            _repository = repository;
            _loggedInUserService = loggedInUser;
            _entryList = entryList;

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
        }
    }
}
