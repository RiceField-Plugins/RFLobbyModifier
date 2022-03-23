using System.Collections.Generic;
using System.Xml.Serialization;
using RFLobbyModifier.Models;
using Rocket.API;

namespace RFLobbyModifier
{
    public class Configuration : IRocketPluginConfiguration
    {
        public bool Enabled;
        public bool RevertOnUnload;

        public ConfigCommonSetting CommonSetting;
        public ConfigAdvancedSetting AdvancedSetting;

        public void LoadDefaults()
        {
            Enabled = true;
            RevertOnUnload = true;
            CommonSetting = new ConfigCommonSetting
            {
                BrowserDescription = new ConfigEntrySingleString(true, "<color=#00FFFF>Edited by RFLobbyModifier</color>"),
                BrowserIcon = new ConfigEntrySingleString(false, "url.com/image.jpg"),
                LobbyConfiguration = new ConfigEntryStringArrayHideable(false, true, new[]
                {
                    "This.is=RFLobbyModifier",
                    "Test.config=69",
                    "version.plugin=96"
                }),
                LobbyCustomLinks = new ConfigEntryCustomLink(false, true, new List<CustomLink>
                {
                    new CustomLink("Edited by RFLobbyModifier", "Testurl1.com"),
                    new CustomLink("Edited by RFLobbyModifier", "Testurl2.com"),
                    new CustomLink("Edited by RFLobbyModifier", "Testurl3.com"),
                }),
                LobbyDescriptionFull = new ConfigEntryStringArrayHideable(false, true, new[]
                {
                    "<color=white><size=25>Edited by</size></color>",
                    "<color=#00FFFF><size=40>RFLobbyModifier</size></color>"
                }),
                LobbyDescriptionHint = new ConfigEntrySingleString(true, "<color=#00FFFF>Edited by RFLobbyModifier</color>"),
                LobbyPlugins = new ConfigEntryStringArrayHideable(false, true, new[]
                {
                    "Edited by",
                    "RFLobbyModifier"
                }),
                LobbyThumbnail = new ConfigEntrySingleString(false, "url.com/image.jpg")
            };
            AdvancedSetting = new ConfigAdvancedSetting
            {
                BattlEyeSecure = new ConfigEntrySingleBoolean(false, true),
                GameVersion = new ConfigEntrySingleString(false, "imaginary version"),
                GameVersionPacked = new ConfigEntrySingleUInt(false, 69),
                HasCheats = new ConfigEntrySingleBoolean(false, false),
                MapVersion = new ConfigEntrySingleUInt(false, 69),
                NetTransport = new ConfigEntrySingleString(false, "sns"),
                PluginFramework = new ConfigEntrySingleString(false, "rm"),
                ServerGold = new ConfigEntrySingleBoolean(false, false),
                ServerMap = new ConfigEntrySingleString(false, "PEI"),
                ServerMaxPlayer = new ConfigEntrySingleByte(false, 69),
                ServerMode = new ConfigEntrySingleString(false, "normal"),
                ServerMonetization = new ConfigEntrySingleString(false, "any"),
                ServerName = new ConfigEntrySingleString(false, "RFLobbyModifier Server"),
                ServerPassword = new ConfigEntrySingleString(false, "theworldisinyourhand"),
                ServerPerspective = new ConfigEntrySingleString(false, "both"),
                ServerPvP = new ConfigEntrySingleBoolean(false, true),
                VACSecure = new ConfigEntrySingleBoolean(false, true),
                Workshop = new ConfigEntryStringArrayHideable(false, false, new[]
                {
                    "1",
                    "123",
                    "321"
                }),
            };
        }
    }
}
