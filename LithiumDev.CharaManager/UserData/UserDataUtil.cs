using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LithiumDev.CharaManager.UserData
{
    internal static class UserDataUtil
    {
        internal const string UserDataFolder = @"C:\Programs\Servers\FiveM\UserData";

        internal static readonly string OperatorsFile = GetDataPath("ops.json");

        static UserDataUtil()
        {
            Directory.CreateDirectory(UserDataFolder);
        }

        internal static string GetDataPath(string path) => Path.Combine(UserDataFolder, path);
    }
}
