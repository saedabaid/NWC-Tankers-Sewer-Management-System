using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NWC.DTO.Enums
{
    public enum ErrorStatus
    {
        CUSTOM = -1,
        UNEXPECTED_ERROR,
        NOT_FOUNT,
        INPUT_IS_NULL,
        INPUT_INVALID,
        INTERNAL_ERROR,
        COMMIT_FAIL,
        NO_ROWS_AFFECTED,
        UNAUTHORIZED,
        ALREADY_EXIST,
        AlREADY_INUSE,
        //User Error Messages 
        EMAIL_ALREADY_EXIST,
        MOBILE_ALREADY_EXIST,
        USER_ALREADY_EXIST,
        //Forgot Password
        EMAIL_NOT_FOUND,
        InvalidUsername = 2,
        InvalidPassword = 3,
        LockedOut = 4,
        InvalidUserOrPass = 5,
        NotFound = 538,
        /// <summary>
        /// ELM
        WrongUsername = 521,
        WrongPassword = 523,
        blocked = 524,
        Bad_Request = 400,
        MissingRequiredDate = 537,
        Existsbefore = 531,
        Invaliddatatype = 539,
        DeletedUser = 522,
        language_Not_Supported = 529,
        Empty_List = 541,
        LevelRepeated = 542,
        PreviousLevel = 543,

    }

    public enum LoginStatus
    {
        Success = 1,
        InvalidUsername = 2,
        InvalidPassword = 3,
        LockedOut = 4,
        InvalidUserOrPass = 5
    }

    public enum EventTypeEnum
    {
        WorkOrder_Create = 1,
        WorkOrder_Update = 2,
        WorkOrder_UpdatePaymentStatus = 5,
        WO_Vehicle_Assign = 6,
        WO_Vehicle_Deassign = 7,
        WorkOrder_OutForDelivery = 16,
        WorkOrder_Arrived = 17,
        WorkOrder_Delivered = 18,
        WorkOrder_Cancelled = 19,
        WorkOrder_FailedToDeliver = 20,
        WorkOrder_OnHold = 21,
        WorkOrder_NotAssigned = 22,
        WO_Vehicle_ArrivedToStation = 23,
        WO_AddComment = 24,
        WO_AddAttachment = 25,
        WO_DeleteAttachment = 26,
        WO_AddComplaint = 27,
        WO_UpdateComplaint = 28,
        WorkOrder_PreAssign = 29,
        SW_Assign = 30,
        SW_Confirm = 31,
        Sw_Arrived = 32,
        SW_Complete = 32,
        SW_Vehicle_GoToStation = 33,
        SW_Vehicle_Dumping = 34,
        SW_Cancelled = 35,
    }

    public enum ActionLogTypeEnum
    {
        WorkOrder_Create = 1,
        WorkOrder_Update = 2,
        WorkOrder_Assign = 3,
        WorkOrder_Deassign = 4,
        WorkOrder_OutForDelivery = 5,
        WorkOrder_Arrived = 6,
        WorkOrder_Delivered = 7,
        WorkOrder_Cancelled = 8,
        WorkOrder_FailedToDeliver = 9,
        WorkOrder_OnHold = 10,
        WorkOrder_NotAssigned = 11,
        WO_Vehicle_ArrivedToStation = 12,

        SW_Confirmed = 13,
        SW_Completed = 14,



    }

    public enum WorkOrderStatusEnum
    {
        New = 1,
        Onhold = 2,
        Failed_To_Deliver = 3,
        Delivered = 4,
        Assigned = 5,
        Out_For_Delivery = 6,
        Arrived = 7,
        Cancelled = 8,
        PreAssign = 9,
        Confirmed = 10,
        Completed = 11
    }

    public enum VehicleStatusEnum
    {
        Available = 0,
        UnderMaintenance = 2,
        OutOfService = 3,
        InService = 6,
        Assigned = 11,
        Blacklisted = 12,
        Parking = 13,
        ArrivedToCustomer = 14,
        Delivered = 15,
        Dumping = 20,
    }


    public enum VehicleActionLogTypeEnum
    {
        Entry = 1,
        Exist = 2
    }

    public enum ContractStatusEnum
    {
        New = 1,
        Active = 2,
        Finished = 3,
        Terminated = 4
    }
    public enum PermitStatusEnum
    {
        Active = 1,
        Expired = 2,
        Hold = 3
    }
    public enum StationStatusEnum
    {
        InService = 1,
        OutOfService = 2,
        NotApproved = 3
    }

    public enum PaymentTypeEnum
    {
        Potable_Water_Delivery = 1,
        Non_Potable_Water_Delivery = 2,
        Sewage_Removal = 3
    }
    public enum TransporterSource
    {
        NWC = 1,
        ELM = 2
    }
    public enum WorkOrderInvoiceStatusEnum
    {
        Paid = 1,
        Un_Paid = 2,
        Partially_Paid = 3
    }

    public enum StatusReasonEnum
    {
        FailReason_1 = 1,
        FailReason_2 = 2,
        CancelReason_1 = 3,
        CancelReason_2 = 4,
        CancelReason_3 = 9
    }

    public enum SewerOrderType
    {
        Chargerd = 11,
        Free = 12,

    }

    public enum ServiceTypeEnum
    {
        Ashyab = 1,
        Soqya = 2,
        SewageRemoval = 3
    }

    public enum HayatWorkOrderLogStatusEnum
    {
        Pending = 1,
        Success = 2,
        Failed = 3
    }

}
