using NWC_CCB_Integration.DTO.Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace NWC.DTO.Helpers
{
    public static class ImageHelper
    {
        public static byte[] ImageToByteArray(string imgPath)
        {
            try
            {
                //HttpContext.Current.Server.MapPath(

                byte[] imgdata = System.IO.File.ReadAllBytes(imgPath);
                return imgdata;
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex));
                return null;
            }
        }
    }
}
