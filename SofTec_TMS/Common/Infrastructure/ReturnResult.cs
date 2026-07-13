using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure
{
    public class ReturnResult
    {
        public ReturnResultCode ResultCode { get; set; }

        public string ResultMessage { get; set; }
    }
    public enum ReturnResultCode
    {
        NullOrEmptyId = 1,
        NullOrEmptyUserName = 2,
        NullOrEmptyRequiredValue = 3,
        Exception = 4,
        AlreadyExists = 5,
        NotExist = 6,
        WcfServiceException = 7,
        NoResultFound = 8,
        Success = 9,
        AlreadyExistsButDeleted = 10, 
        FailedWhileSaving = 11,
        InvalidPassword = 12
    }
}
