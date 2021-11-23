using CitizenFX.Core;
using LemonUI;
using LemonUI.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LithiumDev.CharaClient.Ui
{
    internal class InteractionMenu
    {
        private int _counter;
        private readonly ObjectPool _pool = new ObjectPool();

        private readonly NativeMenu _menu = new NativeMenu(Game.Player.Name, "Interaction Menu");
        private readonly NativeItem _itemSuicide = new NativeItem("Suicide", "Do you really wanna do this?");

        internal InteractionMenu()
        {
            _pool.Add(_menu);
            _menu.Add(_itemSuicide);

            _itemSuicide.Activated += (sender, e) => Game.Player.Character.Kill();
        }

        internal void Update()
        {
            if (_counter > 0) _counter--;
            _pool.Process();
        }

        internal void OpenOrClose()
        {
            if (_counter > 0) return;
            _menu.Visible = !_menu.Visible;
            _counter = 50;
        }
    }
}
