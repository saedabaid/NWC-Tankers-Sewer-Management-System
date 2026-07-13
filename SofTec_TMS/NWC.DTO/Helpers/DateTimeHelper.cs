using NWC.DTO.Constants;
using NWC_CCB_Integration.DTO.Logger;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NWC.DTO.Helpers
{
    public static class DateTimeHelper
    {
        public static DateTime GetDateTimeNow()
        {
            try
            {
                DateTime dt = Convert.ToDateTime(new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second), 
                                CultureInfo.GetCultureInfo(LanguagesKeys.English).DateTimeFormat);

                return dt;
            }
            catch(Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex));
                return DateTime.Now;
            }
        }

        public static DateTime ConvertDateTimeToUSCultureInfo(DateTime date)
        {
            try
            {
                DateTime dt = Convert.ToDateTime(date, CultureInfo.GetCultureInfo(LanguagesKeys.English).DateTimeFormat);

                return dt;
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex));
                return DateTime.Now;
            }
        }

        //string date = ConvertDateCalendar(DateTime.Now, "Hijri", "en-US")
        public static string ConvertDateCalendar(DateTime DateConv, string Calendar, string DateLangCulture)
        {
            DateTimeFormatInfo DTFormat;
            DateLangCulture = DateLangCulture.ToLower();
            /// We can't have the hijri date writen in English. We will get a runtime error

            if (Calendar == "Hijri" && DateLangCulture.StartsWith("en-"))
            {
                DateLangCulture = "ar-sa";
            }

            /// Set the date time format to the given culture
            DTFormat = new System.Globalization.CultureInfo(DateLangCulture, false).DateTimeFormat;

            /// Set the calendar property of the date time format to the given calendar
            switch (Calendar)
            {
                case "Hijri":
                    DTFormat.Calendar = new System.Globalization.UmAlQuraCalendar();
                    break;

                case "Gregorian":
                    DTFormat.Calendar = new System.Globalization.GregorianCalendar();
                    break;

                default:
                    return "";
            }

            /// We format the date structure to whatever we want
            DTFormat.ShortDatePattern = "dd/MM/yyyy";
            return (DateConv.Date.ToString("f", DTFormat));
        }
        
        public static long ConvertDateToHijriAsLong(DateTime DateConv)
        {
            //var strHijriDate = string.Empty;
            //long lngHijriDate = 0;

            var calendar = new UmAlQuraCalendar();

            var hijriYear = calendar.GetYear(DateConv);
            var hijriMonth = calendar.GetMonth(DateConv);
            var hijriDay = calendar.GetDayOfMonth(DateConv);

            //var strMonth = hijriMonth < 10 ? "0" + hijriMonth.ToString() : hijriMonth.ToString();
            //var strDay = hijriDay < 10 ? "0" + hijriDay.ToString() : hijriDay.ToString();

            //strHijriDate = string.Format("{0}{1}{2}", hijriYear, strMonth, strDay);

            //long.TryParse(strHijriDate, out lngHijriDate);
            //return lngHijriDate;

            return (hijriYear * 10000) + (hijriMonth * 100) + hijriDay;
        }
    }
}
