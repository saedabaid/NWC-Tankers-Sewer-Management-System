using NWC.DAL.NWCEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NWC.DTO.Models
{
    public class PrintCustomerInvoice
    {
        public string CustomerName { get; set; }
        public string CustomerMobile { get; set; }
        public string CustomerAddress { get; set; }
        public string OrderNumber { get; set; }
        public int OrderQuantity { get; set; }
        public decimal TotalCost { get; set; }
        public System.DateTime ScheduledDeliveryTime { get; set; }
        public string EncryptedConfirmationCode { get; set; }
        public string ZoneName { get; set; }
        public string StationName { get; set; }
        public string ContractorFullName { get; set; }
        public string TaxNumber { get; set; }
        public string CompanyAddress { get; set; }
        public System.Guid assignedStation { get; set; }
        public string plateNo { get; set; }
        public Guid VehicleID { get; set; }
        public decimal NetCost { get; set; }
        public string InvoiceNo { get; set; }
        public string TransporterCode { get; set; }
        public string CategoryName { get; set; }
        public string Category { get; set; }
        public int? CategoryID { get; set; }

        //public string SubContractorFullName { get; set; }
        //public string SubContTaxNumber { get; set; }
        //public string SubContCompanyAddress { get; set; }
    }
}
