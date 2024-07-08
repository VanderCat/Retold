using System.Net.Sockets;
using VkNet;
using VkNet.Exception;
using VkNet.Model;
using VkNet.Utils;
using VkNet.Utils.BotsLongPoll;

namespace Retold; 

public static class Vk {
    public static void Initialize() {
        Api = new VkApi();
        Api.Authorize(new ApiAuthParams {
            AccessToken = Settings.Instance.Credentials.Vk.Token,
        });
        StartLongpoll();
        
        Service = new VkApi();
        Service.Authorize(new ApiAuthParams {
            AccessToken = Settings.Instance.Credentials.Vk.AppToken
        });
    }

    public delegate void NewMessageEvent(Message messageNew);

    public static event NewMessageEvent OnNewMessage;
    

    public static async void StartLongpoll() { 
        var longPoll = new BotsLongPollUpdatesHandler(new(Api, Settings.Instance.Credentials.Vk.GroupId) {
            Ts = null,
            GetPause = () => false,
            OnException = (exception) => 
            {
                Console.WriteLine(exception);
            },
            OnWarn = (exception) =>
            {
                switch (exception)
                {
                    case PublicServerErrorException:
                    case HttpRequestException:
                    case SocketException:
                    {
                        // Игнорируем ошибки, связанные с интернетом или сервером ВКонтакте
                        return;
                    }
                    default:
                    {
                        Console.WriteLine(exception);
                        return;
                    }
                }
            },

            OnUpdates = (e) =>
            {
                var updates = new List<GroupUpdate>();
                foreach (var updateEvent in e.Updates)
                {
                    if (updateEvent.Update is not null)
                    {
                        updates.Add(updateEvent.Update);
                        continue;
                    }
                    if (updateEvent.Exception is null)
                        continue;
                }
                if (!updates.Any()) return;
                
                var newMessages = updates.Where(x => x.Instance is MessageNew)
                    .Select(x => x.Instance as MessageNew);
                foreach (var message in newMessages) {
                    //try {
                        OnNewMessage.Invoke(message.Message);
                    //}
                    //catch (Exception ex) {
                        //Console.WriteLine(ex);
                    //}
                }
            }
        });
        await longPoll.RunAsync();
    }

    public static VkApi Api;
    public static VkApi Service;

    public static void SendToChat(string message) {
        Api.Messages.Send(new MessagesSendParams {
            PeerId = Settings.Instance.Chat.Vk.PeerId,
            Message = message,
            RandomId = 0
        });
    }
}