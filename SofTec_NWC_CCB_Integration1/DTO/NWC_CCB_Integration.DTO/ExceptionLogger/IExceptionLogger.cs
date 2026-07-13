using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NWC_CCB_Integration.DTO.ExceptionLogger
{
    public interface IExceptionLogger
    {
        void LogException(Exception e);
        void LogException(Exception e, params object[] parameters);
        void LogInformation(string information);
    }
}
