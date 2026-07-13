using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Extentions
{
    public static class TimeSpanExtention
    {
        public static string ToShortTimeString(this TimeSpan? time, string nullValue = "")
        {
            if (time == null)
                return nullValue;
            return Convert.ToDateTime(time.ToString()).ToString("hh:mm tt");
        }

        public static string ToShortTimeString(this TimeSpan time)
        {
            return Convert.ToDateTime(time.ToString()).ToString("hh:mm tt");
        }
    }
}
