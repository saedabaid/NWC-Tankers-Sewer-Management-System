using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logger
{
    [Flags]
    public enum LoggerType
    {
        None = 0,
        FileLogger = 1 << 0,
        VisualStudioOutputWindowLogger = 1 << 1
    }
}