using AutoMapper;
using Social.Application.Dto;
using Social.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tweetinvi.Models;

namespace Social.Application
{
    public class ConversationMessageDtoMappings : Profile
    {
        public ConversationMessageDtoMappings()
        {
            CreateMap<Message, FacebookMessageDto>()
             .ForMember(dest => dest.UserAvatar, src => src.MapFrom(x => x.Sender.Avatar))
             .ForMember(dest => dest.UserId, src => src.MapFrom(x => x.SenderId))
             .ForMember(dest => dest.UserType, src => src.MapFrom(x => x.Sender.Type))
             .ForMember(dest => dest.UserName, src => src.MapFrom(x => x.Sender.Name))
             .ForMember(dest => dest.UserScreenName, src => src.MapFrom(x => x.Sender.ScreenName))
             .ForMember(dest => dest.UserEmail, src => src.MapFrom(x => x.Sender.Email))
             .ForMember(dest => dest.UserLink, src => src.MapFrom(x => x.Sender.OriginalLink))
             .ForMember(dest => dest.SendAgentName, src => src.MapFrom(x => x.SendAgent.Name));

            CreateMap<Message, FacebookPostMessageDto>()
             .ForMember(dest => dest.UserAvatar, src => src.MapFrom(x => x.Sender.Avatar))
             .ForMember(dest => dest.UserId, src => src.MapFrom(x => x.SenderId))
             .ForMember(dest => dest.UserLink, src => src.MapFrom(x => x.Sender.OriginalLink))
             .ForMember(dest => dest.UserType, src => src.MapFrom(x => x.Sender.Type))
             .ForMember(dest => dest.UserName, src => src.MapFrom(x => x.Sender.Name))
             .ForMember(dest => dest.UserScreenName, src => src.MapFrom(x => x.Sender.ScreenName))
             .ForMember(dest => dest.UserEmail, src => src.MapFrom(x => x.Sender.Email))
             .ForMember(dest => dest.Content, src => src.MapFrom(x => x.Content ?? x.Story));

            CreateMap<Message, FacebookPostCommentMessageDto>()
             .ForMember(dest => dest.UserAvatar, src => src.MapFrom(x => x.Sender.Avatar))
             .ForMember(dest => dest.UserId, src => src.MapFrom(x => x.SenderId))
             .ForMember(dest => dest.UserLink, src => src.MapFrom(x => x.Sender.OriginalLink))
             .ForMember(dest => dest.UserType, src => src.MapFrom(x => x.Sender.Type))
             .ForMember(dest => dest.UserName, src => src.MapFrom(x => x.Sender.Name))
             .ForMember(dest => dest.UserScreenName, src => src.MapFrom(x => x.Sender.ScreenName))
             .ForMember(dest => dest.UserEmail, src => src.MapFrom(x => x.Sender.Email))
             .ForMember(dest => dest.SendAgentName, src => src.MapFrom(x => x.SendAgent.Name));

            CreateMap<Message, TwitterDirectMessageDto>()
             .ForMember(dest => dest.UserAvatar, src => src.MapFrom(x => x.Sender.Avatar))
             .ForMember(dest => dest.UserId, src => src.MapFrom(x => x.SenderId))
             .ForMember(dest => dest.UserLink, src => src.MapFrom(x => x.Sender.OriginalLink))
             .ForMember(dest => dest.UserType, src => src.MapFrom(x => x.Sender.Type))
             .ForMember(dest => dest.UserName, src => src.MapFrom(x => x.Sender.Name))
             .ForMember(dest => dest.UserScreenName, src => src.MapFrom(x => x.Sender.ScreenName))
             .ForMember(dest => dest.UserEmail, src => src.MapFrom(x => x.Sender.Email))
             .ForMember(dest => dest.SendAgentName, src => src.MapFrom(x => x.SendAgent.Name));

            CreateMap<Message, TwitterTweetMessageDto>()
             .ForMember(dest => dest.UserAvatar, src => src.MapFrom(x => x.Sender.Avatar))
             .ForMember(dest => dest.UserId, src => src.MapFrom(x => x.SenderId))
             .ForMember(dest => dest.UserLink, src => src.MapFrom(x => x.Sender.OriginalLink))
             .ForMember(dest => dest.UserType, src => src.MapFrom(x => x.Sender.Type))
             .ForMember(dest => dest.UserName, src => src.MapFrom(x => x.Sender.Name))
             .ForMember(dest => dest.UserScreenName, src => src.MapFrom(x => x.Sender.ScreenName))
             .ForMember(dest => dest.UserEmail, src => src.MapFrom(x => x.Sender.Email))
             .ForMember(dest => dest.SendAgentName, src => src.MapFrom(x => x.SendAgent.Name));

            CreateMap<Message, BeQuotedTweetDto>()
             .ForMember(dest => dest.UserAvatar, src => src.MapFrom(x => x.Sender.Avatar))
             .ForMember(dest => dest.UserLink, src => src.MapFrom(x => x.Sender.OriginalLink))
             .ForMember(dest => dest.UserName, src => src.MapFrom(x => x.Sender.Name))
             .ForMember(dest => dest.UserScreenName, src => src.MapFrom(x => x.Sender.ScreenName));

            CreateMap<MessageAttachment, MessageAttachmentDto>();
        }
    }
}
