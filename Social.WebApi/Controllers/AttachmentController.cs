using Social.Application.AppServices;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Results;

namespace Social.WebApi.Controllers
{
    [RoutePrefix("api/attachments")]
    public class AttachmentController:ApiController
    {
        private IMessageAttachmentAppService _messageAttachmentAppService;

        public AttachmentController(IMessageAttachmentAppService messageAttachmentAppService)
        {
            _messageAttachmentAppService = messageAttachmentAppService;
        }
        [Route("{id}", Name = "GetAttachment")]
        public async Task<HttpResponseMessage> GetAttachment(int id)
        {
            HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
            var messageAttachmentDto = _messageAttachmentAppService.GetAttachmentDto(id);
            if(messageAttachmentDto == null)
            {
                return new HttpResponseMessage(HttpStatusCode.NotFound);
            }
            if (messageAttachmentDto.RawData == null || messageAttachmentDto.MimeType == null)
            {
                return new HttpResponseMessage(HttpStatusCode.NoContent);
            }
            result.Content = new ByteArrayContent(messageAttachmentDto.RawData);

            result.Headers.CacheControl = new System.Net.Http.Headers.CacheControlHeaderValue { Private = true, MaxAge = TimeSpan.FromSeconds(24 * 365 * 60) };
            result.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(messageAttachmentDto.MimeType);

            return result;
        }
    }
}