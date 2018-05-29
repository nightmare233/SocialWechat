using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Framework.Core
{
    public class ExceptionWithCode : Exception
    {
        private int _errorCode;
        private HttpStatusCode _httpStatusCode;

        public ExceptionWithCode(int errorCode, string message) : this(HttpStatusCode.BadRequest, errorCode, message)
        {

        }

        public ExceptionWithCode(HttpStatusCode httpStatusCode, int errorCode, string message) : base(message)
        {
            _httpStatusCode = httpStatusCode;
            _errorCode = errorCode;
        }

        public ExceptionWithCode(int errorCode, string message, Exception innertException) : this(HttpStatusCode.BadRequest, errorCode, message, innertException)
        {
        }

        public ExceptionWithCode(HttpStatusCode httpStatusCode, int errorCode, string message, Exception innertException) : base(message, innertException)
        {
            _httpStatusCode = httpStatusCode;
            _errorCode = errorCode;
        }

        public int ErrorCode
        {
            get { return _errorCode; }
        }

        public HttpStatusCode HttpStatusCode
        {
            get { return _httpStatusCode; }
        }
    }
}
