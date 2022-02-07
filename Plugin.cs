using System;
using System.Globalization;
using System.Linq;
using RFServerMess.Models;
using Rocket.Core;
using Rocket.Core.Plugins;
using SDG.Framework.Modules;
using SDG.Unturned;
using Steamworks;
using Logger = Rocket.Core.Logging.Logger;

namespace RFServerMess
{
    public class Plugin : RocketPlugin<Configuration>
    {
        internal ServerModel OrigServerModel;
        internal GameTagsModel OrigGameTagsModel;
        public Plugin Inst;
        public Configuration Conf;

        protected override void Load()
        {
            Inst = this;
            Conf = Configuration.Instance;

            if (Conf.Enabled)
            {
                if (Level.isLoaded)
                    EditServer();

                Level.onPostLevelLoaded += OnPostLevelLoaded;
            }
            else
                Logger.LogError("[RFServerMess] Plugin: DISABLED");

            Logger.LogWarning("[RFServerMess] Plugin loaded successfully!");
            Logger.LogWarning("[RFServerMess] RFServerMess v1.0.0");
            Logger.LogWarning("[RFServerMess] Made with 'rice' by RiceField Plugins!");
        }

        protected override void Unload()
        {
            if (Conf.Enabled)
            {
                if (Conf.RevertOnUnload)
                    RevertServer();

                Level.onPostLevelLoaded -= OnPostLevelLoaded;
            }

            Inst = null;
            Conf = null;

            Logger.LogWarning("[RFServerMess] Plugin unloaded successfully!");
        }

        private void OnPostLevelLoaded(int a)
        {
            EditServer();
        }

        private int GetConfigurationCount()
        {
            return (string.Join(",", typeof(ModeConfigData).GetFields()
                .SelectMany(x => x.FieldType.GetFields().Select(y => y.GetValue(x.GetValue(Provider.modeConfigData))))
                .Select(x => x is bool v ? v ? "T" : "F" : (string.Empty + x)).ToArray()).Length - 1) / 120 + 1;
        }

        private int GetWorkshopCount()
        {
            return (string.Join(",", Provider.getServerWorkshopFileIDs().Select(x => x.ToString()).ToArray()).Length -
                    1) / 120 + 1;
        }

        public static bool IsOpenModPresent() =>
            AppDomain.CurrentDomain.GetAssemblies().Any(x => x.GetName().Name == "OpenMod.Core");
        
