// Blade™ Client for Grand Theft Auto V (MP)
// (C) BudPlaza & contributors
// See COPYING for more information

using CitizenFX.Core;
using CitizenFX.Core.Native;
using CitizenFX.Core.UI;
using LemonUI;
using LemonUI.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BudPlaza.BladeXClient.Ui
{
    internal class InteractionMenu
    {
        private Vehicle _requested;
        private Blip _requestedBlip;

        private int _counter;
        public static readonly ObjectPool PublicPool = new ObjectPool();

        private readonly NativeMenu _menu = new NativeMenu(Game.Player.Name, Game.GetGXTEntry("INPUT_INTERACTION_MENU"));
        private readonly NativeItem _itemSuicide = new NativeItem("Suicide", "Do you really wanna do this?");
        private readonly NativeItem _itemRequestVehicle = new NativeItem("Request Vehicle", "Requests a vehicle");
        private readonly NativeItem _itemGetCoords = new NativeItem("Get coords", "Sends the coordinate to your location.");

        internal InteractionMenu()
        {
            PublicPool.Add(_menu);
            _menu.Add(_itemSuicide);
            _menu.Add(_itemRequestVehicle);
            _menu.Add(_itemGetCoords);

            _itemSuicide.Activated += (sender, e) => Game.Player.Character.Kill();
            _itemRequestVehicle.Activated += ItemRequestVehicle_Activated;
            _itemGetCoords.Activated += _itemGetCoords_Activated;
        }

        private void _itemGetCoords_Activated(object sender, EventArgs e)
        {
            API.BeginTextCommandThefeedPost("STRING");
            API.AddTextComponentSubstringPlayerName($"~BLIP_EX_VECH_1~ Your current coordinates are {Game.PlayerPed.Position.X}, {Game.PlayerPed.Position.Y}, {Game.PlayerPed.Position.Z} (Yaw {Game.PlayerPed.Heading})");
            API.EndTextCommandThefeedPostMpticker(true, true);
        }

        private void ItemRequestVehicle_Activated(object sender, EventArgs e)
        {
            World.CreateRandomVehicle(Game.PlayerPed.GetOffsetPosition(Vector3.ForwardLH));
        }

        internal void Update()
        {
            if (_counter > 0)
            {
                _counter--;
            }

            if (_requested?.Exists() == true)
            {
                _itemRequestVehicle.Enabled = false;
            }
            else
            {
                if (_requestedBlip?.Exists() == true)
                {
                    _requestedBlip.Delete();
                }
            }

            PublicPool.Process();
        }

        internal void OpenOrClose()
        {
            if (_counter > 0)
            {
                return;
            }

            _menu.Visible = !_menu.Visible;
            _counter = 50;
        }
    }
}
