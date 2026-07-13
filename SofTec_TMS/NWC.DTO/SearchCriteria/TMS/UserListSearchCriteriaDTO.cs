using NWC.DTO.Common;
using NWC.DTO.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NWC.DTO.SearchCriteria
{
    public class UserListSearchCriteriaDTO
    {
        public PageFilter PageFilter { set; get; }
        public string Search { set; get; }
    }
}
