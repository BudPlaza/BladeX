// Blade™ Client for Grand Theft Auto V (MP)
// (C) BudPlaza & contributors
// See COPYING for more information

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LemonUI.Menus;

namespace BudPlaza.BladeX.Client.Ui
{
    internal class CharactorCreatorMenu
    {
        private readonly NativeMenu menu = new NativeMenu("Charactor Creator", "Menu");
        private readonly NativeItem itemHeadBlend = new NativeItem("Head Blend", "Modifies the head blend of the character.");
    }
}
