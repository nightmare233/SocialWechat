using Facebook;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Social.Infrastructure.Facebook
{
    public class FbPagingData<T>
    {
        public List<T> data { get; set; }
        public FbPaging paging { get; set; }
        public FbPagingSummary summary { get; set; }
    }
}
