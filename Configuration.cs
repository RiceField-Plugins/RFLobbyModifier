using System.Collections.Generic;
using System.Xml.Serialization;
using RFServerMess.Models;
using Rocket.API;

namespace RFServerMess
{
    public class Configuration : IRocketPluginConfiguration
    {
        public bool Enabled;
        public bool RevertOnUnload;
        public bool EditServerMaxPlayer;
        public byte ServerMaxPlayer;
        public bool EditServerName;
        public string ServerName;
        public bool EditServerPassword;
        public string ServerPassword;
        public bool EditServerMap;
        public string ServerMap;
        public bool EditServerVACSecure;
        public bool ServerIsVACSecure;
        public bool EditGameVersionPacked;
        public uint GameVersionPacked;
        public bool EditMapVersion;
        public uint MapVersion;
        public bool EditGameVersion;
        public string GameVersion;
        public bool EditIsPvP;
        public bool IsPVP;
        public bool EditHasCheats;
        public bool HasCheats;
        public bool EditMode;
        public string Mode;
        public bool EditPerspective;
        public string Perspective;
        public bool EditIsGold;
        public bool IsGold;
        public bool EditIsBattlEyeSecure;
        public bool IsBattlEyeSecure;
        public bool EditMonetization;
        public string Monetization;
        public bool EditNetTransport;
        public string NetTransport;
        public bool EditPluginFramework;
        public string PluginFramework;
        public bool EditDescriptionServerList;
        public string DescriptionServerList;
        public bool EditThumbnail;
        public string Thumbnail;
        public bool EditBrowserIcon;
        public string BrowserIcon;
        public bool EditBrowserDescriptionHint;
        public string BrowserDescriptionHint;
        public bool HideBrowserDescriptionFull;
        public bool EditBrowserDescriptionFull;
        public string[] BrowserDescriptionFullLines;
        public bool HideWorkshop;
        public bool EditWorkshop;
        public string[] WorkshopLines;
        public bool HideConfiguration;
        public bool EditConfiguration;
        public string[] ConfigurationLines;
        public bool HidePlugins;
        public bool EditPlugins;
        public string[] PluginLines;
        public bool HideCustomLinks;
        public bool EditCustomLinks;
        [XmlArrayItem("CustomLink")]
        public List<CustomLinkModel> CustomLinkLines;
        public bool HideRocket;
        public bool IsVanilla;

        public void LoadDefaults()
        {
            Enabled = true;
            RevertOnUnload = true;
            EditServerMaxPlayer = false;
            ServerMaxPlayer = 24;
            EditServerName = false;
            ServerName = "Enter name";
            EditServerPassword = false;
            ServerPassword = "";
            EditServerMap = false;
            ServerMap = "PEI";
            EditServerVACSecure = false;
            ServerIsVACSecure = true;
            EditGameVersionPacked = false;
            GameVersionPacked = 123;
            EditMapVersion = false;
            MapVersion = 123;
            EditGameVersion = false;
            GameVersion = "test version";
            EditIsPvP = false;
            IsPVP = false;
            EditHasCheats = false;
            HasCheats = false;
            EditMode = false;
            Mode = "normal";
            EditPerspective = false;
            Perspective = "both";
            EditIsGold = false;
            IsGold = false;
            EditIsBattlEyeSecure = false;
            IsBattlEyeSecure = true;
            EditMonetization = false;
            Monetization = "any";
            EditNetTransport = false;
            NetTransport = "sns";
            EditPluginFramework = false;
            PluginFramework = "rm";
            EditDescriptionServerList = false;
            DescriptionServerList = "This is description in server list";
            EditThumbnail = false;
            Thumbnail = "url.com/image.jpg";
            EditBrowserIcon = false;
            BrowserIcon = "url.com/image.jpg";
            EditBrowserDescriptionHint = false;
            BrowserDescriptionHint = "This is description hint in lobby";
            HideBrowserDescriptionFull = false;
            EditBrowserDescriptionFull = true;
            BrowserDescriptionFullLines = new[]
            {
                "This is achieved with",
                "RFServerMess",
                "v1.0.0"
            };
            HideWorkshop = false;
            EditWorkshop = false;
            WorkshopLines = new[]
            {
                "1",
                "123",
                "321"
            };
            HideConfiguration = false;
            EditConfiguration = true;
            ConfigurationLines = new[]
            {
                "This.is=Config",
                "Test.config=2",
                "version.plugin=1"
            };
            HidePlugins = false;
            EditPlugins = true;
            PluginLines = new[]
            {
                "This is achieved with",
                "RFServerMess",
                "v1.0.0"
            };
            HideCustomLinks = false;
            EditCustomLinks = true;
            CustomLinkLines = new List<CustomLinkModel>
            {
                new CustomLinkModel("Test message1", "Testurl1.com"),
                new CustomLinkModel("Test message2", "Testurl2.com"),
                new CustomLinkModel("Test message3", "Testurl3.com"),
            };
            HideRocket = false;
            IsVanilla = false;
        }
    }
}
