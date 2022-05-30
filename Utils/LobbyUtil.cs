using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using RFLobbyModifier.Models;
using Rocket.Core;
using Rocket.Core.Logging;
using SDG.Framework.Modules;
using SDG.Unturned;
using Steamworks;

namespace RFLobbyModifier.Utils
{
    internal static class LobbyUtil
    {
        private static int GetConfigurationCount()
        {
            return (string.Join(",", typeof(ModeConfigData).GetFields()
                .SelectMany(x => x.FieldType.GetFields().Select(y => y.GetValue(x.GetValue(Provider.modeConfigData))))
                .Select(x => x is bool v ? v ? "T" : "F" : (string.Empty + x)).ToArray()).Length - 1) / 120 + 1;
        }

        private static int GetWorkshopCount()
        {
            return (string.Join(",", Provider.getServerWorkshopFileIDs().Select(x => x.ToString()).ToArray()).Length -
                    1) / 120 + 1;
        }

        private static bool IsOpenModPresent() =>
            AppDomain.CurrentDomain.GetAssemblies().Any(x => x.GetName().Name == "OpenMod.Core");

        internal static void ModServer()
        {
            #region ServerRelated

            var origMaxPlayer = Provider.maxPlayers;
            var origServerName = Provider.serverName ?? string.Empty;
            var origServerPassword = Provider.serverPassword ?? string.Empty;
            var origServerPasswordProtected = Provider.serverPassword != "" ? "PASS" : "SSAP";
            var origMapName = Provider.map ?? string.Empty;
            var origVacSecure = Provider.configData.Server.VAC_Secure ? "VAC_ON" : "VAC_OFF";
            var origGameVersionPacked = VersionUtils.binaryToHexadecimal(Provider.APP_VERSION_PACKED) ?? string.Empty;
            var origMapVersion = VersionUtils.binaryToHexadecimal(Level.packedVersion) ?? string.Empty;
            var origGameVersion = Provider.APP_VERSION ?? string.Empty;
            var origDescriptionServerList = Provider.configData.Browser.Desc_Server_List ?? " ";
            var origBrowserIcon = Provider.configData.Browser.Icon ?? string.Empty;
            var origBrowserDescriptionHint = Provider.configData.Browser.Desc_Hint ?? string.Empty;
            var origBrowserDescriptionFull = Provider.configData.Browser.Desc_Full ?? string.Empty;
            var origServerWorkshopFileIDs = Provider.getServerWorkshopFileIDs() ?? new List<ulong>();
            var origCustomLinks = Provider.configData.Browser.Links ?? Array.Empty<BrowserConfigData.Link>();

            var editServerPasswordProtected = Plugin.Conf.AdvancedSetting.ServerPassword.Value != ""
                ? "PASS"
                : "SSAP";
            var editVacSecure = Plugin.Conf.AdvancedSetting.VACSecure.Value ? "VAC_ON" : "VAC_OFF";
            var editGameVersionPacked =
                VersionUtils.binaryToHexadecimal(
                    Convert.ToUInt32(Plugin.Conf.AdvancedSetting.GameVersionPacked.Value)) ?? string.Empty;
            var editMapVersion =
                VersionUtils.binaryToHexadecimal(Convert.ToUInt32(Plugin.Conf.AdvancedSetting.MapVersion.Value)) ??
                string.Empty;

            if (Plugin.Conf.AdvancedSetting.ServerMaxPlayer.Edit)
                SteamGameServer.SetMaxPlayerCount(Convert.ToByte(Plugin.Conf.AdvancedSetting.ServerMaxPlayer.Value));

            if (Plugin.Conf.AdvancedSetting.ServerName.Edit)
                SteamGameServer.SetServerName(Plugin.Conf.AdvancedSetting.ServerName.Value);

            if (Plugin.Conf.AdvancedSetting.ServerPassword.Edit)
                SteamGameServer.SetPasswordProtected(
                    !string.IsNullOrWhiteSpace(Plugin.Conf.AdvancedSetting.ServerPassword.Value));

            if (Plugin.Conf.AdvancedSetting.ServerMap.Edit)
                SteamGameServer.SetMapName(Plugin.Conf.AdvancedSetting.ServerMap.Value);

            SteamGameServer.SetGameData(
                (Plugin.Conf.AdvancedSetting.ServerPassword.Edit
                    ? editServerPasswordProtected
                    : origServerPasswordProtected) + "," +
                (Plugin.Conf.AdvancedSetting.VACSecure.Edit ? editVacSecure : origVacSecure) + ",GAME_VERSION_" +
                (Plugin.Conf.AdvancedSetting.GameVersionPacked.Edit ? editGameVersionPacked : origGameVersionPacked) +
                ",MAP_VERSION_" +
                (Plugin.Conf.AdvancedSetting.MapVersion.Edit ? editMapVersion : origMapVersion));

            if (Plugin.Conf.AdvancedSetting.GameVersion.Edit)
                SteamGameServer.SetKeyValue("GameVersion",
                    Plugin.Conf.AdvancedSetting.GameVersion.Value);

            if (Plugin.Conf.CommonSetting.BrowserDescription.Edit)
            {
                if (Plugin.Conf.CommonSetting.BrowserDescription.Value.Length > 64)
                    Logger.LogError($"[{Plugin.Inst.Name}] [ERROR] BrowserDescription char limit is 64 chars");

                SteamGameServer.SetGameDescription(Plugin.Conf.CommonSetting.BrowserDescription.Value.ApplyRich());
            }

            if (Plugin.Conf.CommonSetting.BrowserIcon.Edit)
                SteamGameServer.SetKeyValue("Browser_Icon",
                    Plugin.Conf.CommonSetting.BrowserIcon.Value);
            if (Plugin.Conf.CommonSetting.LobbyDescriptionHint.Edit)
                SteamGameServer.SetKeyValue("Browser_Desc_Hint",
                    Plugin.Conf.CommonSetting.LobbyDescriptionHint.Value.ApplyRich());

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

            var editIsPvP = Plugin.Conf.AdvancedSetting.ServerPvP.Value ? "PVP" : "PVE";
            var editHasCheats = Plugin.Conf.AdvancedSetting.ServerPvP.Value ? "CHy" : "CHn";
            var editWorkshop = Plugin.Conf.AdvancedSetting.Workshop.Values.Length > 0 ? "WSy" : "WSn";
            var editIsGold = Plugin.Conf.AdvancedSetting.ServerGold.Value ? "GLD" : "F2P";
            var editBattlEyeSecure =
                Plugin.Conf.AdvancedSetting.BattlEyeSecure.Value ? "BEy" : "BEn";
            var editThumbnail = Plugin.Conf.CommonSetting.LobbyThumbnail.Value;

            string editMode;
            switch (Plugin.Conf.AdvancedSetting.ServerMode.Value.ToLower().Trim())
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
            switch (Plugin.Conf.AdvancedSetting.ServerPerspective.Value.ToLower().Trim())
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
            switch (Plugin.Conf.AdvancedSetting.ServerMonetization.Value.ToLower().Trim())
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
            switch (Plugin.Conf.AdvancedSetting.NetTransport.Value.ToLower().Trim())
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
            switch (Plugin.Conf.AdvancedSetting.PluginFramework.Value.ToLower().Trim())
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

            string gameTags = string.Concat(Plugin.Conf.AdvancedSetting.ServerPvP.Edit ? editIsPvP : origIsPvP, ",",
                Plugin.Conf.AdvancedSetting.HasCheats.Edit ? editHasCheats : origHasCheats, ",",
                Plugin.Conf.AdvancedSetting.ServerMode.Edit ? editMode : origMode, ",",
                Plugin.Conf.AdvancedSetting.ServerPerspective.Edit ? editPerspective : origCameraMode, ",",
                Plugin.Conf.AdvancedSetting.Workshop.Edit ? editWorkshop : origWorkshop, ",",
                Plugin.Conf.AdvancedSetting.ServerGold.Edit ? editIsGold : origIsGold, ",",
                Plugin.Conf.AdvancedSetting.BattlEyeSecure.Edit ? editBattlEyeSecure : origBattlEyeSecure, ",",
                Plugin.Conf.AdvancedSetting.ServerMonetization.Edit ? editMonetization : origMonetization, ",<tn>",
                Plugin.Conf.CommonSetting.LobbyThumbnail.Edit ? editThumbnail : origThumbnail, "</tn>", ",<net>",
                Plugin.Conf.AdvancedSetting.NetTransport.Edit ? editNetTransport : origNetTransport, "</net>", ",<pf>",
                Plugin.Conf.AdvancedSetting.PluginFramework.Edit ? editPluginFramework : origPluginFramework, "</pf>");
            SteamGameServer.SetGameTags(gameTags);

            #endregion

            #region Plugins

            if (!Plugin.Conf.CommonSetting.LobbyPlugins.Hide)
            {
                if (Plugin.Conf.CommonSetting.LobbyPlugins.Edit)
                    SteamGameServer.SetKeyValue("rocketplugins",
                        string.Join(",", Plugin.Conf.CommonSetting.LobbyPlugins.Values));
                else
                    SteamGameServer.SetKeyValue("rocketplugins",
                        string.Join(",", R.Plugins.GetPlugins().Select(p => p.Name).ToArray()));
            }
            else
            {
                SteamGameServer.SetKeyValue("rocketplugins", "");
            }

            if (Plugin.Conf.AdvancedSetting.IsVanilla)
            {
                SteamGameServer.SetBotPlayerCount(0);
                SteamGameServer.SetKeyValue("rocketplugins", "");
                SteamGameServer.SetKeyValue("rocket", "");
            }
            else
            {
                if (!Plugin.Conf.CommonSetting.LobbyPlugins.Hide && !Plugin.Conf.CommonSetting.LobbyPlugins.Edit)
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

            if (Plugin.Conf.CommonSetting.LobbyDescriptionFull.Hide)
                SteamGameServer.SetKeyValue("Browser_Desc_Full_Count", "0");

            if (Plugin.Conf.CommonSetting.LobbyDescriptionFull.Edit)
            {
                var browserDescriptionFullCount = Plugin.Conf.CommonSetting.LobbyDescriptionFull.Values.Length;
                var i = 0;
                while (i < browserDescriptionFullCount)
                {
                    SteamGameServer.SetKeyValue("Browser_Desc_Full_Line_" + i.ToString(CultureInfo.InvariantCulture),
                        Plugin.Conf.CommonSetting.LobbyDescriptionFull.Values[i]);
                    i++;
                }

                SteamGameServer.SetKeyValue("Browser_Desc_Full_Count", i.ToString(CultureInfo.InvariantCulture));
            }

            #endregion

            #region Workshop

            if (Plugin.Conf.AdvancedSetting.Workshop.Hide)
            {
                SteamGameServer.SetKeyValue("Mod_Count", "0");
            }
            else if (Plugin.Conf.AdvancedSetting.Workshop.Edit)
            {
                var txt = string.Join(",", Plugin.Conf.AdvancedSetting.Workshop.Values);
                SteamGameServer.SetKeyValue("Mod_Count", ((txt.Length - 1) / 120 + 1).ToString());
                var line = 0;
                for (var i = 0; i < txt.Length; i += 120)
                {
                    var num6 = 120;
                    if (i + num6 > txt.Length)
                        num6 = txt.Length - i;

                    var pValue2 = txt.Substring(i, num6);
                    SteamGameServer.SetKeyValue("Mod_" + line, pValue2);
                    line++;
                }
            }

            #endregion

            #region Configuration

            if (Plugin.Conf.CommonSetting.LobbyConfiguration.Hide)
                SteamGameServer.SetKeyValue("Cfg_Count", "0");
            
            if (Plugin.Conf.CommonSetting.LobbyConfiguration.Edit)
            {
                var configurationCount = Plugin.Conf.CommonSetting.LobbyConfiguration.Values.Length;
                var i = 0;
                while (i < configurationCount)
                {
                    SteamGameServer.SetKeyValue("Cfg_" + i.ToString(CultureInfo.InvariantCulture),
                        Plugin.Conf.CommonSetting.LobbyConfiguration.Values[i]);
                    i++;
                }

                SteamGameServer.SetKeyValue("Cfg_Count", i.ToString(CultureInfo.InvariantCulture));
            }

            #endregion

            #region Custom Links

            if (Plugin.Conf.CommonSetting.LobbyCustomLinks.Hide)
                SteamGameServer.SetKeyValue("Custom_Links_Count", "0");

            if (Plugin.Conf.CommonSetting.LobbyCustomLinks.Edit)
            {
                var customLinkCount = Plugin.Conf.CommonSetting.LobbyCustomLinks.Values.Count;
                var i = 0;
                while (i < customLinkCount)
                {
                    SteamGameServer.SetKeyValue("Custom_Link_Message_" + i.ToString(CultureInfo.InvariantCulture),
                        Plugin.Conf.CommonSetting.LobbyCustomLinks.Values[i].Message);
                    SteamGameServer.SetKeyValue("Custom_Link_Url_" + i.ToString(CultureInfo.InvariantCulture),
                        Plugin.Conf.CommonSetting.LobbyCustomLinks.Values[i].Url);

                    i++;
                }

                SteamGameServer.SetKeyValue("Custom_Links_Count", i.ToString(CultureInfo.InvariantCulture));
            }

            #endregion

            if (!Plugin.Conf.RevertOnUnload)
                return;

            Plugin.Inst.OrigServerModel = new ServerModel
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

            Plugin.Inst.OrigGameTagsModel = new GameTagsModel
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

        internal static void RevertServer()
        {
            if (!Level.isLoaded)
                return;
            
            try
            {
                if (Plugin.Conf.AdvancedSetting.ServerMaxPlayer.Edit)
                    SteamGameServer.SetMaxPlayerCount(Plugin.Inst.OrigServerModel.MaxPlayers);

                if (Plugin.Conf.AdvancedSetting.ServerName.Edit)
                    SteamGameServer.SetServerName(Plugin.Inst.OrigServerModel.ServerName);

                if (Plugin.Conf.AdvancedSetting.ServerPassword.Edit)
                    SteamGameServer.SetPasswordProtected(Plugin.Inst.OrigServerModel.ServerPassword != "");

                if (Plugin.Conf.AdvancedSetting.ServerMap.Edit)
                    SteamGameServer.SetMapName(Plugin.Inst.OrigServerModel.MapName);

                SteamGameServer.SetGameData(
                    (Plugin.Inst.OrigServerModel.ServerPassword != ""
                        ? "PASS"
                        : "SSAP") + "," + Plugin.Inst.OrigServerModel.VacSecure + ",GAME_VERSION_" +
                    Plugin.Inst.OrigServerModel.GameVersionPacked + ",MAP_VERSION_" +
                    Plugin.Inst.OrigServerModel.MapVersion);

                if (Plugin.Conf.AdvancedSetting.GameVersion.Edit)
                    SteamGameServer.SetKeyValue("GameVersion", Plugin.Inst.OrigServerModel.GameVersion);

                if (Plugin.Conf.CommonSetting.BrowserDescription.Edit)
                {
                    if (Plugin.Inst.OrigServerModel.DescriptionServerList.Length > 64)
                    {
                        CommandWindow.LogWarning("Server browser thumbnail URL is " +
                                                 (Plugin.Inst.OrigServerModel.DescriptionServerList.Length - 64) +
                                                 " characters over budget!");
                        CommandWindow.LogWarning("Server will not list properly until this URL is adjusted!");
                    }

                    SteamGameServer.SetGameDescription(Plugin.Inst.OrigServerModel.DescriptionServerList);
                }

                if (Plugin.Conf.CommonSetting.BrowserIcon.Edit)
                    SteamGameServer.SetKeyValue("Browser_Icon", Plugin.Inst.OrigServerModel.BrowserIcon);

                if (Plugin.Conf.CommonSetting.BrowserDescription.Edit)
                    SteamGameServer.SetKeyValue("Browser_Desc_Hint",
                        Plugin.Inst.OrigServerModel.BrowserDescriptionHint);

                if (Plugin.Conf.CommonSetting.LobbyDescriptionFull.Edit)
                {
                    if (string.IsNullOrWhiteSpace(Plugin.Inst.OrigServerModel.BrowserDescriptionFull))
                    {
                        SteamGameServer.SetKeyValue("Browser_Desc_Full_Count", "0");
                        SteamGameServer.SetKeyValue("Browser_Desc_Full_Line_0", string.Empty);
                    }
                    else
                    {
                        var num5 = (Plugin.Inst.OrigServerModel.BrowserDescriptionFull.Length - 1) / 120 + 1;
                        var num6 = 0;
                        SteamGameServer.SetKeyValue("Browser_Desc_Full_Count", num5.ToString());
                        for (var k = 0; k < Plugin.Inst.OrigServerModel.BrowserDescriptionFull.Length; k += 120)
                        {
                            var num7 = 120;
                            if (k + num7 > Plugin.Inst.OrigServerModel.BrowserDescriptionFull.Length)
                            {
                                num7 = Plugin.Inst.OrigServerModel.BrowserDescriptionFull.Length - k;
                            }

                            var pValue = Plugin.Inst.OrigServerModel.BrowserDescriptionFull.Substring(k, num7);
                            SteamGameServer.SetKeyValue("Browser_Desc_Full_Line_" + num6, pValue);
                            num6++;
                        }
                    }
                }

                if (Plugin.Conf.AdvancedSetting.Workshop.Edit)
                {
                    if (Plugin.Inst.OrigServerModel.ServerWorkshopFileIDs.Count > 0)
                    {
                        var text7 = string.Empty;
                        for (var l = 0; l < Plugin.Inst.OrigServerModel.ServerWorkshopFileIDs.Count; l++)
                        {
                            if (text7.Length > 0)
                            {
                                text7 += ",";
                            }

                            text7 += Plugin.Inst.OrigServerModel.ServerWorkshopFileIDs[l];
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

                if (Plugin.Conf.CommonSetting.LobbyCustomLinks.Edit)
                {
                    if (Plugin.Inst.OrigServerModel.CustomLinks != null &&
                        Plugin.Inst.OrigServerModel.CustomLinks.Length != 0)
                    {
                        SteamGameServer.SetKeyValue("Custom_Links_Count",
                            Plugin.Inst.OrigServerModel.CustomLinks.Length.ToString());
                        for (var n = 0; n < Plugin.Inst.OrigServerModel.CustomLinks.Length; n++)
                        {
                            SteamGameServer.SetKeyValue("Custom_Link_Message_" + n,
                                Plugin.Inst.OrigServerModel.CustomLinks[n].Message);
                            SteamGameServer.SetKeyValue("Custom_Link_Url_" + n,
                                Plugin.Inst.OrigServerModel.CustomLinks[n].Url);
                        }
                    }
                }

                if (Plugin.Conf.CommonSetting.LobbyPlugins.Edit)
                {
                    SteamGameServer.SetKeyValue("rocketplugins",
                        string.Join(",", R.Plugins.GetPlugins().Select(p => p.Name).ToArray()));
                }

                if (Plugin.Conf.CommonSetting.LobbyConfiguration.Edit)
                {
                    var modeConfig = ConfigData.CreateDefault(false).getModeConfig(Provider.mode);
                    if (modeConfig == null)
                    {
                        CommandWindow.LogError("Unable to compare default for advertise Plugin.Config");
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
                                CommandWindow.LogErrorFormat("Unable to advertise Plugin.Config type: {0}", fieldType);
                            }

                            if (string.IsNullOrEmpty(text)) 
                                continue;
                            
                            var pKey = "Browser_Plugin.Config_" + num.ToString(CultureInfo.InvariantCulture);
                            num++;
                            SteamGameServer.SetKeyValue(pKey, text);
                        }
                    }

                    SteamGameServer.SetKeyValue("Browser_Plugin.Config_Count",
                        num.ToString(CultureInfo.InvariantCulture));
                }

                var gameTags = string.Concat(Plugin.Inst.OrigGameTagsModel.IsPvP, ",",
                    Plugin.Inst.OrigGameTagsModel.HasCheats, ",",
                    Plugin.Inst.OrigGameTagsModel.Mode, ",", Plugin.Inst.OrigGameTagsModel.CameraMode, ",",
                    Plugin.Inst.OrigGameTagsModel.Workshop, ",",
                    Plugin.Inst.OrigGameTagsModel.IsGold, ",", Plugin.Inst.OrigGameTagsModel.BattlEyeSecure, ",",
                    Plugin.Inst.OrigGameTagsModel.Monetization, ",<tn>", Plugin.Inst.OrigGameTagsModel.Thumbnail,
                    "</tn>");
                if (Plugin.Conf.AdvancedSetting.NetTransport.Edit)
                    gameTags += $",<net>{Plugin.Inst.OrigGameTagsModel.NetTransport}</net>";
                if (Plugin.Conf.AdvancedSetting.PluginFramework.Edit)
                    gameTags += $",<pf>{Plugin.Inst.OrigGameTagsModel.PluginFramework}</pf>";

                SteamGameServer.SetGameTags(gameTags);
            }
            catch (Exception e)
            {
                Logger.LogError($"[{Plugin.Inst.Name}] [ERROR] Details: " + e);
            }
        }

        internal static string ApplyRich(this string s)
        {
            return s.Replace("-=", "<").Replace("=-", ">");
        }
    }
}