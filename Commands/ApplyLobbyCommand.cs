using System.Collections.Generic;
using RFLobbyModifier.Utils;
using Rocket.API;

namespace RFLobbyModifier.Commands
{
    public class ApplyLobbyCommand : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Both;
        public string Name => "applylobby";
        public string Help => "";
        public string Syntax => "/applylobby <original|mod>";
        public List<string> Aliases => new List<string>();
        public List<string> Permissions => new List<string> {"applylobby"};
        public void Execute(IRocketPlayer caller, string[] command)
        {
            if (command.Length != 1)
            {
                return;
            }

            switch (command[0].ToLower())
            {
                case "original":
                    LobbyUtil.RevertServer();
                    break;
                case "mod":
                    Plugin.Inst.Configuration.Load();
                    LobbyUtil.ModServer();
                    break;
            }
        }
    }
}