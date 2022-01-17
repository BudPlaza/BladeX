using System.Threading.Tasks;
using BudPlaza.BladeXClient.Entities;
using CitizenFX.Core;
using CitizenFX.Core.UI;

namespace BudPlaza.BladeXClient.Spawning
{
    internal static class SpawnUtil
    {
        private static bool _spawnLocked;

        internal static async void SpawnPlayer(Vector3 centerPosition)
        {
            if (_spawnLocked)
            {
                return;
            }

            _spawnLocked = true;
            
            Screen.Fading.FadeOut(500);

            while (!Screen.Fading.IsFadedOut)
            {
                await Task.Delay(1);
            }
            
            Game.Player.Freeze(true);
        }
    }
}