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
    public interface ICustomerSMSService
    {
        DescriptiveResponse<SearchResult<CustomerSMSDTO>> GetCustomerSMSs(CustomerSMSSearchCriteria searchCriteria);
        DescriptiveResponse<bool> UpdateSuccessCustomerSMS(long smsID);
        DescriptiveResponse<bool> UpdateFailCustomerSMS(long smsID);
    }
}
