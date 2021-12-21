using fNbt;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LithiumDev.CharaManager.UserData
{
    public static class NbtUtil
    {
        public static async void SaveGameDataForPlayer(string playerId, int health, int armor)
        {
            var dataPath = UserDataUtil.GetPlayerDataFilePath(playerId);

            if (File.Exists(dataPath))
            {
                var file = new NbtFile(dataPath);
                file.RootTag.Get<NbtInt>("health").Value = health;
                file.RootTag.Get<NbtInt>("armor").Value = armor;
                await SaveNbtToFile(dataPath, file).ConfigureAwait(false);
            }
            else
            {
                var file = new NbtFile();
                file.RootTag.Add(new NbtInt("health", health));
                file.RootTag.Add(new NbtInt("armor", armor));
                await SaveNbtToFile(dataPath, file).ConfigureAwait(false);
            }
        }

        public static Task SaveNbtToFile(string path, NbtFile file)
        {
            file.SaveToFile(path, NbtCompression.GZip);
            return Task.FromResult(0);
        }
    }
}
