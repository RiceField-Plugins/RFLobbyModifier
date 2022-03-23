namespace RFLobbyModifier.Models
{
    public class ConfigAdvancedSetting
    {
        public ConfigEntrySingleBoolean BattlEyeSecure { get; set; }
        public ConfigEntrySingleString GameVersion { get; set; }
        public ConfigEntrySingleUInt GameVersionPacked { get; set; }
        public ConfigEntrySingleBoolean HasCheats { get; set; }
        public bool IsVanilla { get; set; }
        public ConfigEntrySingleUInt MapVersion { get; set; }
        public ConfigEntrySingleString NetTransport { get; set; }
        public ConfigEntrySingleString PluginFramework { get; set; }
        public ConfigEntrySingleBoolean ServerGold { get; set; }
        public ConfigEntrySingleString ServerMap { get; set; }
        public ConfigEntrySingleByte ServerMaxPlayer { get; set; }
        public ConfigEntrySingleString ServerMode { get; set; }
        public ConfigEntrySingleString ServerMonetization { get; set; }
        public ConfigEntrySingleString ServerName { get; set; }
        public ConfigEntrySingleString ServerPassword { get; set; }
        public ConfigEntrySingleString ServerPerspective { get; set; }
        public ConfigEntrySingleBoolean ServerPvP { get; set; }
        public ConfigEntrySingleBoolean VACSecure { get; set; }
        public ConfigEntryStringArrayHideable Workshop { get; set; }
        public ConfigAdvancedSetting()
        {
            
        }
    }
}