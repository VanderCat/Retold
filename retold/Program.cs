using VkNet.Enums.Filters;
using VkNet.Model;

namespace Retold;

public static class Program {
    public static async Task Main(string[] args) {
        Vk.Initialize();
        Discord.Initialize();

        Vk.OnNewMessage += newMessage => {
            var usename = "Misingno";
            var url =
                "https://sun1-93.userapi.com/impf/DW4IDqvukChyc-WPXmzIot46En40R00idiUAXw/l5w5aIHioYc.jpg?quality=96&as=32x32,48x48,72x72,108x108,160x160,240x240,360x360&sign=10ad7d7953daabb7b0e707fdfb7ebefd&u=I6EtahnrCRLlyd0MhT2raQt6ydhuyxX4s72EHGuUSoM&cs=200x200";
            if (newMessage.FromId is null) {
                return;
            }
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
            //Discord.SetProfile();
            Discord.SendMessage(newMessage.Text, usename, url);
            
        };
        
        await Task.Delay(-1);
    }
}