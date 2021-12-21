// Blade™ Server X
// Copyright (C) 2021, 2022 BudPlaza project & contributors.
// Licensed under GNU AGPL v3 or any later version; see COPYING for information.

using CitizenFX.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BudPlaza.BladeX
{
    internal static class ManagementProcess
    {
        private static readonly List<Player> _players = new List<Player>();
        internal static readonly string[] DisabledCommands =
        {
            "/restart",
            "/stop",
            "/start"
        };

        internal static void RegisterJoinedPlayer(Player player)
        {
            _players.Add(player);
        }
    }
}
