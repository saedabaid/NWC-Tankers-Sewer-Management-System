using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NWC.Service.Authentication.WebAPI.Models
{
    public class MobileAccountDto
    {
        public string Name { get; set; }
        public object OperatorId { get; set; }
        public Guid? SubId { get; set; }
        public Guid Id { get; set; }
        public bool IsConsumer { get; set; }
        public long? ParentId { get; set; }
        public Guid? StaffId { get; set; }
        public string Image { get; set; }
    }
}