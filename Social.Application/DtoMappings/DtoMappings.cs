using AutoMapper;
using Social.Application.Dto;
using Social.Application.Dto.UserInfo;
using Social.Domain.Core;
using Social.Domain.DomainServices;
using Social.Domain.Entities;
using Social.Domain.Entities.General;
using Social.Infrastructure.Cache;
using Social.Infrastructure.Enum;
using Social.Infrastructure.Facebook;
using System.Linq;

namespace Social.Application
{
    public class DtoMappings : Profile
    {
        public DtoMappings()
        {
            CreateMap<Conversation, ConversationDto>()
                .ForMember(dto => dto.LastMessageSenderName, opt => opt.MapFrom(s => s.LastMessageSender.Name))
                .ForMember(dto => dto.LastMessageSenderAvatar, opt => opt.MapFrom(s => s.LastMessageSender.Avatar))
                .ForMember(dto => dto.LastMessageSenderUrl, opt => opt.MapFrom(s => s.LastMessageSender.OriginalLink))
                .ForMember(dto => dto.AgentName, opt => opt.MapFrom(s => s.Agent.Name))
                .ForMember(dto => dto.DepartmentName, opt => opt.MapFrom(s => s.Department.Name));
            CreateMap<ConversationCreateDto, Conversation>();
            CreateMap<FilterCreateDto, Filter>();
            CreateMap<Filter, FilterListDto>();
            CreateMap<Filter, FilterDetailsDto>();
            CreateMap<Filter, FilterManageDto>();
            CreateMap<FilterConditionCreateDto, FilterCondition>();
            CreateMap<FilterCondition, FilterConditionCreateDto>();
            CreateMap<FilterUpdateDto, Filter>();
            CreateMap<FilterConditionDto, FilterCondition>();
            CreateMap<FilterCondition, FilterConditionDto>();
            CreateMap<ConversationUpdateDto, Conversation>();
            CreateMap<ConversationDto, Conversation>();
            CreateMap<ConversationField, ConversationFieldDto>();
            CreateMap<ConversationFieldOption, ConversationFieldOptionDto>();

            CreateMap<ConversationLog, ConversationLogDto>();


            CreateMap<AddFaceboookPageDto, SocialAccount>()
                .ForMember(dest => dest.Token, src => src.MapFrom(x => x.AccessToken))
                .ForMember(dest => dest.FacebookPageCategory, src => src.MapFrom(x => x.Category))
                .ForMember(dest => dest.FacebookSignInAs, src => src.MapFrom(x => x.SignInAs));

            CreateMap<AddFaceboookPageDto, SocialUser>()
                .ForMember(dest => dest.OriginalId, src => src.MapFrom(x => x.FacebookId))
                .ForMember(dest => dest.Source, src => src.UseValue(SocialUserSource.Facebook))
                .ForMember(dest => dest.Type, src => src.UseValue(SocialUserType.IntegrationAccount))
                .ForMember(dest => dest.Avatar, src => src.MapFrom(x => x.Avatar))
                .ForMember(dest => dest.OriginalLink, src => src.MapFrom(x => x.Link));

            CreateMap<SocialAccount, FacebookPageListDto>()
                .ForMember(dest => dest.Category, src => src.MapFrom(x => x.FacebookPageCategory))
                .ForMember(dest => dest.SignInAs, src => src.MapFrom(x => x.FacebookSignInAs))
                .ForMember(dest => dest.Name, src => src.MapFrom(x => x.SocialUser.Name))
                .ForMember(dest => dest.Avatar, src => src.MapFrom(x => x.SocialUser.Avatar));

            CreateMap<SocialAccount, FacebookPageDto>()
                .ForMember(dest => dest.Category, src => src.MapFrom(x => x.FacebookPageCategory))
                .ForMember(dest => dest.SignInAs, src => src.MapFrom(x => x.FacebookSignInAs))
                .ForMember(dest => dest.Name, src => src.MapFrom(x => x.SocialUser.Name))
                .ForMember(dest => dest.Avatar, src => src.MapFrom(x => x.SocialUser.Avatar))
                .ForMember(dest => dest.FacebookId, src => src.MapFrom(x => x.SocialUser.OriginalId));

            CreateMap<UpdateFacebookPageDto, SocialAccount>();

            CreateMap<SocialAccount, TwitterAccountListDto>()
                .ForMember(dest => dest.Name, src => src.MapFrom(x => x.SocialUser.Name))
                .ForMember(dest => dest.ScreenName, src => src.MapFrom(x => x.SocialUser.ScreenName))
                .ForMember(dest => dest.Avatar, src => src.MapFrom(x => x.SocialUser.Avatar));
            CreateMap<SocialAccount, TwitterAccountDto>()
                .ForMember(dest => dest.Name, src => src.MapFrom(x => x.SocialUser.Name))
                .ForMember(dest => dest.ScreenName, src => src.MapFrom(x => x.SocialUser.ScreenName))
                .ForMember(dest => dest.Avatar, src => src.MapFrom(x => x.SocialUser.Avatar));
            CreateMap<UpdateTwitterAccountDto, SocialAccount>();

            CreateMap<Agent, AgentDto>();
            CreateMap<Department, DepartmentDto>();
            CreateMap<MessageAttachment, MessageAttachmentUrlDto>();

            CreateMap<Filter, FilterCacheItem>();
            CreateMap<FilterCacheItem, Filter>();
            CreateMap<FilterCondition, FilterConditionCache>();
            CreateMap<FilterConditionCache, FilterCondition>();
            CreateMap<ConversationField, ConversationFieldCache>();
            CreateMap<ConversationFieldCache, ConversationField>();
        }
    }
}
