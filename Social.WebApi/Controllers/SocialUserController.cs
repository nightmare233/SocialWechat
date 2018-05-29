using Social.Application.AppServices;
using Social.Application.Dto;
using Social.Application.Dto.UserInfo;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;

namespace Social.WebApi.Controllers
{
    [RoutePrefix("api/social-users")]
    public class SocialUserController : ApiController
    {
        private ISocialUserAppService _appService;

        public SocialUserController(ISocialUserAppService appService)
        {
            _appService = appService;
        }

        /// <summary>
        /// Get public info for social user
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("{id}/public-info")]
        public async Task<UserInfoDto> GetUserInfo(int id)
        {
            return await _appService.FindPublicInfoAsync(id);
        }

        /// <summary>
        /// Get conversations which social user involved.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("{id}/conversations")]
        public IList<ConversationDto> GetUserConversations(int id)
        {
            return _appService.FindConversations(id);
        }
    }
}