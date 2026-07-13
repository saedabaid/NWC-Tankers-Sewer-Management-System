using NWC.Localization.Constants;
using System;
using System.IO;
using System.Text;
using System.Web.Script.Serialization;

namespace NWC.Localization.ExceptionLogger
{
    public class TextExceptionLogger : IExceptionLogger
    {
        private string _directoryPath;

        public TextExceptionLogger(string directoryPath)
        {
            if (!string.IsNullOrEmpty(directoryPath))
                this._directoryPath = directoryPath;
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
                    tw.Dispose();
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
                    tw.Dispose();
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
