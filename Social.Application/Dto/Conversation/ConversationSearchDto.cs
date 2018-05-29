using Framework.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Social.Application.Dto
{
    public class ConversationSearchDto
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

        /// <summary>
        /// Filter Id
        /// </summary>
        public int? FilterId { get; set; }

        /// <summary>
        /// Subject, note, message, sender or ricpient.
        /// </summary>
        [MaxLength(100)]
        public string Keyword { get; set; }

        ///// <summary>
        ///// The result will contains conversations which create time greater than Since date.
        ///// </summary>
        //public DateTime? Since { get; set; }


        ///// <summary>
        ///// The result will contains conversations which create time less or equal to Until date.
        ///// </summary>
        //public DateTime? Until { get; set; }

        [Range(0, int.MaxValue)]
        public int? UserId { get; set; }

        /// <summary>
        /// The result will contains conversations which last message send time less to MessageSendTimeLessThan (Unix TimeStamp).
        /// </summary>
        public long? LastMessageSendTimeLessThan { get; set; }
    }
}
