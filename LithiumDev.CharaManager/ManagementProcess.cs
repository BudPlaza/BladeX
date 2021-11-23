using CitizenFX.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LithiumDev.CharaManager
{
    internal static class ManagementProcess
    {
        private static readonly List<Player> _players = new List<Player>();
        internal static readonly string[] DisabledCommands =
        {
            "/restart",
            "/stop",
            "/start"
        };

        internal static void RegisterJoinedPlayer(Player player)
        {
            _players.Add(player);
        }
    }
}
