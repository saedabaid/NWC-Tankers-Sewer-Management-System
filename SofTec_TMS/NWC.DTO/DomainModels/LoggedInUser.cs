using System;

namespace NWC.DTO.DomainModels
{
    public class LoggedInUser
    {
        public string Token { get; set; }
        public Guid StaffId { get; set; }
        public Guid SubscriberId { get; set; }
        public Guid? StaffRoleID { get; set; }
        public string Lang { get; set; }
    }
}
