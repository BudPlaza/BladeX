// Blade™ Server X
// Copyright (C) 2021, 2022 BudPlaza project & contributors.
// Licensed under GNU AGPL v3 or any later version; see COPYING for information.

using BudPlaza.BladeX.UserData.Structs;
using CitizenFX.Core;
using LiteDB;
using log4net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;

namespace BudPlaza.BladeX.UserData
{
    internal static class AccountDataUtil
    {
        private static readonly LiteDatabase database;
        private static readonly ILog logger = LogManager.GetLogger("AccountData");
        private static readonly string dbPath = UserDataUtil.GetDataPath("data.db");

        static AccountDataUtil()
        {
            logger.Info($"Starting database on {dbPath}");
            database = new LiteDatabase(dbPath);
        }

        internal static void InitializeDb()
        {
            // Empty nullsub for static construction
        }

        public static PlayerData InitializeUser(Player player)
        {
            logger.Info($"Creating player #{player.Identifiers["fivem"]}");
            var col = database.GetCollection<PlayerData>();

            var data = new PlayerData()
            {
                Account = player.Identifiers["fivem"],
                HasVessel = false,
                UserName = player.Name,
                Vessel = null
            };

            col.Insert(data);
            return data;
        }

        public static PlayerData CreateIfNotExist(Player player)
        {
            if (!TryGetPlayerViaId(player.Identifiers["fivem"], out var result))
            {
                return InitializeUser(player);
            }

            return result;
        }

        public static bool TryGetPlayerViaId(string id, out PlayerData result)
        {
            var col = database.GetCollection<PlayerData>();
            var data = col.FindOne(x => x.Account == id);
            if (data == null)
            {
                result = null;
                return false;
            }

            result = data;
            return true;
        }
    }
}
