using NWC.DAL.NWCEntities;
using NWC.DTO.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NWC.DTO.Wrapper
{
    public static class ViolationWrapper
    {
        public static VehicleViolationDTO WrapToVehicleViolationDTO(this vw_NWC_VehicleViolation input, bool languageIsEnglish)
        {
            return new VehicleViolationDTO()
            {
                ID = input.Id,
                IncidentTime = input.IncidentTime,
                ViolationTicketNumber = input.ViolationTicketNumber,
                TermCategory = languageIsEnglish ? input.CategoryEn : input.CategoryAr,
                ViolationStatus = languageIsEnglish ? input.ViolationPaymentStatusEn : input.ViolationPaymentStatusAr,
                VehicleId = input.VehicleId.Value,
                TotalPenalty = input.TotalPenalty,
                TermCode = input.TermCode,
                TermName = input.TermName
            };
        }
    }
}
