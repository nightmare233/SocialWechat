using AutoMapper;
using AutoMapper.QueryableExtensions;
using Framework.Core;
using Social.Application.Dto;
using Social.Domain.Entities;
using Social.Infrastructure.Enum;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Social.Application.AppServices
{
    public interface IMessageAttachmentAppService
    {
        Task GetRawDataJob();
        MessageAttachmentUrlDto GetAttachmentDto(int id);
    }
    public class MessageAttachmentAppService : ServiceBase, IMessageAttachmentAppService
    {
        IRepository<MessageAttachment> _messageAttachmentRepo;

        public MessageAttachmentAppService(IRepository<MessageAttachment> messageAttachmentRepo)
        {
            _messageAttachmentRepo = messageAttachmentRepo;
        }

        public async Task<List<MessageAttachmentRawDto>> GetMessageAttachments()
        {
            DateTime dateTime = DateTime.UtcNow.AddHours(-1);
            //var messageAttachments = _messageAttachmentRepo.FindAll()
            //   .Where(t => t.Id == 100)
            //   .ToList();
            var messageAttachments = _messageAttachmentRepo.FindAll()
                .Where(t => t.Message.Source == MessageSource.FacebookMessage || t.Message.Source == MessageSource.FacebookPost || t.Message.Source == MessageSource.FacebookPostComment
                || t.Message.Source == MessageSource.FacebookPostReplyComment)
                .Where(t => (t.MimeType.Contains("image") || t.MimeType.Contains("audio") || t.MimeType.Contains("video"))
                && t.RawData == null && t.Message.SendTime > dateTime)
                .ToList();
            var messageAttachmentRawDtos = new List<MessageAttachmentRawDto>();
            foreach (var messageAttachment in messageAttachments)
            {
                var messageAttachmentRawDto = new MessageAttachmentRawDto();
                messageAttachmentRawDto.Id = messageAttachment.Id;
                messageAttachmentRawDto.Url = messageAttachment.Url;
                messageAttachmentRawDtos.Add(messageAttachmentRawDto);
            }
            return messageAttachmentRawDtos;
        }

        public async Task<List<MessageAttachmentRawDto>> AddRawData(List<MessageAttachmentRawDto> messageAttachmentRawDtos)
        {
            foreach (var messageAttachmentRawDto in messageAttachmentRawDtos)
            {
                string UrlImg = messageAttachmentRawDto.Url;
                if (UrlCheck(UrlImg))
                {
                    HttpClient httpClient = new HttpClient();
                    byte[] byteData =await httpClient.GetByteArrayAsync(UrlImg);
                    messageAttachmentRawDto.RawData = byteData;
                }
            }
            return messageAttachmentRawDtos;
        }
        private bool UrlCheck(string strUrl)
        {
            if (!strUrl.Contains("http://") && !strUrl.Contains("https://"))
            {
                strUrl = "http://" + strUrl;
            }
            try
            {
                HttpWebRequest myRequest = (HttpWebRequest)WebRequest.Create(strUrl);
                myRequest.Method = "HEAD";
                myRequest.Timeout = 10000;  //超时时间10秒
                HttpWebResponse res = (HttpWebResponse)myRequest.GetResponse();
                return (res.StatusCode == HttpStatusCode.OK);
            }
            catch
            {
                return false;
            }
        }
        public async Task UpdateMessageAttachment(List<MessageAttachmentRawDto> messageAttachmentRawDtos)
        {
            foreach (var messageAttachmentRawDto in messageAttachmentRawDtos)
            {
                var messageAttachment = _messageAttachmentRepo.Find(messageAttachmentRawDto.Id);
                messageAttachment.RawData = messageAttachmentRawDto.RawData;
                _messageAttachmentRepo.Update(messageAttachment);
                await CurrentUnitOfWork.SaveChangesAsync();
            }
        }

        public async Task GetRawDataJob()
        {
            List<MessageAttachmentRawDto> messageAttachmentRawDtos = await GetMessageAttachments();
            messageAttachmentRawDtos = await AddRawData(messageAttachmentRawDtos);
            await UpdateMessageAttachment(messageAttachmentRawDtos);
        }

        public MessageAttachmentUrlDto GetAttachmentDto(int id)
        {
            var messageAttachment = _messageAttachmentRepo.Find(id);
            return Mapper.Map<MessageAttachmentUrlDto>(messageAttachment);
        }
    }
}
