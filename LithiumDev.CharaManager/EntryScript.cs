using CitizenFX.Core;
using System;
using static CitizenFX.Core.Native.API;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LithiumDev.CharaManager.Users;
using LithiumDev.CharaManager.UserData;

namespace LithiumDev.CharaManager
{
    public class EntryScript : BaseScript
    {
        public EntryScript()
        {
            EventHandlers["onServerResourceStart"] += new Action<string>(ServerResourcesStart);
        }

        private void ChatMessage(string playerSrc, string player, string msg)
        {
            if (ManagementProcess.DisabledCommands.Contains(msg))
            {
                Debug.WriteLine("contains msg");
            }

            if (msg == "=)")
            {
                Debug.WriteLine("Sent smileface");
                TriggerClientEvent("chara:sendMessage", "=)");
            }
        }

        private void PlayerJoin([FromSource] Player player, string oldId)
        {
            ManagementProcess.RegisterJoinedPlayer(player);
            Console.WriteLine($"Player {player.Name} joined with endpoint {player.EndPoint}");
            TriggerClientEvent(player, "chara:displayWelcomeMessage");
        }

        private void ServerResourcesStart(string resName)
        {
            if (GetCurrentResourceName() != resName) return;

            Debug.WriteLine("Initialized LithiumDev.CharaManager =)");
            
            if (DateTime.Now.Month == 12 && DateTime.Now.Day == 21)
            {
                Debug.WriteLine("Happy birthday WithLithum!");
            }

            EventHandlers["playerJoining"] += new Action<Player, string>(PlayerJoin);
            EventHandlers["chatMessage"] += new Action<string, string, string>(ChatMessage);
            EventHandlers["chara:gamedataUpdate"] += new Action<int, int, int>(UpdateData);

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
