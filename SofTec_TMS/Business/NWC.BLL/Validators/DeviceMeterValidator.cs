using FluentValidation;
using Infrastructure;
using NWC.BLL.Interfaces;
using NWC.DAL.NWCEntities;
using NWC.DTO.Enums;
using NWC.DTO.Models;
using NWC.DTO.Resources;
using System;

namespace NWC.BLL.Validators
{
    public class DeviceMeterValidator : AbstractValidator<DeviceMeterDTO>
    {
        IRepository<NWC_DeviceMeter> _deviceMeterRepository;
        ILoggedInUserService _loggedInUser;

        public DeviceMeterValidator(ValidationMode mode, ILoggedInUserService loggedInUser, IRepository<NWC_DeviceMeter> readingRepository)
        {
            this._deviceMeterRepository = readingRepository;
            this._loggedInUser = loggedInUser;
            switch (mode)
            {
                case ValidationMode.Create:
                    Initialize();
                    break;
            }
        }

        private void Initialize()
        {
            RuleFor(a => a).NotEmpty().WithMessage(ValidationMessagesKeys.EmptyContractor);
            RuleFor(a => a.ConnectorTubeNumber).NotEmpty().WithMessage(ValidationMessagesKeys.ChooseDeviceMeter);
            RuleFor(a => a.MeterSerialNumber).NotEmpty().WithMessage(ValidationMessagesKeys.ChooseDeviceMeter);
            RuleFor(a => a.ManholeNumber).NotEmpty().WithMessage(ValidationMessagesKeys.ChooseDeviceMeter);

            RuleFor(a => a.StationID).NotEmpty().NotEqual(Guid.Empty).WithMessage(ValidationMessagesKeys.ChooseStation);
            RuleFor(a => a.ServiceTypeID).GreaterThan(0).WithMessage(ValidationMessagesKeys.ChooseServiceType);
        }
    }
}
