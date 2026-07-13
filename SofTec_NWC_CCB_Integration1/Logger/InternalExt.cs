using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logger
{
    internal static class InternalExt
    {
        public static string Repeat(this string str, int howManyTimes)
        {
            string result = string.Empty;
            for (int i = 0; i < howManyTimes; i++)
            {
                result += str;
            }
            return result;
        }
    }
}