using CitizenFX.Core;
using static CitizenFX.Core.Native.API;

namespace BudPlaza.BladeXClient.Entities
{
    public static class PlayerExtensions
    {
        /// <summary>
        /// Totally disables the control of this instance.
        /// </summary>
        /// <param name="player">The player.</param>
        /// <param name="toggle">On or off.</param>
        public static void Freeze(this Player player, bool toggle)
        {
            player.CanControlCharacter = !toggle;

            var chara = player.Character;
            chara.IsVisible = toggle;
            chara.IsCollisionEnabled = toggle;
            chara.IsPositionFrozen = toggle;
            chara.IsInvincible = toggle;

            if (toggle && !IsPedFatallyInjured(chara.Handle))
            {
                chara.Task.ClearAllImmediately();
            }
        }
    }
}