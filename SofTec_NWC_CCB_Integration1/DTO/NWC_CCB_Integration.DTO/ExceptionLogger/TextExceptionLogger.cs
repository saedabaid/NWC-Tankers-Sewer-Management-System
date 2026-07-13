using NWC_CCB_Integration.DTO.Constants;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace NWC_CCB_Integration.DTO.ExceptionLogger
{
    public class TextExceptionLogger : IExceptionLogger
    {
        string logsPath = ConfigurationManager.AppSettings[ExceptionHandlerConstants.LogsPathKey];

        private static TextExceptionLogger instance;

        private string _directoryPath;

        public static TextExceptionLogger GetInstance
        {
            get { return instance ?? (instance = new TextExceptionLogger()); }
        }

        private TextExceptionLogger()
        {
            if (!string.IsNullOrEmpty(logsPath))
                this._directoryPath = logsPath;
            else
                this._directoryPath = AppDomain.CurrentDomain.BaseDirectory;
        }

        public void LogException(Exception e)
        {
            WriteExceptionToFile(PrepareExceptionString(e));
        }

        public void LogInformation(string information)
        {
            WriteInformationToFile(information);
        }

        public void LogException(Exception e, params object[] parameters)
        {
            WriteExceptionToFile(PrepareExceptionString(e) + PrepareParameterString(parameters));
        }

        #region "Exception Log"
        private string PrepareExceptionString(Exception e)
        {
            StringBuilder exceptionString = new StringBuilder();
            exceptionString.AppendLine("Exception : Time:" + DateTime.Now.ToString());
            exceptionString.AppendLine("---------");
            exceptionString.AppendLine(e.ToString());
            exceptionString.AppendLine();

            if (e.InnerException != null)
            {
                exceptionString.AppendLine("InnerException :");
                exceptionString.AppendLine("---------");
                exceptionString.AppendLine(e.InnerException.ToString());
                exceptionString.AppendLine();
            }
            if (e.Source != null)
            {
                exceptionString.AppendLine("Source :");
                exceptionString.AppendLine("---------");
                exceptionString.AppendLine(e.Source.ToString());
                exceptionString.AppendLine();
            }
            if (e.StackTrace != null)
            {
                exceptionString.AppendLine("StackTrace :");
                exceptionString.AppendLine("---------");
                exceptionString.AppendLine(e.StackTrace.ToString());
                exceptionString.AppendLine("__________________________________");
            }
            return exceptionString.ToString();
        }

        private string PrepareParameterString(params object[] parameter)
        {
            try
            {
                StringBuilder parameterString = new StringBuilder();
                parameterString.AppendLine("parameter:");
                parameterString.AppendLine("--------------------------------------");

                foreach (var item in parameter)
                {
                    parameterString.AppendLine("parameters starts here:");
                    parameterString.AppendLine("--------------------------------------");
                    parameterString.AppendLine(new JavaScriptSerializer().Serialize(item));
                    parameterString.AppendLine("parameters ends here:");
                }
                return parameterString.ToString();
            }
            catch (Exception e)
            {
                return string.Empty;
            }
        }

        private string CreateExceptionDirectoryIfnotExist(string folderName = @"Log")
        {
            //string path = AppDomain.CurrentDomain.BaseDirectory + folderName;
            string path = string.Format("{0}\\{1}", this._directoryPath, folderName);

            if (Directory.Exists(path) == false)
            {
                Directory.CreateDirectory(path);
            }
            return path;
        }

        private string CreateDailyDirectory(string folderName = @"Log")
        {
            string directoryPath = CreateExceptionDirectoryIfnotExist(folderName);

            string folderPath = directoryPath + @"\" + DateTime.Now.ToString("dd-MM-yyyy");

            if (Directory.Exists(folderPath) == false)
            {
                Directory.CreateDirectory(folderPath);
            }

            return folderPath;
        }

        private void WriteExceptionToFile(string exception)
        {
            try
            {
                var directoryPath = CreateDailyDirectory();
                var fileName = DateTime.Now.Hour.ToString() + ".txt";
                var fullPath = directoryPath + @"\" + fileName;
                if (File.Exists(fullPath) == false)
                {
                    var exceptionFile = File.Create(fullPath);
                    exceptionFile.Close();
                }
                using (TextWriter tw = new StreamWriter(fullPath, true))
                {
                    tw.WriteLine(exception);
                    tw.Close();
                }
            }
            catch (Exception e)
            {
            }
        }
        #endregion

        #region "Information Log"

        private void WriteInformationToFile(string information)
        {
            try
            {
                var directoryPath = CreateDailyDirectory("Tracing");
                var fileName = DateTime.Now.Hour.ToString() + ".txt";
                var fullPath = directoryPath + @"\" + fileName;

                information = information + " Time:" + DateTime.Now.ToString() + "\n";

                if (File.Exists(fullPath) == false)
                {
                    var exceptionFile = File.Create(fullPath);
                    exceptionFile.Close();
                }
                using (TextWriter tw = new StreamWriter(fullPath, true))
                {
                    tw.WriteLine(information);
                    tw.Close();
                }
            }
            catch (Exception ex)
            {
                LogException(ex);
            }
        }
        #endregion
    }
}
