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
    public class DeviceMeterReadingValidator: AbstractValidator<MeterReadingDTO>
    {
        IRepository<NWC_DeviceMeterReading> _readingRepository;
        ILoggedInUserService _loggedInUser;

        public DeviceMeterReadingValidator(ValidationMode mode, ILoggedInUserService loggedInUser, IRepository<NWC_DeviceMeterReading> readingRepository)
        {
            this._readingRepository = readingRepository;
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
            RuleFor(a => a.DeviceMeterID).NotEmpty().WithMessage(ValidationMessagesKeys.ChooseDeviceMeter);
            RuleFor(a => a.MeterReading).NotNull().WithMessage(ValidationMessagesKeys.MeterReadingRequired)
                .GreaterThan(0).WithMessage(ValidationMessagesKeys.MeterReadingMustPositive);

            RuleFor(a => a.ReadingTime).NotEmpty().WithMessage(ValidationMessagesKeys.ReadingTimeRequired);
            RuleFor(a => a.ReadingComment).MaximumLength(1000).WithMessage(ValidationMessagesKeys.ExceedMaxCharReadingComment);
        }
    }
}
