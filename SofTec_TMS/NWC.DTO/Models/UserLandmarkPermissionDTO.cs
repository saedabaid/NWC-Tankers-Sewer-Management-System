using NWC.DTO.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NWC.DTO.Models
{
    public class UserLandmarkPermissionDTO
    {
        public UserLandmarkPermissionDTO()
        {
            this.PermittedLandmarkIDs = new List<Guid>();
        }

        public Guid StaffID { get; set; }
        public string FullName { get; set; }
        public List<Guid> PermittedLandmarkIDs { get; set; }
        public List<string> DBPermittedLandmarks { get; set; }
        public List<int> PermittedServiceIDs { get; set; }
    }

}
