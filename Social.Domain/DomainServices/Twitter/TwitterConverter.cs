using Framework.Core;
using Social.Domain.Entities;
using Social.Infrastructure.Enum;
using System;
using System.Linq;
using Tweetinvi.Models;
using Tweetinvi.Models.Entities;

namespace Social.Domain.DomainServices.Twitter
{
    public static class TwitterConverter
    {
        public static Message ConvertToMessage(IMessage directMsg)
        {
            var message = new Message
            {
                Source = MessageSource.TwitterDirectMessage,
                Content = directMsg.Text,
                OriginalId = directMsg.Id.ToString(),
                SendTime = directMsg.CreatedAt.ToUniversalTime()
            };

            return message;
        }

        public static Message ConvertToMessage(ITweet tweet)
        {
            var message = new Message
            {
                Source = tweet.QuotedStatusId == null ? MessageSource.TwitterTypicalTweet : MessageSource.TwitterQuoteTweet,
                OriginalId = tweet.IdStr,
                SendTime = tweet.CreatedAt.ToUniversalTime(),
                Content = string.IsNullOrWhiteSpace(tweet.Text) ? tweet.FullText : tweet.Text,
                OriginalLink = tweet.Url
            };
            if (tweet.QuotedStatusId != null)
            {
                message.QuoteTweetId = tweet.QuotedStatusIdStr;
            }

            if (tweet.Media != null)
            {
                foreach (var media in tweet.Media)
                {
                    message.Attachments.Add(ConvertToMessageAttachment(media));
                }
            }

            return message;
        }

        private static MessageAttachment ConvertToMessageAttachment(IMediaEntity media)
        {
            var messageAttachment = new MessageAttachment { };
            MessageAttachmentType type = MessageAttachmentType.File;
            if (media.MediaType == "animated_gif")
            {
                type = MessageAttachmentType.AnimatedImage;
            }
            if (media.MediaType == "photo")
            {
                type = MessageAttachmentType.Image;
            }
            if (media.MediaType == "vedio" || media.MediaType == "video")
            {
                type = MessageAttachmentType.Video;
            }
            messageAttachment = new MessageAttachment
            {
                Type = type,
                Url = media.MediaURL,
                PreviewUrl = media.MediaURL,
                MimeType = new Uri(media.MediaURL).GetMimeType(),
                OriginalId = media.IdStr,
                OriginalLink = media.URL
            };
            if (media.VideoDetails != null && media.VideoDetails.Variants.Any())
            {
                var video = media.VideoDetails.Variants.FirstOrDefault(t => t.Bitrate > 0);
                if (video == null)
                {
                    video = media.VideoDetails.Variants.FirstOrDefault();
                }

                messageAttachment = new MessageAttachment
                {
                    Type = type,
                    Url = video.URL,
                    PreviewUrl = media.MediaURL,
                    MimeType = video.ContentType,
                    OriginalId = media.IdStr,
                    OriginalLink = media.URL
                };
            }
                       
            NormarlizeAttachmentType(messageAttachment);

            return messageAttachment;
        }

        private static void NormarlizeAttachmentType(MessageAttachment attachment)
        {
            if (!string.IsNullOrEmpty(attachment.MimeType))
            {
                if (attachment.MimeType.StartsWith("image/", StringComparison.InvariantCultureIgnoreCase))
                {
                    if (attachment.Type != MessageAttachmentType.AnimatedImage)
                    {
                        attachment.Type = MessageAttachmentType.Image;
                    }
                }

                if (attachment.MimeType.StartsWith("video/", StringComparison.InvariantCultureIgnoreCase))
                {
                    attachment.Type = MessageAttachmentType.Video;
                }
            }
        }

    }
}
