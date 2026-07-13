using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace NWC_CCB_Integration.DTO.Resources
{
    public static class ResourcesManager
    {
        public static string ReadResourceValue(string file, string key)
        {
            string resourceValue = string.Empty;
            try
            {
                string resourceFile = file;
                string filePath = System.AppDomain.CurrentDomain.BaseDirectory.ToString();
                ResourceManager resourceManager = ResourceManager.CreateFileBasedResourceManager(resourceFile, filePath, null);
                resourceValue = resourceManager.GetString(key);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                resourceValue = string.Empty;
            }
            return resourceValue;
        }
    }
}
