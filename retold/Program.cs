using System.Net;
using Discord;
using VkNet.Enums.Filters;
using VkNet.Model;
using Attachment = VkNet.Model.Attachment;
using Poll = VkNet.Model.Poll;

namespace Retold;

public static class Program {
    public static async Task Main(string[] args) {
        Vk.Initialize();
        Discord.Initialize();

        Vk.OnNewMessage += async newMessage => {
            var usename = "Misingno";
            var url =
                "https://sun1-93.userapi.com/impf/DW4IDqvukChyc-WPXmzIot46En40R00idiUAXw/l5w5aIHioYc.jpg?quality=96&as=32x32,48x48,72x72,108x108,160x160,240x240,360x360&sign=10ad7d7953daabb7b0e707fdfb7ebefd&u=I6EtahnrCRLlyd0MhT2raQt6ydhuyxX4s72EHGuUSoM&cs=200x200";
            if (newMessage.FromId is null) {
                return;
            }
            if (newMessage.FromId == -224595944) return;
            if (newMessage.FromId > 0) {
                var user = Vk.Api.Users.Get(new [] {newMessage.FromId.Value}, ProfileFields.Photo200).First();
                usename = user.FirstName + " " + user.LastName;
                if (user.Photo200 is not null)
                    url = user.Photo200.AbsoluteUri;
            }
            else {
                var id = 0 - newMessage.FromId;
                var group = Vk.Api.Groups.GetById(null, id.ToString(), new GroupsFields()).First();
                usename = group.Name;
                if (group.Photo200 is not null)
                    url = group.Photo200.AbsoluteUri;
            }

            var embeds = new List<Embed>();
            if (newMessage.Attachments.Count > 0) {
                var photos = newMessage.Attachments.GetAttachments<Photo>();

                foreach (var photo in photos) {
                    var embed = new EmbedBuilder().WithImageUrl(photo.Sizes.Last().Url.AbsoluteUri).Build();
                    embeds.Add(embed);
                }

                var documents = newMessage.Attachments.GetAttachments<Document>();

                foreach (var document in documents) {
                    Embed embed;
                    if (document.Ext is "gif" or "png" or "jpg" or "jpeg")
                        embed = new EmbedBuilder().WithImageUrl(ResolveUrl(document.Uri)).Build();
                    else {
                        var builder = new EmbedBuilder();
                        builder.WithTitle(document.Title);
                        builder.WithUrl(document.Uri);
                        if (document.Preview is not null)
                            builder.WithThumbnailUrl(document.Preview.Photo.Sizes.Last().Url.AbsoluteUri);
                        embed = builder.Build();
                    }
                    embeds.Add(embed);
                }
                
                var stickers = newMessage.Attachments.GetAttachments<Sticker>();

                foreach (var sticker in stickers) {
                    var embed = new EmbedBuilder().WithImageUrl(sticker.ImagesWithBackground.Last().Url.AbsoluteUri).Build();
                    embeds.Add(embed);
                }
                
                var videos = newMessage.Attachments.GetAttachments<Video>();

                foreach (var video in videos) {
                    var embed = new EmbedBuilder()
                        .WithImageUrl(video.Image.Last().Url.AbsoluteUri)
                        .WithTitle(video.Title)
                        .WithDescription(video.Description)
                        .WithUrl($"https://vk.com/video{video.OwnerId}_{video.Id}")
                        .Build();
                    embeds.Add(embed);
                }

                var audios = newMessage.Attachments.GetAttachments<Audio>();
                
                foreach (var audio in audios) {
                    using (var client = new HttpClient()) {
                        await Discord.UploadAudio(await client.GetStreamAsync(audio.Url));
                    }
                    
                    var embed = new EmbedBuilder()
                        .WithTitle($"{audio.Artist} - {audio.Title}")
                        .WithDescription(TimeSpan.FromSeconds(audio.Duration).ToString("g"))
                        .Build();
                    embeds.Add(embed);
                }
            }
            var vkPoll = newMessage.Attachments.GetAttachments<Poll>().FirstOrDefault();
            PollProperties? poll = null;
            if (vkPoll is not null) {
                poll = new();
                poll.Answers = new List<PollMediaProperties>();
                poll.Question = new PollMediaProperties {Text = vkPoll.Question};
                poll.Duration = (uint)Math.Clamp((vkPoll.EndDate - DateTime.Now).Value.TotalHours, 1, 168);
                poll.LayoutType = PollLayout.Default;
                poll.AllowMultiselect = vkPoll.Multiple ?? false;
                    foreach (var answers in vkPoll.Answers) {
                    poll.Answers.Add(new PollMediaProperties{Text=answers.Text});
                }
            }
            //Discord.SetProfile();
            Discord.SendMessage(newMessage.Text, usename, url, embeds, poll);
            
        };
        
        await Task.Delay(-1);
    }

    public static string ResolveUrl(string url) {
        var req = (HttpWebRequest)WebRequest.Create(url);
        req.AllowAutoRedirect = false;
        var resp = req.GetResponse();
        return resp.Headers["Location"];
    }

    public static IEnumerable<T> GetAttachments<T>(this IEnumerable<Attachment> attachments) where T : MediaAttachment {
       return attachments.Where(
            attachment =>
                attachment.Instance.GetType().IsAssignableTo(typeof(T))
        ).Select(attachment => attachment.Instance).Cast<T>();
    }
}