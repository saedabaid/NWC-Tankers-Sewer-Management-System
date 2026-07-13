using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NWC.DTO.Models
{
    public class TankerAccessDTO
    {
        public string PlateNo { get; set; }
        public Guid permitNo { get; set; }     //transporterBrand
        public Guid TransactionId { get; set; }    //transporterProductionYear
        public DateTime RequestTime { get; set; }    //licenseNo
        public bool status { get; set; }    //transporterProductionYear
        public string Reason { get; set; }    //transporterProductionYear
        public string token { get; set; }    //transporterProductionYear
        public string Source { get; set; }
    }
}
