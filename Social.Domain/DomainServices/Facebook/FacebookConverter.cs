using Framework.Core;
using Social.Domain.Entities;
using Social.Infrastructure.Enum;
using Social.Infrastructure.Facebook;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Social.Domain.DomainServices.Facebook
{
    public class FacebookConverter
    {
        public Message ConvertMessage(FbMessage fbMessage, SocialUser Sender, SocialUser Receiver)
        {
            Message message = new Message
            {
                SenderId = Sender.Id,
                ReceiverId = Receiver.Id,
                Source = MessageSource.FacebookMessage,
                OriginalId = fbMessage.Id,
                SendTime = fbMessage.SendTime,
                Content = fbMessage.Content
            };

            foreach (var attachment in fbMessage.Attachments)
            {
                message.Attachments.Add(new MessageAttachment
                {
                    OriginalId = attachment.Id,
                    Name = attachment.Name,
                    MimeType = attachment.MimeType,
                    Type = attachment.Type,
                    Size = attachment.Size,
                    Url = attachment.Url,
                    PreviewUrl = attachment.PreviewUrl
                });
            }

            return message;
        }

        public Message ConvertToMessage(FbPost post)
        {
            var message = new Message
            {
                Source = MessageSource.FacebookPost,
                OriginalId = post.id,
                SendTime = post.created_time.ToUniversalTime(),
                Content = post.message,
                OriginalLink = post.permalink_url,
                Story = post.story
            };

            if (post.attachments != null)
            {
                foreach (var attachment in post.attachments.data)
                {
                    if (attachment.media != null && attachment.media.image != null)
                    {
                        message.Attachments.Add(ConvertToAttachment(attachment));
                    }
                }
            }

            return message;
        }

        public Message ConvertToMessage(FbComment comment)
        {
            Message message = new Message
            {
                Source = MessageSource.FacebookPostComment,
                OriginalId = comment.id,
                SendTime = comment.created_time.ToUniversalTime(),
                Content = comment.message,
                OriginalLink = comment.permalink_url,
            };

            if (comment.attachment != null && comment.attachment.media != null)
            {
                message.Attachments.Add(ConvertToAttachment(comment.attachment));
            }

            return message;
        }

        public MessageAttachment ConvertToAttachment(FbAttachment attachment)
        {
            MessageAttachment result;

            if (attachment.type == "photo" || attachment.type == "sticker")
            {
                result = new MessageAttachment
                {
                    OriginalLink = attachment.url,
                    Url = attachment.media.image.src,
                    PreviewUrl = attachment.media.image.src,
                    Type = MessageAttachmentType.Image,
                    MimeType = new Uri(attachment.media.image.src).GetMimeType()
                };
            }
            else if (attachment.type.Contains("animated_image"))
            {
                result = new MessageAttachment
                {
                    OriginalLink = attachment.url,
                    PreviewUrl = attachment.media.image.src,
                    Url = attachment.url,
                    Type = MessageAttachmentType.AnimatedImage,
                    MimeType = new Uri(attachment.url).GetMimeType()
                };
            }

            else if (attachment.type.Contains("video"))
            {
                result = new MessageAttachment
                {
                    OriginalLink = attachment.url,
                    PreviewUrl = attachment.media.image.src,
                    Url = attachment.url,
                    Type = MessageAttachmentType.Video,
                    MimeType = new Uri(attachment.url).GetMimeType()
                };
            }
            else
            {
                result = new MessageAttachment
                {
                    OriginalLink = attachment.url,
                    Url = attachment.url,
                    Type = MessageAttachmentType.File,
                    MimeType = new Uri(attachment.url).GetMimeType()
                };
            }

            NormarlizeAttachmentType(result);
            return result;
        }

        private void NormarlizeAttachmentType(MessageAttachment attachment)
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
                else if (attachment.MimeType.StartsWith("video/", StringComparison.InvariantCultureIgnoreCase))
                {
                    attachment.Type = MessageAttachmentType.Video;
                }

                else if (attachment.MimeType.StartsWith("audio/", StringComparison.InvariantCultureIgnoreCase))
                {
                    attachment.Type = MessageAttachmentType.Audio;
                }
                else
                {
                    attachment.Type = MessageAttachmentType.File;
                }

            }
        }
    }
}
