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
        public Account Account { get; set; }
        [DataMember]
        [Required]
        public SoqyaService SoqyaService { get; set; }
        [DataMember]
        [Required]
        public Person Person { get; set; }
        [DataMember]
        [Required]
        public Premise Premise { get; set; }
    }

    [ServiceContract]
    public class Account
    {
        [DataMember]
        [Required]
        public string ACCOUNTID { get; set; }
        [DataMember]
        [Required]
        public string CUSTOMERCLASS { get; set; }
    }

    [ServiceContract]
    public class SoqyaService
    {
        [DataMember]
        [Required]
        [Editable(false)]
        [Display(Name = "STARTDATE")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy hh:mm:ss}", ApplyFormatInEditMode = true)]
        public string STARTDATE { get; set; }
        [DataMember]
        [Required]
        [Editable(false)]
        [Display(Name = "ENDDATE")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy hh:mm:ss}", ApplyFormatInEditMode = true)]
        public string ENDDATE { get; set; }
        [DataMember]
        [Required]
        public string ELIGIBLECOMSP { get; set; }
        [DataMember]
        [Required]
        public string NOTE { get; set; }
    }

    [ServiceContract]
    public class Person
    {
        [DataMember]
        [Required]
        public string PERSONID { get; set; }
        [DataMember]
        [Required]
        public string PERSONPRIMARYNAME { get; set; }
        [DataMember]
        [Required]
        public string PERSONIDTYPE { get; set; }
        [DataMember]
        [Required]
        public string PERSONIDVALUE { get; set; }
        [DataMember]
        [Required]
        public string MOBILENUMBER { get; set; }
    }

    [ServiceContract]
    public class Premise
    {
        [DataMember]
        [Required]
        public string PREMISID { get; set; }
        [DataMember]
        [Required]
        public string PROVINCE { get; set; }
        [DataMember]
        [Required]
        public string CITY { get; set; }
        [DataMember]
        [Required]
        public string CENTER { get; set; }
        [DataMember]
        [Required]
        public string VILLAGE { get; set; }
        [DataMember]
        [Required]
        public string XYCOORDINATESGF { get; set; }
    }
}
