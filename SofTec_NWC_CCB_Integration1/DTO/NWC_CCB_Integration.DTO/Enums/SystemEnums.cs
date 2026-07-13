using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NWC_CCB_Integration.DTO.Enums
{
    public enum LoginStatus
    {
        Success = 1,
        InvalidUsername = 2,
        InvalidPassword = 3,
        LockedOut = 4,
        InvalidUserOrPass = 5
    }

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
        /// ELM
        WrongPassword = 523,
        WrongUsername = 521,
        blocked = 524,
        EM_UNAUTHORIZED = 528,
        ELM_NoToken=526,
        Bad_Request= 400,
        MissingRequiredDate = 537,
        Existsbefore = 531,
        Invaliddatatype = 539,
        expired_Token= 528,
        NotFound = 538,
        DeletedUser = 522,
        language_Not_Supported = 529,
        Empty_List = 541

    }

    public enum OrderRequestStatuEnum
    {
        Pending = 1,
        Success = 2,
        Fail = 3
    }

    public enum OperationTypeEnum
    {
        CreateWorkOrder = 1,
        UpdateWorkOrder = 2,
        UpdateWorkOrderInfo = 3,
        ChangeWorkOrderStatus = 4,
        AddComment = 5
    }

    public enum OperationStepEnum
    {
        Criteria = 1,
        ParsingXML = 2,
        DTOMapping = 3,
        GettingZoneByIntegrationID = 4,
        BeforeCreateCustomer = 5,
        AfterCreateCustomer = 6,
        BeforeCreateCustomerLocation = 7,
        AfterCreateCustomerLocation = 8,
        CallingGIS = 9,
        GettingMainZoneStation = 10,
        BeforeCreateWorkOrder = 11,
        AfterCreateWorkOrder = 12,
        CreateWorkOrderSuccess = 13,
        CreateWorkOrderFail = 14,
        CreateWorkOrderException = 15,
        UpdateWONotificationService = 16,
        ChangeWorkOrderStatus = 17,
        NoCoverage = 18,
        GettingAvailableTankers = 19,
        Deserialize_WorkOrderRequestDTO = 20,
        GettingTankersSizesCount = 21,
        BeforeCreateCustomerAccount = 22,
        AfterCreateCustomerAccount = 23
    }

    public enum ResponseStatusEnum
    {
        OK = 1,
        KO = 2,
        FAILED = 3
    }
}
