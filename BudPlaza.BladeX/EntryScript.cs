// Blade™ Server X
// Copyright (C) 2021, 2022 BudPlaza project & contributors.
// Licensed under GNU AGPL v3 or any later version; see COPYING for information.

using CitizenFX.Core;
using System;
using static CitizenFX.Core.Native.API;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BudPlaza.BladeX.Users;
using BudPlaza.BladeX.UserData;
using log4net.Config;
using log4net;

namespace BudPlaza.BladeX
{
    public class EntryScript : BaseScript
    {
        private readonly ILog _log;

        public EntryScript()
        {
            XmlConfigurator.Configure();
            _log = LogManager.GetLogger("main");
            EventHandlers["onServerResourceStart"] += new Action<string>(ServerResourcesStart);
        }

        private void ChatMessage(string playerSrc, string player, string msg)
        {
            // reserved
        }

        private void PlayerJoin([FromSource] Player player, string oldId)
        {
            ManagementProcess.RegisterJoinedPlayer(player);
            _log.InfoFormat("Player {0} with ID {1} joined with endpoint {2} (src {3})", player.Name, player.Identifiers["fivem"], player.EndPoint, player.Handle);
            TriggerClientEvent(player, "bladex:displayWelcomeMessage");
        }

        private void ServerResourcesStart(string resName)
        {
            if (GetCurrentResourceName() != resName) return;

            Debug.WriteLine("Initialized LithiumDev.bladexManager =)");
            
            if (DateTime.Now.Month == 12 && DateTime.Now.Day == 21)
            {
                Debug.WriteLine("Happy birthday WithLithum!");
            }

            EventHandlers["playerJoining"] += new Action<Player, string>(PlayerJoin);
            EventHandlers["chatMessage"] += new Action<string, string, string>(ChatMessage);
            EventHandlers["bladex:gamedataUpdate"] += new Action<int, int, int>(UpdateData);

            RegisterCommand("op", new Action<int>(SetAsOp), true);
        }

        private void UpdateData(int playerHandle, int health, int armor)
        {
            var player = GetPlayerFromIndex(playerHandle);
            NbtUtil.SaveGameDataForPlayer(GetPlayerIdentifier(player, 0), health, armor);
        }

        internal void SetAsOp(int fivemId)
        {
            Permission.AddNewOp(fivemId.ToString());
        }

    }
}
