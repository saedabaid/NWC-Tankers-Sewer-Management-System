using Newtonsoft.Json;
using NWC.DTO.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NWC.DTO.Helpers
{
    public static class Security
    {
        private const string EncrptKey = "yur542165ertyhpeioyy721ufegdiuoo";
        static readonly char[] padding = { '=' };
        public static string Base64Encode(Object tokenObject)
        {
            string json = JsonConvert.SerializeObject(tokenObject);
            var plainTextBytes = Encoding.UTF8.GetBytes(json);
            return Convert.ToBase64String(plainTextBytes).TrimEnd(padding);
        }

        public static string Base64Encode(AuthenticationDTO authenticationObject)
        {
            string json = JsonConvert.SerializeObject(authenticationObject);
            var plainTextBytes = Encoding.UTF8.GetBytes(json);
            return System.Convert.ToBase64String(plainTextBytes).TrimEnd(padding);
        }

        public static AuthenticationDTO Base64Decode(string base64EncodedObject)
        {
            switch (base64EncodedObject.Length % 4)
            {
                case 2: base64EncodedObject += "=="; break;
                case 3: base64EncodedObject += "="; break;
            }

            byte[] byteArray = Convert.FromBase64String(base64EncodedObject);
            string jsonBack = Encoding.UTF8.GetString(byteArray);
            var ObjectBack = JsonConvert.DeserializeObject<AuthenticationDTO>(jsonBack);
            return ObjectBack;
        }



        public static string EncrptToken(AuthenticationDTO authenticationObject)
        {
            string json = JsonConvert.SerializeObject(authenticationObject);
            return AesOperation.EncryptString(EncrptKey, json)
                .TrimEnd(padding).Replace('+', '-').Replace('/', '_');
        }

        public static AuthenticationDTO DecryptToken(string encodedObject)
        {
            string incoming = encodedObject.Replace('_', '/').Replace('-', '+');
            switch (encodedObject.Length % 4)
            {
                case 2: incoming += "=="; break;
                case 3: incoming += "="; break;
            }

            string jsonBack = AesOperation.DecryptString(EncrptKey, incoming);
            var ObjectBack = JsonConvert.DeserializeObject<AuthenticationDTO>(jsonBack);
            return ObjectBack;
        }



    }
}
