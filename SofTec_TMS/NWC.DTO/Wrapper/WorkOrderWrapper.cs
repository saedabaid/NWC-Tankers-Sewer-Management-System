using NWC.DAL.NWCEntities;
using NWC.DTO.Constants;
using NWC.DTO.Helpers;
using NWC.DTO.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NWC.DTO.Wrapper
{
    public static class WorkOrderWrapper
    {
        public static WorkOrderDTO WrapToWorkOrderDTO(this vw_NWC_WorkOrderBasicDetails WOD)
        {
            var customCustomerMobile = WOD.CustomerMobile;

            if (!string.IsNullOrEmpty(WOD.CustomerMobile) && !WOD.CustomerMobile.StartsWith("0"))
                customCustomerMobile = $"0{WOD.CustomerMobile}";

            return new WorkOrderDTO()
            {
                WorkOrderID = WOD.OrderId,
                RequestTime = WOD.OrderPlacedOn,
                Status = LanguageIsEnglish ? WOD.StatusNameEn : WOD.StatusNameAr,
                IsAssigned = WOD.IsAssigned,
                orderPlacedOn = WOD.OrderPlacedOn,
                CustomerName = WOD.CustomerName,
                CustomerMobile = customCustomerMobile,
                CustomerCode = WOD.CustomerCode,
                CustomerAddress = WOD.CustomerAddress,
                TotalCost = WOD.TotalCost,
                OrderQuantity = WOD.OrderQuantity,
                ScheduledDeliveryTime = WOD.ScheduledDeliveryTime,
                StationName = WOD.StationName,
                ServiceType = LanguageIsEnglish ? WOD.ServiceTypeEn : WOD.ServiceTypeAr,
                TankerPlateNo = WOD.TankerPlateNo,
                DriverName = WOD.DriverName,
                AccessoryNames = LanguageIsEnglish ? WOD.AccessoriesEn : WOD.AccessoriesAr,
                AssignedDriverID = WOD.AssignedDriverID,
                AssignedStationID = WOD.AssignedStationID,
                AssignedVehicleID = WOD.AssignedVehicleID,
                CustomerLocationID = WOD.CustomerLocationId,
                ServiceTypeID = WOD.ServiceTypeId,
                CustomerID = WOD.CustomerId,
                transporterID = WOD.AssignedVehicleID,
                OrderNumber = WOD.OrderNumber,
                customerLocationLng = WOD.customerLocationLng.ToString(),
                customerLocationLat = WOD.customerLocationLat.ToString(),
                StationLat = WOD.StationLat.ToString(),
                StationLng = WOD.StationLng.ToString(),
                RouteLatLngString = WOD.RouteLatLngString,
                ZoneID = WOD.ZoneID,
                LastStatusID = WOD.LastStatusID,
                VehicleStatusName = LanguageIsEnglish ? WOD.VehicleStatusName : WOD.vehicleStatusNameAr,
                LastStatusTime = WOD.LastStatusTime,
                LastStatusReason = WOD.LastStatusReason,
                LastStatusTimeVehicle = WOD.LastStatusTimeVehicle,
                LastStatusName = LanguageIsEnglish ? WOD.StatusNameEn : WOD.StatusNameAr,
                VehicleCodePlateNo = WOD.VehicleCodePlateNo,
                VehicleCapacity = WOD.VehicleCapacity,
                VehicleCapacityUnit = WOD.VehicleCapacityUnit,
                WorkOrderInvoiceStatus = LanguageIsEnglish ? WOD.WorkOrderInvoiceStatusNameEn : WOD.WorkOrderInvoiceStatusNameAr,
                AreaName = WOD.Area,
                CityName = WOD.CityName,
                ZoneName = WOD.ZoneName,
                CostBeforVAT = WOD.CostBeforVAT, 
                VAT = WOD.VAT.HasValue ? WOD.VAT.Value : 0, 
                CostAfterVAT = WOD.CostAfterVAT, 
                PaymentStatusAr = WOD.PaymentStatusAr, 
                PaymentStatusEn = WOD.PaymentStatusEn,
                CustomerAccountID = WOD.CustomerAccountId,
                DriverMobileNumber = WOD.DriverMobileNumber,
                CategoryName = LanguageIsEnglish ? WOD.CategoryEn : WOD.CategoryAr, 
                ConfirmationCode = WOD.ConfirmationCode,
                CategoryID = WOD.CategoryID,
                //TODO : IsVirtualStation -- vw_NWC_WorkOrderBasicDetails
                IsVirtualStation = WOD.IsVirtualStation
            };
        }

        public static WorkOrderDTO WrapToOrderBasicDetailsDTO(this vw_NWC_WorkOrderList wo)
        {
            var customCustomerMobile = wo.CustomerMobile;

            if (!string.IsNullOrEmpty(wo.CustomerMobile) && !wo.CustomerMobile.StartsWith("0"))
                customCustomerMobile = $"0{wo.CustomerMobile}";

            var workOrderDto = new WorkOrderDTO()
            {
                WorkOrderID = wo.WorkOrderID,
                OrderNumber = wo.OrderNumber,
                RequestTime = wo.RequestTime,
                ScheduledDeliveryTime = wo.ScheduledDeliveryTime,
                OrderQuantity = wo.OrderQuantity,
                ServiceTypeID = wo.ServiceTypeID,
                CustomerLocationID = wo.CustomerLocationID,
                CustomerID = wo.CustomerID,
                CustomerName = wo.CustomerName,
                CustomerCode = wo.CustomerCode,
                CustomerLocationClassID = wo.ClassID,
                PriorityID = wo.PriorityID,
                ZoneID = wo.ZoneID,
                CityID = wo.CityID.HasValue ? wo.CityID.Value : Guid.Empty,
                CityName = wo.CityName,
                AssignedStationID = wo.AssignedStationID,
                LandmarkID = wo.LandmarkID,
                StationName = wo.StationName,
                AssignedVehicleID = wo.AssignedVehicleID,
                VehicleCodePlateNo = wo.VehicleCodePlateNo,
                AssignedDriverID = wo.AssignedDriverID,
                DriverName = wo.DriverName,
                LastStatusID = wo.LastStatusID,
                LastStatusBy = wo.LastStatusBy,
                LastStatusByUserName = wo.LastStatusByUserName,
                LastStatusTime = wo.LastStatusTime,
                LastStatusTimeVehicle = wo.LastStatusTimeVehicle,
                IsAssigned = wo.IsAssigned,
                VehicleCapacity = wo.vehicleCapacity,
                VehicleCapacityUnit = wo.vehicleCapacityUnit,
                ConfirmationCode = wo.ConfirmationCode,
                CustomerAddress = wo.CustomerAddress,
                LastStatusReason = wo.LastStatusReason,
                DriverCode = wo.DriverCode,
                TankerPlateNo = wo.TankerPlateNo,
                StatusColor = wo.StatusColor,
                customerLocationLat = wo.CustomerLocationLat.HasValue ? wo.CustomerLocationLat.Value.ToString() : string.Empty,
                customerLocationLng = wo.CustomerLocationLng.HasValue ? wo.CustomerLocationLng.Value.ToString() : string.Empty,
                CostBeforVAT = wo.CostBeforVAT,
                VAT = wo.VAT.HasValue ? wo.VAT.Value : 0,
                CostAfterVAT = wo.CostAfterVAT,
                PaymentStatusAr = wo.PaymentStatusAr,
                PaymentStatusEn = wo.PaymentStatusEn, 
                CustomerMobile = customCustomerMobile,
                //SourceApplication = wo.SourceApplication
                DriverMobileNumber = wo.DriverMobileNumber,
                TotalCost = wo.TotalCost,
                CustomerAccountID = wo.CustomerAccountId,
              CategoryID   =wo.CategoryID,
                CategoryName = LanguageIsEnglish ? wo.CategoryEn : wo.CategoryAr,

                //TODO : IsVirtualStation - vw_NWC_WorkOrderList
                IsVirtualStation = wo.IsVirtualStation
            };

            workOrderDto.ServiceType = LanguageIsEnglish ? wo.ServiceTypeEN : wo.ServiceTypeAR;
            workOrderDto.ClassName = LanguageIsEnglish ? wo.ClassNameEn : wo.ClassNameAr;
            workOrderDto.PriorityName = LanguageIsEnglish ? wo.PriorityNameEn : wo.PriorityNameAr;
            workOrderDto.ZoneName = wo.ZoneName;
            workOrderDto.LastStatusName = LanguageIsEnglish ? wo.LastStatusNameEn : wo.LastStatusNameAr;
            workOrderDto.VehicleStatusName = LanguageIsEnglish ? wo.vehiclestatusName : wo.vehiclestatusNameAr;
            workOrderDto.AccessoryNames = LanguageIsEnglish ? wo.AccessoriesEn : wo.AccessoriesAr;
            workOrderDto.SourceApplication = !string.IsNullOrEmpty(wo.SourceApplication) ? wo.SourceApplication : "TMS";
            workOrderDto.IsVirtualStation = wo.IsVirtualStation;

            return workOrderDto;
        }

        public static WorkOrderDTO WrapToOrderBasicDetailsDTO(this sp_NWC_GetAssignableWorkOrders_Result wo)
        {
            return new WorkOrderDTO()
            {
                WorkOrderID = wo.WorkOrderID,
                OrderNumber = wo.OrderNumber,
                RequestTime = wo.RequestTime,
                ScheduledDeliveryTime = wo.ScheduledDeliveryTime,
                OrderQuantity = wo.OrderQuantity,
                ServiceTypeID = wo.ServiceTypeID,
                CustomerLocationID = wo.CustomerLocationID,
                CustomerLocationClassID = wo.ClassID,
                AssignedStationID = wo.AssignedStationID,
                AssignedVehicleID = wo.AssignedVehicleID,
                AssignedDriverID = wo.AssignedDriverID,
                LastStatusID = wo.LastStatusID,
                LastStatusBy = wo.LastStatusBy,
                //LastStatusTime = wo.LastStatusTime,
                LastStatusTime = wo.LastStatusTime,
                // LastStatusTimeVehicle = wo.LastStatusTimeVehicle,
                CustomerAddress = wo.CustomerAddress,
                ZoneName = wo.ZoneNameEn,
                CustomerLocationPriority = LanguageIsEnglish ? wo.CustomerLocationPriorityNameEn : wo.CustomerLocationPriorityNameAr
            };
        }

        public static WorkOrderDTO WrapToOrderBasicDetailsDTO(this sp_NWC_WorkOrderList_Result wo)
        {
            var workOrderDto = new WorkOrderDTO()
            {
                WorkOrderID = wo.OrderId,
                OrderNumber = wo.OrderNumber,
                RequestTime = wo.RequestTime,
                ScheduledDeliveryTime = wo.ScheduledDeliveryTime,
                OrderQuantity = wo.OrderQuantity,
                ServiceTypeID = wo.ServiceTypeID,
                CustomerLocationID = wo.CustomerLocationID,
                CustomerID = wo.CustomerID,
                CustomerName = wo.CustomerName,
                CustomerCode = wo.CustomerCode,
                CustomerLocationClassID = wo.ClassID,
                PriorityID = wo.PriorityID,
                ZoneID = wo.ZoneID,
                CityID = wo.CityID,
                CityName = wo.CityName,
                AssignedStationID = wo.AssignedStationID,
                LandmarkID = wo.LandmarkID,
                StationName = wo.StationName,
                AssignedVehicleID = wo.AssignedVehicleID,
                VehicleCodePlateNo = wo.VehicleCodePlateNo,
                AssignedDriverID = wo.AssignedDriverID,
                DriverName = wo.DriverName,
                LastStatusID = wo.LastStatusID,
                LastStatusBy = wo.LastStatusBy,
                LastStatusByUserName = wo.LastStatusByUserName,
                // LastStatusTime = wo.LastStatusTime
            };

            workOrderDto.ServiceType = LanguageIsEnglish ? wo.ServiceTypeEN : wo.ServiceTypeAR;
            workOrderDto.ClassName = LanguageIsEnglish ? wo.ClassNameEn : wo.ClassNameAr;
            workOrderDto.PriorityName = LanguageIsEnglish ? wo.PriorityNameEn : wo.PriorityNameAr;
            workOrderDto.ZoneName = LanguageIsEnglish ? wo.ZoneNameEn : wo.ZoneNameAr;
            workOrderDto.LastStatusName = LanguageIsEnglish ? wo.LastStatusNameEn : wo.LastStatusNameAr;

            return workOrderDto;
        }

        public static WorkOrderCommentDTO WrapToCommentDTO(this NWC_WorkOrderComment wo)
        {
            return new WorkOrderCommentDTO()
            {
                ID = wo.ID,
                WorkOrderID = wo.WorkOrderID,
                Comment = wo.Comment,
                CreatedTime = wo.CreatedTime,
                CreatedBy = wo.CreatedBy,
                IsDeleted = wo.IsDeleted,
                CreatedByName = wo.Staff?.FirstName + ' ' + wo.Staff?.MiddleName + ' ' + wo.Staff?.LastName,
                Role = wo.Staff?.StaffRoleName
            };
        }

        public static WorkOrderComplaintDTO WrapToComplaintDTO(this NWC_WorkOrderComplaint complaint)
        {
            return new WorkOrderComplaintDTO()
            {
                WorkOrderID = complaint.WorkOrderID,
                ComplaintNumber = complaint.Number,
                RaisedBy = complaint.RaisedBy,
                RaisedByName = complaint.Staff?.FirstName + complaint.Staff?.LastName,
                RaisedTime = complaint.RaisedTime,
                Priority = LanguageIsEnglish ? complaint.NWC_ComplaintPriority.PriorityEn : complaint.NWC_ComplaintPriority.PriorityAr,
                Category = LanguageIsEnglish ? complaint.NWC_ComplaintCategory.CategoryEn : complaint.NWC_ComplaintCategory.CategoryAr,
                Status = LanguageIsEnglish ? complaint.NWC_ComplaintStatus.StatusEn : complaint.NWC_ComplaintStatus.StatusAr,
                Description = complaint.Description
            };
        }

        public static WorkOrderDTO WrapToStateWorkOrderDTO(this NWC_StateWorkOrder state)
        {
            return new WorkOrderDTO()
            {
                WorkOrderID = state.WorkOrderId,
                OrderNumber = state.OrderNumber,
                //CreateTime = state.CreateTime,
                //CreatedBy = state.CreatedBy,
                //LastModifiedTime = state.LastModifiedTime,
                //LastModifiedBy = state.LastModifiedBy,
                LastStatusID = state.LastStatusID,
                LastStatusBy = state.LastStatusBy,
                LastStatusTime = state.LastStatusTime,
                OrderQuantity = state.OrderQuantity,
                ScheduledDeliveryTime = state.ScheduledDeliveryTime,
                RequestTime = state.RequestTime,
                CustomerLocationID = state.CustomerLocationID,
                ServiceTypeID = state.NWC_CustomerAccount.ServiceTypeId,
                TotalCost = state.TotalCost,
                //IsPaid = state.IsPaid,
                //PaymentTime = state.PaymentTime,
                //PaymentTypeID = state.PaymentTypeID,
                //PaymentComment = state.PaymentComment,
                CustomerLocationClassID = state.NWC_CustomerAccount.NWC_CustomerLocation.ClassID,
                IsAssigned = state.IsAssigned,
                AssignedVehicleID = state.AssignedVehicleID,
                AssignedDriverID = state.AssignedDriverID,
                AssignedStationID = state.AssignedStationID,
                ZoneID = state.NWC_CustomerAccount.NWC_CustomerLocation.ZoneID,
                CategoryID = state.CategoryID

                //Accessories = state.Accessories,
                //ConfirmationCode = state.ConfirmationCode,
                //EncryptedConfirmationCode = state.EncryptedConfirmationCode,
                //IsDeleted = state.IsDeleted
            };
        }

        public static AccessoryDTO WrapToAccessoryDTO(this NWC_Accessory input)
        {
            return new AccessoryDTO()
            {
                ID = input.ID,
                Code = input.Code,
                Name = LanguageIsEnglish ? input.NameEn : input.NameAr,
                // NameEn = input.NameEn
            };
        }

        public static AccessoryDTO WrapToAccessoryDTO(this NWC_WorkOrderAccessory input)
        {
            return new AccessoryDTO()
            {
                ID = input.AccessoryID,
                Code = input.NWC_Accessory.Code,
                Name = LanguageIsEnglish ? input.NWC_Accessory.NameEn : input.NWC_Accessory.NameAr,
                // NameEn = input.NWC_Accessory.NameEn
            };
        }

        public static WorkOrderTransactionDTO WrapToWorkOrderTransactionDTO(this NWC_WorkOrderTransaction input)
        {
            return new WorkOrderTransactionDTO()
            {
                ID = input.ID,
                WorkOrderID = input.ID,
                PaymentTime = input.TransactionDate,
                TotalPaid = input.Amount,
                CreatedBy = input.CreatedBy,
                IsDeleted = input.IsDeleted
            };
        }

        public static WorkOrderStatusLogDTO WrapToWorkOrderStatusLogDTO(this vw_NWC_WorkOrderLogs input)
        {
            return new WorkOrderStatusLogDTO()
            {
                WorkOrderID = input.WorkOrderId,
                ChangedTime = input.CreateTime.HasValue ? input.CreateTime.Value : DateTimeHelper.GetDateTimeNow(),
                ChangedBy = input.CreatedBy.HasValue ? input.CreatedBy.Value : Guid.Empty,
                Status = LanguageIsEnglish ? input.WOStatusEn : input.WOStatusAr,
                StatusReason = LanguageIsEnglish ? input.ReasonEn : input.ReasonAr,
                ChangedByName = input.ChangedByName,
                ActionLogTypeID = input.ActionLogTypeID
            };
        }

        public static WorkOrderChangeLogDTO WrapToWorkOrderChangeLogDTO(this vw_NWC_WorkOrderLogs input)
        {
            var to = new WorkOrderChangeLogDTO();

            //to.ID = input.ActionLogTypeID;
            to.WorkOrderId = input.WorkOrderId;
            to.ActionLogTypeID = input.ActionLogTypeID;
            //to.EventOrderId = input.EventOrderID;
            //to.ParentWorkOrderId = input.ParentWorkOrderID;
            //to.EventId = input.EventID;
            to.CreatedBy = input.CreatedBy;
            to.CreatedByName = input.ChangedByName;
            to.CreateTime = input.CreateTime;
            //to.EventTypeID = input.EventTypeID;
            to.StatusID = input.StatusID;
            to.StatusName = LanguageIsEnglish ? input.WOStatusEn : input.WOStatusAr;
            to.StatusComment = input.StatusComment;
            to.DeassignReasonID = input.DeassignReasonID;
            //to.DeassignReasonName = LanguageIsEnglish ? input.ReasonEn : input.ReasonAr;
            to.OrderQuantity = input.OrderQuantity;
            to.CustomerLocationID = input.CustomerLocationID;
            to.ServiceTypeID = input.ServiceTypeID;
            to.NetCost = input.NetCost;
            to.TotalCost = input.TotalCost;
            to.Accessories = LanguageIsEnglish ? input.AccessoriesEn : input.AccessoriesAr;
            to.Distance = input.Distance;
            to.VehicleID = input.VehicleID;
            to.DriverID = input.DriverID;
            to.VehicleStatusId = input.VehicleStatusID;
            to.ActionLogTypeName = LanguageIsEnglish ? input.ActionLogTypeEn : input.ActionLogTypeAr;
            to.VehicleStatusName = LanguageIsEnglish ? input.VehicleStatusEn : input.VehicleStatusAr;
            to.ServiceTypeName = LanguageIsEnglish ? input.ServiceTypeEn : input.ServiceTypeAr;
            to.VehicleCodePlateNo = input.VehicleCodePlateNo;
            to.DriverName = input.DriverName;
            //to.OrderNumber = input.OrderNumber;
            to.ScheduledDeliveryTime = input.ScheduledDeliveryTime;
            to.StatusTime = input.StatusTime;
            //to.IsPaid = input.IsPaid;
            //to.CommentId = input.CommentId;
            //to.Comment = input.Comment;
            //to.CommentIsDeleted = input.CommentIsDeleted;
            to.StationName = input.StationName;
            to.CustomerAddress = input.CustomerAddress;
            to.ZoneName = input.ZoneName;
            to.CityName = input.cityName;
            to.DeassignReasonName = LanguageIsEnglish ? input.WOStatusReasonEn : input.WOStatusReasonAr;


            return to;
        }

        public static DailyOrderSummaryDTO WrapToDailyOrderSummaryDTO(this vw_NWC_Report_DailyOrderSummary wo)
        {
            var workOrderDto = new DailyOrderSummaryDTO()
            {
                ServiceTypeID = wo.ServiceTypeID,
                StationName = wo.StationName,
                StationID = wo.StationID,
                StationCode = wo.StationCode,

                TotalCount = wo.TotalCount,
                TotalSum = wo.TotalSum,
                FailedToDeliverCount = wo.FailedToDeliverCount,
                FailedToDeliverSum = wo.FailedToDeliverSum,
                DeliveredCount = wo.DeliveredCount,
                DeliveredSum = wo.DeliveredSum,
                CancelledCount = wo.CancelledCount,
                CancelledSum = wo.CancelledSum,
                CreateDate = wo.CreateDate
            };
            workOrderDto.ServiceTypeName = LanguageIsEnglish ? wo.ServiceTypeEN : wo.ServiceTypeAR;

            return workOrderDto;
        }

        public static WorkOrderDTO WrapToOrderBasicDetailsDTO(this vw_NWC_Report_DailyOrderDetails wo)
        {
            var workOrderDto = new WorkOrderDTO()
            {
                //Area name(of customer location) ////
                WorkOrderID = wo.WorkOrderID,
                OrderNumber = wo.OrderNumber,
                RequestTime = wo.RequestTime,
                ScheduledDeliveryTime = wo.ScheduledDeliveryTime,
                OrderQuantity = wo.OrderQuantity,
                ServiceTypeID = wo.ServiceTypeID,
                CustomerLocationID = wo.CustomerLocationID,
                CustomerID = wo.CustomerID,
                CustomerName = wo.CustomerName,
                CustomerCode = wo.CustomerCode,
                CustomerLocationClassID = wo.ClassID,
                PriorityID = wo.PriorityID,
                ZoneID = wo.ZoneID,
                CityID = wo.CityID,
                CityName = wo.CityName,
                AssignedStationID = wo.AssignedStationID,
                LandmarkID = wo.AssignedStationID,
                StationName = wo.StationName,
                AssignedVehicleID = wo.VehicleID,
                //VehicleCodePlateNo = $"{wo.TankerCode} | {wo.TankerPlateNo}" ,
                AssignedDriverID = wo.DriverID,
                DriverName = wo.DriverName,
                LastStatusID = wo.LastStatusID,
                LastStatusBy = wo.LastStatusBy,
                //LastStatusByUserName = wo.LastStatusByUserName,
                LastStatusTime = wo.LastStatusTime,
                //LastStatusTimeVehicle = wo.LastStatusTimeVehicle,
                IsAssigned = wo.IsAssigned,
                //VehicleCapacity = wo.vehicleCapacity,
                //VehicleCapacityUnit = wo.vehicleCapacityUnit,
                //ConfirmationCode = wo.ConfirmationCode,
                CustomerAddress = wo.CustomerAddress,
                LastStatusReason = wo.LastStatusReason,
                DriverCode = wo.DriverCode,
                TankerPlateNo = wo.TankerPlateNo,
                StatusColor = wo.StatusColor,
                NetCost = wo.NetCost,
                AssignedOn = wo.AssignedOn,
                OutForDeliveryOn = wo.OutForDeliveryOn,
                ArrivedOn = wo.ArrivedOn,
                DeliverOn = wo.DeliverOn,
                FailedToDeliverOn = wo.FailedToDeliverOn,
                CancelledOn = wo.CancelledOn,
                InvoiceStatusID = wo.InvoiceStatusID,
                TotalCost = wo.TotalCost,
                CreatedOn = wo.CreateTime,
                //SourceApplication = wo.SourceApplication
                LastStatusByCategory = wo.LastStatusByCategory,
                InvoiceNo = wo.InvoiceNo,
                CategoryName = LanguageIsEnglish ? wo.CategoryEn : wo.CategoryAr,
                Comments = wo.Comments,
                ClosedByCode = wo.ClosedByCode == null ? "unknown" : wo.ClosedByCode ==false ? "WithoutCode" : "WithCode"

            };

            workOrderDto.ServiceType = LanguageIsEnglish ? wo.ServiceTypeEN : wo.ServiceTypeAR;
            workOrderDto.ClassName = LanguageIsEnglish ? wo.CustomerClassNameEn : wo.CustomerClassNameAr;
            workOrderDto.PriorityName = LanguageIsEnglish ? wo.PriorityNameEn : wo.PriorityNameAr;
            workOrderDto.ZoneName = wo.ZoneName;
            workOrderDto.ZoneWithOutTankers = wo.ZoneWithoutTanker != null || wo.ZoneWithoutTanker == true ? "yes" : "no";
            workOrderDto.LastStatusName = LanguageIsEnglish ? wo.LastStatusNameEn : wo.LastStatusNameAr;
            //workOrderDto.VehicleStatusName = LanguageIsEnglish ? wo.vehiclestatusName : wo.vehiclestatusNameAr;
            workOrderDto.AccessoryNames = LanguageIsEnglish ? wo.AccessoriesEn : wo.AccessoriesAr;
            workOrderDto.InvoiceStatusName = LanguageIsEnglish ? wo.InvoiceStatusEn : wo.InvoiceStatusAr;

            workOrderDto.VehicleCodePlateNo = !string.IsNullOrEmpty(wo.TankerCode) ? $"{wo.TankerCode} | {wo.TankerPlateNo}" : wo.TankerPlateNo;

            if (workOrderDto.DeliverOn.HasValue && workOrderDto.CreatedOn.HasValue)
            {
                var interval = workOrderDto.DeliverOn - workOrderDto.CreatedOn;
                workOrderDto.CreateToDeliveredTime = interval.HasValue
                                ? ((interval.Value.Days * 24) + interval.Value.Hours).ToString()
                                + interval.Value.ToString(@"\:mm\:ss") //$"{(interval.Value.Days * 24) + interval.Value.Hours}:{interval.Value.Minutes}:{interval.Value.Seconds}"
                                : string.Empty;
            }
            if (workOrderDto.OutForDeliveryOn.HasValue && workOrderDto.DeliverOn.HasValue)
            {
                var interval = workOrderDto.DeliverOn - workOrderDto.OutForDeliveryOn;
                workOrderDto.OutToDeliveredTime = interval.HasValue
                                ? ((interval.Value.Days * 24) + interval.Value.Hours).ToString()
                                + interval.Value.ToString(@"\:mm\:ss") //$"{(interval.Value.Days * 24) + interval.Value.Hours}:{interval.Value.Minutes}:{interval.Value.Seconds}"
                                : string.Empty;
            }
            if (workOrderDto.CreatedOn.HasValue && workOrderDto.OutForDeliveryOn.HasValue)
            {
                var interval = workOrderDto.OutForDeliveryOn - workOrderDto.CreatedOn;
                workOrderDto.CreateToOutTime = interval.HasValue
                                ? ((interval.Value.Days * 24) + interval.Value.Hours).ToString()
                                + interval.Value.ToString(@"\:mm\:ss") //$"{(interval.Value.Days * 24) + interval.Value.Hours}:{interval.Value.Minutes}:{interval.Value.Seconds}"
                                : string.Empty;
            }
            workOrderDto.SourceApplication = !string.IsNullOrEmpty(wo.SourceApplication) ? wo.SourceApplication : "TMS";

            return workOrderDto;
        }


        public static DeferredOrderDTO WrapToOrderDeferredWorkOrderDTO(this NWC_DeferredWorkOrder wo)
        {
            if (wo == null) return null;

            var dto = new DeferredOrderDTO
            {
                ID = wo.ID,
                ORDERNUMBER = wo.ORDERNUMBER,
                CISDIVISION = wo.CISDIVISION,
                COMMENT = wo.COMMENT,
                CONFIRMATIONCODE = wo.CONFIRMATIONCODE,
                CONTACTMOBILE = wo.CONTACTMOBILE,
                CONTACTNAME = wo.CONTACTNAME,
                CREDTTM = wo.CREDTTM,
                SCHEDDTTM = wo.SCHEDDTTM,
                SERVICETYPE = wo.SERVICETYPE,
                SOURCEAPPLICATION = wo.SOURCEAPPLICATION,
                TANKERSIZE = wo.TANKERSIZE,
                TRANSACTIONID = wo.TRANSACTIONID,
                ACCOUNTID = wo.ACCOUNTID,
                CUSTOMERCLASS = wo.CUSTOMERCLASS,
                MOBILENUMBER = wo.MOBILENUMBER,
                PERSONID = wo.PERSONID,
                PERSONIDTYPE = wo.PERSONIDTYPE,
                PERSONIDVALUE = wo.PERSONIDVALUE,
                PERSONPRIMARYNAME = wo.PERSONIDVALUE,
                PREMISEID = wo.PREMISEID,
                XYCOORDINATESGF = wo.XYCOORDINATESGF,
                StatusId = wo.StatusId,
                ErrorMSG = wo.ErrorMSG,
                CreateTime = wo.CreateTime,
                LastUpdatedTime = wo.LastUpdatedTime,
                StatusName = wo.NWC_DeferredWorkOrderStatus.Name

            };

            return dto;
        }

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


