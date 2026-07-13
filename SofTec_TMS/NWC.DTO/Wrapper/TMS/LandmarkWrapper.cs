using NWC.DAL.NWCEntities;
using NWC.DTO.Models.TMS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NWC.DTO.Wrapper.TMS
{
    public static class LandmarkWrapper
    {
        public static AreaDTO WrapToLandmarkDTO(this Landmark landmark)
        {
            return new AreaDTO()
            {
                Id = landmark.Id,
                name = landmark.name,
                location = landmark.address,
                description = landmark.descr,
                telephone1 = landmark.telephone,
                telephone2 = landmark.telephone2,
                mobile = landmark.mobile,
                fax = landmark.fax,
                parentBranchId = landmark.branchId,
            };
        }
    }
}
