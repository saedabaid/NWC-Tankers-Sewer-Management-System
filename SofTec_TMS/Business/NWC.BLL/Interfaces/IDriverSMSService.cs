using Infrastructure;
using NWC.DTO.Common;
using NWC.DTO.Models;
using NWC.DTO.SearchCriteria;
using NWC.DTO.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NWC.BLL.Interfaces
{
    public interface IDriverSMSService
    {
        DescriptiveResponse<SearchResult<DriverSMSDTO>> GetDriverSMSs(DriverSMSSearchCriteria searchCriteria);
        DescriptiveResponse<bool> UpdateSuccessDriverSMS(long smsID);
        DescriptiveResponse<bool> UpdateFailDriverSMS(long smsID);
    }
}