        private void EditServer()
        {
            #region ServerRelated

            var origMaxPlayer = Provider.maxPlayers;
            var origServerName = Provider.serverName;
            var origServerPassword = Provider.serverPassword;
            var origServerPasswordProtected = Provider.serverPassword != "" ? "PASS" : "SSAP";
            var origMapName = Provider.map;
            var origVacSecure = Provider.configData.Server.VAC_Secure ? "VAC_ON" : "VAC_OFF";
            var origGameVersionPacked = VersionUtils.binaryToHexadecimal(Provider.APP_VERSION_PACKED);
            var origMapVersion = VersionUtils.binaryToHexadecimal(Level.packedVersion);
            var origGameVersion = Provider.APP_VERSION;
            var origDescriptionServerList = Provider.configData.Browser.Desc_Server_List;
            var origBrowserIcon = Provider.configData.Browser.Icon;
            var origBrowserDescriptionHint = Provider.configData.Browser.Desc_Hint;
            var origBrowserDescriptionFull = Provider.configData.Browser.Desc_Full;
            var origServerWorkshopFileIDs = Provider.getServerWorkshopFileIDs();
            var origCustomLinks = Provider.configData.Browser.Links;

            var editServerPasswordProtected = Conf.ServerPassword != "" ? "PASS" : "SSAP";
            var editVacSecure = Conf.ServerIsVACSecure ? "VAC_ON" : "VAC_OFF";
            var editGameVersionPacked = VersionUtils.binaryToHexadecimal(Conf.GameVersionPacked);
            var editMapVersion = VersionUtils.binaryToHexadecimal(Conf.MapVersion);

            if (Conf.EditServerMaxPlayer)
                SteamGameServer.SetMaxPlayerCount(Conf.ServerMaxPlayer);
            if (Conf.EditServerName)
                SteamGameServer.SetServerName(Conf.ServerName);
            if (Conf.EditServerPassword)
                SteamGameServer.SetPasswordProtected(Conf.ServerPassword != "");
            if (Conf.EditServerMap)
                SteamGameServer.SetMapName(Conf.ServerMap);

            SteamGameServer.SetGameData(
                (Conf.EditServerPassword ? editServerPasswordProtected : origServerPasswordProtected) + "," +
                (Conf.EditServerVACSecure ? editVacSecure : origVacSecure) + ",GAME_VERSION_" +
                (Conf.EditGameVersionPacked ? editGameVersionPacked : origGameVersionPacked) + ",MAP_VERSION_" +
                (Conf.EditMapVersion ? editMapVersion : origMapVersion));

            if (Conf.EditGameVersion)
                SteamGameServer.SetKeyValue("GameVersion", Conf.GameVersion);
            if (Conf.EditDescriptionServerList)
            {
                if (Conf.DescriptionServerList.Length > 64)
                    Logger.LogError("[RFServerMess] Error: DescriptionServerList char limit is 64 chars");
                SteamGameServer.SetGameDescription(Conf.DescriptionServerList);
            }

            if (Conf.EditBrowserIcon)
                SteamGameServer.SetKeyValue("Browser_Icon", Conf.BrowserIcon);
            if (Conf.EditBrowserDescriptionHint)
                SteamGameServer.SetKeyValue("Browser_Desc_Hint", Conf.BrowserDescriptionHint);

            #endregion

            #region GameTags

            var origIsPvP = Provider.isPvP ? "PVP" : "PVE";
            var origHasCheats = Provider.hasCheats ? "CHy" : "CHn";
            var origMode = Provider.getModeTagAbbreviation(Provider.mode);
            var origCameraMode = Provider.getCameraModeTagAbbreviation(Provider.cameraMode);
            var origWorkshop = (Provider.getServerWorkshopFileIDs().Count > 0) ? "WSy" : "WSn";
            var origIsGold = Provider.isGold ? "GLD" : "F2P";
            var origBattlEyeSecure = (Provider.configData.Server.BattlEye_Secure ? "BEy" : "BEn");
            var origMonetization = Provider.GetMonetizationTagAbbreviation(Provider.configData.Browser.Monetization);
            var origThumbnail = Provider.configData.Browser.Thumbnail;
            var origNetTransport = "sns";
            var origPluginFramework = IsOpenModPresent() ? "om" : "rm";

            var editIsPvP = Conf.IsPVP ? "PVP" : "PVE";
            var editHasCheats = Conf.HasCheats ? "CHy" : "CHn";
            var editWorkshop = (Conf.WorkshopLines.Length > 0) ? "WSy" : "WSn";
            var editIsGold = Conf.IsGold ? "GLD" : "F2P";
            var editBattlEyeSecure = (Conf.IsBattlEyeSecure ? "BEy" : "BEn");
            var editThumbnail = Conf.Thumbnail;

            string editMode;
            switch (Conf.Mode.ToLower().Trim())
            {
                case "easy":
                    editMode = "EZY";
                    break;
                case "hard":
                    editMode = "HRD";
                    break;
                case "normal":
                    editMode = "NRM";
                    break;
                default:
                    editMode = "NRM";
                    break;
            }

            string editPerspective;
            switch (Conf.Perspective.ToLower().Trim())
            {
                case "first":
                    editPerspective = "1Pp";
                    break;
                case "both":
                    editPerspective = "2Pp";
                    break;
                case "third":
                    editPerspective = "3Pp";
                    break;
                case "vehicle":
                    editPerspective = "4Pp";
                    break;
                default:
                    editPerspective = "2Pp";
                    break;
            }

            string editMonetization;
            switch (Conf.Monetization.ToLower().Trim())
            {
                // case "unspecified":
                //     editMonetization = "MTXy";
                //     break;
                // case "any":
                //     editMonetization = "MTXy";
                //     break;
                case "none":
                    editMonetization = "MTXn";
                    break;
                // case "nongameplay":
                //     editMonetization = "MTXy";
                //     break;
                default:
                    editMonetization = "MTXy";
                    break;
            }

            string editNetTransport;
            switch (Conf.NetTransport.ToLower().Trim())
            {
                case "systemsocket":
                    editNetTransport = "sys";
                    break;
                case "steamnetworkingsockets":
                    editNetTransport = "sns";
                    break;
                case "steamnetworking":
                    editNetTransport = "def";
                    break;
                default:
                    editNetTransport = "sns";
                    break;
            }

            string editPluginFramework;
            switch (Conf.PluginFramework.ToLower().Trim())
            {
                case "rocket":
                    editPluginFramework = "rm";
                    break;
                case "openmod":
                    editPluginFramework = "om";
                    break;
                default:
                    editPluginFramework = "rm";
                    break;
            }

            string gameTags = string.Concat(Conf.EditIsPvP ? editIsPvP : origIsPvP, ",",
                Conf.EditHasCheats ? editHasCheats : origHasCheats, ",", Conf.EditMode ? editMode : origMode, ",",
                Conf.EditPerspective ? editPerspective : origCameraMode, ",",
                Conf.EditWorkshop ? editWorkshop : origWorkshop, ",", Conf.EditIsGold ? editIsGold : origIsGold, ",",
                Conf.EditIsBattlEyeSecure ? editBattlEyeSecure : origBattlEyeSecure, ",",
                Conf.EditMonetization ? editMonetization : origMonetization, ",<tn>",
                Conf.EditThumbnail ? editThumbnail : origThumbnail, "</tn>", ",<net>",
                Conf.EditNetTransport ? editNetTransport : origNetTransport, "</net>", ",<pf>",
                Conf.EditPluginFramework ? editPluginFramework : origPluginFramework, "</pf>");
            SteamGameServer.SetGameTags(gameTags);

            #endregion

            #region Plugins

            if (Configuration.Instance.HideRocket)
            {
                SteamGameServer.SetBotPlayerCount(0);
            }

            if (!Configuration.Instance.HidePlugins)
            {
                if (Configuration.Instance.EditPlugins)
                    SteamGameServer.SetKeyValue("rocketplugins", string.Join(",", Configuration.Instance.PluginLines));
                else
                    SteamGameServer.SetKeyValue("rocketplugins",
                        string.Join(",", R.Plugins.GetPlugins().Select(p => p.Name).ToArray()));
            }
            else
            {
                SteamGameServer.SetKeyValue("rocketplugins", "");
            }

            if (Configuration.Instance.IsVanilla)
            {
                SteamGameServer.SetBotPlayerCount(0);
                SteamGameServer.SetKeyValue("rocketplugins", "");
                SteamGameServer.SetKeyValue("rocket", "");
            }
            else
            {
                if (!Configuration.Instance.HideRocket)
                {
                    SteamGameServer.SetBotPlayerCount(1);
                }

                if (!Configuration.Instance.HidePlugins && !Configuration.Instance.EditPlugins)
                {
                    SteamGameServer.SetKeyValue("rocketplugins",
                        string.Join(",", R.Plugins.GetPlugins().Select(p => p.Name).ToArray()));
                }

                string version = ModuleHook.modules.Find(a => a.config.Name == "Rocket.Unturned")?.config.Version ??
                                 "0.0.0.69";
                SteamGameServer.SetKeyValue("rocket", version);
            }

            #endregion

            #region Browser Description Full

            if (Conf.HideBrowserDescriptionFull)
                SteamGameServer.SetKeyValue("Browser_Desc_Full_Count", "0");
            if (Conf.EditBrowserDescriptionFull)
            {
                var browserDescriptionFullCount = Conf.BrowserDescriptionFullLines.Length;
                var i = 0;
                while (i < browserDescriptionFullCount)
                {
                    SteamGameServer.SetKeyValue("Browser_Desc_Full_Line_" + i.ToString(CultureInfo.InvariantCulture),
                        Conf.BrowserDescriptionFullLines[i]);
                    i++;
                }

                SteamGameServer.SetKeyValue("Browser_Desc_Full_Count", i.ToString(CultureInfo.InvariantCulture));
            }

            #endregion

            #region Workshop

            if (Conf.HideWorkshop)
            {
                SteamGameServer.SetKeyValue("Browser_Workshop_Count", "0");
            }
            else if (Conf.EditWorkshop)
            {
                var txt = string.Join(",", Conf.WorkshopLines);
                SteamGameServer.SetKeyValue("Browser_Workshop_Count", ((txt.Length - 1) / 120 + 1).ToString());

                var line = 0;
                for (var i = 0; i < txt.Length; i += 120)
                {
                    var num6 = 120;

                    if (i + num6 > txt.Length)
                        num6 = txt.Length - i;

                    var pValue2 = txt.Substring(i, num6);
                    SteamGameServer.SetKeyValue("Browser_Workshop_Line_" + line, pValue2);
                    line++;
                }
            }

            #endregion

            #region Configuration

            if (Conf.HideConfiguration)
                SteamGameServer.SetKeyValue("Browser_Config_Count", "0");
            if (Conf.EditConfiguration)
            {
                var configurationCount = Conf.ConfigurationLines.Length;
                var i = 0;
                while (i < configurationCount)
                {
                    SteamGameServer.SetKeyValue("Browser_Config_" + i.ToString(CultureInfo.InvariantCulture),
                        Conf.ConfigurationLines[i]);
                    i++;
                }

                SteamGameServer.SetKeyValue("Browser_Config_Count", i.ToString(CultureInfo.InvariantCulture));
            }

            #endregion

            #region Custom Links

            if (Conf.HideCustomLinks)
                SteamGameServer.SetKeyValue("Custom_Links_Count", "0");
            if (Conf.EditCustomLinks)
            {
                var customLinkCount = Conf.CustomLinkLines.Count;
                var i = 0;
                while (i < customLinkCount)
                {
                    SteamGameServer.SetKeyValue("Custom_Link_Message_" + i.ToString(CultureInfo.InvariantCulture),
                        Conf.CustomLinkLines[i].Message);
                    SteamGameServer.SetKeyValue("Custom_Link_Url_" + i.ToString(CultureInfo.InvariantCulture),
                        Conf.CustomLinkLines[i].Url);

                    i++;
                }

                SteamGameServer.SetKeyValue("Custom_Links_Count", i.ToString(CultureInfo.InvariantCulture));
            }

            #endregion

            if (!Conf.RevertOnUnload)
                return;
            OrigServerModel = new ServerModel
            {
                BrowserIcon = origBrowserIcon,
                CustomLinks = origCustomLinks,
                MapName = origMapName,
                MapVersion = origMapVersion,
                MaxPlayers = origMaxPlayer,
                ServerName = origServerName,
                ServerPassword = origServerPassword,
                VacSecure = origVacSecure,
                BrowserDescriptionFull = origBrowserDescriptionFull,
                BrowserDescriptionHint = origBrowserDescriptionHint,
                DescriptionServerList = origDescriptionServerList,
                GameVersionPacked = origGameVersionPacked,
                GameVersion = origGameVersion,
                ServerWorkshopFileIDs = origServerWorkshopFileIDs
            };

            OrigGameTagsModel = new GameTagsModel
            {
                Mode = origMode,
                Monetization = origMonetization,
                Thumbnail = origThumbnail,
                Workshop = origWorkshop,
                CameraMode = origCameraMode,
                HasCheats = origHasCheats,
                IsGold = origIsGold,
                NetTransport = origNetTransport,
                PluginFramework = origPluginFramework,
                BattlEyeSecure = origBattlEyeSecure,
                IsPvP = origIsPvP
            };
        }

