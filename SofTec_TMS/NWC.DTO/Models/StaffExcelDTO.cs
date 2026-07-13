using NWC.DTO.Models.TMS;
using System;
using System.Collections.Generic;

namespace NWC.DTO.Models
{
    public class StaffExcelDTO
    {
        public string IDs { get; set; }
        public string Branch { get; set; }
        public string Landmark { get; set; }        
        public string SubBranch { get; set; }
        public string Code { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public string StaffRole { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
        public string PermittedBranch { get; set; }
        public long? ExcelSheetRowId { get; set; }
        public string ExcelValidation { get; set; }
    }
}
