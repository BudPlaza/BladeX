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
using System.IO;
using Newtonsoft.Json;
using CitizenFX.Core.Native;

namespace BudPlaza.BladeX
{
    public class EntryScript : BaseScript
    {
        private readonly ILog _log;

        public EntryScript()
        {
            XmlConfigurator.Configure(new FileInfo(UserDataUtil.GetDataPath("log4net.config")));

            _log = LogManager.GetLogger("main");
            EventHandlers["onServerResourceStart"] += new Action<string>(ServerResourcesStart);
        }

        private void ChatMessage(string playerSrc, string player, string msg)
        {
            // reserved
        }

        private void PlayerJoin([FromSource] Player player, string oldId)
        {
            if (string.IsNullOrWhiteSpace(player.Identifiers["fivem"]))
            {
                _log.InfoFormat("Dropped player {0} because they are not logged in", player.Handle);
                player.Drop("Cfx.re account ID not found, which this server uses to uniquely identify you. Log your client into your Cfx.re account before entering the server.");
                return;
            }

            ManagementProcess.RegisterJoinedPlayer(player);
            _log.InfoFormat("Player {0} with ID {1} joined with endpoint {2} (src {3})", player.Name, player.Identifiers["fivem"], player.EndPoint, player.Handle);
            TriggerClientEvent(player, "bladex:displayWelcomeMessage");
        }

        private void ServerResourcesStart(string resName)
        {
            if (GetCurrentResourceName() != resName) return;

            _log.Info("Blade™ server software for CitizenFX.re Servers (Grand Theft Auto V)");
            _log.Info("Initializing");
            
            if (DateTime.Now.Month == 12 && DateTime.Now.Day == 21)
            {
                
            }

            EventHandlers["playerJoining"] += new Action<Player, string>(PlayerJoin);
            EventHandlers["chatMessage"] += new Action<string, string, string>(ChatMessage);
            EventHandlers["bladex:gamedataUpdate"] += new Action<int, int, int>(UpdateData);
            EventHandlers["baseevents:onPlayerDied"] += new Action<int, float[]>(OnPlayerDied);
            EventHandlers["bladex:clientInquireVessel"] += new Action<Player>(ClientInquireVessel);
            EventHandlers["bladex:characterCreation"] += new Action<Player>(CreatorAdd);

            RegisterCommand("op", new Action<int>(SetAsOp), true);
        }

        private void CreatorAdd([FromSource] Player obj)
        {
            SetPlayerModel(obj.Handle, (uint)GetHashKey("mp_f_freemode_01"));
            obj.Character.IsPositionFrozen = false;
            obj.Character.Heading = 331;
            obj.TriggerEvent("bladex:creationGoAhead");
        }

        private void ClientInquireVessel([FromSource] Player player)
        {
            _log.Info($"Client #{player.Handle} is inquring for an vessel.");

            var data = AccountDataUtil.CreateIfNotExist(player);
            if (!data.HasVessel || data.Vessel == null)
            {
                player.TriggerEvent("bladex:vesselResponse", false);
                return;
            }

            player.TriggerEvent("bladex:vesselResponse", true);
        }

        private void OnPlayerDied(int pType, float[] coords)
        {
            TriggerClientEvent("bladex:broadcast", "oops someone died on " + pType);
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
