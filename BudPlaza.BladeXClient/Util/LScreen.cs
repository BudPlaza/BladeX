// Blade™ Client for Grand Theft Auto V (MP)
// (C) BudPlaza & contributors
// See COPYING for more information

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using static CitizenFX.Core.Native.API;

namespace BudPlaza.BladeXClient.Util
{
    internal static class LScreen
    {
        public static void SetPedNewUnf(this Ped ped)
        {
            SetPedComponentVariation(ped.Handle, 0, 0, 0, 0);
            SetPedComponentVariation(ped.Handle, 1, 0, 0, 0);
            SetPedComponentVariation(ped.Handle, 2, 4, 0, 0);
            SetPedComponentVariation(ped.Handle, 3, 15, 0, 0);
            SetPedComponentVariation(ped.Handle, 4, 15, 0, 0);
            SetPedComponentVariation(ped.Handle, 6, 9, 0, 0);
            SetPedComponentVariation(ped.Handle, 7, -1, 0, 0);
            SetPedComponentVariation(ped.Handle, 8, 35, 0, 0);
            SetPedComponentVariation(ped.Handle, 9, 7, 1, 0);
            SetPedComponentVariation(ped.Handle, 10, 0, 0, 0);
            SetPedComponentVariation(ped.Handle, 11, -1, 0, 0);
        }

        public static PedHeadBlendData RandomHeadBlend()
        {
            var rdm = new Random();

            var result = new PedHeadBlendData(rdm.Next(48), rdm.Next(48), rdm.Next(48), 0, 0, 0, 0.5f, 0f, 0f, false);
            return result;
        }

        internal static void DisplayHelp(string text, bool beep, int duration)
        {
            BeginTextCommandDisplayHelp("STRING");
            AddTextComponentSubstringPlayerName(text);
            EndTextCommandDisplayHelp(0, false, beep, duration);
        }
    }
}
