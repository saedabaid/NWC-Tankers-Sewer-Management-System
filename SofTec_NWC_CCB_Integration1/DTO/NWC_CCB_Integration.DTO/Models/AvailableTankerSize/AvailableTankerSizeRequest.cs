using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace NWC_CCB_Integration.DTO.Models.AvailableTankerSize
{
    [ServiceContract]
    public class Schema
    {
        [DataMember]
        [Required]
        public Input Input { get; set; }

    }

    [ServiceContract]
    public class Input
    {
        [DataMember]
        [Required]
        public string LONGLAT { get; set; }

        [DataMember]
        [Required]
        public string SOURCEAPPLICATION { get; set; }

        [DataMember]
        [Required(AllowEmptyStrings = true)]
        public string CISDIVISION { get; set; }
        
        [DataMember]
        [Required(AllowEmptyStrings = true)]
        public string TRANSACTIONID { get; set; }

        [DataMember]
        [Required(AllowEmptyStrings = true)]
        public string TANKERTYPE { get; set; }
    }
}
