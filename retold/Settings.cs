using Microsoft.Extensions.Configuration;

namespace Retold; 

public sealed class Settings {
    public sealed class CredentialsSettings {
        public sealed class VkCredentialsSettings {
            public required string Token { get; set; }
            public required string AppToken { get; set; }
            public required ulong GroupId { get; set; }
        }
        public sealed class DiscordCredentialsSettings {
            public required string Webhook { get; set; }
            public required string Token { get; set; }
        }
        public required VkCredentialsSettings Vk { get; set; }
        public required DiscordCredentialsSettings Discord { get; set; }
    }

    public sealed class ChatSettings {
        public sealed class DiscordChatSettings {
            public required long GuildId { get; set; }
            public required long ChannelId { get; set; }
        }
        public sealed class VkChatSettings {
            public required long PeerId { get; set; }
        }

        public required DiscordChatSettings Discord { get; set; }
        public required VkChatSettings Vk { get; set; }
    }

    public required CredentialsSettings Credentials { get; set; }
    public required ChatSettings Chat { get; set; }

    private static Settings? _instance;
    public static Settings Instance => _instance??=new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json", true)
        .Build().Get<Settings>();
}

