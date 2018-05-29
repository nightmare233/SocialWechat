using Framework.Core;
using Social.Domain.DomainServices.Facebook;
using Social.Domain.Entities;
using Social.Infrastructure.Enum;
using Social.Infrastructure.Facebook;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Social.UnitTest.DomainServices.Facebook
{
    public class FacebookConverterTest
    {
        [Fact]
        public void ShouldConvertFbMessageToEntity()
        {
            // Arrange
            var fbAttachment = MakeFbAttachment(1);
            var fbMessage = new FbMessage
            {
                Id = "Test_Id",
                Content = "Test Message",
                SendTime = new DateTime(2020, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                Attachments = new List<FbMessageAttachment>
                {
                    fbAttachment
                }
            };
            var sender = new SocialUser { Id = 2 };
            var receiver = new SocialUser { Id = 3 };
            var conveter = new FacebookConverter();

            // Act
            Message message = conveter.ConvertMessage(fbMessage, sender, receiver);

            // Assert
            Assert.Equal(fbMessage.Id, message.OriginalId);
            Assert.Equal(fbMessage.SendTime, message.SendTime);
            Assert.Equal(fbMessage.Content, message.Content);
            Assert.Equal(MessageSource.FacebookMessage, message.Source);
            Assert.Equal(sender.Id, message.SenderId);
            Assert.Equal(receiver.Id, message.ReceiverId);
            Assert.Equal(fbAttachment.Id, message.Attachments[0].OriginalId);
            Assert.Equal(fbAttachment.Name, message.Attachments[0].Name);
            Assert.Equal(fbAttachment.MimeType, message.Attachments[0].MimeType);
            Assert.Equal(fbAttachment.Type, message.Attachments[0].Type);
            Assert.Equal(fbAttachment.Size, message.Attachments[0].Size);
            Assert.Equal(fbAttachment.Url, message.Attachments[0].Url);
            Assert.Equal(fbAttachment.PreviewUrl, message.Attachments[0].PreviewUrl);
        }

        [Fact]
        public void ShouldConvertToMessageAttachmentForPhoto()
        {
            // Arrange
            var fbAttachment = new FbAttachment
            {
                type = "photo",
                url = "http://test/test.jpg",
                media = new FbAttachmentMedia
                {
                    image = new FbAttachmentImage
                    {
                        src = "http://test/123.jpg"
                    }
                }
            };
            var conveter = new FacebookConverter();

            // Act
            MessageAttachment message = conveter.ConvertToAttachment(fbAttachment);

            // Assert
            Assert.Equal(MessageAttachmentType.Image, message.Type);
            Assert.Equal(fbAttachment.url, message.OriginalLink);
            Assert.Equal(fbAttachment.media.image.src, message.PreviewUrl);
            Assert.Equal("image/jpeg", message.MimeType);
        }

        [Fact]
        public void ShouldConvertToMessageAttachmentForSticker()
        {
            // Arrange
            var fbAttachment = new FbAttachment
            {
                type = "sticker",
                url = "http://test/test.jpg",
                media = new FbAttachmentMedia
                {
                    image = new FbAttachmentImage
                    {
                        src = "http://test/123.jpg"
                    }
                }
            };
            var conveter = new FacebookConverter();

            // Act
            MessageAttachment message = conveter.ConvertToAttachment(fbAttachment);

            // Assert
            Assert.Equal(MessageAttachmentType.Image, message.Type);
            Assert.Equal(fbAttachment.url, message.OriginalLink);
            Assert.Equal(fbAttachment.media.image.src, message.PreviewUrl);
            Assert.Equal("image/jpeg", message.MimeType);
        }

        [Fact]
        public void ShouldConvertToMessageAttachmentForAnimatedImage()
        {
            // Arrange
            var fbAttachment = new FbAttachment
            {
                type = "animated_image",
                url = "http://test/test.gif",
                media = new FbAttachmentMedia
                {
                    image = new FbAttachmentImage
                    {
                        src = "http://test/123.jpg"
                    }
                }
            };
            var conveter = new FacebookConverter();

            // Act
            MessageAttachment message = conveter.ConvertToAttachment(fbAttachment);

            // Assert
            Assert.Equal(MessageAttachmentType.AnimatedImage, message.Type);
            Assert.Equal(fbAttachment.url, message.OriginalLink);
            Assert.Equal(fbAttachment.media.image.src, message.PreviewUrl);
            Assert.Equal("image/gif", message.MimeType);
        }

        [Fact]
        public void ShouldConvertToMessageAttachmentForVideo()
        {
            // Arrange
            var fbAttachment = new FbAttachment
            {
                type = "video",
                url = "http://test/test.mp4",
                media = new FbAttachmentMedia
                {
                    image = new FbAttachmentImage
                    {
                        src = "http://test/123.gif"
                    }
                }
            };
            var conveter = new FacebookConverter();

            // Act
            MessageAttachment message = conveter.ConvertToAttachment(fbAttachment);

            // Assert
            Assert.Equal(MessageAttachmentType.Video, message.Type);
            Assert.Equal(fbAttachment.url, message.OriginalLink);
            Assert.Equal(fbAttachment.media.image.src, message.PreviewUrl);
            Assert.Equal("video/mp4", message.MimeType);
        }


        [Fact]
        public void ShouldConvertToMessageAttachmentForFile()
        {
            // Arrange
            var fbAttachment = new FbAttachment
            {
                type = "xxxaaa",
                url = "http://test/test.txt",
            };
            var conveter = new FacebookConverter();

            // Act
            MessageAttachment message = conveter.ConvertToAttachment(fbAttachment);

            // Assert
            Assert.Equal(MessageAttachmentType.File, message.Type);
            Assert.Equal(fbAttachment.url, message.OriginalLink);
            Assert.Equal("text/plain", message.MimeType);
        }

        [Fact]
        public void ShouldNormalizeAttachmentType()
        {
            // Arrange
            var fbAttachment = new FbAttachment
            {
                type = "animated_image",
                url = "http://test/test.mp4",
                media = new FbAttachmentMedia
                {
                    image = new FbAttachmentImage
                    {
                        src = "http://test/123.jpg"
                    }
                }
            };
            var conveter = new FacebookConverter();

            // Act
            MessageAttachment message = conveter.ConvertToAttachment(fbAttachment);


            // Assert
            Assert.Equal(MessageAttachmentType.Video, message.Type);
            Assert.Equal("video/mp4", message.MimeType);
        }


        private FbMessageAttachment MakeFbAttachment(int Id)
        {
            return new FbMessageAttachment
            {
                Id = $"Test_Attach_{Id}",
                Name = $"Test_Attach_Name_{Id}",
                MimeType = "text/plain",
                Type = MessageAttachmentType.File,
                Size = 100,
                Url = $"http://test/{Id}",
                PreviewUrl = $"http://test/{Id}/preview"
            };
        }
    }
}
