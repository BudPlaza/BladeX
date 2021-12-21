using CitizenFX.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LithiumDev.CharaClient.Util
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
