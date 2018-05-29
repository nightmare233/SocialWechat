using Social.Application.AppServices;
using Social.Application.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace Social.WebApi.Controllers
{
    /// <summary>
    /// api about conversation fields.
    /// </summary>
    [RoutePrefix("api/conversation-fields")]
    public class ConversationFieldsController : ApiController
    {
        private IConversationFieldAppService _appService;

        /// <summary>
        /// ConversationFieldsController
        /// </summary>
        /// <param name="appService"></param>
        public ConversationFieldsController(IConversationFieldAppService appService)
        {
            _appService = appService;
        }

        /// <summary>
        /// Get fileds of conversation.
        /// </summary>
        /// <returns></returns>
        [Route()]
        public List<ConversationFieldDto> GetConversationFields()
        {
            return _appService.FindAll();
        }

        /// <summary>
        /// Get fileds by filed id.
        /// </summary>
        /// <param name="id">conversation field id</param>
        /// <returns></returns>
        [Route("{id}", Name = "GetConversationField")]
        public ConversationFieldDto GetConversationField(int id)
        {
            return _appService.Find(id);
        }
    }
}