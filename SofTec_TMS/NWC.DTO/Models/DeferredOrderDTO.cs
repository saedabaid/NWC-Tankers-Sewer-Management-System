using System;

namespace NWC.DTO.Models
{
    public class DeferredOrderDTO
    {
        public long ID { get; set; }
        public string ORDERNUMBER { get; set; }
        public string CISDIVISION { get; set; }
        public string COMMENT { get; set; }
        public string CONFIRMATIONCODE { get; set; }
        public string CONTACTMOBILE { get; set; }
        public string CONTACTNAME { get; set; }
        public string CREDTTM { get; set; }
        public string SCHEDDTTM { get; set; }
        public string SERVICETYPE { get; set; }
        public string SOURCEAPPLICATION { get; set; }
        public string TANKERSIZE { get; set; }
        public string TRANSACTIONID { get; set; }
        public string ACCOUNTID { get; set; }
        public string CUSTOMERCLASS { get; set; }
        public string MOBILENUMBER { get; set; }
        public string PERSONID { get; set; }
        public string PERSONIDTYPE { get; set; }
        public string PERSONIDVALUE { get; set; }
        public string PERSONPRIMARYNAME { get; set; }
        public string PREMISEID { get; set; }
        public string XYCOORDINATESGF { get; set; }
        public Nullable<int> StatusId { get; set; }
        public string ErrorMSG { get; set; }
        public Nullable<System.Guid> LastUpdatedBy { get; set; }
        public Nullable<System.DateTime> LastUpdatedTime { get; set; }
        public Nullable<System.DateTime> CreateTime { get; set; }


        public string StatusName { get; set; }

        //helpers
        public int helper_CustomerClassId { get; set; }
        public int helper_ServiceTypeId { get; set; }
        public long helper_ZoneId { get; set; }
        public int helper_PersonIdTypeID { get; set; }
        public double? helper_latitude { get; set; }
        public double? helper_longitude { get; set; }
        public string helper_ZoneName { get; set; }
        public DateTime helper_scheduleTime { get; set; }

    }
}
