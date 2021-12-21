using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static CitizenFX.Core.Native.API;

namespace BudPlaza.BladeXClient.Util
{
    internal static class LScreen
    {
        internal static void DisplayHelp(string text, bool beep, int duration)
        {
            BeginTextCommandDisplayHelp("STRING");
            AddTextComponentSubstringPlayerName(text);
            EndTextCommandDisplayHelp(0, false, beep, duration);
        }
    }
}
