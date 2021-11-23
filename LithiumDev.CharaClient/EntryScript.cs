using CitizenFX.Core;
using static CitizenFX.Core.Native.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core.UI;
using LithiumDev.CharaClient.Commands;
using LithiumDev.CharaClient.Util;
using LithiumDev.CharaClient.Ui;

namespace LithiumDev.CharaClient
{
    public class EntryScript : BaseScript
    {
        private bool _staminaDisplayed;
        private InteractionMenu _menu;
        private int _uploadInterval = 25000;

        internal static bool IsOp { get; private set; }

        public EntryScript()
        {
            EventHandlers["onClientResourceStart"] += new Action<string>(OnClientResourceStart);
            Util.Chat.Script = this;
        }

        private void DisplayMsg(string text)
        {
            Debug.WriteLine("Receiving client event requesting to send " + text);
            TriggerEvent("chat:addMessage", new
            {
                color = new[] { 100, 25, 250 },
                args = new[] { "[LDev]", text }
            });
        }

        private void DisplayWelcome()
        {
            LScreen.DisplayHelp("HEY EVERY!", true, 1000);
        }

        private void OnClientResourceStart(string name)
        {
            if (name != GetCurrentResourceName()) return;

            Debug.WriteLine("LithiumDev CharaClient expiermental =)");
            Debug.WriteLine("Powered by CitizenFX.re");

            Debug.WriteLine("Registering events");
            EventHandlers["chara:displayWelcomeMessage"] += new Action(DisplayWelcome);
            EventHandlers["chara:sendMessage"] += new Action<string>(DisplayMsg);
            EventHandlers["chara:permissionChange"] += new Action<bool>(PermissionChange);

            Debug.WriteLine("Registering commands");
            CommandRegistry.RegisterCommands();

            _menu = new InteractionMenu();

            Tick += EntryScript_Tick;
        }

        private Task EntryScript_Tick()
        {
            _menu.Update();

            if (Game.IsControlPressed(0, Control.InteractionMenu))
            {
                _menu.OpenOrClose();
            }

            if ((Game.Player.Character.IsRunning || Game.Player.Character.IsSprinting) && Game.Player.RemainingSprintStamina < 0.03f && !_staminaDisplayed)
            {
                _staminaDisplayed = true;
                LScreen.DisplayHelp("If you keep running while your stamina is running out you will begin to lose health.", true, 5000);
            }
            else
            {
                _staminaDisplayed = false;
            }

            if ((Game.Player.Character.IsRunning || Game.Player.Character.IsSprinting) && Game.Player.RemainingSprintStamina < 0f)
            {
                Game.Player.Character.HealthFloat -= 0.03f;
            }

            if (_uploadInterval > 0)
            {
                _uploadInterval--;
            }
            else
            {
                _uploadInterval = 25000;
                TriggerServerEvent("chara:gamedataUpdate", Game.Player.Handle, Game.PlayerPed.Health, Game.PlayerPed.Armor);
            }

            return Task.FromResult(0);
        }

        private void PermissionChange(bool obj)
        {
            IsOp = obj;
        }
    }
}
