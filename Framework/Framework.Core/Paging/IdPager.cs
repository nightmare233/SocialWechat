using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework.Core
{
    public class IdPager
    {
        /// <summary>
        /// Number of data retrieved from api.
        /// </summary>
        public int MaxNumberOfDataRetrieve { get; set; }

        /// <summary>
        /// The result will contains data which id greater than SinceId.
        /// </summary>
        public int? SinceId { get; set; }

        /// <summary>
        /// The result will contains data which id lesss or equal to MaxId.
        /// </summary>
        public int? MaxId { get; set; }
    }
}
