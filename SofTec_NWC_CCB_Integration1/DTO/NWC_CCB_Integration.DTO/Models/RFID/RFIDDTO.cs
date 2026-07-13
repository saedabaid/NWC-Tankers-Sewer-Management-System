using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NWC_CCB_Integration.DTO.Models
{
public    class RFIDDTO
    {
        public long ID { get; set; }
        public Guid TransporterID { get; set; }
        public string RFID { get; set; }
    }
}
