using NWC.Localization.Constants;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Web.Script.Serialization;

namespace NWC.Localization.ExceptionLogger
{
    public class DatabaseExceptionLogger : IExceptionLogger
    {
        public void LogException(Exception e)
        {
            try
            {
                var connectionstring = ConfigurationManager.ConnectionStrings[ExceptionHandlerConstants.DatabaseNameKey].ConnectionString;
                using (SqlConnection con = new SqlConnection(connectionstring))
                {
                    string query = "INSERT INTO [dbo].[ICCCLog] (LogDetails, LogType) VALUES (@LogDetails, @LogType) ";
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.Add("@LogDetails", SqlDbType.VarChar).Value = PrepareExceptionString(e);
                    cmd.Parameters.Add("@LogType", SqlDbType.VarChar, 200).Value = ExceptionHandlerConstants.ExceptionLogType;

                    if (con.State != ConnectionState.Open)
                        con.Open();

                    cmd.ExecuteNonQuery();
                    con.Close();
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {

            }
        }

        public void LogException(Exception e, params object[] parameters)
        {
            try
            {
                var connectionstring = ConfigurationManager.ConnectionStrings[ExceptionHandlerConstants.DatabaseNameKey].ConnectionString;
                using (SqlConnection con = new SqlConnection(connectionstring))
                {
                    string query = "INSERT INTO [dbo].[ICCCLog] (LogDetails, LogType) VALUES (@LogDetails, @LogType) ";
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.Add("@LogDetails", SqlDbType.VarChar).Value = PrepareExceptionString(e) + PrepareParameterString(parameters);
                    cmd.Parameters.Add("@LogType", SqlDbType.VarChar, 200).Value = ExceptionHandlerConstants.ExceptionLogType;

                    if (con.State != ConnectionState.Open)
                        con.Open();

                    cmd.ExecuteNonQuery();
                    con.Close();
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {

            }
        }

        public void LogInformation(string information)
        {
            throw new NotImplementedException();
        }

        private string PrepareExceptionString(Exception e)
        {
            StringBuilder exceptionString = new StringBuilder();
            exceptionString.AppendLine("Exception :");
            exceptionString.AppendLine("---------");
            exceptionString.AppendLine(e.ToString());
            if (e.InnerException != null)
            {
                exceptionString.AppendLine("InnerException :");
                exceptionString.AppendLine("---------");
                exceptionString.AppendLine(e.InnerException.ToString());
            }
            if (e.Source != null)
            {
                exceptionString.AppendLine("Source :");
                exceptionString.AppendLine("---------");
                exceptionString.AppendLine(e.Source.ToString());
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
                parameterString.AppendLine("parameters:");
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
    }
}
