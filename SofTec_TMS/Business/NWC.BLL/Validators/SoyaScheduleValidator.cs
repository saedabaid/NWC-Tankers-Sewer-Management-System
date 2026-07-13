using FluentValidation;
using Infrastructure;
using NWC.BLL.Services;
using NWC.DAL.NWCEntities;
using NWC.DTO.Enums;
using NWC.DTO.Helpers;
using NWC.DTO.Models;
using NWC.DTO.Resources;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NWC.BLL.Validators
{
    public class SoyaScheduleValidator : AbstractValidator<SoqyaScheduleDTO>
    {
        private readonly IRepository<NWC_SoqyaSchedules> _SoqyaSchedules;
        private readonly SoqyaService _SoqyaService;
        List<SoqyaScheduleDTO> _entryList;

        public SoyaScheduleValidator(
            ValidationMode mode,
            IRepository<NWC_SoqyaSchedules> SoqyaSchedules,
            SoqyaService SoqyaService,
            List<SoqyaScheduleDTO> entryList = null)
        {
            this._SoqyaSchedules = SoqyaSchedules;
            this._SoqyaService = SoqyaService;
            this._entryList = entryList;

            switch (mode)
            {
                case ValidationMode.Create:
                    Initialize();

                    RuleFor(a => a.CustomerAccountId).NotEmpty().WithMessage(ValidationMessagesKeys.CustomerAccountIdRequired);
                    RuleFor(a => a.MonthYearAddIds).NotEmpty().WithMessage(ValidationMessagesKeys.MonthYearRequired);
                    RuleFor(a => a.ScheculeDayListAdd).NotEmpty().WithMessage(ValidationMessagesKeys.ScheduleDateMissed);
                    RuleFor(a => a).Must(IsFutureDayForMonth_Add).WithMessage(ValidationMessagesKeys.SoqyaSchedulingAdd_PreviousDaySelected);
                    RuleFor(a => a).Must(IsVaildDaysOfMonth_Add).WithMessage(ValidationMessagesKeys.SoqyaSchedulingAdd_InValidDaysOfMonth);
                    RuleFor(a => a).Must(IsValidQuantities_Add).WithMessage(ValidationMessagesKeys.SoqyaSchedulingAdd_InValidQuantities);
                    break;
                case ValidationMode.CreateExcel:
                    Initialize();
                    RuleFor(a => a.CustomerId).NotEmpty().WithMessage(ValidationMessagesKeys.CustomerIdRequired);
                    RuleFor(a => a.AccountId).NotEmpty().WithMessage(ValidationMessagesKeys.AccountIdRequired);
                    RuleFor(a => a.CustomerAccountId).NotEmpty().WithMessage(ValidationMessagesKeys.customerAccountNotFound);
                    RuleFor(a => a.ScheduleDate).NotEmpty().WithMessage(ValidationMessagesKeys.SoqyaExcel_ScheduleDateMissed);
                    RuleFor(a => a).Must(IsVaildQuantity_AddList).WithMessage(ValidationMessagesKeys.SoqyaExcel_QuantityGreaterBalance);
                    RuleFor(a => a).Must(IsFutureSceduleDate_Update).WithMessage(ValidationMessagesKeys.SoqyaExcel_PreviousScheduleDate);
                    break;
                case ValidationMode.Update:
                    Initialize();
                    RuleFor(a => a.Id).NotEmpty();

                    RuleFor(a => a.CustomerAccountId).NotEmpty().WithMessage(ValidationMessagesKeys.CustomerAccountIdRequired);
                    RuleFor(a => a.ScheduleDate).NotEmpty().WithMessage(ValidationMessagesKeys.ScheduleDateMissed);

                    RuleFor(a => a).Must(IsValidQuantity_update).WithMessage(ValidationMessagesKeys.SoqyaSchedulingAdd_InValidQuantities);
                    RuleFor(a => a).Must(IsFutureSceduleDate_Update).WithMessage(ValidationMessagesKeys.SoqyaSchedulingAdd_PreviousDaySelected);
                    break;
            }
        }


        private void Initialize()
        {
            RuleFor(a => a).NotEmpty();
            RuleFor(a => a.Quantity).NotEmpty().WithMessage(ValidationMessagesKeys.QuantityMissed)
                .GreaterThanOrEqualTo(1).WithMessage(ValidationMessagesKeys.InvalidQuantity);

            RuleFor(a => a.TimeSlotFrom).NotEmpty().WithMessage(ValidationMessagesKeys.TimeSlotFromMissed);
            RuleFor(a => a.TimeSlotTo).NotEmpty().WithMessage(ValidationMessagesKeys.TimeSlotToMissed);

            RuleFor(a => a).Must(IsValidTimeSlots).WithMessage(ValidationMessagesKeys.TimeSlotToAfterFrom);

        }

        private bool IsValidTimeSlots(SoqyaScheduleDTO model)
        {
            if (model == null
                || string.IsNullOrEmpty(model.TimeSlotFrom)
                || string.IsNullOrEmpty(model.TimeSlotTo)) return true;


            TimeSpan from, to;

            if (!TimeSpan.TryParse(model.TimeSlotFrom, out from)
                || !TimeSpan.TryParse(model.TimeSlotTo, out to)
                || to <= from)
            {
                return false;
            }    

            return true;
        }

        private bool IsFutureDayForMonth_Add(SoqyaScheduleDTO model)
        {
            if (model == null
                || model.MonthYearAddIds == null || !model.MonthYearAddIds.Any()
                || model.ScheculeDayListAdd == null || !model.ScheculeDayListAdd.Any()
               ) return true;

            var months = model.MonthYearAddIds.Select(s => new
            {
                year = s / 100,
                month = s - ((s / 100) * 100)
            });

            var today = DateTimeHelper.GetDateTimeNow();

            var isDayPassed = model.ScheculeDayListAdd.Any(b => b < today.Day);

            if (months.Any(a => a.year < today.Year
                            || (a.year == today.Year && a.month < today.Month)
                            || (a.year == today.Year && a.month == today.Month && isDayPassed))
                )
            {
                return false;
            }

            return true;
        }

        private bool IsVaildDaysOfMonth_Add(SoqyaScheduleDTO model)
        {
            if (model == null
                || model.MonthYearAddIds == null || !model.MonthYearAddIds.Any()
                || model.ScheculeDayListAdd == null || !model.ScheculeDayListAdd.Any()
                || !model.ScheculeDayListAdd.Any(s => s > 28)) return true;

            var monthsExceed31 = new int[] { 4, 6, 9, 11 };

            var months = model.MonthYearAddIds.Select(s => new
            {
                year = s / 100,
                month = s - ((s / 100) * 100)
            });

            if (model.ScheculeDayListAdd.Any(s => s > 30) && months.Any(a => monthsExceed31.Contains(a.month)))
                return false;

            if (months.Any(a => a.month == 2))
            {
                if (model.ScheculeDayListAdd.Any(s => s > 29))
                    return false;
                else if (model.ScheculeDayListAdd.Any(s => s == 29))
                {
                    foreach (var item in months)
                    {
                        if (!DateTime.IsLeapYear(item.year))
                            return false;
                    }
                }
            }

            return true;
        }

        private bool IsValidQuantities_Add(SoqyaScheduleDTO model)
        {
            if (model == null
                || model.MonthYearAddIds == null || !model.MonthYearAddIds.Any()
                || model.ScheculeDayListAdd == null || !model.ScheculeDayListAdd.Any()
                || model.Quantity < 1
                ) return true;

            var daysCount = model.ScheculeDayListAdd.Count();
            var totalQuantity = daysCount * model.Quantity;

            foreach (var monthYear in model.MonthYearAddIds)
            {
                #region prep.
                var usedBalance = this._SoqyaService.GetBalanceAndUsed(model.CustomerAccountId, monthYear);
                if (usedBalance.IsErrorState) return false;

                var used = usedBalance.Value.UsedQuantity;
                var balance = usedBalance.Value.Balance; 
                #endregion

                if ((used + totalQuantity) > balance)
                {
                    return false;
                }

            }


            return true;
        }


        private bool IsValidQuantity_update(SoqyaScheduleDTO model)
        {
            if (model == null
                || model.CustomerAccountId < 1 || model.Quantity < 1
                ) return true;

            //var startDate = model.ScheduleDate.AddDays(-model.ScheduleDate.Day + 1);

            var usedBalance = this._SoqyaService.GetBalanceAndUsed(model.CustomerAccountId, 0, model.Id, model.ScheduleDate);
            if (usedBalance.IsErrorState) return false;

            var used = usedBalance.Value.UsedQuantity;
            var balance = usedBalance.Value.Balance;

            if ((used + model.Quantity) > balance)
            {
                return false;
            }

            return true;
        }

        private bool IsFutureSceduleDate_Update(SoqyaScheduleDTO model)
        {
            if (model == null
               || model.ScheduleDate == null
              ) return true;

            //if (model.ScheduleDate < DateTimeHelper.GetDateTimeNow()) return false;

            return !(model.ScheduleDate < DateTimeHelper.GetDateTimeNow().Date);
        }
     

        private bool IsVaildQuantity_AddList(SoqyaScheduleDTO model)
        {
            if (model == null || model.CustomerAccountId < 1 || model.ExcelExceedQuantity == false) return true;

            if (model.ExcelExceedQuantity == true) return false;

            var usedBalance = this._SoqyaService.GetBalanceAndUsed(model.CustomerAccountId, 0, null, model.ScheduleDate);
            if (usedBalance.IsErrorState) return false;

            var used = usedBalance.Value.UsedQuantity;
            var balance = usedBalance.Value.Balance;

            var startDate = model.ScheduleDate.AddDays(1 - model.ScheduleDate.Day);
            var endDate = startDate.AddMonths(1);

            var entryQuantities = this._entryList.Where(s => s.CustomerAccountId == model.CustomerAccountId
                                                          && s.ScheduleDate >= startDate && s.ScheduleDate < endDate
                                                             )
                                        .Select(s => s.Quantity).DefaultIfEmpty(0)
                                        .Sum();


            if ((used + entryQuantities) > balance)
            {
                foreach (var item in this._entryList)
                {
                    item.ExcelExceedQuantity = true;
                }

                return false;
            }

            foreach (var item in this._entryList)
            {
                item.ExcelExceedQuantity = false;
            }

            return true;
        }

    }
}
