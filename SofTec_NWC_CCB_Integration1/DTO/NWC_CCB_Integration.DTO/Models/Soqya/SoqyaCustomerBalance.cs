using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace NWC_CCB_Integration.DTO.Models.Soqya
{
    [ServiceContract]
    public class Output
    {
        [DataMember]
        public string STATUS { get; set; }

        [DataMember]
        public string RESPONSECODE { get; set; }

        [DataMember]
        public string RESPONSEDESCRIPTION { get; set; }

        [DataMember]
        public string ERRORCODE { get; set; }

        [DataMember]
        public string ERRORDESCRIPTION { get; set; }

        [DataMember]
        [Required]
        public string COVERAGESTATUS { get; set; }
    }
}
