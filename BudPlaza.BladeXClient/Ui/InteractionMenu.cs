using CitizenFX.Core;
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
        private readonly ObjectPool _pool = new ObjectPool();

        private readonly NativeMenu _menu = new NativeMenu(Game.Player.Name, "Interaction Menu");
        private readonly NativeItem _itemSuicide = new NativeItem("Suicide", "Do you really wanna do this?");
        private readonly NativeItem _itemRequestVehicle = new NativeItem("Request Vehicle", "Requests a vehicle");

        internal InteractionMenu()
        {
            _pool.Add(_menu);
            _menu.Add(_itemSuicide);
            _menu.Add(_itemRequestVehicle);

            _itemSuicide.Activated += (sender, e) => Game.Player.Character.Kill();
            _itemRequestVehicle.Activated += ItemRequestVehicle_Activated;
        }

        private void ItemRequestVehicle_Activated(object sender, EventArgs e)
        {
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

            _pool.Process();
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
