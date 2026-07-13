using NWC.DAL.NWCEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NWC.DTO.Models.TMS
{
    
    public class AreaDTO
    {
        #region Properties
        public Guid Id { get; set; }
        public string name { get; set; }
        public string location { get; set; }
        public string description { get; set; }
        public string telephone1 { get; set; }
        public string telephone2 { get; set; }
        public string mobile { get; set; }
        public string fax { get; set; }
        public string website { get; set; }
        public string isLive { get; set; }
        public bool IsSubBranch { get; set; }

        public Nullable<Guid> parentBranchId { get; set; }
        #endregion

        #region Constructor
        public AreaDTO(Branch branch)
        {
            this.Id = branch.Id;
            this.name = branch.name;
            this.location = branch.address;
            this.description = branch.descr;
            this.telephone1 = branch.telephone;
            this.telephone2 = branch.telephone2;
            this.mobile = branch.mobile;
            this.fax = branch.fax;
            this.website = branch.website;
        }

        public AreaDTO()
        {
        }

        #endregion

        #region Helper
        public static AreaDTO MapToAreaDTO(Branch branch)
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
                IsSubBranch = branch.IsSubBranch,
                parentBranchId = branch.parentBranchId,
            };
        }

        public static Branch MapToArea(AreaDTO dto)
        {
            return new Branch()
            {
                Id = dto.Id,
                name = dto.name,
                address = dto.location,
                descr = dto.description,
                telephone = dto.telephone1,
                telephone2 = dto.telephone2,
                mobile = dto.mobile,
                fax = dto.fax,
                website = dto.website,
                IsSubBranch = dto.IsSubBranch,
                parentBranchId = dto.parentBranchId,
            };
        }

        public static Landmark MapToLandmark(AreaDTO dto)
        {
            return new Landmark()
            {
                Id = dto.Id,
                name = dto.name,
                address = dto.location,
                descr = dto.description,
                telephone = dto.telephone1,
                telephone2 = dto.telephone2,
                mobile = dto.mobile,
                fax = dto.fax,
                branchId = dto.parentBranchId,
            };
        }
        #endregion
    }
}
