using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NWC_CCB_Integration.DTO.Models
{
    public class ZoneDTO
    {
        #region Properties
        public long ID { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public bool? IsDeleted { get; set; }
        public Guid? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public Guid? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid? DeletedBy { get; set; }
        public DateTime? DeletedDate { get; set; }
        public Guid CityID { get; set; }
        public Guid? AreaID { get; set; }
        #endregion
    }
}
