using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace NWC_CCB_Integration.DTO.Models.CreateWorkOrder
{
    [ServiceContract]
    public class Schema
    {
        [DataMember]
        [Required]
        public Input Input { get; set; }

        public override string ToString()
        {
            return $"schema:{{Input: {this.Input}}}";
        }

    }

    [ServiceContract]
    public class Input
    {
        [DataMember]
        [Required]
        public Order Order { get; set; }

        [DataMember]
        [Required]
        public Account Account { get; set; }

        [DataMember]
        [Required]
        public Person Person { get; set; }


        [DataMember]
        [Required]
        //free or charge
        public string ServiceChargeType { get; set; }

        [DataMember]
        [Required]
        public Premise Premise { get; set; }

        public override string ToString()
        {
            return $"{{Order: {this.Order}, Account: {this.Account}, Person: {this.Person}, Premise: {this.Premise} }}";
        }
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

        public override string ToString()
        {
            return $"{{ACCOUNTID: {this.ACCOUNTID}, CUSTOMERCLASS: {this.CUSTOMERCLASS} }}";
        }
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
        public string MOBILENUMBER { get; set; }

        public override string ToString()
        {
            return $"{{PERSONID : {this.PERSONID }, PERSONPRIMARYNAME: {this.PERSONPRIMARYNAME}, PERSONIDTYPE : {this.PERSONIDTYPE }, PERSONIDVALUE: {this.PERSONIDVALUE}, MOBILENUMBER: {this.MOBILENUMBER} }}";
        }
    }

    [ServiceContract]
    public class Premise
    {
        [DataMember]
        [Required]
        public string PREMISEID { get; set; }

        [DataMember]
        [Required]
        public string XYCOORDINATESGF { get; set; }

        public override string ToString()
        {
            return $"{{PREMISEID: {this.PREMISEID}, XYCOORDINATESGF: {this.XYCOORDINATESGF}}}";
        }

    }

    [ServiceContract]
    public class Order
    {
        public Order()
        {
            TANKERACCESSORIES = new List<string>();
        }

        [DataMember]
        [Required]
        public string ORDERNUMBER { get; set; }

        [DataMember]
        [Required]
        [Editable(false)]
        [Display(Name = "CREDTTM")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy hh:mm:ss}", ApplyFormatInEditMode = true)]
        public string CREDTTM { get; set; }

        [DataMember]
        [Required]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy hh:mm:ss}", ApplyFormatInEditMode = true)]
        public string SCHEDDTTM { get; set; }

        [DataMember]
        [Required]
        public string SERVICETYPE { get; set; }

        [DataMember]
        [Required]
        public string CONTACTNAME { get; set; }

        [DataMember]
        public string CONTACTMOBILE { get; set; }

        [DataMember]
        [Required]
        public int TANKERSIZE { get; set; }

        [DataMember]
        public string COMMENT { get; set; }

        [DataMember]
        [Required]
        public string CONFIRMATIONCODE { get; set; }

        [DataMember]
        public List<string> TANKERACCESSORIES { get; set; }
        
        [DataMember]
        [Required]
        public string SOURCEAPPLICATION { get; set; }

        [DataMember]
        public string CISDIVISION { get; set; }

        [DataMember]
        [Required(AllowEmptyStrings = true)]
        public string TRANSACTIONID { get; set; }

        public override string ToString()
        {
            return $"{{ORDERNUMBER : {this.ORDERNUMBER }, CREDTTM: {this.CREDTTM}, SCHEDDTTM: {this.SCHEDDTTM }, SERVICETYPE: {this.SERVICETYPE}, CONTACTNAME: {this.CONTACTNAME}, CONTACTMOBILE: {this.CONTACTMOBILE}, TANKERSIZE: {this.TANKERSIZE}, COMMENT: {this.COMMENT}, CONFIRMATIONCODE: {this.CONFIRMATIONCODE}, SOURCEAPPLICATION: {this.SOURCEAPPLICATION}, CISDIVISION: {this.CISDIVISION}, TRANSACTIONID: {this.TRANSACTIONID}, TANKERACCESSORIES: {this.TANKERACCESSORIES} }}";
        }

    }
}
