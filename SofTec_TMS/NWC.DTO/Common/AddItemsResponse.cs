using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NWC.DTO.Common
{
    public class AddItemsResponse
    {
        public int success { set; get; }
        public int failed { set; get; }
        public IEnumerable<string> message { set; get; }
    }
}
