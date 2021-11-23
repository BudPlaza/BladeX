using CitizenFX.Core;
using LithiumDev.CharaClient.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CitizenFX.Core.Native.API;

namespace LithiumDev.CharaClient.Commands
{
    internal static class CommandRegistry
    {
        internal static void RegisterCommands()
        {
            RegisterCommand("tp-marker", new Action(TpMarker), true);
        }

        #region Command Handlers

        private static void TpMarker()
        {
            if (!Game.IsWaypointActive)
            {
                Chat.SendMessage("You do not have a waypoint!");
                return;
            }

            Blip way = new Blip(GetFirstBlipInfoId(8));
            if (way.Exists())
            {
                var pos = 0f;

                GetGroundZFor_3dCoord(way.Position.X, way.Position.Y, way.Position.Z, ref pos, false);
                if (pos != 0f)
                {
                    Game.PlayerPed.Position = new Vector3(way.Position.X, way.Position.Y, pos);
                }
                else
                {
                    Chat.SendMessage("Something went wrong (pos = 0f)");
                }
            }
            else
            {
                Chat.SendMessage("Either something went wrong or you do not have a waypoint");
            }
        }

        #endregion
    }
}
