using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace NWC_CCB_Integration.DTO.Models.UpdateWorkOrder
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
        public Input()
        {
            PARAMETERS = new List<Parameter>();
        }

        [DataMember]
        [Required]
        public string ORDERNUMBER { get; set; } //100

        [DataMember]
        public string ORDERSTATUS { get; set; }

        [DataMember]
        [Required]
        [Editable(false)]
        [Display(Name = "UPDATEDTTM")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy hh:mm:ss}", ApplyFormatInEditMode = true)]
        public string UPDATEDTTM { get; set; }

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
        public List<Parameter> PARAMETERS { get; set; }
    }

    [ServiceContract]
    public class Parameter
    {
        [DataMember]
        [Required]
        public string PARAMETERNAME { get; set; }

        [DataMember]
        [Required]
        public string PARAMETERVALUE { get; set; }
    }
}
