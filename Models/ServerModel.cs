using System.Collections.Generic;
using SDG.Unturned;

namespace ServerMess.Models
{
    public class ServerModel
    {
        public byte MaxPlayers { get; set; }
        public string ServerName { get; set; }
        public string ServerPassword { get; set; }
        public string MapName { get; set; }
        public string VacSecure { get; set; }
        public string GameVersionPacked { get; set; }
        public string MapVersion { get; set; }
        public string GameVersion { get; set; }
        public string DescriptionServerList { get; set; }
        public string BrowserIcon { get; set; }
        public string BrowserDescriptionHint { get; set; }
        public string BrowserDescriptionFull { get; set; }
        public List<ulong> ServerWorkshopFileIDs { get; set; }
        public BrowserConfigData.Link[] CustomLinks { get; set; }
    }
}