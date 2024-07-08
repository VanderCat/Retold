using Discord;
using Discord.Rest;
using Discord.Webhook;
using Discord.WebSocket;

namespace Retold; 

public static class Discord {
    private static DiscordWebhookClient _webhookClient;
    private static DiscordSocketClient _client;
    public static async void Initialize() {
        _webhookClient = new DiscordWebhookClient(Settings.Instance.Credentials.Discord.Webhook);
        _client = new DiscordSocketClient();
        await _client.LoginAsync(TokenType.Bot, Settings.Instance.Credentials.Discord.Token);
        await _client.StartAsync();
    }

    public static async void SetProfile(string name, Image? image = null) {
        await _webhookClient.ModifyWebhookAsync(properties => {
            properties.Name = name;
            if (image is not null)
                properties.Image = image;
        });
    }

    public static async Task UploadAudio(Stream audio) {
        (_client.GetGuild(721432646583976039).GetChannel(1259557174527655998) as IMessageChannel).SendFileAsync(audio, "audio.mp3");
    }

    
    public static async void SendMessage(string message, string name, string imgUrl, IEnumerable<Embed>? embeds = null, PollProperties? poll = null) {
        await _webhookClient.SendMessageAsync(message, username: name, avatarUrl:imgUrl, embeds:embeds, poll: poll);
    }
}