using NWC.DAL.NWCEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NWC.DTO.Models
{
    public class WorkOrderTransactionDTO
    {
        #region Properties
        public long ID { set; get; }
        public long WorkOrderID { set; get; }
        public DateTime? PaymentTime { set; get; }
        public Guid? CreatedBy { set; get; }
        public string CreatedByName { set; get; }
        public int PaymentTypeID { set; get; }
        public bool? IsDeleted { set; get; }
        public decimal TotalPaid { set; get; }
        public bool? IsPaid { set; get; }
        public string PaymentComment { set; get; }

        #endregion

        #region Constructors
        public WorkOrderTransactionDTO()
        {

        }
        #endregion
    }
}
