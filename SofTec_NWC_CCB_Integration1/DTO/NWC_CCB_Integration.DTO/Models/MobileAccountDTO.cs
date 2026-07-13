using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NWC_CCB_Integration.DTO.Models
{
    public class MobileAccountDTO
    {
        public string Name { get; set; }
        public object OperatorId { get; set; }
        public Guid? SubId { get; set; }
        public Guid Id { get; set; }
        public bool IsConsumer { get; set; }
        public long? ParentId { get; set; }
        public Guid? StaffId { get; set; }
        public string Image { get; set; }
    }
}
