using NWC_CCB_Integration.DTO.Models.Soqya;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace Soqya.WCF
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    [ServiceContract]
    public interface ISoqya
    {
        [OperationContract]
        [WebInvoke(Method = "POST",
           UriTemplate = "lookup",
           BodyStyle = WebMessageBodyStyle.Bare,
           ResponseFormat = WebMessageFormat.Xml,
            RequestFormat = WebMessageFormat.Xml)]
        Output AddSoqyaCustomerBalance(Schema schema);
    }
}
