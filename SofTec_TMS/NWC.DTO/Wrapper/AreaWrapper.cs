using NWC.DAL.NWCEntities;
using NWC.DTO.Models.TMS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NWC.DTO.Wrapper
{
    public static class AreaWrapper
    {
        public static AreaDTO WarapTOAreaDTO(this Branch branch)
        {
            return new AreaDTO()
            {
                Id = branch.Id,
                name = branch.name,
                location = branch.address,
                description = branch.descr,
                telephone1 = branch.telephone,
                telephone2 = branch.telephone2,
                mobile = branch.mobile,
                fax = branch.fax,
                website = branch.website,
                parentBranchId = branch.parentBranchId,
            };
        }
    }
}
