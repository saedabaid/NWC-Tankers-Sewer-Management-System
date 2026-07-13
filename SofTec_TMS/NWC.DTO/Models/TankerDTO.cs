using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NWC.DTO.Models.ELM;

namespace NWC.DTO.Models
{
    public class TankerDTO
    {
        public string PlateNo { get; set; }
        public string transporterBrand { get; set; }     //transporterBrand
        public string transporterProductionYear { get; set; }    //transporterProductionYear
        public string licenseNo { get; set; }    //licenseNo
        public Nullable<System.DateTime> licenseExpiryDate { get; set; }    //licenseExpiryDate
        public Nullable<System.DateTime> entranceDate { get; set; }   //entranceDate
        public Nullable<double> TankCapacity { get; set; }  //TankCapacity
        public string plateNo_Numbers { get; set; }   //plateNo_Numbers
        public string plateNo_Characters { get; set; }   //plateNo_Characters
        public Nullable<long> TransporterColor { get; set; }  //TransporterColor   
        public List<ELMDriverDTO> Drivers { get; set; }
        public Nullable<Guid> permitNo { get; set; }
        public string PermitVersion { get; set; }
        public string PermitIssue { get; set; }
        public DateTime PermitExpiration { get; set; }
        public string Source { get; set; }
        public string TransactionId { get; set; }
            

    }
    public class permitNoDto
    {
        public Nullable<Guid> permitNo { get; set; }

    }


}
