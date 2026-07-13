using Infrastructure;
using NWC.DTO.Common.ELM;
using NWC.DTO.Models;
using NWC.DTO.SearchCriteria;
using NWC.DTO.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace NWC.BLL.Interfaces.ELM
{
    public interface IELMUserService
    {
        DescriptiveResponse<ELMLoginDTO> AuthenticateUser(string userName, string password);

    }
}
