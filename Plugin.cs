using System;
using System.Globalization;
using System.Linq;
using RFLobbyModifier.Models;
using RFLobbyModifier.Utils;
using Rocket.Core;
using Rocket.Core.Plugins;
using SDG.Framework.Modules;
using SDG.Unturned;
using Steamworks;
using Logger = Rocket.Core.Logging.Logger;

namespace RFLobbyModifier
{
    public class Plugin : RocketPlugin<Configuration>
    {
        private static int Major = 1;
        private static int Minor = 0;
        private static int Patch = 3;
        
        internal ServerModel OrigServerModel;
        internal GameTagsModel OrigGameTagsModel;
        public static Plugin Inst;
        public static Configuration Conf;

        protected override void Load()
        {
            Inst = this;
            Conf = Configuration.Instance;

            if (Conf.Enabled)
            {
                Level.onPostLevelLoaded += OnPostLevelLoaded;
                
                if (Level.isLoaded)
                    LobbyUtil.ModServer();
            }
            else
                Logger.LogWarning($"[{Name}] Plugin: DISABLED");

            Logger.LogWarning($"[{Name}] Plugin loaded successfully!");
            Logger.LogWarning($"[{Name}] {Name} v{Major}.{Minor}.{Patch}");
            Logger.LogWarning($"[{Name}] Made with 'rice' by RiceField Plugins!");
        }

        protected override void Unload()
        {
            if (Conf.Enabled)
            {
                Level.onPostLevelLoaded -= OnPostLevelLoaded;
                
                if (Conf.RevertOnUnload)
                    LobbyUtil.RevertServer();
            }

            Inst = null;
            Conf = null;

            Logger.LogWarning($"[{Name}] Plugin unloaded successfully!");
        }

        private static void OnPostLevelLoaded(int a)
        {
            LobbyUtil.ModServer();
        }
    }
}