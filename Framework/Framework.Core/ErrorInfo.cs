using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework.Core
{
    public class ErrorInfo
    {
        public int Code { get; set; }

        public string Message { get; set; }

        public ErrorInfo()
        {
        }

        public ErrorInfo(string message)
        {
            Message = message;
        }

        public ErrorInfo(int code)
        {
            Code = code;
        }

        public ErrorInfo(int code, string message)
            : this(message)
        {
            Code = code;
        }
    }
}
