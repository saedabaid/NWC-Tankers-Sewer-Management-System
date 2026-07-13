using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NWC_CCB_Integration.DTO.Common
{
    public class Filters<T>
    {
        public PageFilter PageFilter { get; set; }
        public T SearchKeyword { get; set; }
    }
}
