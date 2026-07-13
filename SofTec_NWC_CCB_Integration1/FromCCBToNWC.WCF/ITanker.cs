using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Xml.Linq;

namespace FromCCBToNWC.WCF
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "ITanker" in both code and config file together.
    [ServiceContract]
    public interface ITanker
    {
        [OperationContract]
        [WebInvoke(Method = "POST",
           UriTemplate = "lookup",
           BodyStyle = WebMessageBodyStyle.Bare,
           ResponseFormat = WebMessageFormat.Xml,
            RequestFormat = WebMessageFormat.Xml)]
        NWC_CCB_Integration.DTO.Models.AvailableTankerSize.Output GetAvailableTankerSizes(NWC_CCB_Integration.DTO.Models.AvailableTankerSize.Schema schema);
    }
}
