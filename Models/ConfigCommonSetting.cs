namespace RFLobbyModifier.Models
{
    public class ConfigCommonSetting
    {
        public ConfigEntrySingleString BrowserDescription { get; set; }
        public ConfigEntrySingleString BrowserIcon { get; set; }
        public ConfigEntryStringArrayHideable LobbyConfiguration { get; set; }
        public ConfigEntryStringArrayHideable LobbyDescriptionFull { get; set; }
        public ConfigEntrySingleString LobbyDescriptionHint { get; set; }
        public ConfigEntryStringArrayHideable LobbyPlugins { get; set; }
        public ConfigEntryCustomLink LobbyCustomLinks { get; set; }
        public ConfigEntrySingleString LobbyThumbnail { get; set; }
        public ConfigCommonSetting()
        {
            
        }
    }
}