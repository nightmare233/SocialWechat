using Social.Domain.Entities;
using Social.Infrastructure.Enum;
using Social.Infrastructure.Facebook;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tweetinvi.Models;

namespace Social.Application.Dto.UserInfo
{
    public class UserInfoDto
    {
        public int Id { get; set; }
        public SocialUserSource Source { get; set; }

        public string OriginalId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Avatar { get; set; }
        public string Link { get; set; }


        //Twitter
        public string Website { get; set; }
        public string ScreenName { get; set; }
        public DateTime? JoinedDate { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public int? FollowersCount { get; set; }
        public int? FriendsCount { get; set; }
        public int? StatusesCount { get; set; }
    }
}
