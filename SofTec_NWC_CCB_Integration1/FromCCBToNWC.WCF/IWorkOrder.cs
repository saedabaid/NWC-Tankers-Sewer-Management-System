using NWC_CCB_Integration.DTO.Common;
using NWC_CCB_Integration.DTO.Output;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Security;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Web;
using System.Text;
using System.Xml.Linq;

namespace FromCCBToNWC.WCF
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    [ServiceContract]
    public interface IWorkOrder
    {
        [OperationContract]
        [WebInvoke(Method = "POST",
           UriTemplate = "lookup",
           BodyStyle = WebMessageBodyStyle.Bare,
           ResponseFormat = WebMessageFormat.Xml, 
            RequestFormat = WebMessageFormat.Xml)]
        NWC_CCB_Integration.DTO.Models.CreateWorkOrder.Output CreateWorkOrder(NWC_CCB_Integration.DTO.Models.CreateWorkOrder.Schema schema);

        [OperationContract]
        [WebInvoke(Method = "POST",
           UriTemplate = "lookup",
           BodyStyle = WebMessageBodyStyle.Bare,
           ResponseFormat = WebMessageFormat.Xml, 
            RequestFormat = WebMessageFormat.Xml)]
        NWC_CCB_Integration.DTO.Models.UpdateWorkOrder.Output UpdateWorkOrder(NWC_CCB_Integration.DTO.Models.UpdateWorkOrder.Schema schema);
    }
}
