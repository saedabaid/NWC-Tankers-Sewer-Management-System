using NWC.DTO.Enums;
using NWC_CCB_Integration.DTO.Logger;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace NWC.DTO.Helpers
{
    public static class Utilities
    {

        public static string MoveFile(AttachmentType type, string fileName, long? fk_Id)
        {
            #region New File Path
            string folderPath;
            switch (type)
            {
                case AttachmentType.Contract:
                    folderPath = ConfigurationManager.AppSettings["contract"];
                    break;
                case AttachmentType.WorkOrder:
                    folderPath = ConfigurationManager.AppSettings["workOrder"];
                    break;
                case AttachmentType.Contractor:
                    folderPath = ConfigurationManager.AppSettings["contractor"];
                    break;
                case AttachmentType.ContractViolation:
                    folderPath = ConfigurationManager.AppSettings["ContractViolation"];
                    break;
                case AttachmentType.PermitDetectionform:
                    folderPath = ConfigurationManager.AppSettings["PermitDetectionform"];
                    break;
                case AttachmentType.PermitDeclaration:
                    folderPath = ConfigurationManager.AppSettings["PermitDeclaration"];
                    break;
                case AttachmentType.PermitOtherFIle:
                    folderPath = ConfigurationManager.AppSettings["PermitOtherFIle"];
                    break;
                default:
                    folderPath = string.Empty;
                    break;
            }

            if (fk_Id != null)
                folderPath = folderPath + "/" + fk_Id;

            if (!System.IO.Directory.Exists(folderPath))
                System.IO.Directory.CreateDirectory(folderPath); //Create directory if it doesn't exist

            string newFilePath = folderPath + "/" + fileName;
            #endregion

            string oldFilePath = ConfigurationManager.AppSettings["temp"] + "/" + fileName;

            System.IO.File.Move(oldFilePath, newFilePath);

            if (fk_Id != null)
                return $"{fk_Id}/{fileName}";
            else
                return fileName;
        }

        public static string MoveFile(AttachmentType type, string fileName, string fk_Id)
        {
            #region New File Path
            string folderPath;
            switch (type)
            {
                case AttachmentType.Contract:
                    folderPath = ConfigurationManager.AppSettings["contract"];
                    break;
                case AttachmentType.WorkOrder:
                    folderPath = ConfigurationManager.AppSettings["workOrder"];
                    break;
                case AttachmentType.Contractor:
                    folderPath = ConfigurationManager.AppSettings["contractor"];
                    break;
                case AttachmentType.ContractViolation:
                    folderPath = ConfigurationManager.AppSettings["ContractViolation"];
                    break;
                case AttachmentType.PermitDetectionform:
                    folderPath = ConfigurationManager.AppSettings["PermitDetectionform"];
                    break;
                case AttachmentType.PermitDeclaration:
                    folderPath = ConfigurationManager.AppSettings["PermitDeclaration"];
                    break;
                case AttachmentType.PermitOtherFIle:
                    folderPath = ConfigurationManager.AppSettings["PermitOtherFIle"];
                    break;
                default:
                    folderPath = string.Empty;
                    break;
            }

            if (fk_Id != null)
                folderPath = folderPath + "/" + fk_Id;

            if (!System.IO.Directory.Exists(folderPath))
                System.IO.Directory.CreateDirectory(folderPath); //Create directory if it doesn't exist

            string newFilePath = folderPath + "/" + fileName;
            #endregion

            string oldFilePath = ConfigurationManager.AppSettings["temp"] + "/" + fileName;

            System.IO.File.Move(oldFilePath, newFilePath);

            if (fk_Id != null)
                return $"{fk_Id}/{fileName}";
            else
                return fileName;
        }
        //protected string ToTinyURLS(string txt)
        //{
        //    Regex regx = new Regex("http://([\\w+?\\.\\w+])+([a-zA-Z0-9\\~\\!\\@\\#\\$\\%\\^\\&amp;\\*\\(\\)_\\-\\=\\+\\\\\\/\\?\\.\\:\\;\\'\\,]*)?", RegexOptions.IgnoreCase);

        //    MatchCollection mactches = regx.Matches(txt);

        //    foreach (Match match in mactches)
        //    {
        //        string tURL = MakeTinyUrl(match.Value);
        //        txt = txt.Replace(match.Value, tURL);
        //    }

        //    return txt;
        //}

        public static string MakeTinyUrl(string Url)
        {
            try
            {
                if (Url.Length <= 12)
                {
                    return Url;
                }
                if (!Url.ToLower().StartsWith("http") && !Url.ToLower().StartsWith("ftp"))
                {
                    Url = "http://" + Url;
                }
                var request = WebRequest.Create("http://tinyurl.com/api-create.php?url=" + Url);
                var res = request.GetResponse();
                string text;
                using (var reader = new StreamReader(res.GetResponseStream()))
                {
                    text = reader.ReadToEnd();
                }
                return text;
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex));
                return Url;
            }
        }

        public static string YearMonthLongToString(long input)
        {
            try
            {
                long year = input / 100;
                int month = (int)(input - (year * 100));

                return $"{CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(month)} {year}";
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex));
                return string.Empty;
            }
        }


        public static List<int> ConvertToList(string idsAsString)
        {
            if (!string.IsNullOrEmpty(idsAsString))
            {
                var result = new List<int>();
                var ids = idsAsString.Split(',');
                foreach (var item in ids)
                {
                    int convertedItem;
                    if (int.TryParse(item, out convertedItem))
                    {
                        result.Add(convertedItem);
                    }
                }
                return result;
            }
            return null;
        }

        public static string ConvertToString(List<int> ids)
        {
            if (ids != null && ids.Any())
            {
                return string.Join(",", ids.Select(n => n.ToString()).ToArray());
            }

            return string.Empty;
        }

    }
}
