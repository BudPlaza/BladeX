// Blade™ Client for Grand Theft Auto V (MP)
// (C) BudPlaza & contributors
// See COPYING for more information

using CitizenFX.Core;
using static CitizenFX.Core.Native.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core.UI;
using BudPlaza.BladeXClient.Commands;
using BudPlaza.BladeXClient.Util;
using BudPlaza.BladeXClient.Ui;
using System.Runtime.InteropServices;

namespace BudPlaza.BladeXClient
{
    public class EntryScript : BaseScript
    {
        private bool _staminaDisplayed;
        private InteractionMenu _menu;
        private int _uploadInterval = 120000;
        private const int _uploadTotal = 120000;
        private static bool _mi;
        private static bool _prevMi;
        private static int _miCooldown;
        private static int _miTimeout;

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

            Debug.WriteLine("BladeX™ Software for Microsoft Windows® Clients");
            Debug.WriteLine("Powered by CitizenFX.re");

            Debug.WriteLine("Registering events");
            EventHandlers["bladex:displayWelcomeMessage"] += new Action(DisplayWelcome);
            EventHandlers["bladex:sendMessage"] += new Action<string>(DisplayMsg);
            EventHandlers["bladex:permissionChange"] += new Action<bool>(PermissionChange);

            Debug.WriteLine("Registering commands");
            CommandRegistry.RegisterCommands();

#if SERVER_AUTHORITY_SYNC
            Debug.WriteLine("Disabling client authority ambience");
            SetPedPopulationBudget(0);
            SetVehiclePopulationBudget(0);
#endif

            StatSetInt((uint)GetHashKey("BANK_BALANCE"), 500, true);

            _menu = new InteractionMenu();

            Tick += EntryScript_Tick;
        }

        private Task EntryScript_Tick()
        {
            _menu.Update();
            if (_miCooldown > 0) _miCooldown--;
            if (_miTimeout > 0)
            {
                _miTimeout--;
            }
            else if (_mi)
            {
                _mi = false;
            }

            if (Game.IsControlPressed(0, Control.InteractionMenu))
            {
                _menu.OpenOrClose();
            }

            if (Game.IsControlPressed(0, Control.MultiplayerInfo) && _miCooldown == 0)
            {
                _mi = !_mi;
                _miCooldown = 75;
                _miTimeout = 150;
            }

            if (_mi && !_prevMi)
            {
                N_0x170f541e1cadd1de(true);
                SetMultiplayerWalletCash();
                SetMultiplayerBankCash();
                N_0x170f541e1cadd1de(false);
                SetBigmapActive(true, false);
                _prevMi = true;
            }

            if (!_mi && _prevMi)
            {
                RemoveMultiplayerWalletCash();
                RemoveMultiplayerBankCash();
                SetBigmapActive(false, false);
                _prevMi = false;
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

#if SERVER_AUTHORITY_SYNC
            if (_uploadInterval > 0)
            {
                _uploadInterval--;
            }
            else
            {
                _uploadInterval = _uploadTotal;
                TriggerServerEvent("bladex:gamedataUpdate", Game.Player.Handle, Game.PlayerPed.Health, Game.PlayerPed.Armor);
            }
#endif
            return Task.FromResult(0);
        }

        private void PermissionChange(bool obj)
        {
            IsOp = obj;
        }
    }
}
