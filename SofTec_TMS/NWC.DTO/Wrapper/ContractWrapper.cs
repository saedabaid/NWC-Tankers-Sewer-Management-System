using Infrastructure;
using NWC.DAL.NWCEntities;
using NWC.DTO.Common;
using NWC.DTO.Constants;
using NWC.DTO.Helpers;
using NWC.DTO.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace NWC.DTO.Wrapper
{
    public static class ContractWrapper
    {
        #region Contract
        #region Domain ==> DTO
        public static ContractDTO WrapToContractDTO(this vw_NWC_ContractList from)
        {
            var contractDto = new ContractDTO();

            //contractDto.SubID = from.SubID;
            contractDto.ID = from.ID;
            contractDto.Code = from.Code;
            contractDto.ContractorID = from.ContractorID;
            contractDto.ContractorFullName = from.ContractorFullName;
            contractDto.ContractTypeID = from.ContractTypeID;
            contractDto.ContractTypeName = LanguageIsEnglish ? from.ContractTypeEn : from.ContractTypeAr;
            contractDto.ConfirmationNo = from.ConfirmationNo;
            contractDto.AwardLetterNo = from.AwardLetterNo;
            contractDto.ContractStartDate = from.ContractStartDate;
            contractDto.ContractEndDate = from.ContractEndDate;
            contractDto.ContractStatusID = from.ContractStatusID;
            contractDto.ContractStatusName = LanguageIsEnglish ? from.ContractStatusEn : from.ContractStatusAr;
            contractDto.ContractStatusEnumId = from.ContractStatusEnumId;
            //contractDto.TerminatedDate = from.TerminatedDate;
            contractDto.IsDeleted = from.IsDeleted;
            contractDto.StationsCount = from.StationsCount;
            contractDto.Description = from.Description;
            //contractDto.TerminationReasonID = from.TerminationReasonID;
            //contractDto.IsTerminated = from.IsTerminated;

            return contractDto;
        }

        public static AttachmentDTO WrapContractAttachment(this NWC_ContractAttachment from)
        {
            if (from == null)
                return null;

            var to = new AttachmentDTO();
            to.ID = from.ID;
            to.DocumentName = from.DocumentName;
            to.RelativePath = from.RelativePath;
            to.IsDeleted = from.IsDeleted;

            return to;
        }

        #endregion

        #region DTO ==> Domain
        public static NWC_Contract WrapToContract(this ContractDTO from)
        {
            if (from == null)
                return null;

            var contract = new NWC_Contract();

            contract.ID = from.ID;
            contract.Code = from.Code;
            contract.ContractTypeID = from.ContractTypeID;
            contract.AwardLetterNo = from.AwardLetterNo;
            contract.ConfirmationNo = from.ConfirmationNo;
            contract.ContractorID = from.ContractorID;
            contract.ContractStartDate = from.ContractStartDate;
            contract.ContractEndDate = from.ContractEndDate;
            contract.Description = from.Description;

            //contract.ContractStatusID = from.ContractStatusID;

            //contractDto.IsTerminated = from.IsTerminated; //need review use to deticate terminated 
            //if (from.IsTerminated == true)
            //{
            //    contract.TerminatedDate = from.TerminatedDate;
            //    contract.TerminationReasonID = from.TerminationReasonID;
            //}

            //contractDto.SubID = from.SubID;
            //contractDto.IsDeleted = from.IsDeleted;
            return contract;
        }
        #endregion 

        #endregion

        #region ContractAccessory
        public static List<NWC_ContractAccessory> WrapToContractAccessories(this ContractAccessoryDTO dto, Guid userID)
        {
            var accList = new List<NWC_ContractAccessory>();

            for (int index = 0; index < dto.StationIDs.Count; index++)
            {
                for (int index2 = 0; index2 < dto.AccessoryIDs.Count; index2++)
                {
                    accList.Add(new NWC_ContractAccessory()
                    {
                        ID = dto.ID,
                        ContractID = dto.ContractID,
                        StationID = dto.StationIDs[index],
                        AccessoryID = dto.AccessoryIDs[index2],
                        Charge = dto.Charge,
                        IsDeleted = false,
                        CreatedDate = DateTimeHelper.GetDateTimeNow(),
                        CreatedBy = userID
                    });
                }
            }

            return accList;
        }
        public static NWC_ContractAccessory WrapToContractAccessory(this ContractAccessoryDTO dto, NWC_ContractAccessory input, Guid userID)
        {
            input.ID = dto.ID;
            input.ContractID = dto.ContractID;
            input.StationID = dto.StationID;
            input.AccessoryID = dto.AccessoryID;
            input.Charge = dto.Charge;
            input.UpdatedDate = DateTimeHelper.GetDateTimeNow();
            input.UpdatedBy = userID;

            return input;
        }

        public static ContractAccessoryDTO WrapToContractAccessory(this vw_NWC_ContractAccessory input)
        {
            return new ContractAccessoryDTO()
            {
                ID = input.ID,
                ContractID = input.ContractID,
                StationID = input.StationID,
                AccessoryID = input.AccessoryID,
                Charge = input.Charge,
                StationName = input.StationName,
                AccessoryName = LanguageIsEnglish ? input.AccessoryNameEn : input.AccessoryNameAr
            };
        }
        #endregion

        #region Tariff
        #region Domain ==> DTO
        public static ContractTariffDTO WrapToTariffDTO(this vw_NWC_ContractTariff from)
        {
            var tariffDto = new ContractTariffDTO();

            tariffDto.ID = from.ID;
            tariffDto.ContractID = from.ContractID;
            tariffDto.CityName = from.CityName;
            tariffDto.StationID = from.StationID;
            tariffDto.StationName = from.StationName;
            tariffDto.ZoneID = from.ZoneID;
            tariffDto.ZoneName = from.Zone;
            tariffDto.CustomerLocationClassID = from.CustomerLocationClassID;
            tariffDto.CustomerClassName = LanguageIsEnglish ? from.CustomerClassEn : from.CustomerClassAr;
            tariffDto.ServiceTypeID = from.ServiceTypeID;
            tariffDto.ServiceTypeName = LanguageIsEnglish ? from.ServiceTypeEn : from.ServiceTypeAr;
            tariffDto.DateFrom = from.DateFrom;
            tariffDto.DateTo = from.DateTo;
            tariffDto.CubicMeterCharge = from.CubicMeterCharge;
            tariffDto.DistanceCharge = from.DistanceCharge;
            tariffDto.AfterFirstKM = from.AfterFirstKM;
            tariffDto.TanckerCapacityId = from.TanckerCapacityId;
            tariffDto.DateFromHijri = from.DateFromHijri;
            tariffDto.DateToHijri = from.DateToHijri;

            return tariffDto;
        }
        #endregion

        #region DTO ==> Domain

        //public static NWC_ContractTariff WrapToTariff(this ContractTariffDTO from)
        //{
        //    if (from == null) return null;

        //    var tariff = new NWC_ContractTariff();

        //    tariff.ID = from.ID;
        //    tariff.ContractID = from.ContractID;
        //    tariff.StationID = from.StationID;
        //    tariff.ZoneID = from.ZoneID;
        //    tariff.CustomerLocationClassID = from.CustomerLocationClassID;
        //    tariff.ServiceTypeID = from.ServiceTypeID;
        //    tariff.DateFrom = from.DateFrom;
        //    tariff.DateTo = from.DateTo;
        //    tariff.CubicMeterCharge = from.CubicMeterCharge;
        //    tariff.DistanceCharge = from.DistanceCharge;
        //    tariff.AfterFirstKM = from.AfterFirstKM;

        //    return tariff;
        //}

        public static List<NWC_ContractTariff> WrapToTariffsAdd(this ContractTariffDTO from)
        {
            if (from == null)
                return null;

            if (from.ZoneAddIds != null && from.ZoneAddIds.Count() > 0)
            {
                var to = new List<NWC_ContractTariff>();

                foreach (var zoneId in from.ZoneAddIds)
                {
                    to.AddRange(HelperWrapToTariffsAdd2(from, zoneId));
                }

                return to;
            }
            else
            {
                return HelperWrapToTariffsAdd2(from, null);
            }

        }

        //private static List<NWC_ContractTariff> HelperWrapToTariffsAdd(ContractTariffDTO from, long? _zoneId)
        //{
        //    var to = new List<NWC_ContractTariff>();

        //    foreach (var stationId in from.StationsAddIds)
        //    {
        //        foreach (var classId in from.CustomerLocationClassAddIds)
        //        {
        //            foreach (var serviceId in from.ServiceTypeAddIds)
        //            {
        //                var tariff = new NWC_ContractTariff();

        //                tariff.ContractID = from.ContractID;
        //                tariff.DateFrom = from.DateFrom;
        //                tariff.DateTo = from.DateTo;
        //                tariff.CubicMeterCharge = from.CubicMeterCharge;
        //                tariff.DistanceCharge = from.DistanceCharge;
        //                tariff.AfterFirstKM = from.AfterFirstKM;


        //                tariff.StationID = stationId;
        //                tariff.ZoneID = _zoneId;
        //                tariff.CustomerLocationClassID = classId;
        //                tariff.ServiceTypeID = serviceId;

        //                to.Add(tariff);
        //            }
        //        }
        //    }

        //    return to;
        //}

        private static List<NWC_ContractTariff> HelperWrapToTariffsAdd2(ContractTariffDTO from, long? _zoneId)
        {
            if (from.TanckerCapacityAddIds != null && from.TanckerCapacityAddIds.Count() > 0)
            {
                var to = new List<NWC_ContractTariff>();

                foreach (var capacityId in from.TanckerCapacityAddIds)
                {
                    to.AddRange(HelperWrapToTariffsAdd3(from, _zoneId, capacityId));
                }

                return to;
            }
            else
            {
                return HelperWrapToTariffsAdd3(from, _zoneId, null);
            }

        }

        private static List<NWC_ContractTariff> HelperWrapToTariffsAdd3(ContractTariffDTO from, long? _zoneId, int? TanckerCapacityId)
        {
            var to = new List<NWC_ContractTariff>();

            foreach (var stationId in from.StationsAddIds)
            {
                foreach (var classId in from.CustomerLocationClassAddIds)
                {
                    foreach (var serviceId in from.ServiceTypeAddIds)
                    {
                        var tariff = new NWC_ContractTariff();

                        tariff.ContractID = from.ContractID;
                        tariff.DateFrom = from.DateFrom;
                        tariff.DateTo = from.DateTo;
                        tariff.CubicMeterCharge = from.CubicMeterCharge;
                        tariff.DistanceCharge = from.DistanceCharge;
                        tariff.AfterFirstKM = from.AfterFirstKM;
                        tariff.DateFromHijri = from.DateFromHijri;
                        tariff.DateToHijri = from.DateToHijri;

                        tariff.StationID = stationId;
                        tariff.ZoneID = _zoneId;
                        tariff.CustomerLocationClassID = classId;
                        tariff.ServiceTypeID = serviceId;
                        tariff.TanckerCapacityId = TanckerCapacityId;

                        to.Add(tariff);
                    }
                }
            }

            return to;
        }

        public static NWC_ContractTariff WrapToTariff(this ContractTariffDTO from)
        {
            if (from == null)
                return null;

            var to = new NWC_ContractTariff();

            to.ContractID = from.ContractID;
            to.StationID = from.StationID;
            to.ZoneID = from.ZoneID;
            to.CustomerLocationClassID = from.CustomerLocationClassID;
            to.ServiceTypeID = from.ServiceTypeID;
            //to.DateFrom = from.DateFrom;
            //to.DateTo = from.DateTo;
            to.CubicMeterCharge = from.CubicMeterCharge;
            to.DistanceCharge = from.DistanceCharge;
            to.AfterFirstKM = from.AfterFirstKM;
            to.TanckerCapacityId = from.TanckerCapacityId;
            to.DateFromHijri = from.DateFromHijri;
            to.DateToHijri = from.DateToHijri;

            return to;
        }


        #endregion
        #endregion

        #region station
        public static ContractStationListDTO WrapToContractStationListDTO(this vw_NWC_ContractStations from)
        {
            return new ContractStationListDTO()
            {
                StationID = from.StationID,
                ContractID = from.ContractID,
                ContactPersonID = from.ContactPersonID,
                stationName = from.stationName,   //station name
                stationCode = from.stationCode,
                BranchName = from.BranchName,
                FullName = from.FullName,
                Mobile = from.Mobile,
                FirstName = from.FirstName,
                SecondName = from.SecondName,
                LastName = from.LastName,
                LandlineNumber = from.LandlineNumber,
                LandlineNumbeCode = from.LandlineNumbeCode,
                MobileCode = from.MobileCode,
                Email = from.Email,
                PersonAddress = from.PersonAddress,
                PersonAddressPostalCode = from.PersonAddressPostalCode,
                Position = from.Position,
                PersonalIDType = from.PersonalIDType,
                isDeleted = from.ContractStationIsDeleted,
                branchId = from.branchId,
                AreaId = from.AreaID,
                SubId = from.SubId,
                PersonalIDNumber = from.PersonalIDNumber,
                contractStationID = from.contractStationID
            };
        }

        public static NWC_ContactPerson WrapToNWC_ContactPerson(this ContactPersonDTO from)
        {
            return new NWC_ContactPerson()
            {

                FullName = from.FirstName + " " + from.SecondName + " " + from.LastName,
                Mobile = from.Mobile,
                FirstName = from.FirstName,
                SecondName = from.SecondName,
                LastName = from.LastName,
                LandlineNumber = from.LandlineNumber,
                LandlineNumbeCode = from.LandlineNumbeCode,
                MobileCode = from.MobileCode,
                Email = from.Email,
                PersonAddress = from.PersonAddress,
                PersonAddressPostalCode = from.PersonAddressPostalCode,
                Position = from.Position,
                PersonalIDType = from.PersonalIDType,
                PersonalIDNumber = from.PersonalIDNumber

            };
        }

        #endregion

        #region price
        public static ContractPriceDTO WrapToContractPriceDTO(this vw_NWC_ContractPriceList dto)
        {
            return new ContractPriceDTO()
            {
                StationName = dto.StationName,
                ServiceTypeName = LanguageIsEnglish ? dto.ServiceTypeNameEn : dto.ServiceTypeNameAr,
                CustomerLocationClassName = LanguageIsEnglish ? dto.CustomerLocationClassNameEn : dto.CustomerLocationClassNameAr,
                PriceCharge = dto.PriceCharge,
                StationID = dto.StationID,
                ContractID = dto.ContractID,
                ContractPriceID = dto.ContractPriceID
            };
        }
        #endregion

        #region Terms
        public static vw_NWC_ContractTermsDTO WrapToContractTermsDTO(this vw_NWC_ContractTerms ct)
        {
            return new vw_NWC_ContractTermsDTO()
            {
                ID = ct.ID,
                ContractTermName = ct.ContractTermName,
                Description = ct.Description,
                TermsCategoryID = ct.TermsCategoryID,
                ContractID = ct.ContractID,
                StationID = ct.StationID,
                stationName = ct.stationName,
                Category = LanguageIsEnglish ? ct.CategoryEn : ct.CategoryAr,
                stationCode = ct.stationCode,
                ContractTermCode = ct.ContractTermCode,
                TotalValue = ct.TotalValue,
                TotalValueUnitId = ct.TotalValueUnitId,
                TotalValueUnitName = LanguageIsEnglish ? ct.TotalValueUnitNameEn : ct.TotalValueUnitNameAr

            };
        }
        #endregion

        #region Terms Violations
        #region Domain => DTO
        public static ContractTermsViolationsDTO WrapToContractViolationsDTO(this vw_NWC_ContractTermsViolations from)
        {
            if (from == null)
                return null;

            var to = new ContractTermsViolationsDTO
            {
                Id = from.Id,
                ContractTermId = from.ContractTermId,
                ViolationTicketNumber = from.ViolationTicketNumber,
                ViolationLocation = from.ViolationLocation,
                IncidentTime = from.IncidentTime,
                TotalPenalty = from.TotalPenalty,
                IssueDate = from.IssueDate,
                PaymentDueDate = from.PaymentDueDate,
                PaymentStatusDate = from.PaymentStatusDate,
                ViolationDescription = from.ViolationDescription,
                IsDeleted = from.IsDeleted,
                ContractTermCode = from.ContractTermCode,
                ContractTermName = from.ContractTermName,
                ContractTermDescription = from.ContractTermDescription,
                ContractID = from.ContractID,
                ContractCode = from.ContractCode,
                CategoryId = from.CategoryId,
                CategoryAr = from.CategoryAr,
                CategoryEn = from.CategoryEn,
                StationID = from.StationID,
                StationName = from.StationName,
                PaymentStatusId = from.PaymentStatusId,
                PaymentStatusAr = from.PaymentStatusAr,
                PaymentStatusEn = from.PaymentStatusEn,
                VehicleId = from.VehicleId,
                VehicleCode = from.VehicleCode,
                VehiclePlateNo = from.VehiclePlateNo,
                DriverId = from.DriverId,
                DriverCode = from.DriverCode,
                DriverMobileNumber = from.DriverMobileNumber,
                TermUnitNoOfDays = from.TermUnitNoOfDays,
                AddVehicleToBlacklist = from.AddVehicleToBlacklist,
                StatusId = from.StatusId,
                StatusNameAr = from.StatusNameAr,
                StatusNameEn = from.StatusNameEn,
                CancelReasonId = from.CancelReasonId,
                CancelReasonAr = from.CancelReasonAr,
                CancelReasonEn = from.CancelReasonEn,
                IsFinalDecision = from.IsFinalDecision,
                CurrentApprovalStatus = from.CurrentApprovalStatus,
            };

            to.DriverName = $"{from.DriverFirstName ?? string.Empty} {from.DriverMiddleName ?? string.Empty} {from.DriverLastName ?? string.Empty}";
            //to.VehicleCodePlateNo = $"{from.VehicleCode ?? string.Empty} | {from.VehiclePlateNo ?? string.Empty}";

            return to;
        }


        public static NWC_ViolationsApprovals WrapToViolationApprovalsDTO(this ViolationApprovalsDTO input) => input.WrapToViolationApprovalsDTO(new NWC_ViolationsApprovals());

        public static NWC_ViolationsApprovals WrapToViolationApprovalsDTO(this ViolationApprovalsDTO input, NWC_ViolationsApprovals entity)
        {
            if (input == null)
                return null;
            entity.ID = input.ID.HasValue ? input.ID.Value : 0;
            entity.LevelID = input.LevelNo;
            entity.IsDeleted = false;
            entity.LandmarkID = input.Landmark.Value;
            entity.StaffID = input.StaffId;
            entity.CreateTime = DateTimeHelper.GetDateTimeNow();

            return entity;
        }

        public static ViolationApprovalsDTO WrapDTOToViolationApprovalsDTO(this vw_NWC_ViolationsApprovals from)
        {
            
            if (from == null)
                return null;

            var to = new ViolationApprovalsDTO
            {
                ID = from.ID,
                StaffId = from.StaffID,
                LandmarkName = from.name,
                SubBranchName = from.FirstName+" "+from.MiddleName + " " + from.LastName,
                LevelNo=from.LevelID
            };

            return to;
        }

        public static AttachmentDTO WrapContractViolationAttachment(this NWC_ContractViolationAttachment from)
        {
            if (from == null)
                return null;

            var to = new AttachmentDTO();
            to.ID = from.ID;
            to.DocumentName = from.DocumentName;
            to.RelativePath = from.RelativePath;
            to.IsDeleted = from.IsDeleted;

            return to;
        }

        public static ContractTermsViolationsLogsDTO WrapToContractViolationsLogsDTO(this vw_NWC_ContractTermsViolationsLogs from)
        {
            if (from == null)
                return null;

            var to = new ContractTermsViolationsLogsDTO
            {
                Id = from.Id,
                TermViolationId = from.TermViolationId,
                ContractTermId = from.ContractTermId,
                ViolationTicketNumber = from.ViolationTicketNumber,
                ViolationLocation = from.ViolationLocation,
                IncidentTime = from.IncidentTime,
                TotalPenalty = from.TotalPenalty,
                IssueDate = from.IssueDate,
                PaymentDueDate = from.PaymentDueDate,
                PaymentStatusDate = from.PaymentStatusDate,
                ViolationDescription = from.ViolationDescription,
                IsDeleted = from.IsDeleted,
                ContractTermCode = from.ContractTermCode,
                ContractTermName = from.ContractTermName,
                ContractTermDescription = from.ContractTermDescription,
                ContractID = from.ContractID,
                ContractCode = from.ContractCode,
                CategoryId = from.CategoryId,
                CategoryAr = from.CategoryAr,
                CategoryEn = from.CategoryEn,
                StationID = from.StationID,
                StationName = from.StationName,
                PaymentStatusId = from.PaymentStatusId,
                PaymentStatusAr = from.PaymentStatusAr,
                PaymentStatusEn = from.PaymentStatusEn,
                VehicleId = from.VehicleId,
                VehicleCode = from.VehicleCode,
                VehiclePlateNo = from.VehiclePlateNo,
                DriverId = from.DriverId,
                DriverCode = from.DriverCode,
                DriverMobileNumber = from.DriverMobileNumber,
                TermUnitNoOfDays = from.TermUnitNoOfDays,
                AddVehicleToBlacklist = from.AddVehicleToBlacklist,
                StatusId = from.StatusId,
                StatusNameAr = from.StatusNameAr,
                StatusNameEn = from.StatusNameEn,
                CancelReasonId = from.CancelReasonId,
                CancelReasonAr = from.CancelReasonAr,
                CancelReasonEn = from.CancelReasonEn,
                AttachementsDocumentsIds = from.AttachementsDocumentsIds,
                AttachementsDocumentsNames = from.AttachementsDocumentsNames,
                CreatedDate = from.CreatedDate,
         
            };

            to.DriverName = $"{from.DriverFirstName ?? string.Empty} {from.DriverMiddleName ?? string.Empty} {from.DriverLastName ?? string.Empty}";
            to.CreatedByName = $"{from.CreatedByFirstName ?? string.Empty} {from.CreatedByMiddleName ?? string.Empty} {from.CreatedByLastName ?? string.Empty}";

            return to;
        }
        public static ContractTermsApprovalViolationsLogsDTO WrapToContractApprovalViolationsLogsDTO(this vw_NWC_ViolationsApprovalsLogs from)
        {
            if (from == null)
                return null;

            var to = new ContractTermsApprovalViolationsLogsDTO
            {
                Id = from.ID,
                ContractTermId = from.ContractTermId,
                ViolationTicketNumber = from.ViolationTicketNumber,
                ViolationLocation = from.ViolationLocation,
                IncidentTime = from.IncidentTime,
                TotalPenalty = from.TotalPenalty,
                IssueDate = from.IssueDate,
                ViolationDescription = from.ViolationDescription,
                IsDeleted = from.IsDeleted.Value,

                ContractTermCode = from.ContractTermCode,
                ContractTermName = from.ContractTermName,
                ContractTermDescription = from.ContractTermDescription,
                ContractID = from.ContractID,
                ContractCode = from.ContractTermCode,
                PaymentStatusId = from.PaymentStatusId,
                PaymentStatusAr = from.PaymentStatusAr,
                PaymentStatusEn = from.PaymentStatusEn,
                VehicleId = from.VehicleId,
                VehicleCode = from.VehicleCode,
                VehiclePlateNo = from.VehiclePlateNo,
                LevelID = from.LevelID,
                DecisionType = from.DecisionType == 1 ? "Approved" : "Rejected",
                CreatedDate = from.CreateTime
            };
            to.CreatedByName = $"{from.CreatedByFirstName ?? string.Empty} {from.CreatedByMiddleName ?? string.Empty} {from.CreatedByLastName ?? string.Empty}";
            return to;
        }

        public static ContractTermsViolationsInVoiceDTO WrapToContractViolationsInvoiceDTO(this vw_NWC_ContractTermsViolationsInvoices from)
        {
            if (from == null)
                return null;

            var to = new ContractTermsViolationsInVoiceDTO
            {
                Id = from.Id,
                ViolationId = from.ViolationId,
                InvoiceNo = from.InvoiceNo,
                ViolationNo = from.ViolationNo,
                ContractTermName = from.ContractTermName,
                StationID = from.StationID,
                StationName = from.StationName,
                VehicleId = from.VehicleId,
                VehicleCode = from.VehicleCode,
                VehiclePlateNo = from.VehiclePlateNo,
                DriverId = from.DriverId,
                DriverCode = from.DriverCode,
                DriverMobileNumber = from.DriverMobileNumber,
                VAT = from.VAT,
                Value = from.Value,
                ValueWithVAT = from.ValueWithVAT,
                CreatedDate = from.CreatedDate,
                CreatedBy = from.CreatedBy,
            };

            to.DriverName = $"{from.DriverFirstName ?? string.Empty} {from.DriverMiddleName ?? string.Empty} {from.DriverLastName ?? string.Empty}";
            to.CreatedByName = $"{from.CreatedByFirstName ?? string.Empty} {from.CreatedByMiddleName ?? string.Empty} {from.CreatedByLastName ?? string.Empty}";

            return to;
        }

        #endregion

        #region DTo ==> Domain
        public static NWC_ContractTermsViolations WrapToViolation(this ContractTermsViolationsDTO from, bool isApprovalRequired)
        {
            if (from == null)
                return null;

            var violation = new NWC_ContractTermsViolations
            {
                Id = from.Id,
                ContractTermId = from.ContractTermId,
                ViolationTicketNumber = from.ViolationTicketNumber,
                ViolationLocation = from.ViolationLocation,
                TotalPenalty = from.TotalPenalty,
                IncidentTime = from.IncidentTime,
                IssueDate = from.IssueDate,
                PaymentDueDate = from.PaymentDueDate,
                PaymentStatusId = from.PaymentStatusId,
                PaymentStatusDate = from.PaymentStatusDate,
                DriverId = from.DriverId,
                VehicleId = from.VehicleId,
                Description = from.ViolationDescription,
                TermUnitNoOfDays = from.TermUnitNoOfDays,
                AddVehicleToBlacklist = from.AddVehicleToBlacklist,
                StatusId = from.StatusId,
                CancelReasonId = from.CancelReasonId,
                IsFinalDecision = !isApprovalRequired,
                CurrentApprovalStatus = isApprovalRequired ? 1 : 0
            };

            return violation;
        }
        #endregion


        #endregion


        #region Helper
        private static bool LanguageIsEnglish
        {
            get
            {
                return Thread.CurrentThread.CurrentCulture.Name == LanguagesKeys.English;
            }
        }
        #endregion
    }
}



