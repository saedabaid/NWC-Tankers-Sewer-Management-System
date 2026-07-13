using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NWC.DTO.Common
{
  public  class StationSizesDTO
    {
        public System.Guid id { get; set; }
        public int dailyquota { get; set; }
        public string SizesList { get; set; }
    }
}
