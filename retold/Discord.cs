using Discord;
using Discord.Rest;
using Discord.Webhook;
using Discord.WebSocket;

namespace Retold; 

public static class Discord {
    private static DiscordWebhookClient _webhookClient;
    public static void Initialize() {
        _webhookClient = new DiscordWebhookClient(Settings.Instance.Credentials.Discord.Webhook);
    }

    public static async void SetProfile(string name, Image? image = null) {
        await _webhookClient.ModifyWebhookAsync(properties => {
            properties.Name = name;
            if (image is not null)
                properties.Image = image;
        });
    }

    
    public static async void SendMessage(string message, string name, string imgUrl) {
        await _webhookClient.SendMessageAsync(message, username: name, avatarUrl:imgUrl);
    }
}