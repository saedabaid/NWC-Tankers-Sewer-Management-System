using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NWC.DTO.Models
{
    public class ContractTermDTO
    {
        public long ID { set; get; }
        public String ContractTermName { set; get; }
       public String ContractTermCode { set; get; }
       public String Description { set; get; }
       public long TermsCategoryID { set; get; }
       public List<Guid?> StationIDs { set; get; }
       public long ContractID { set; get; }
        public int index { set; get; }
        public Nullable<decimal> TotalValue { get; set; }
        public Nullable<int> TotalValueUnitId { get; set; }

    }

    public class ViolationApproveReject
    {
        public long violationId { get; set; }
        public bool Approval { get; set; }
    }
}
