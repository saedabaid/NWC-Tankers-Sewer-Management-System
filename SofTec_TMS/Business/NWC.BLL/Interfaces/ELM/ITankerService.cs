using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NWC.DAL.NWCEntities;
using NWC.DTO.Common.ELM;
using NWC.DTO.Models;
using NWC.DTO.Models.ELM;

namespace NWC.BLL.Interfaces.ELM
{
    public interface ITankerService
    {
        DescriptiveResponse<string> Add(TankerDTO TankerDTO);
        DescriptiveResponse<string> SaveTankerToTransporter(Guid SubId,int? retrials);
        DescriptiveResponse<ELMTransactionDTO> TankerAccessAuthorization(TankerAccessDTO TankerDTO);
        DescriptiveResponse<ELMTransactionDTO> TankerCheckStatus(string baseurl,TankerAccessDTO TankerDTO);

  
    }
}
