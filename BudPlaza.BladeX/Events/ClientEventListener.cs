using CitizenFX.Core;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BudPlaza.BladeX.Events
{
    internal class ClientEventListener
    {
        internal ClientEventListener(EventHandlerDictionary dict)
        {
            logger.Info("Initializing");
            dict["playerJoining"] += new Action<Player, string>(OnPlayerJoin);
        }

        private readonly ILog logger = LogManager.GetLogger("Client Handler #1");

        public void OnPlayerJoin([FromSource] Player player, string oldId)
        {
            if (player.Identifiers["fivem"] == null)
            {
                logger.Info($"Player #{player.Handle} has not logged in");
                player.Drop("Please log your client into Cfx.re and try again.");
                return;
            }

            logger.Info($"Player {player.Name} (#{player.Handle}) with handshake <{oldId}> joined from endpoint {player.EndPoint} with ped {player.Character.Handle}");
            ManagementProcess.RegisterJoinedPlayer(player);
        }
    }
}
