using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NWC_CCB_Integration.DTO.Common
{
    public class LookUpDTO<T>
    {
        public T Id { get; set; }
        public string Name { get; set; }
        public string IntegrationId { get; set; }
    }
}
