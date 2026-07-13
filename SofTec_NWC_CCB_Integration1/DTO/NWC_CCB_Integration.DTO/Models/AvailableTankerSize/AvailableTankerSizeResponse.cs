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
    public class Output
    {
        public Output()
        {
            TANKERSIZES = new List<TankerSize>();
        }

        [DataMember]
        public string SOURCEAPPLICATION { get; set; }

        [DataMember]
        public string TRANSACTIONID { get; set; }

        [DataMember]
        public List<TankerSize> TANKERSIZES { get; set; }

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

    [ServiceContract]
    public class TankerSize
    {
        public TankerSize()
        {
            TANKERACCESSORIZES = new List<TankerAccessorize>();
        }

        [DataMember]
        public int TANKERSIZEVALUE { get; set; }

        [DataMember]
        public decimal? TANKERPRICE { get; set; }

        [DataMember]
        [Editable(false)]
        [Display(Name = "CREDTTM")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy hh:mm:ss}", ApplyFormatInEditMode = true)]
        public DateTime TANKERDELIVERYDDTM { get; set; }

        [DataMember]
        public List<TankerAccessorize> TANKERACCESSORIZES { get; set; }
    }

    [ServiceContract]
    public class TankerAccessorize
    {
        [DataMember]
        public string ACCESSORIZEID { get; set; }

        [DataMember]
        public float ACCESSORIZEPRICE { get; set; }
    }
}