        private void RevertServer()
        {
            try
            {
                if (Conf.EditServerMaxPlayer)
                    SteamGameServer.SetMaxPlayerCount(OrigServerModel.MaxPlayers);
                if (Conf.EditServerName)
                    SteamGameServer.SetServerName(OrigServerModel.ServerName);
                if (Conf.EditServerPassword)
                    SteamGameServer.SetPasswordProtected(OrigServerModel.ServerPassword != "");
                if (Conf.EditServerMap)
                    SteamGameServer.SetMapName(OrigServerModel.MapName);

                SteamGameServer.SetGameData(
                    (OrigServerModel.ServerPassword != ""
                        ? "PASS"
                        : "SSAP") + "," + OrigServerModel.VacSecure + ",GAME_VERSION_" +
                    OrigServerModel.GameVersionPacked + ",MAP_VERSION_" +
                    OrigServerModel.MapVersion);

                if (Conf.EditGameVersion)
                    SteamGameServer.SetKeyValue("GameVersion", OrigServerModel.GameVersion);
                if (Conf.EditDescriptionServerList)
                {
                    if (OrigServerModel.DescriptionServerList.Length > 64)
                    {
                        CommandWindow.LogWarning("Server browser thumbnail URL is " +
                                                 (OrigServerModel.DescriptionServerList.Length - 64) +
                                                 " characters over budget!");
                        CommandWindow.LogWarning("Server will not list properly until this URL is adjusted!");
                    }

                    SteamGameServer.SetGameDescription(OrigServerModel.DescriptionServerList);
                }

                if (Conf.EditBrowserIcon)
                    SteamGameServer.SetKeyValue("Browser_Icon", OrigServerModel.BrowserIcon);
                if (Conf.EditBrowserDescriptionHint)
                    SteamGameServer.SetKeyValue("Browser_Desc_Hint", OrigServerModel.BrowserDescriptionHint);

                if (Conf.EditBrowserDescriptionFull)
                {
                    var num5 = (OrigServerModel.BrowserDescriptionFull.Length - 1) / 120 + 1;
                    var num6 = 0;
                    SteamGameServer.SetKeyValue("Browser_Desc_Full_Count", num5.ToString());
                    for (var k = 0; k < OrigServerModel.BrowserDescriptionFull.Length; k += 120)
                    {
                        var num7 = 120;
                        if (k + num7 > OrigServerModel.BrowserDescriptionFull.Length)
                        {
                            num7 = OrigServerModel.BrowserDescriptionFull.Length - k;
                        }

                        var pValue = OrigServerModel.BrowserDescriptionFull.Substring(k, num7);
                        SteamGameServer.SetKeyValue("Browser_Desc_Full_Line_" + num6, pValue);
                        num6++;
                    }
                }

                if (Conf.EditWorkshop)
                {
                    if (OrigServerModel.ServerWorkshopFileIDs.Count > 0)
                    {
                        var text7 = string.Empty;
                        for (var l = 0; l < OrigServerModel.ServerWorkshopFileIDs.Count; l++)
                        {
                            if (text7.Length > 0)
                            {
                                text7 += ",";
                            }

                            text7 += OrigServerModel.ServerWorkshopFileIDs[l];
                        }

                        var num8 = (text7.Length - 1) / 120 + 1;
                        var num9 = 0;
                        SteamGameServer.SetKeyValue("Browser_Workshop_Count", num8.ToString());
                        for (var m = 0; m < text7.Length; m += 120)
                        {
                            var num10 = 120;
                            if (m + num10 > text7.Length)
                            {
                                num10 = text7.Length - m;
                            }

                            var pValue2 = text7.Substring(m, num10);
                            SteamGameServer.SetKeyValue("Browser_Workshop_Line_" + num9, pValue2);
                            num9++;
                        }
                    }
                }

                if (Conf.EditCustomLinks)
                {
                    if (OrigServerModel.CustomLinks != null && OrigServerModel.CustomLinks.Length != 0)
                    {
                        SteamGameServer.SetKeyValue("Custom_Links_Count",
                            OrigServerModel.CustomLinks.Length.ToString());
                        for (var n = 0; n < OrigServerModel.CustomLinks.Length; n++)
                        {
                            SteamGameServer.SetKeyValue("Custom_Link_Message_" + n,
                                OrigServerModel.CustomLinks[n].Message);
                            SteamGameServer.SetKeyValue("Custom_Link_Url_" + n, OrigServerModel.CustomLinks[n].Url);
                        }
                    }
                }

                if (Conf.EditConfiguration)
                {
                    var modeConfig = ConfigData.CreateDefault(false).getModeConfig(Provider.mode);
                    if (modeConfig == null)
                    {
                        CommandWindow.LogError("Unable to compare default for advertise config");
                        return;
                    }

                    var num = 0;
                    foreach (var fieldInfo in Provider.modeConfigData.GetType().GetFields())
                    {
                        var value = fieldInfo.GetValue(Provider.modeConfigData);
                        var value2 = fieldInfo.GetValue(modeConfig);
                        foreach (var fieldInfo2 in value.GetType().GetFields())
                        {
                            var value3 = fieldInfo2.GetValue(value);
                            var value4 = fieldInfo2.GetValue(value2);
                            string text = null;
                            var fieldType = fieldInfo2.FieldType;
                            if (fieldType == typeof(bool))
                            {
                                var flag = (bool) value3;
                                var flag2 = (bool) value4;
                                if (flag != flag2)
                                {
                                    text = string.Concat(fieldInfo.Name, ".", fieldInfo2.Name, "=", flag ? "T" : "F");
                                }
                            }
                            else if (fieldType == typeof(float))
                            {
                                var a = (float) value3;
                                var b = (float) value4;
                                if (!MathfEx.IsNearlyEqual(a, b, 0.0001f))
                                {
                                    text = string.Concat(fieldInfo.Name, ".", fieldInfo2.Name, "=",
                                        a.ToString(CultureInfo.InvariantCulture));
                                }
                            }
                            else if (fieldType == typeof(uint))
                            {
                                var num2 = (uint) value3;
                                var num3 = (uint) value4;
                                if (num2 != num3)
                                {
                                    text = string.Concat(fieldInfo.Name, ".", fieldInfo2.Name, "=",
                                        num2.ToString(CultureInfo.InvariantCulture));
                                }
                            }
                            else
                            {
                                CommandWindow.LogErrorFormat("Unable to advertise config type: {0}", fieldType);
                            }

                            if (!string.IsNullOrEmpty(text))
                            {
                                var pKey = "Browser_Config_" + num.ToString(CultureInfo.InvariantCulture);
                                num++;
                                SteamGameServer.SetKeyValue(pKey, text);
                            }
                        }
                    }

                    SteamGameServer.SetKeyValue("Browser_Config_Count", num.ToString(CultureInfo.InvariantCulture));
                }

                var gameTags = string.Concat(OrigGameTagsModel.IsPvP, ",", OrigGameTagsModel.HasCheats, ",",
                    OrigGameTagsModel.Mode, ",", OrigGameTagsModel.CameraMode, ",", OrigGameTagsModel.Workshop, ",",
                    OrigGameTagsModel.IsGold, ",", OrigGameTagsModel.BattlEyeSecure, ",",
                    OrigGameTagsModel.Monetization, ",<tn>", OrigGameTagsModel.Thumbnail, "</tn>");
                // if (Conf.EditNetTransport) ;
                //     gameTags += $",<net>{editNetTransport}</net>";
                // if (Conf.EditPluginFramework) ;
                //     gameTags += $",<pf>{editPluginFramework}</pf>";

                SteamGameServer.SetGameTags(gameTags);
            }
            catch (Exception e)
            {
                Logger.LogError("[RFServerMess] Error: " + e);
            }
        }
    }
}