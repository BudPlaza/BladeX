// Blade™ Client for Grand Theft Auto V (MP)
// (C) BudPlaza & contributors
// See COPYING for more information

using CitizenFX.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BudPlaza.BladeXClient.Util
{
    internal static class Chat
    {
        internal static EntryScript Script { get; set; }

        internal static void SendMessage(string text)
        {
            BaseScript.TriggerEvent("chat:addMessage", new
            {
                color = new[] { 10, 255, 10 },
                args = new[] { "[Server]", text }
            });
        }
    }
}
